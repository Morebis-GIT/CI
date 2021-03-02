using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ImagineCommunications.Gameplan.Integration.Handlers.Common
{
    public static class Extensions
    {
        public static IEnumerable<T> GetBatches<T>(this IEnumerable<T> data, int batchSize, int pagesToSkip)
        {
            var dataToProcess = data
                .Skip(batchSize * pagesToSkip).Take(batchSize);
            return dataToProcess;
        }

        public static void EnumerateBatches<T>(this IEnumerable<T> enumerable, int batchSize, Action<IEnumerator<T>> batchAction)
        {
            if (enumerable is null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            if (batchSize <= 0)
            {
                throw new ArgumentException("Batch size should be greater zero.", nameof(batchSize));
            }

            if (batchAction is null)
            {
                throw new ArgumentNullException(nameof(batchAction));
            }

            using (var enumerator = enumerable.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    using (var batchEnumerator = new BatchEnumerator<T>(enumerator, batchSize))
                    {
                        batchAction(batchEnumerator);
                    }
                }
            }
        }

        public static string GenerateBreakExternalRef(this string externalRef, int salesAreaCustomId, DateTime scheduleDate)
        {
            var dateString = scheduleDate.ToUniversalTime().ToString("yyMMddHH");
            return $"{externalRef}-{salesAreaCustomId}-{dateString}";
        }

        private class BatchEnumerator<T> : IEnumerator<T>
        {
            private readonly IEnumerator<T> _enumerator;
            private readonly int _batchSize;
            private int _idx;

            public BatchEnumerator(IEnumerator<T> enumerator, int batchSize)
            {
                _enumerator = enumerator;
                _batchSize = batchSize;
            }

            public bool MoveNext()
            {
                if (_idx == 0)
                {
                    _idx++;
                    return true;
                }

                if (_idx < _batchSize)
                {
                    var res = _enumerator.MoveNext();
                    if (res)
                    {
                        _idx++;
                    }

                    return res;
                }

                return false;
            }

            public void Reset() => throw new NotSupportedException();

            public T Current => _enumerator.Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }
    }
}
