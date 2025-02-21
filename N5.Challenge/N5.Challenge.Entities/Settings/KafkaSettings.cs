using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace N5.Challenge.Entities.Settings
{
    public class KafkaSettings
    {
        public ProducerSettings ProducerSettings { get; set; }
        public ConsumerSettings ConsumerSettings { get; set; }
        public string PermissionsTopic { get; set; }
    }

    public class ProducerSettings
    {
        public string BootstrapServers { get; set; }
    }

    public class ConsumerSettings
    {
        public string BootstrapServers { get; set; }
        public string GroupId { get; set; }
    }
}