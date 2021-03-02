using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using CsvHelper;
using xggameplan.CSVImporter;

// CMF Added

namespace xggameplan.Repository.CSV
{
    /// <summary>
    /// Base class for import repository.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public abstract class CSVImportRepositoryBase<T1> : IDisposable     // T1=Item
    {
        protected CSVImportSettings _settings;
        protected long _currentRecordNo = -1;
        protected CsvReader _reader = null;
        protected StreamReader _streamReader = null;

        public void Dispose()
        {
            if (_streamReader != null)
            {
                _streamReader.Close();
                _streamReader = null;
            }
            if (_reader != null)
            {
                _reader = null;
            }
        }

        /// <summary>
        /// Resets to first record
        /// </summary>
        protected void ResetFirst()
        {
            // Clear
            Dispose();

            _streamReader = new StreamReader(_settings.FileName);
            _reader = new CsvReader(_streamReader);
            _reader.Configuration.HasHeaderRecord = _settings.IsCSV;
            _reader.Configuration.RegisterClassMap(_settings.MapType);
            _currentRecordNo = -1;  // So that first read is record 0                
        }

        public virtual IEnumerable<T1> GetAll()
        {
            var items = new List<T1>();

            //XGG162 Disabled
            using (var reader = new CsvReader(new StreamReader(_settings.FileName)))
            {
                reader.Configuration.HasHeaderRecord = _settings.IsCSV;
                reader.Configuration.RegisterClassMap(_settings.MapType);
                while (reader.Read())
                {
                    // Read
                    var item = reader.GetRecord<T1>();
                    if (item != null)
                    {
                        items.Add(item);
                    }
                }
            }

            return items;

            /* Causes 'Out of Memory' exception due to loading all data in to memory
            using (var b = File.OpenText(_file))
            {
                var csv = new CsvReader(b);
                csv.Configuration.RegisterClassMap<FailureMap>();
                var failures = csv.GetRecords<FailureImport>().ToList().GroupBy(x => new { x.CampaignNumber, x.FailureType })
                            .Select(y => new FailureImportSummary
                            {
                                CampaignNumber = y.Key.CampaignNumber,
                                FailureType = y.Key.FailureType,
                                NumberOfFailures = y.Sum(y1 => y1.NumberOfFailures)
                            });
                return failures;
            }
            */
        }

        public virtual IEnumerable<T1> GetList(Expression<Func<T1, bool>> expression)
        {
            var items = new List<T1>();
            long firstRecordNo = 0;
            long batchSize = 2;
            bool done = false;

            do
            {
                var spotImportRange = GetRange(firstRecordNo, firstRecordNo + batchSize - 1).ToList();
                done = !spotImportRange.Any();
                if (spotImportRange.Any())
                {
                    IQueryable<T1> query = spotImportRange.AsQueryable<T1>();
                    var results = query.Where(expression);
                    foreach (var item in results)
                    {
                        items.Add(item);
                    }
                    firstRecordNo += batchSize;  // Advance to next batch
                }
            } while (!done);

            return items;
        }

        public virtual IEnumerable<T1> GetRange(long firstRecordNo, long lastRecordNo)
        {
            var items = new List<T1>();

            if (firstRecordNo < 0)
            {
                throw new ArgumentOutOfRangeException("Cannot most to record no before 0");
            }
            else if (lastRecordNo < firstRecordNo)
            {
                throw new ArgumentException("Last record no cannot be less than first record no");
            }

            // Move to first record
            if (MoveToRecordNo(firstRecordNo))   // Moved OK
            {
                bool done = false;
                do
                {
                    // Read record
                    var item = _reader.GetRecord<T1>();
                    items.Add(item);

                    // Advance to next record if not there
                    if (_currentRecordNo < lastRecordNo)
                    {
                        if (_reader.Read())
                        {
                            _currentRecordNo++;
                        }
                        else     // At last record
                        {
                            done = true;
                        }
                    }
                    else if (_currentRecordNo >= lastRecordNo)      // Read last record
                    {
                        done = true;
                    }
                } while (!done);
            }
            return items;
        }

        /// <summary>
        /// Move to record no
        /// </summary>
        /// <param name="recordNo"></param>
        /// <returns></returns>
        protected bool MoveToRecordNo(long recordNo)
        {
            bool moved = false;
            if (_reader == null)
            {
                ResetFirst();
            }

            // Reset if we've gone past the record that we need, necessary as this is a forward only
            if (_currentRecordNo > recordNo)
            {
                ResetFirst();
            }

            // Move to required record
            bool done = false;
            while (_currentRecordNo < recordNo && done == false)
            {
                if (_reader.Read())   // Moved to next record
                {
                    _currentRecordNo++;
                }
                else    // Insufficient records
                {
                    done = true;
                }

                // Check if we're at record
                if (recordNo == _currentRecordNo)
                {
                    moved = true;
                    done = true;
                }
            }
            return moved;
        }
    }
}
