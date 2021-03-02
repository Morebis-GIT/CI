using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using ImagineCommunications.GamePlan.Domain.AWSInstanceConfigurations;
using ImagineCommunications.GamePlan.Domain.Generic.Extensions;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.common.Services;
using xggameplan.Model;

namespace xggameplan.AutoBooks.AWS
{
    /// <summary>
    /// For managing all AutoBooks collectively in AWS
    /// </summary>
    public class AWSAutoBooks : IAutoBooks
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IAutoBookRepository _autoBookRepository;
        private readonly IAutoBookInstanceConfigurationRepository _autoBookInstanceConfigurationRepository;
        private readonly IAWSInstanceConfigurationRepository _awsInstanceConfigurationRepository;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly AutoBookSettings _autoBookSettings;
        private readonly IAutoBooksAPI<AWSPAAutoBook, AWSPACreateAutoBook> _autoBooksApi;       // Provisioning API (AutoBooks)
        private readonly string _accessToken;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="autoBookRepository">AutoBook repository</param>
        /// <param name="auditEventRepository">AuditEvent repository</param>
        /// <param name="autoBooksSettings">AutoBook settings</param>
        public AWSAutoBooks(IRepositoryFactory repositoryFactory,
            IAutoBookRepository autoBookRepository,
            IAutoBookInstanceConfigurationRepository autoBookInstanceConfigurationRepository,
            IAWSInstanceConfigurationRepository awsInstanceConfigurationRepository,
            IAuditEventRepository auditEventRepository,
            AutoBookSettings autoBooksSettings,
            IAutoBooksAPI<AWSPAAutoBook, AWSPACreateAutoBook> autoBooksApi, string accessToken)
        {
            _repositoryFactory = repositoryFactory;
            _autoBookRepository = autoBookRepository;
            _autoBookInstanceConfigurationRepository = autoBookInstanceConfigurationRepository;
            _awsInstanceConfigurationRepository = awsInstanceConfigurationRepository;
            _auditEventRepository = auditEventRepository;
            _autoBookSettings = autoBooksSettings;
            _autoBooksApi = autoBooksApi;
            _accessToken = accessToken;
        }

        public int Instances => _autoBookRepository.CountAll;

        public bool TestProvisioning()
        {
            bool result = false;
            try
            {
                var autoBookObjects = _autoBooksApi.GetAll();
                result = true;
            }
            catch { };
            return result;
        }

        /// <summary>
        /// Creates new AutoBook instance
        /// </summary>
        /// <param name="autoBook"></param>
        /// <param name="waitTimeout"></param>
        /// <returns></returns>
        public void Create(AutoBook autoBook)
        {
            if (String.IsNullOrEmpty(_autoBookSettings.ApplicationVersion))
            {
                throw new Exception("Cannot create AutoBook because Application Version is not set");
            }

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                $"Creating AutoBook instance (InstanceConfigurationId={autoBook.InstanceConfigurationId})"));

            // Get AutoBook instance configuration
            var autoBookInstanceConfiguration = _autoBookInstanceConfigurationRepository.Get(autoBook.InstanceConfigurationId);

            // Create AutoBook using AutoBooks API
            AWSPACreateAutoBook paCreateAutoBookObject = new AWSPACreateAutoBook()
            {
                Version = _autoBookSettings.ApplicationVersion,
                InstanceType = autoBookInstanceConfiguration.InstanceType,
                StorageSizeGb = autoBookInstanceConfiguration.StorageSizeGb
            };
            AWSPAAutoBook paAutoBook = _autoBooksApi.Create(paCreateAutoBookObject);

            if (paAutoBook is null)
            {
                throw new Exception("Failed requesting AutoBook Provider API to create a new AutoBook");
            }

            // Set the AutoBook.Id and AutoBook.Api from the Id and Url returned by the Autobook Provider API
            autoBook.Id = paAutoBook.Id;

            // Log event
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForAutoBookEvent(0, 0, autoBook.Id, AutoBookEventIDs.AutoBookCreated,
                                $"Created AutoBook (Version={paCreateAutoBookObject.Version}, Instance Configuration={autoBookInstanceConfiguration.Description})"));

            // Commit details of new AutoBook to GamePlan Database
            _autoBookRepository.Add(autoBook);
            _autoBookRepository.SaveChanges();

            return;
        }

        /// <summary>
        /// Deletes AutoBook instance
        /// </summary>
        /// <param name="autoBook"></param>
        /// <param name="waitTimeout"></param>
        public void Delete(AutoBook autoBook)
        {
            // Delete using AutoBooks API
            var paAutoBook = _autoBooksApi.Get(autoBook.Id);
            if (paAutoBook != null)
            {
                _autoBooksApi.Delete(paAutoBook.Id);
            }

            // Log event
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForAutoBookEvent(0, 0, autoBook.Id, AutoBookEventIDs.AutoBookDeleted,
                $"Deleting AutoBook"));

            // Remove from database
            _autoBookRepository.Delete(autoBook.Id);
            _autoBookRepository.SaveChanges();

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForAutoBookEvent(0, 0, autoBook.Id, AutoBookEventIDs.AutoBookDeleted,
               $"Deleted AutoBook"));

            return;
        }

        /// <summary>
        /// Restarts AutoBook instance, if timeout indicated then waits for completion/timeout. AutoBook.Status must be set to
        /// Provisioning during the restart, will be reset to Idle when AutoBook restarts and notifies us.
        /// </summary>
        /// <param name="autoBook"></param>
        /// <param name="waitTimeout"></param>
        public void Restart(AutoBook autoBook)
        {
            //Restart via AutoBooks API
            var paAutoBook = _autoBooksApi.Get(autoBook.Id);
            if (paAutoBook is null)
            {
                throw new Exception("AutoBook does not exist in AutoBooks API");
            }

            AutoBookStatuses oldStatus = autoBook.Status;   // Store status so that we can revert if restart fails
            bool restartCalled = false;
            try
            {
                autoBook.Status = AutoBookStatuses.Provisioning;
                _autoBookRepository.Update(autoBook);
                _autoBookRepository.SaveChanges();
                restartCalled = true;
                _autoBooksApi.Restart(paAutoBook.Id);
            }
            catch
            {
                if (restartCalled)     // Restart failed, revert status
                {
                    autoBook.Status = oldStatus;
                    _autoBookRepository.Update(autoBook);
                    _autoBookRepository.SaveChanges();
                }
                else    // Some error not caused by trying to restart, throw it
                {
                    throw;
                }
            }

            // Log event
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForAutoBookEvent(0, 0, autoBook.Id, AutoBookEventIDs.AutoBookRestarted, null));

            return;
        }

        /// <summary>
        /// Checks the minimum number of AutoBook instances are respected. If no min limit is defined then we delete all AutoBooks that have
        /// reached their min lifetime, typically happens in environments where we need to save costs and create AutoBooks on demand.
        /// </summary>
        private void CheckAutoBookMinInstances(bool isActiveRuns)
        {
            if (isActiveRuns)   // Do nothing
            {
                return;
            }

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                $"Checking AutoBook min instances (Min={_autoBookSettings.MinInstances})"));

            int countCreated = 0;
            int countDeleted = 0;

            // Get list of AutoBooks
            var autoBooks = _autoBookRepository.GetAll();

            if (_autoBookSettings.MinInstances > 0)     // Min instances required
            {
                // Start creating new AutoBooks, don't wait for each one here, wait for all collectively below
                var creatingAutoBooks = new List<AutoBook>();
                while (autoBooks.Count() < _autoBookSettings.MinInstances)
                {
                    // Provision new AutoBook instance
                    AutoBook newAutoBook = new AutoBook()
                    {
                        Locked = false,
                        Status = AutoBookStatuses.Provisioning,
                        TimeCreated = DateTime.UtcNow,
                        InstanceConfigurationId = _autoBookInstanceConfigurationRepository.GetAll().FirstOrDefault().Id
                    };
                    creatingAutoBooks.Add(newAutoBook);

                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                        "Creating new AutoBook to meet minimum instances requirement"));
                    Create(newAutoBook);

                    // Refresh AutoBook list
                    autoBooks = _autoBookRepository.GetAll();
                }
            }
            else if (_autoBookSettings.MinInstances == 0)   // No min instances, delete all free AutoBooks, typically to reduce costs
            {
                // Start deleting free AutoBooks
                var deletingAutoBooks = new List<AutoBook>();
                foreach (var autoBook in autoBooks)
                {
                    if ((_autoBookSettings.MinLifetime.ToTimeSpan().TotalMilliseconds == 0) || (autoBook.TimeCreated.Add(_autoBookSettings.MinLifetime.ToTimeSpan()) < DateTime.UtcNow))
                    {
                        // Get AutoBook status
                        IAutoBook autoBookInterface = GetInterface(autoBook);
                        AutoBookStatuses? autoBookStatus = null;
                        try
                        {
                            autoBookStatus = autoBookInterface.GetStatus();
                        }
                        catch { };          // Ignore error

                        if (autoBookStatus != null && AutoBook.IsOKForDelete((AutoBookStatuses)autoBookStatus))
                        {
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                                $"Deleting AutoBook {autoBook.Id} because there is no minimum instances requirement, fewer instances reduces costs"));
                            Delete(autoBook);
                            deletingAutoBooks.Add(autoBook);
                        }
                    }
                }
            }
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                $"Checked AutoBook min instances (Created={countCreated}, Deleted={countDeleted})"));
        }

        /// <summary>
        /// Checks if max number of AutoBook instances has been exceeded, unprovisions any if necessary. Busy AutoBooks will not be
        /// touched.
        /// </summary>
        private void CheckAutoBookMaxInstances(bool isActiveRuns)
        {
            if (isActiveRuns)       // Do nothing
            {
                return;
            }
            if (_autoBookSettings.MaxInstances > 0)     // Max instances required
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                    $"Checking AutoBook max instances (Max={_autoBookSettings.MaxInstances})"));

                int countDeleted = 0;

                // Get list of AutoBooks
                var autoBooks = _autoBookRepository.GetAll();

                // Start deleting excess AutoBooks if we have too many, we'll wait collectively at the end for all to be deleted. If we can't
                // can't find enough idle AutoBooks to delete then we'll wait until a timeout.
                DateTime startDeleteTimeout = DateTime.UtcNow.AddSeconds(30);  // Timeout for trying to find AutoBooks to start deleting
                var deletingAutoBooks = new List<AutoBook>();
                while (autoBooks.Count() > _autoBookSettings.MaxInstances && DateTime.UtcNow < startDeleteTimeout)
                {
                    foreach (var autoBook in autoBooks)
                    {
                        // Get AutoBook status
                        IAutoBook autoBookInterface = GetInterface(autoBook);
                        AutoBookStatuses? autoBookStatus = null;
                        try
                        {
                            autoBookStatus = autoBookInterface.GetStatus();
                        }
                        catch { };      // Ignore error

                        bool canDelete = false;
                        if (autoBookStatus != null && AutoBook.IsOKForDelete((AutoBookStatuses)autoBookStatus))
                        {
                            if (_autoBookSettings.MinLifetime.BclCompatibleTicks > 0)    // Can't delete until min lifetime reached
                            {
                                canDelete = (autoBook.TimeCreated.Add(_autoBookSettings.MinLifetime.ToTimeSpan()) < DateTime.UtcNow);
                            }
                            else    // No minimum lifetime
                            {
                                canDelete = true;
                            }
                        }
                        if (canDelete)
                        {
                            try
                            {
                                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                                    $"Deleting AutoBook {autoBook.Id} to meet maximum instances requirement"));
                                Delete(autoBook);
                                countDeleted++;
                                deletingAutoBooks.Add(autoBook);
                            }
                            catch (Exception exception)
                            {
                                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0,
                                    $"Error deleting AutoBook {autoBook.Id} to meet maximum instances requirement",
                                    exception));
                            }
                        }
                    }

                    // Refresh AutoBook list
                    autoBooks = _autoBookRepository.GetAll();

                    // If still too many then wait before trying to find other AutoBooks to delete
                    if (autoBooks.Count() > _autoBookSettings.MaxInstances && DateTime.UtcNow < startDeleteTimeout)
                    {
                        System.Threading.Thread.Sleep(10000);
                    }
                }

                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                    $"Checked AutoBook max instances (Deleted={countDeleted})"));
            }
        }

        /// <summary>
        /// Checks AutoBooks for max lifetime exceeded, unprovisions if necessary. We expect that CheckAutoBookMinInstances will be
        /// called afterwards to ensure that we have minimum instances.
        /// </summary>
        private void CheckAutoBookMaxLifetime(bool isActiveRuns)
        {
            if (isActiveRuns)   // Do nothing
            {
                return;
            }
            if (_autoBookSettings.MaxLifetime.BclCompatibleTicks > 0)     // Max instances required
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                    $"Checking AutoBook max lifetime (Max lifetime={_autoBookSettings.MaxLifetime.ToTimeSpan().TotalSeconds} secs)"));

                int countDeleted = 0;

                // Get list of AutoBooks
                var autoBooks = _autoBookRepository.GetAll();

                // Start deleting AutoBooks that have exceeded their max lifetime
                var deletingAutoBooks = new List<AutoBook>();
                foreach (var autoBook in autoBooks)
                {
                    // Get AutoBook status
                    IAutoBook autoBookInterface = GetInterface(autoBook);
                    AutoBookStatuses? autoBookStatus = null;
                    try
                    {
                        autoBookStatus = autoBookInterface.GetStatus();
                    }
                    catch { };          // Ignore error

                    // Determine if we can delete
                    bool canDelete = false;
                    if (autoBookStatus != null && AutoBook.IsOKForDelete((AutoBookStatuses)autoBookStatus))
                    {
                        if (_autoBookSettings.MaxLifetime.BclCompatibleTicks > 0)    // Has max lifetime
                        {
                            canDelete = (autoBook.TimeCreated.Add(_autoBookSettings.MaxLifetime.ToTimeSpan()) < DateTime.UtcNow);
                        }
                        else     // No max lifetime
                        {
                            canDelete = true;
                        }
                        if (canDelete && _autoBookSettings.MinLifetime.BclCompatibleTicks > 0)   // Has min lifetime
                        {
                            canDelete = (autoBook.TimeCreated.Add(_autoBookSettings.MinLifetime.ToTimeSpan()) < DateTime.UtcNow);
                        }
                    }
                    if (canDelete)
                    {
                        try
                        {
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                                $"Deleting AutoBook {autoBook.Id} to meet maximum lifetime limit of {_autoBookSettings.MaxLifetime.ToTimeSpan().TotalSeconds} secs"));
                            Delete(autoBook);
                            countDeleted++;
                            deletingAutoBooks.Add(autoBook);
                        }
                        catch (Exception exception)
                        {
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0,
                                $"Error deleting AutoBook {autoBook.Id} to meet maximum lifetime limit of {_autoBookSettings.MaxLifetime.ToTimeSpan().TotalSeconds} secs",
                                exception));
                        }
                    }
                }

                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, $"Checked AutoBook max lifetime (Deleted={countDeleted})"));
            }
        }

        /// <summary>
        /// Checks crashed AutoBook instances, attempts to restart or delete. We pick up AutoBooks with Status=Fatal_Error, stuck provisioning or stuck
        /// stuck on Task_Error (because AutoBook shouldn't be in this state for a long period).
        /// </summary>
        private void CheckAutoBookCrashes()
        {
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, "Checking AutoBook crashes"));

            int countRestarted = 0;
            int countDeleted = 0;

            // Get list of AutoBooks
            var autoBooks = _autoBookRepository.GetAll();

            var paAutoBooks = _autoBooksApi.GetAll();

            // Start restarting or deleting crashed AutoBooks, will wait at the end for them to collectively restart or delete
            var restartingAutoBooks = new List<AutoBook>();
            var deletingAutoBooks = new List<AutoBook>();
            foreach (AutoBook autoBook in autoBooks)
            {
                // Get AutoBookObject from AutoBooks API
                var paAutoBook = GetPAAutoBook(autoBook.Id, paAutoBooks);
                if (paAutoBook != null)     // No point in checking if AutoBook not in AutoBooks API
                {
                    // Check AutoBook status
                    IAutoBook autoBookInterface = GetInterface(autoBook);
                    string restartReason = null;        // Set this to restart
                    string deleteReason = null;         // Set this to delete
                    AutoBookStatuses status = (paAutoBook.Provisioned == false ? AutoBookStatuses.Provisioning : AutoBookStatuses.Fatal_Error);   // Default to error status that requires a restart
                    if (paAutoBook.Provisioned)
                    {
                        try
                        {
                            status = autoBookInterface.GetStatus();
                            if (status == AutoBookStatuses.Task_Error)
                            {
                                // Task error, should only exist for a matter of seconds, if it's stuck on this status then we should restart. See XGG-668.
                                DateTime wait = DateTime.UtcNow.AddSeconds(60);
                                do
                                {
                                    System.Threading.Thread.Sleep(1000);
                                } while (DateTime.UtcNow < wait);
                                status = autoBookInterface.GetStatus();
                                restartReason = (status == AutoBookStatuses.Task_Error ? $"the status has been stuck on {AutoBookStatuses.Fatal_Error}" : restartReason);
                            }
                            restartReason = (status == AutoBookStatuses.Fatal_Error ? $"the status is {AutoBookStatuses.Fatal_Error}" : restartReason);
                        }
                        catch    // Error checking the status
                        {
                            restartReason = (paAutoBook.Provisioned == false ? null : "an error happened when checking the status");
                        }
                    }

                    // Check if stuck provisioning, must be deleted
                    if (paAutoBook.Provisioned == false && _autoBookSettings.CreationTimeout.BclCompatibleTicks > 0 && autoBook.TimeCreated.Add(_autoBookSettings.CreationTimeout.ToTimeSpan()) < DateTime.UtcNow)
                    {
                        deleteReason = "it is stuck provisioning";
                    }

                    // Restart or delete if necessary
                    if (!String.IsNullOrEmpty(restartReason))
                    {
                        try
                        {
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                                $"Restarting AutoBook {autoBook.Id} because {restartReason}"));
                            Restart(autoBook);
                            countRestarted++;
                            restartingAutoBooks.Add(autoBook);
                        }
                        catch (Exception exception)
                        {
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0,
                                $"Error restarting AutoBook {autoBook.Id} because {restartReason}", exception));
                        }
                    }
                    else if (!String.IsNullOrEmpty(deleteReason))
                    {
                        try
                        {
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                                $"Deleting AutoBook {autoBook.Id} because {deleteReason}"));
                            Delete(autoBook);
                            countDeleted++;
                            deletingAutoBooks.Add(autoBook);
                        }
                        catch (Exception exception)
                        {
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0,
                                $"Error deleting AutoBook {autoBook.Id} because {deleteReason}", exception));
                        }
                    }
                }
            }
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                $"Checked AutoBook crashes (Restarted={countRestarted}, Delete={countDeleted})"));
        }

        /// <summary>
        /// Checks for orphaned AutoBooks, instances should exist in database and in AutoBooks API.
        ///
        /// This shouldn't really happen in a Production environment but might in a Dev environment.
        /// </summary>
        private void CheckAutoBookOrphans(bool isActiveRuns)
        {
            if (isActiveRuns)   // Do nothing
            {
                return;
            }
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, "Checking AutoBook orphans"));

            // Get list of AutoBooks in AutoBooks API
            var paAutoBooks = _autoBooksApi.GetAll();

            // Check for AutoBooks in AutoBooks API but not in the database, we'll be charged for those
            foreach (var paAutoBook in paAutoBooks)
            {
                var autoBook = _autoBookRepository.Get(paAutoBook.Id);
                if (autoBook is null)   // In Provisioning API but not database
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                        $"AutoBook {paAutoBook.Name} is an orphan because it exists in the AutoBooks API but not in the database"));

                    var instanceConfigurations = _autoBookInstanceConfigurationRepository.GetAll();
                    var matchingSize = instanceConfigurations.FirstOrDefault(ic => ic.Description.Contains(paAutoBook.InstanceType) && ic.Description.Contains(paAutoBook.StorageSizeGb));
                    // Create AutoBook in database so that we can actually use it.
                    autoBook = new AutoBook()
                    {
                        Id = paAutoBook.Id,
                        TimeCreated = DateTime.UtcNow,
                        Api = paAutoBook.Url,
                        Locked = false,
                        Status = (paAutoBook.Provisioned ? AutoBookStatuses.Idle : AutoBookStatuses.Provisioning),
                        InstanceConfigurationId = matchingSize is null ? instanceConfigurations.FirstOrDefault().Id : matchingSize.Id
                    };
                    _autoBookRepository.Add(autoBook);
                    _autoBookRepository.SaveChanges();
                }
            }

            // Check for AutoBooks in database but not in the AutoBooks API, we won't be charged for them but we can't use them
            var autoBooks = _autoBookRepository.GetAll();
            foreach (var autoBook in autoBooks)
            {
                var paAutoBook = GetPAAutoBook(autoBook.Id, paAutoBooks);
                if (paAutoBook == null) // In database but not Provisioning API
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                        $"AutoBook {autoBook.Id} is an orphan because it exists in the database but not in the AutoBooks API, deleting from database"));

                    // Remove from database
                    _autoBookRepository.Delete(autoBook.Id);
                    _autoBookRepository.SaveChanges();
                }
            }

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, "Checked AutoBook orphans"));
        }

        /// <summary>
        /// Checks AutoBook versions, if incorrect then unprovisions old AutoBook and creates new instances with correct version.
        ///
        /// It is not possible to create an AutoBook with the same ID in AWS until AWS has deleted the old instance. We do not know when the instance
        /// has actually been deleted because AWS does not provide us with a notification when it is complete.
        /// </summary>
        private void CheckAutoBookVersions(bool isActiveRuns)
        {
            if (isActiveRuns)   // Don't update versions while run active
            {
                return;
            }
            if (String.IsNullOrEmpty(_autoBookSettings.ApplicationVersion))     // Control of specific version if disabled
            {
                return;
            }

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                $"Checking AutoBook versions (Current={_autoBookSettings.ApplicationVersion})"));

            int countUpdated = 0;

            // Get list of AutoBooks
            var autoBooks = _autoBookRepository.GetAll();

            // Start deleting AutoBooks that are running the wrong version
            var deletingAutoBooks = new List<AutoBook>();
            foreach (AutoBook autoBook in autoBooks)
            {
                // Get AutoBook status, we can only delete if not in use
                IAutoBook autoBookInterface = GetInterface(autoBook);
                AutoBookStatuses? autoBookStatus = null;
                GetAutoBookVersionModel autoBookVersionModel = null;
                try
                {
                    // Get status
                    autoBookStatus = autoBookInterface.GetStatus();

                    // Get version
                    autoBookVersionModel = autoBookInterface.GetVersion();
                }
                catch { };      // Ignore error

                // Determine if this is the correct version
                bool isCorrectVersion = (autoBookVersionModel != null && autoBookVersionModel.Version.ToUpper() == _autoBookSettings.ApplicationVersion.ToUpper());

                if (isCorrectVersion)    // No action needed
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, String.Format("AutoBook {0} is running the correct version ({1})", autoBook.Id, autoBookVersionModel.Version)));
                }
                else if (autoBookStatus != null && autoBookVersionModel != null && !isCorrectVersion && AutoBook.IsOKForDelete((AutoBookStatuses)autoBookStatus))   // Needs updating and can be updated
                {
                    try
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                            $"Deleting AutoBook {autoBook.Id} so that the version can be updated from {autoBookVersionModel.Version} to {_autoBookSettings.ApplicationVersion}"));
                        Delete(autoBook);
                        deletingAutoBooks.Add(autoBook);
                    }
                    catch (Exception exception)
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0,
                            $"Error deleting AutoBook {autoBook.Id} so that the version can be updated from {autoBookVersionModel.Version} to {_autoBookSettings.ApplicationVersion}", exception));
                    }
                }
                else if (autoBookStatus == null)    // Provisioning/Restarting/Connectivity error
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                        $"Cannot check if AutoBook {autoBook.Id} is running the correct version ({_autoBookSettings.ApplicationVersion}) because there was an error checking the status"));
                }
                else if (autoBookStatus != null && autoBookVersionModel != null && !isCorrectVersion)    // Wrong version but can't be updated due to status
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                        $"AutoBook {autoBook.Id} is running version {autoBookVersionModel.Version} but it cannot be updated to {_autoBookSettings.ApplicationVersion} because the status is {autoBookStatus.Value.ToString()}"));
                }
            }

            if (deletingAutoBooks.Any())
            {
                // Start creating new AutoBook instances with correct version.
                var creatingAutoBooks = new List<AutoBook>();
                foreach (var deletingAutoBook in deletingAutoBooks)
                {
                    var newAutoBook = new AutoBook()
                    {
                        TimeCreated = DateTime.UtcNow,
                        Api = "",
                        Locked = false,
                        Status = AutoBookStatuses.Provisioning,
                        InstanceConfigurationId = deletingAutoBook.InstanceConfigurationId
                    };

                    try
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                            $"Creating AutoBook for previously deleted AutoBook {deletingAutoBook.Id} so that the version can be updated to {_autoBookSettings.ApplicationVersion}"));
                        Create(newAutoBook);
                        creatingAutoBooks.Add(newAutoBook);
                    }
                    catch (Exception exception)
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0,
                            $"Error creating AutoBook for previously deleted AutoBook so that the version can be updated to {_autoBookSettings.ApplicationVersion}", exception));
                    }
                }
            }

            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                $"Checked AutoBook versions"));
        }

        /// <summary>
        /// Waits for all AutoBooks to be in working state or until timeout
        /// </summary>
        /// <param name="autoBooks"></param>
        /// <param name="timeout"></param>
        /// <returns>AutoBooks successfully working</returns>
        public List<AutoBook> WaitForAutoBooksWorking(List<AutoBook> autoBooks, TimeSpan timeout)
        {
            DateTime failTimeout = DateTime.UtcNow.Add(timeout);
            HashSet<string> workingAutoBookIds = new HashSet<string>();
            do
            {
                using var scope = _repositoryFactory.BeginRepositoryScope();
                var autoBookRepository = scope.CreateRepository<IAutoBookRepository>();
                foreach (var currentAutoBook in autoBooks.Where(ab => !workingAutoBookIds.Contains(ab.Id)))
                {
                    var autoBook = autoBookRepository.Get(currentAutoBook.Id);
                    if (!String.IsNullOrEmpty(autoBook.Api))        // API property set when AutoBook notifies Gameplan API
                    {
                        IAutoBook autoBookInterface = GetInterface(autoBook);
                        AutoBookStatuses? autoBookStatus = null;
                        try
                        {
                            autoBookStatus = autoBookInterface.GetStatus();
                        }
                        catch { };      // Ignore error

                        if (autoBookStatus != null && AutoBook.IsWorkingStatus(autoBookStatus.Value))
                        {
                            _ = workingAutoBookIds.Add(autoBook.Id);
                        }
                    }
                }
                if (workingAutoBookIds.Count != autoBooks.Count && DateTime.UtcNow < failTimeout)
                {
                    System.Threading.Thread.Sleep(10000);     // Wait before checking status again
                }
            } while (workingAutoBookIds.Count != autoBooks.Count && DateTime.UtcNow < failTimeout);
            return autoBooks.Where(ab => workingAutoBookIds.Contains(ab.Id)).ToList();      // Return working AutoBooks
        }

        public AutoBookSettings Settings
        {
            get { return _autoBookSettings; }
        }

        public AutoBook GetCurrentAutoBookForRun(Run run, Guid scenarioId)
        {
            return _autoBookRepository
                .GetAll()
                .FirstOrDefault(currentAutoBook => currentAutoBook.Task == null &&
                                                   currentAutoBook.Task.RunId == run.Id &&
                                                   currentAutoBook.Task.ScenarioId == scenarioId);
        }

        public List<AutoBook> WorkingAutoBooks
        {
            get
            {
                var workingAutoBooks = new List<AutoBook>();
                foreach (AutoBook autoBook in _autoBookRepository.GetAll())
                {
                    IAutoBook autoBookInterface = GetInterface(autoBook);
                    try
                    {
                        AutoBookStatuses autoBookStatus = autoBookInterface.GetStatus();
                        if (AutoBook.IsWorkingStatus(autoBookStatus))
                        {
                            workingAutoBooks.Add(autoBook);
                        }
                    }
                    catch { };      // Ignore error checking status
                }
                return workingAutoBooks;
            }
        }

        /// <summary>
        /// Returns the first autobook >= configuration id required and has sufficient available storage
        /// </summary>
        /// <param name="autoBookInstanceConfiguration"></param>
        /// <param name="requiredStorageGB"></param>
        /// <param name="lockAutoBook"></param>
        /// <returns>AutoBooks successfully working</returns>
        public AutoBook GetFirstAdequateIdleAutoBook(AutoBookInstanceConfiguration autoBookInstanceConfiguration, double requiredStorageGB, bool lockAutoBook)
        {
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                            $"{nameof(GetFirstAdequateIdleAutoBook)} for : " +
                            $"autoBookInstanceConfiguration id ({autoBookInstanceConfiguration.Id.ToString()}) " +
                            $"type ({autoBookInstanceConfiguration.InstanceType}) " +
                            $"size ({autoBookInstanceConfiguration.StorageSizeGb.ToString()}) " +
                            $"cost ({autoBookInstanceConfiguration.Cost.ToString()}) " +
                            $"requiredStorageGB ({requiredStorageGB.ToString()}) "));

            AutoBook FirstFreeAutoBook = null;

            foreach (AutoBook autoBook in _autoBookRepository.GetAll().Where(x => !x.Locked && x.InstanceConfigurationId >= autoBookInstanceConfiguration.Id))
            {
                if (!String.IsNullOrEmpty(autoBook.Api))    //check we can use the interface, if it is still provisioning it doesn't yet know it's API url
                {
                    IAutoBook autoBookInterface = GetInterface(autoBook);
                    AutoBookStatuses status = autoBookInterface.GetStatus();
                    if (status == AutoBookStatuses.Idle)
                    {
                        GetAutoBookStorageInfoModel storageInfo = autoBookInterface.GetStorageInfo();
                        double availableStorageGB = (storageInfo == null ? AutoBookSettings.UnknownStorageGB : ConvertAvailableGBStringToDouble(storageInfo.Available));

                        if (requiredStorageGB == 0 || availableStorageGB >= requiredStorageGB || availableStorageGB == AutoBookSettings.UnknownStorageGB)
                        {
                            if (lockAutoBook)
                            {
                                autoBook.Locked = true;
                                _autoBookRepository.Update(autoBook);
                                _autoBookRepository.SaveChanges();
                            }
                            FirstFreeAutoBook = autoBook;
                            break;
                        }
                    }
                }
            }
            return FirstFreeAutoBook;
        }

        /// <summary>
        /// Returns interface to interact with AutoBook instance
        /// </summary>
        /// <param name="autoBook"></param>
        /// <returns></returns>
        public IAutoBook GetInterface(AutoBook autoBook) =>
            new AWSAutoBook(autoBook, _autoBookRepository, CreateAutoBookApi(autoBook));

        /// <summary>
        /// Returns the PAAutoBook (Provisioning API) for a specific AutoBook.Id. Originally then AutoBook.Id = AutoBookObject.Name but this has been changed to
        /// remove Name so that AutoBook.Id = AutoBookObject.Id although AutoBookObject.Id is shorter and needs padding.
        /// </summary>
        /// <param name="autoBookId"></param>
        /// <param name="autoBookObjects"></param>
        /// <returns></returns>
        private AWSPAAutoBook GetPAAutoBook(string autoBookId, List<AWSPAAutoBook> autoBookObjects)
        {
            string paAutoBookId = autoBookId.Replace("AutoBooks/", String.Empty); // Make sure that the RavenDB collection name is excluded from the Id
            return autoBookObjects.FirstOrDefault(ab => ab.Id == paAutoBookId);
        }

        /// <summary>
        /// Updates AutoBookSettings document
        /// </summary>
        private static void UpdateAutoBookSettings(IRepositoryFactory repositoryFactory, bool? locked, DateTime? autoProvisioningLastActive)
        {
            using var scope = repositoryFactory.BeginRepositoryScope();
            var autoBookSettingsRepository = scope.CreateRepository<IAutoBookSettingsRepository>();
            var autoBookSettings = autoBookSettingsRepository.Get();
            var settingsChanged = false;
            if (locked != null)
            {
                autoBookSettings.Locked = locked.Value;
                settingsChanged = true;
            }
            if (autoProvisioningLastActive != null)
            {
                autoBookSettings.AutoProvisioningLastActive = autoProvisioningLastActive.Value;
                settingsChanged = true;
            }

            if (settingsChanged)
            {
                autoBookSettingsRepository.AddOrUpdate(autoBookSettings);
                autoBookSettingsRepository.SaveChanges();
            }
        }

        /// <summary>
        /// Executes AutoBook provisioning. Restarts crashed instances (including stuck provisioning), ensures that we have sufficient instances running as
        /// per min/max instance limits, recycles old instances, ensures that instances are running correct versions, identifies orphans that can't be
        /// used (because they need to exist in the database and in AutoBooks API).
        /// </summary>
        public void ExecuteProvisionining()
        {
            // Reset locked flag if stuck
            if (_autoBookSettings.AutoProvisioning && _autoBookSettings.Locked)
            {
                if (_autoBookSettings.AutoProvisioningLastActive.AddMinutes(15) < DateTime.UtcNow)
                {
                    try
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, "Resetting AutoBook provisioning Locked property"));
                        UpdateAutoBookSettings(_repositoryFactory, false, null);
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, "Reset AutoBook provisioning Locked property"));
                    }
                    catch (Exception exception)
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, "Error resetting AutoBook provisioning Locked property", exception));
                    }
                }
            }

            if (_autoBookSettings.AutoProvisioning && !_autoBookSettings.Locked)
            {
                bool autoBookSettingsLocked = false;
                try
                {
                    // Prevent multiple threads simultaneously provisioning
                    using (MachineLock.Create("xggameplan.AWSAutoBooks.CheckProvisioning", new TimeSpan(0, 1, 0)))
                    {
                        int autoBookCount = this.Instances;
                        bool isActiveRuns = false;
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                            $"Checking AutoBook provisioning (Instances={autoBookCount})"));

                        // Lock AutoBook settings so that they can't be modified while provisioning is running
                        UpdateAutoBookSettings(_repositoryFactory, true, DateTime.UtcNow);
                        autoBookSettingsLocked = true;

                        // Check if active runs, need to limit activity. E.g. Deleting AutoBook would cause an issue if it's about to be used
                        using (var scope = _repositoryFactory.BeginRepositoryScope())
                        {
                            var runRepository = scope.CreateRepository<IRunRepository>();
                            isActiveRuns = runRepository.GetAllActive().Any();
                            if (isActiveRuns)
                            {
                                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0, "AutoBook provisioning has detected that runs are active, activity will be limited"));
                            }
                        }

                        // Create default AutoBook instance configurations if not present
                        try
                        {
                            _ = CreateDefaultInstanceConfigurations();
                        }
                        catch (Exception exception)
                        {
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, "Error creating default instance configurations", exception));
                        }

                        // Check AutoBook statuses
                        try
                        {
                            if (autoBookCount > 0)
                            {
                                CheckAutoBookStatuses();
                            }
                        }
                        catch (Exception exception)
                        {
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, "Error checking AutoBook statuses", exception));
                        }

                        // Restart crashed AutoBooks
                        try
                        {
                            if (autoBookCount > 0)
                            {
                                CheckAutoBookCrashes();
                            }
                        }
                        catch (Exception exception)
                        {
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, "Error checking AutoBook crashes", exception));
                        }

                        // Check orphaned AutoBook instances
                        try
                        {
                            CheckAutoBookOrphans(isActiveRuns);
                        }
                        catch (Exception exception)
                        {
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, "Error checking AutoBook orphans", exception));
                        }

                        // Unprovision AutoBooks that have reached max lifetime. If we need to have a mininum number then they'll be started below.
                        try
                        {
                            if (autoBookCount > 0)
                            {
                                CheckAutoBookMaxLifetime(isActiveRuns);
                            }
                        }
                        catch (Exception exception)
                        {
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, "Error checking AutoBook max lifetimes", exception));
                        }

                        // Check min number of AutoBook instances
                        try
                        {
                            CheckAutoBookMinInstances(isActiveRuns);
                        }
                        catch (Exception exception)
                        {
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, "Error checking AutoBook min instances", exception));
                        }

                        // Check max number of AutoBook instances
                        try
                        {
                            if (autoBookCount > 0)
                            {
                                CheckAutoBookMaxInstances(isActiveRuns);
                            }
                        }
                        catch (Exception exception)
                        {
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, "Error checking AutoBook max instances", exception));
                        }

                        // Ensure AutoBooks are running correct version
                        try
                        {
                            if (autoBookCount > 0)
                            {
                                CheckAutoBookVersions(isActiveRuns);
                            }
                        }
                        catch (Exception exception)
                        {
                            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, "Error checking AutoBook versions", exception));
                        }

                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                            $"Checked AutoBook provisioning (Instances={Instances}, Working Instances={WorkingAutoBooks.Count})"));
                    }
                }
                catch (MachineLockTimeoutException)   // Unable to get lock provisioning
                {
                    // Ignore
                }
                finally
                {
                    // Unlock AutoBook settings
                    if (autoBookSettingsLocked)
                    {
                        UpdateAutoBookSettings(_repositoryFactory, false, DateTime.UtcNow);
                    }
                }
            }
        }

        /// <summary>
        /// Validates before deleting AutoBook
        /// </summary>
        /// <param name="autoBook"></param>
        public void ValidateForDelete(AutoBook autoBook)
        {
            // Get AutoBook status
            IAutoBook autoBookInterface = GetInterface(autoBook);
            AutoBookStatuses status = autoBookInterface.GetStatus();

            // Check if status is OK to delete, might be busy
            if (!AutoBook.IsOKForDelete(status))
            {
                throw new Exception($"Cannot delete AutoBook because status is {status.ToString()}");
            }
            if (_autoBookSettings.Locked)
            {
                throw new Exception("Cannot delete while provisioning is active");
            }
        }

        public PAAutoBook GetPAAutoBook(AutoBook autoBook)
        {
            var paAutoBook = _autoBooksApi.Get(autoBook.Id);
            if (paAutoBook is null)
            {
                return null;
            }
            return new PAAutoBook()
            {
                Id = autoBook.Id,
                Provisioned = paAutoBook.Provisioned,
                Version = paAutoBook.Version
            };
        }

        /// <summary>
        /// Returns list of AutoBook instance configurations able to execute the run
        /// based on run criteria and storage required(GB) in ascending cost order
        /// </summary>
        /// <param name="run"></param>
        /// <returns></returns>
        public List<AutoBookInstanceConfiguration> GetInstanceConfigurationsAscByCost(Run run, List<AutoBookInstanceConfiguration> allAutoBookInstanceConfigurations,
                                                                            int salesAreasCount, int campaignsCount, int demographicsCount, int breaksCount)
        {
            var storageRequiredGB = GetRequiredStorageForBreakCountInGB(breaksCount);

            var autoBookInstanceConfigurations = new List<AutoBookInstanceConfiguration>();

            if (allAutoBookInstanceConfigurations.Count == 1)   //single instance
            {
                autoBookInstanceConfigurations.Add(allAutoBookInstanceConfigurations.First());
            }
            else
            {
                TimeSpan span = run.EndDate - run.StartDate;
                int daysCount = ((int)span.TotalDays) + 1;
                autoBookInstanceConfigurations.AddRange(allAutoBookInstanceConfigurations.Where(ic => ic.CanExecuteRun(daysCount, salesAreasCount, campaignsCount, demographicsCount, breaksCount)
                && ic.StorageSizeGb >= storageRequiredGB));
            }

            return autoBookInstanceConfigurations.OrderBy(ic => ic.Cost).ToList();
        }

        /// <summary>
        /// Checks AutoBook statuses, doesn't take any action but logs them for diagnostics.
        /// </summary>
        private void CheckAutoBookStatuses()
        {
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, "Checking AutoBook statuses"));

            // Get all AutoBooks in database
            List<AutoBook> autoBooks = _autoBookRepository.GetAll().ToList();

            // Get all AutoBooks in Provisioning API
            var paAutoBooks = _autoBooksApi.GetAll();

            // Check each AutoBook
            foreach (var autoBook in autoBooks)
            {
                try
                {
                    var paAutoBook = GetPAAutoBook(autoBook.Id, paAutoBooks);
                    if (paAutoBook == null)   // Not in Provisioning API
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                            $"AutoBook status: AutoBookID={autoBook.Id}, Time Created={autoBook.TimeCreated}, Status={"Unknown:Not in Provisioning API"}"));
                    }
                    else if (paAutoBook.Provisioned)        // Provisioned OK
                    {
                        IAutoBook autoBookInterface = GetInterface(autoBook);
                        AutoBookStatuses autoBookStatus = autoBookInterface.GetStatus();

                        // Get storage
                        string storage = "";
                        try
                        {
                            GetAutoBookStorageInfoModel storageInfo = autoBookInterface.GetStorageInfo();
                            storage =
                                $"Available={storageInfo.Available};Total={storageInfo.Total};Used={storageInfo.Used}";
                        }
                        catch (Exception exception)
                        {
                            storage = $"Error:{exception.Message}";
                        }
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                            $"AutoBook status: AutoBookID={autoBook.Id}, Time Created={autoBook.TimeCreated}, Status={autoBookStatus}, API={autoBook.Api}, Version={paAutoBook.Version}, Storage={storage}"));
                    }
                    else    // Provisioning
                    {
                        _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
                            $"AutoBook status: AutoBookID={autoBook.Id}, Time Created={autoBook.TimeCreated}, Status={AutoBookStatuses.Provisioning}"));
                    }
                }
                catch (Exception exception)
                {
                    _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForWarningMessage(0, 0,
                        $"AutoBook status: AutoBookID={autoBook.Id}, Time Created={autoBook.TimeCreated}, Status=Unknown, Error={exception.Message}"));
                }
            }
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, "Checked AutoBook statuses"));
        }

        public List<AutoBookInstanceConfiguration> CreateDefaultInstanceConfigurations()
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            // Get repositories
            var repositories = scope.CreateRepositories(
                typeof(IAutoBookInstanceConfigurationRepository)
            );
            var autoBookInstanceConfigurationRepository = repositories.Get<IAutoBookInstanceConfigurationRepository>();

            // Check if instance configurations exist
            var autoBookInstanceConfigurations = autoBookInstanceConfigurationRepository.GetAll();
            if (!autoBookInstanceConfigurations.Any())
            {
                autoBookInstanceConfigurations = GetDefaultAutoBookInstanceConfigurations();
                autoBookInstanceConfigurationRepository.Add(autoBookInstanceConfigurations);
                autoBookInstanceConfigurationRepository.SaveChanges();
            }
            return autoBookInstanceConfigurations;
        }

        private List<AutoBookInstanceConfiguration> GetDefaultAutoBookInstanceConfigurations()
        {
            List<AutoBookInstanceConfiguration> autoBookInstanceConfigurations = new List<AutoBookInstanceConfiguration>
            {
                new AutoBookInstanceConfiguration()
                {
                    Id = 4,
                    CloudProvider = CloudProviders.AWS,
                    Description = "t3.medium (50GB)",
                    InstanceType = "t3.medium",
                    StorageSizeGb = 50,
                    Cost = 24,
                    CriteriaList = new List<AutoBookInstanceConfigurationCriteria>()
                            { new AutoBookInstanceConfigurationCriteria() { MaxDays = 30, MaxSalesAreas = null, MaxDemographics = null, MaxCampaigns = null, MaxBreaks = null } },
                },

                new AutoBookInstanceConfiguration()
                {
                    Id = 6,
                    CloudProvider = CloudProviders.AWS,
                    Description = "c5.xlarge (50GB)",
                    InstanceType = "c5.xlarge",
                    StorageSizeGb = 50,
                    Cost = 36,
                    CriteriaList = new List<AutoBookInstanceConfigurationCriteria>()
                            { new AutoBookInstanceConfigurationCriteria() { MaxDays = 97, MaxSalesAreas = null, MaxDemographics = null, MaxCampaigns = null, MaxBreaks = null } },
                },

                new AutoBookInstanceConfiguration()
                {
                    Id = 7,
                    CloudProvider = CloudProviders.AWS,
                    Description = "c5.2xlarge (50GB)",
                    InstanceType = "c5.2xlarge",
                    StorageSizeGb = 50,
                    Cost = 54,
                    CriteriaList = new List<AutoBookInstanceConfigurationCriteria>()
                            { new AutoBookInstanceConfigurationCriteria() { MaxDays = 190, MaxSalesAreas = null, MaxDemographics = null, MaxCampaigns = null, MaxBreaks = null } },
                }
            };
            return autoBookInstanceConfigurations;
        }

        public double GetRequiredStorageForBreakCountInGB(int breaksCount)
        {
            return AutoBookSettings.StorageGBPerMillionBreaks * breaksCount / 1000000;
        }

        private static double ConvertAvailableGBStringToDouble(string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                value = value.ToUpper().Replace("GB", "");
                string[] elements = value.Split(' ');
                if (Double.TryParse(elements[0], out double result))   // Typically "n/a" if not known
                {
                    return result;
                }
            }
            return AutoBookSettings.UnknownStorageGB;      // Unknown - static value of '-1'
        }

        public List<AutoBook> WaitForProvisioned()
        {
            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var autoBookRepository = scope.CreateRepository<IAutoBookRepository>();

                // Get provisioning AutoBooks
                var autoBooks = autoBookRepository.GetAll();
                var autoBooksProvisioning = autoBooks.Where(ab => ab.Status == AutoBookStatuses.Provisioning).ToList();
                if (autoBooksProvisioning.Any())
                {
                    var autoBooksWorking = WaitForAutoBooksWorking(autoBooksProvisioning, TimeSpan.FromSeconds(AutoBookSettings.CreateAutoBookTimeoutSeconds));
                    return autoBooksWorking;
                }
            }
            return new List<AutoBook>();
        }

        /// <summary>
        /// Restarts non-working AutoBooks
        /// </summary>
        public List<AutoBook> RestartNonWorking()
        {
            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                var autoBookRepository = scope.CreateRepository<IAutoBookRepository>();

                // Check AutoBook status to identify non-working instances
                var autoBooks = autoBookRepository.GetAll();
                var autoBooksNotWorking = new List<AutoBook>();
                foreach (var autoBook in autoBooks)
                {
                    IAutoBook autoBookInterface = GetInterface(autoBook);
                    try
                    {
                        AutoBookStatuses? autoBookStatus = autoBookInterface.GetStatus();
                        if (!AutoBook.IsWorkingStatus(autoBookStatus.Value))
                        {
                            autoBooksNotWorking.Add(autoBook);
                        }
                    }
                    catch
                    {
                        if (autoBook.Status != AutoBookStatuses.Provisioning)   // Broken
                        {
                            autoBooksNotWorking.Add(autoBook);
                        }
                    }
                }

                // Restart non-working Autobooks, wait until they are working or timeout
                if (autoBooksNotWorking.Any())
                {
                    var autoBooksRestarting = new List<AutoBook>();
                    foreach (var autoBook in autoBooksNotWorking)
                    {
                        try
                        {
                            Restart(autoBook);
                            autoBooksRestarting.Add(autoBook);
                        }
                        catch { };
                    }
                    var autoBooksWorking = WaitForAutoBooksWorking(autoBooksRestarting, TimeSpan.FromSeconds(AutoBookSettings.RestartAutoBookTimeoutSeconds));
                    return autoBooksWorking;
                }
            }
            return new List<AutoBook>();
        }

        protected virtual IAutoBookAPI CreateAutoBookApi(AutoBook autoBook)
        {
            return new AWSAutoBookAPI(autoBook, _autoBookSettings, _accessToken);
        }

        public string CreateAutoBookRequestRun(AutoBookRequestModel autoBookRequestModel)
        {
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0,
               $"AutoBookRequest : " +
               $"ApiVersion: {autoBookRequestModel.version} " +
               $"BinariesVersion: {autoBookRequestModel.binariesVersion} " +
               $"InstanceType: {autoBookRequestModel.instanceType} " +
               $"MaxInstances: {autoBookRequestModel.maxInstances} " +
               $"mock: {autoBookRequestModel.mock} " +
               $"RespondTo: {autoBookRequestModel.respondTo} " +
               $"RunId: {autoBookRequestModel.runId} " +
               $"ScenarioId: {autoBookRequestModel.scenarioId} " +
               $"StorageGB: {autoBookRequestModel.storageSizeGB} "
               ));

            return _autoBooksApi.AutoBookRequestRun(autoBookRequestModel);
        }
    }
}
