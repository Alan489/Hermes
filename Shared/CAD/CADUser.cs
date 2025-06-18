using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hermes.Shared.CAD
{
    public class CADUser
    {
        public int id {  get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public int access_level { get; set; }
        public int timeout { get; set; }
        public string preferences { get; set; }
        public bool change_password { get; set; }
        public bool locked_out { get; set; }
        public int failed_login_count { get; set; }
        public DateTime last_login_time { get; set; }

    }
}
