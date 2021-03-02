using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace xggameplan.Updates
{
    /// <summary>
    /// Abstract class for update
    /// </summary>
    public abstract class UpdateBase
    {
        protected List<IUpdateStep> _updateSteps;
        protected string _updatesFolder;          // Files for this update (E.g. Roll back data) 

        public virtual Guid Id
        {
            get { return Guid.Empty; }
        }

        public virtual string Name
        {
            get { return ""; }
        }

        public virtual string DatabaseVersion
        {
            get { return ""; }
        }

        /// <summary>
        /// Apply update
        /// </summary>
        public virtual void Apply()
        {
            ValidateBeforeRun();

            // Delete old roll back files
            DeleteOldRollbackFiles();

            List<IUpdateStep> updateStepsApplied = new List<IUpdateStep>();            
            foreach (IUpdateStep updateStep in _updateSteps.OrderBy(us => us.Sequence))
            {                
                try
                {
                    // Apply step update
                    updateStep.Apply();

                    // Add to steps applied
                    updateStepsApplied.Add(updateStep);
                }
                catch (System.Exception exception)
                {
                    // Try and roll back the step
                    try
                    {
                        if (updateStep.SupportsRollback)
                        {
                            updateStep.RollBack();
                        }
                    }
                    catch { };

                    // Roll back all update steps that were applied starting from last
                    foreach(IUpdateStep updateStepToRollBack in updateStepsApplied.Where(us => us.SupportsRollback).OrderByDescending(us => us.Sequence))
                    {
                        try
                        {
                            updateStepToRollBack.RollBack();
                        }
                        catch { };                        
                    }
                    throw new Exception(string.Format("Error applying update step {0} for update {1}", updateStep.Name, this.Name), exception);
                }            
            }
        }    

        /// <summary>
        /// Roll back update. It is assumed that each step is independent of others.
        /// </summary>
        public virtual void RollBack()
        {
            List<Exception> rollBackExceptions = new List<Exception>();

            // Try and roll back each step, record any failed steps
            foreach (IUpdateStep updateStep in _updateSteps.OrderByDescending(us => us.Sequence))
            {
                if (updateStep.SupportsRollback)
                {
                    try
                    {
                        updateStep.RollBack();
                    }
                    catch(System.Exception rollbackException)
                    {
                        Exception exception = new Exception(string.Format("Error rolling back update step {0} for update {1}", updateStep.Name, this.Name), rollbackException);
                        rollBackExceptions.Add(exception);                        
                    }
                }
            }

            // Throw any exceptions
            if (rollBackExceptions.Count == 1)
            {
                throw rollBackExceptions.First();
            }
            else if (rollBackExceptions.Count > 1)
            {
                throw new AggregateException(rollBackExceptions);
            }
        }

        public virtual bool SupportsRollback
        {
            get { return _updateSteps.Where(us => us.SupportsRollback).Any(); }
        }

        /// <summary>
        /// Deletes old rollback files if they exist
        /// </summary>
        protected void DeleteOldRollbackFiles()
        {
            string rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            if (Directory.Exists(rollBackFolder))
            {
                foreach(string file in Directory.GetFiles(rollBackFolder))
                {
                    File.Delete(file);
                }
                foreach(string folder in Directory.GetDirectories(rollBackFolder))
                {
                    Directory.Delete(folder, true);
                }
            }            
        }

        private void ValidateBeforeRun()
        {
            _ = UpdateValidator.ValidateUpdateFolderPath(_updatesFolder, throwOnInvalid: true);
        }
    }
}
