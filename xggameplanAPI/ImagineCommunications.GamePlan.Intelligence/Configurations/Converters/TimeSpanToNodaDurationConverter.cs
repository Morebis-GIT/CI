using System;
using AutoMapper;
using NodaTime;

namespace ImagineCommunications.GamePlan.Intelligence.Configurations.Converters
{
    public class TimeSpanToNodaDurationConverter : ITypeConverter<TimeSpan, Duration>
    {
        public Duration Convert(TimeSpan source, Duration destination, ResolutionContext context) => Duration.FromTimeSpan(source);
    }
}
