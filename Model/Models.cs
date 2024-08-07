using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGV_TcpIp_ConsoleApp.Model
{

    class Models
    {
    }
    public class SendTask_pozmda02_body
        {
            public string Name { get; set; }
            public string Details { get; set; }
            public string MachineNumber { get; set; }
        }
    public class ReadTask_pozmda02_body
    {
        public int id { get; set; }
        public string machineNumber { get; set; }
        public string machineGroup { get; set; }
        public string machineStatus { get; set; }
        public string name { get; set; }
        public string details { get; set; }
        public int type { get; set; }
        public int status { get; set; }
        public string statusText { get; set; }
        public int imageCounter { get; set; }
        public int priority { get; set; }
        public bool helpNeeded { get; set; }
        public DateTime addedTime { get; set; }
        public DateTime joinedTime { get; set; }
        public DateTime loginTime { get; set; }
        public DateTime endedTime { get; set; }
    }
}

