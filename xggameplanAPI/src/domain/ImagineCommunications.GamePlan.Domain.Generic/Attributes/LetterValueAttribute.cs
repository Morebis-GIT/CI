using System;

namespace ImagineCommunications.GamePlan.Domain.Generic.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class LetterValueAttribute : Attribute
    {
        public LetterValueAttribute(char value) => Value = value;

        public char Value { get; }
    }
}
