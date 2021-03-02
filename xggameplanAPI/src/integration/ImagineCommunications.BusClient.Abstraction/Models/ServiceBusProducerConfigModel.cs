using System;
using System.Collections.Generic;

namespace ImagineCommunications.BusClient.Abstraction.Models
{
    public class ServiceBusProducerConfigModel
    {
        public List<Producer> Producers { get; set; }

        public class Producer
        {
            public Type EntityType { get; set; }

            public string QueueName { get; set; }

            public string ExchangeName { get; set; }
        }
    }
}
