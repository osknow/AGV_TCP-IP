using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGV_TcpIp_ConsoleApp
{
    public enum EnumErrorType: UInt16
    {
        Unknown = 0,
        SystemAlarm = 1,
        EntityAlarm = 2,
        MachineAlarm = 3,
        SymbolicPointAlarm = 4,
        WaypointAlarm = 5
    }
    public enum  EnumSource: byte
    {
        Unknown = 0,
        FleetControl = 1,
        Machine = 2,
        Supervisor = 3,
        Component = 4,
        MES = 5
    }
    public class ID_310
    {
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
    }

    public class ID_309 {

        public int Id { get; set; }
        public string Machine { get; set; }
        public string Name { get; set; }
        public UInt16 NumberId { get; set; }
        public UInt16 Value { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
    public class ID_349
    {
        public UInt32 Id { get; set; }
        public UInt16 NameStringLength { get; set; }
        public string Name { get; set; }
        public EnumErrorType ErrorType { get; set; }
        public UInt32 EntityID { get; set; }
        public EnumSource Source { get; set; }
        public byte Level { get; set; }
        public UInt16 Value { get; set; }
        public Int16 Priority { get; set; }
    }
}
