using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGV_TcpIp_ConsoleApp.Mapper
{
    public class Mappers
    {
        public static ID_310 dataID_310_toDto ( ID_310 dataToDto)
        {
            return new ID_310
            {
                MachineID = dataToDto.MachineID,
                MachineName = dataToDto.MachineName,
                X = dataToDto.X,
                Y = dataToDto.Y,
                H = dataToDto.H,
                Poziom = dataToDto.Poziom,
                PositionConfidence = dataToDto.PositionConfidence,
                SpeedNavigationPoint = dataToDto.SpeedNavigationPoint,
                State = dataToDto.State,
                BatteryLeve = dataToDto.BatteryLeve,
                AutoOrManual = dataToDto.AutoOrManual,
                PositionInitialized = dataToDto.PositionInitialized,
                LastSymbolPoint = dataToDto.LastSymbolPoint,
                MachineAtLastSymbolPoint = dataToDto.MachineAtLastSymbolPoint,
                TargetSymbolPoint = dataToDto.TargetSymbolPoint,
                MachineAtTarget = dataToDto.MachineAtTarget,
                Operational = dataToDto.Operational,
                InProduction = dataToDto.InProduction,
                LoadStatus = dataToDto.LoadStatus,
                BatteryVoltage = dataToDto.BatteryVoltage,
                ChargingStatus = dataToDto.ChargingStatus,
                DistanceToTarget = dataToDto.DistanceToTarget,
                CurrentDriveThroughPoint = dataToDto.CurrentDriveThroughPoint,
                NextLeveChangePointId = dataToDto.NextLeveChangePointId,
                DistanceToNextLeveelChange = dataToDto.DistanceToNextLeveelChange,
                LastSymbolPointDrivenOver = dataToDto.LastSymbolPointDrivenOver,
                UpdateTime = dataToDto.UpdateTime

            };
        }
    }
}
