using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.AutoBooks.AWS;
using xggameplan.Model;

namespace xggameplan.TestEnv.AutoBook
{
    public class AWSAutoBooksAPIStub : IAutoBooksAPI<AWSPAAutoBook, AWSPACreateAutoBook>, IAutoBooksTestHandler
    {
        protected class AutoBookStubItem : AWSPAAutoBook
        {
            public AutoBookStatuses Status { get; set; }
        }

        private readonly AutoBookSettings _autoBookSettings;
        private readonly IAutoBookRepository _autoBookRepository;
        private readonly IAutoBookInstanceConfigurationRepository _autoBookInstanceConfigurationRepository;

        private readonly List<AutoBookStubItem> _autoBooks = new List<AutoBookStubItem>();

        public AWSAutoBooksAPIStub(AutoBookSettings autoBookSettings, IAutoBookRepository autoBookRepository,
            IAutoBookInstanceConfigurationRepository autoBookInstanceConfigurationRepository)
        {
            _autoBookSettings = autoBookSettings;
            _autoBookRepository = autoBookRepository;
            _autoBookInstanceConfigurationRepository = autoBookInstanceConfigurationRepository;

            CreateInitialAutoBookCollection();
        }

        public AWSPAAutoBook Create(AWSPACreateAutoBook autoBook)
        {
            var item = new AutoBookStubItem
            {
                Id = $"AutoBooks/{Guid.NewGuid().ToString()}",
                StorageSizeGb = autoBook.StorageSizeGb.ToString(CultureInfo.InvariantCulture),
                InstanceType = autoBook.InstanceType,
                Provisioned = true,
                Version = autoBook.Version,
                Url = "http://localhost",
                Status = AutoBookStatuses.Idle
            };
            item.Name = $"{nameof(AWSPAAutoBook.Name)}-{item.Id}";
            _autoBooks.Add(item);

            return item;
        }

        public AWSPAAutoBook Get(string autoBookId)
        {
            return InternalFind(autoBookId);
        }

        public List<AWSPAAutoBook> GetAll()
        {
            return _autoBooks.Cast<AWSPAAutoBook>().ToList();
        }

        public void Update(AWSPAAutoBook autoBook)
        {
            if (autoBook == null)
            {
                throw new ArgumentNullException(nameof(autoBook));
            }

            var item = _autoBooks.FirstOrDefault(ab => ab.Id == autoBook.Id);
            if (item != null)
            {
                item.InstanceType = autoBook.InstanceType;
                item.Name = autoBook.Name;
                item.StorageSizeGb = autoBook.StorageSizeGb;
                item.Url = autoBook.Url;
                item.Version = autoBook.Version;
            }
        }

        public void Delete(string autoBookId)
        {
            var idx = _autoBooks.FindIndex(ab => ab.Id == autoBookId);
            if (idx != -1)
            {
                _autoBooks.RemoveAt(idx);
            }
        }

        public void Restart(string autoBookId)
        {
            var autoBook = InternalGet(autoBookId);
            if (autoBook != null)
            {
                autoBook.Status = AutoBookStatuses.Idle;
                autoBook.Provisioned = true;
            }
        }

        protected AutoBookStubItem InternalFind(string autoBookId)
        {
            return _autoBooks.FirstOrDefault(ab => ab.Id == autoBookId);
        }

        protected AutoBookStubItem InternalGet(string autoBookId)
        {
            return InternalFind(autoBookId) ?? throw new Exception($"Autobook '{autoBookId}' has not been found.");
        }

        private void CreateInitialAutoBookCollection()
        {
            var autobookConfigurations = _autoBookInstanceConfigurationRepository.GetAll();
            _autoBooks.AddRange(_autoBookRepository.GetAll().Select(ab =>
            {
                var autobookConfiguration = autobookConfigurations.FirstOrDefault();
                return new AutoBookStubItem
                {
                    Id = ab.Id,
                    Name = $"{nameof(AWSPAAutoBook.Name)}-{ab.Id}",
                    Url = ab.Api,
                    Provisioned = true,
                    InstanceType = autobookConfiguration?.InstanceType,
                    Version = _autoBookSettings.ApplicationVersion,
                    StorageSizeGb = autobookConfiguration?.StorageSizeGb.ToString(CultureInfo.InvariantCulture),
                    Status = AutoBookStatuses.Idle
                };
            }));
        }

        public AutoBookStatuses GetStatus(string autoBookId) => InternalGet(autoBookId).Status;

        public string GetVersion(string autoBookId) => InternalGet(autoBookId).Version;

        public void ChangeStatus(string autoBookId, AutoBookStatuses newStatus)
        {
            var autoBook = InternalGet(autoBookId);
            autoBook.Status = newStatus;
        }

        public string AutoBookRequestRun(AutoBookRequestModel autoBookRequest)
        {
            return string.Empty;
        }
    }
}
