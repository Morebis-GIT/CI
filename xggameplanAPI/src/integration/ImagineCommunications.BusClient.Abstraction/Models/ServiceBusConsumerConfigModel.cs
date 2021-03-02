using System;
using System.Collections.Generic;

namespace ImagineCommunications.BusClient.Abstraction.Models
{
    public class ServiceBusConsumerConfigModel
    {
        public Type DefaultConsumerType { get; set; }

        public List<Consumer> Consumers { get; set; }

        public class Consumer
        {
            public Type EntityType { get; set; }

            public Type ConsumerType { get; set; }

            public Type ErrorConsumerType { get; set; }

            public string QueueName { get; set; }

            public string ExchangeName { get; set; }

            public string EntityName { get; set; }
        }
    }
}
