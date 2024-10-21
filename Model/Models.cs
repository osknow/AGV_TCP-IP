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
    public class AGV_Machine
    {
        public int Id { get; set; }
        public int MachineID { get; set; }
        public string MachineName { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double H { get; set; }
        public int Poziom { get; set; }
        public byte PositionConfidence { get; set; }
        public double SpeedNavigationPoint { get; set; }
        public byte State { get; set; }
        public double BatteryLeve { get; set; }
        public byte AutoOrManual { get; set; }
        public byte PositionInitialized { get; set; }
        public Int32 LastSymbolPoint { get; set; }
        public byte MachineAtLastSymbolPoint { get; set; }
        public Int32 TargetSymbolPoint { get; set; }
        public byte MachineAtTarget { get; set; }
        public byte Operational { get; set; }
        public byte InProduction { get; set; }
        public byte LoadStatus { get; set; }
        public double BatteryVoltage { get; set; }
        public byte ChargingStatus { get; set; }
        public float DistanceToTarget { get; set; }
        public Int32 CurrentDriveThroughPoint { get; set; }
        public Int32 NextLeveChangePointId { get; set; }
        public float DistanceToNextLeveelChange { get; set; }
        public Int32 LastSymbolPointDrivenOver { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime AlarmOccuredTime { get; set; }
        public bool AlarmSended { get; set; }
        public bool AGV_SerwviceWork { get; set; }
    }
}

