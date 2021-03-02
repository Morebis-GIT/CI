using System;
using System.Xml.Serialization;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using xggameplan.common.Extensions;

namespace xggameplan.Model.AutoGen
{
    public class AgProcessRange
    {
        private RunStepEnum _runStep;
        private DateTime _startDate;
        private DateTime _endDate;

        private TimeSpan _startTime;
        private TimeSpan _endTime;

        /// <summary>
        /// Step RunId
        /// </summary>
        [XmlElement(ElementName = "param_date.aper_no")]
        public int RunId { get; set; }

        /// <summary>
        /// Step Index
        /// </summary>
        [XmlElement(ElementName = "param_date.run_step")]
        public int RunStep
        {
            get => (int)_runStep;
            set => _runStep = (RunStepEnum)value;
        }

        /// <summary>
        /// Step Start Date
        /// </summary>
        [XmlElement(ElementName = "param_date.stt_date")]
        public string StartDate
        {
            get => _startDate.ToString("yyyyMMdd");
            set
            {
                if (!DateTime.TryParse(value, out _startDate))
                {
                    _startDate = DateTime.MinValue;
                }
            }
        }

        /// <summary>
        /// Step End Date
        /// </summary>
        [XmlElement(ElementName = "param_date.end_date")]
        public string EndDate
        {
            get => _endDate.ToString("yyyyMMdd");
            set
            {
                if (!DateTime.TryParse(value, out _endDate))
                {
                    _endDate = DateTime.MinValue;
                }
            }
        }

        /// <summary>
        /// Step Start Time
        /// </summary>
        [XmlElement(ElementName = "param_date.stt_time")]
        public string StartTime
        {
            get => _startTime.ToString("hhmmss");
            set
            {
                if (!TimeSpan.TryParse(value, out _startTime))
                {
                    _startTime = TimeSpan.MinValue;
                }
            }
        }

        /// <summary>
        /// Step End Time
        /// </summary>
        [XmlElement(ElementName = "param_date.end_time")]
        public string EndTime
        {
            get => _endTime.ToString("hhmmss");
            set
            {
                if (!TimeSpan.TryParse(value, out _endTime))
                {
                    _endTime = TimeSpan.MinValue;
                }
            }
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public AgProcessRange() { }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="runStep"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public AgProcessRange(int runId, RunStepEnum runStep, DateTime startDate, DateTime endDate, TimeSpan startTime, TimeSpan endTime)
        {
            RunId = runId;
            _runStep = runStep;
            _startDate = startDate;
            _endDate = endDate;
            _startTime = startTime;
            _endTime = endTime;
        }

        /// <summary>
        /// Create AgProcessRange Method
        /// </summary>
        /// <param name="run"></param>
        /// <param name="runStep"></param>
        /// <returns></returns>
        public static AgProcessRange Create(Run run, RunStepEnum runStep)
        {
            DateRange range;

            switch (runStep)
            {
                case RunStepEnum.Smooth:
                    range = run.SmoothDateRange;
                    break;
                case RunStepEnum.ISR:
                    range = run.ISRDateRange;
                    break;
                case RunStepEnum.RightSizer:
                    range = run.RightSizerDateRange;
                    break;
                case RunStepEnum.Optimiser:
                    range = run.OptimisationDateRange;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(runStep), runStep, null);
            }

            ValidateRange(range, runStep);

            return new AgProcessRange(run.CustomId, runStep, range.Start, range.End, run.StartTime, run.EndTime);
        }

        /// <summary>
        /// Validation Method
        /// </summary>
        /// <param name="range"></param>
        /// <param name="runStep"></param>
        private static void ValidateRange(DateRange range, RunStepEnum runStep)
        {
            if (range is null)
            {
                throw new Exception($"{runStep.GetDescription()} process range is empty, while process is enabled");
            }
        }
    }
}
