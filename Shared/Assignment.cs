using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hermes.Shared
{
    public class Assignment
    {
        public int uid { get; set; }
        public string unit { get; set; }
        public DateTime timeout { get; set; }
        public string address { get; set; }
        public string type { get; set; }
        public string desc { get; set; }
        public string personnel { get; set; }
        public DateTime? arrival_time { get; set; }
        public DateTime? transport_time { get; set; }
        public DateTime? transportdone_time { get; set; }
        public DateTime? cleared_time { get; set; }
        public bool locked { get; set; }
        public string location_unit { get; set; }
        public string? lat {  get; set; }
        public string? lon { get; set; }

        public bool DataEquals(Assignment other)
        {
            if (address != other.address ||
                type != other.type ||
                desc != other.desc
                )
                return false;
            return true;
        }
    }
}
