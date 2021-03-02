using System;

namespace xggameplan.common.BackgroundJobs
{
    public class BackgroundJobParameter<T> : IBackgroundJobParameter
    {
        private readonly Lazy<T> _value;

        public BackgroundJobParameter(Lazy<T> value)
        {
            _value = value;
        }

        public BackgroundJobParameter(T value) : this(new Lazy<T>(() => value))
        {
        }

        public BackgroundJobParameter(string name, Lazy<T> value) : this(value)
        {
            Name = name;
        }

        public BackgroundJobParameter(string name, T value) : this(name, new Lazy<T>(() => value))
        {
            Name = name;
        }

        public string Name { get; }

        public T Value => _value.Value;

        public Type Type => typeof(T);

        object IBackgroundJobParameter.Value => Value;
    }
}
