using TechTalk.SpecFlow.Assist;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Assist
{
    public class BufferValueComparer : IValueComparer
    {
        private readonly BufferValueRetriever _bufferValueRetriever;

        public BufferValueComparer(BufferValueRetriever bufferValueRetriever)
        {
            _bufferValueRetriever = bufferValueRetriever;
        }

        public bool CanCompare(object actualValue) => actualValue is byte[];

        public bool Compare(string expectedValue, object actualValue)
        {
            if (!_bufferValueRetriever.IsValidBuffer(expectedValue))
            {
                return false;
            }

            var buffer = _bufferValueRetriever.GetValueOrDefault(expectedValue);
            return actualValue != null && CompareBuffers((byte[])actualValue, buffer);
        }

        private static bool CompareBuffers(byte[] actualValue, byte[] expectedValue)
        {
            if (actualValue.Length != expectedValue.Length)
            {
                return false;
            }

            for (int i = 0; i < actualValue.Length; i++)
            {
                if (actualValue[i] != expectedValue[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
