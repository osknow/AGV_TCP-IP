using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AGV_TcpIp_ConsoleApp.Model;
using AGV_TcpIp_ConsoleApp.SubPrograms;

namespace AGV_TcpIp_ConsoleApp.SubProgramLogic
{
    public  class SendTask_Logic
    {
        // Zmienne wykrycia przeszkód na trasie AGV
        static bool agv_1_ObstacleDetected = false;
        static bool agv_2_ObstacleDetected = false;
        static bool agv_3_ObstacleDetected = false;
        //
        static bool agv_1_loadSensorError = false;
        static bool agv_2_loadSensorError = false;
        static bool agv_3_loadSensorError = false;
        //
        static bool taskObstacleDetectionSended = false;
        //
        public static async Task UpdateTasks_ForElectricals(List<ID_310> agvMachineStatus)
        {

            try
            {
                bool state = false;
                bool stateReset = false;
                DateTime nowtime = DateTime.Now;
                using (HttpClient client = new HttpClient())
                {
                    List<AGV_Machine> listMachineAGV = await GetMachinesAGV_pozmda02.Get();
                    //
                    List<ReadTask_pozmda02_body> tasksPozmda02 = await ReadTask_pozmda02.Get();
                    //
                    //Sprawdzenie statusu maszyn AGV
                    //
                    if (!(tasksPozmda02 == null && agvMachineStatus.Count == 0))
                    {
                        foreach (var machine in agvMachineStatus)
                        {
                            //znalezienie maszyny 
                            AGV_Machine machineAGV = listMachineAGV.Find(x => x.MachineName == machine.MachineName);
                            //
                            //Sprawdzenie czy maszyna nie ma serwisowego przełącznika aktywnego.
                            if (machine.State == 9 && (! machineAGV.AGV_SerwviceWork))
                            {

                                //Przejrzenie zadań na liście 
                                if (tasksPozmda02.Count > 0)
                                {

                                    foreach (var task in tasksPozmda02)
                                    {
                                        //
                                        //Sprawdzenie czy zadanie nie występuje już na liście.
                                        //
                                        if (task.name == machine.MachineName)
                                        {
                                            state = true;
                                        }
                                        if ((state == false) && task == tasksPozmda02[tasksPozmda02.Count - 1])
                                        {
                                            SendTask_pozmda02_body body = new SendTask_pozmda02_body()
                                            {
                                                Name = machine.MachineName,
                                                Details = "Awaria wózka",
                                                MachineNumber = "AGV"
                                            };
                                            // Aktualizacja czasu wystąpienia awari AGV.
                                            //
                                            if (machineAGV.AlarmOccuredTime.Year == 1)
                                            {
                                                AGV_UpdateAlarm.Get(machineAGV.MachineID, true);
                                            }
                                            else if (nowtime.AddMinutes( - 2) >= machineAGV.AlarmOccuredTime)
                                            {
                                                //Tymczsowo zabolkowane wysyłanie zadań do elektryków - TESTY - 05_11_2024
                                                //
                                                SendTask_pozmda02.POST(body);
                                                //
                                            }
                                            state = false;
                                        }
                                    }
                                }
                                else
                                {
                                    SendTask_pozmda02_body body = new SendTask_pozmda02_body()
                                    {
                                        Name = machine.MachineName,
                                        Details = "Awaria wózka",
                                        MachineNumber = "AGV"
                                    };
                                    // Aktualizacja czasu wystąpienia awari AGV.
                                    //
                                    if (machineAGV.AlarmOccuredTime.Year == 1)
                                    {
                                        AGV_UpdateAlarm.Get(machineAGV.MachineID, true);
                                    }
                                    else if (nowtime.AddMinutes(-2) >= machineAGV.AlarmOccuredTime)
                                    {
                                        //Tymczsowo zabolkowane wysyłanie zadań do elektryków - TESTY - 05_11_2024
                                        //
                                        SendTask_pozmda02.POST(body);
                                        //
                                    }
                                    state = false;

                                }

                            }
                            //Kasowanie zadania które zostało stworzone  a robot przestał zgłaszać błąd.
                            else
                            {
                                int idToDelete = 0;
                                //
                                if ((! (machineAGV.AlarmOccuredTime.Year == 1)) &&  agv_1_ObstacleDetected == false && agv_2_ObstacleDetected == false && agv_3_ObstacleDetected == false)
                                {
                                    AGV_UpdateAlarm.Get(machineAGV.MachineID, false);
                                }
                                //
                                foreach (var task in tasksPozmda02)
                                {
                                    if (task.name == machine.MachineName && task.status == 0 && task.details.Contains("Awaria"))
                                    {
                                        stateReset = true;
                                        idToDelete = task.id;
                                    }
                                    if ((stateReset == true) && task == tasksPozmda02[tasksPozmda02.Count - 1])
                                    {
                                        DeleteDuniTask_pozmda02.DeleteTask(idToDelete);
                                        stateReset = false;
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {

                Console.WriteLine("Error Occurred  in sending alarm list: " + e); ;
            }

                }
        public static async Task RecognizeAlarm_Warning(List<ID_309> agvMachineAlarmsList)
        {
            // Sprawdzenie listy czy nie jest pusta
            //
            if (agvMachineAlarmsList.Count > 0)
            {
                //
                List<AGV_Machine> listMachineAGV = await GetMachinesAGV_pozmda02.Get();
                //
                List<ReadTask_pozmda02_body> tasksPozmda02 = await ReadTask_pozmda02.Get();
                //
                bool agv_1_obstacleActive = false;
                bool agv_2_obstacleActive = false;
                bool agv_3_obstacleActive = false;
                //
                bool state = false;
                bool stateLoadSensor = false;

                int machineID = 0;
                foreach (var item in agvMachineAlarmsList) {
                    //
                    #region AGV_Ids
                    switch (item.Machine)
                    {
                        case "AGV 1":
                            machineID = 1;
                            break;
                        case "AGV 2":
                            machineID = 2;
                            break;
                        case "AGV 3":
                            machineID = 3;
                            break;
                        case "AGV1":
                            machineID = 1;
                            break;
                        case "AGV2":
                            machineID = 2;
                            break;
                        case "AGV3":
                            machineID = 3;
                            break;
                    }
                    #endregion
                    switch (item.NumberId) { 
                    //
                        case 622:
                        case 620:
                            //_______________________________________________________
                            // 
                            // 622 WARNING_OBSTACLE_DETECTED
                            // 620 WARNING_SYMBOL_POINT_OCCUPIED
                            // Stała przeszkoda na drodze
                            // ______________________________________________________

                            #region 622
                            // Nadanie Id dla maszyn AGV niezbędne do wysałania requestu o zadaniu. 
                            switch (machineID)
                            {
                                case 1:

                                    agv_1_obstacleActive = true;
                                    break;
                                case 2:

                                    agv_2_obstacleActive = true;
                                    break;
                                case 3:

                                    agv_3_obstacleActive = true;
                                    break;
                                default:
                                    agv_1_obstacleActive = true;
                                    agv_2_obstacleActive = true;
                                    agv_3_obstacleActive = true;
                                    break;
                            }
                            //
                            DateTime timeNow = DateTime.Now; 
                            //
                            foreach(var agv in listMachineAGV)
                            {
                                if(!(agv.AlarmOccuredTime.Year == 1))
                                {
                                    // Sprawdenie czy na liście zadań nie istnieje już jakiś alarm dla maszyny AGV jeśli istnieje alarm to nie wysyłamy komunikatu o przeszkodzie
                                    // Odwrotnie tak samo -ZASADA że tylko jedno zadanie z AGV na liście u elektryków.
                                    foreach (var task in tasksPozmda02)
                                    {
                                        if (task.name == agv.MachineName)
                                        {
                                            state = true;
                                        }
                                    }
                                    // Tworzenie zadania
                                    //if ((((!(agv.State == 9)) && (timeNow.AddSeconds(-1) >= agv.AlarmOccuredTime)) && (state == false))&& taskObstacleDetectionSended == false)
                                    if (((!(agv.State == 9)) && (timeNow.AddMinutes(-2) >= agv.AlarmOccuredTime)) && (state == false))
                                    {
                                        SendTask_pozmda02_body body = new SendTask_pozmda02_body()
                                        {
                                            Name = agv.MachineName,
                                            Details = "Przeszkoda na drodze",
                                            MachineNumber = "AGV"
                                        };
                                        SendTask_pozmda02.POST(body);
                                        //taskObstacleDetectionSended = true;
                                    }
                                }
                            }

                            #region UpdateTimeOccured Obstacle Detection occure
                            //
                            if (machineID == 1)
                            {
                                if (!(agv_1_ObstacleDetected))
                                {
                                    await AGV_UpdateAlarm.Get(machineID, true);
                                    agv_1_ObstacleDetected = true;
                                }
                            }
                            else if (machineID == 2)
                            {
                                if (!(agv_2_ObstacleDetected))
                                {
                                    await AGV_UpdateAlarm.Get(machineID, true);
                                    agv_2_ObstacleDetected = true;
                                }
                            }
                            else if (machineID == 3)
                            {
                                if (!(agv_3_ObstacleDetected))
                                {
                                    await AGV_UpdateAlarm.Get(machineID, true);
                                    agv_3_ObstacleDetected = true;
                                }
                            }

                            #endregion

                            break;
                        #endregion
                        //
                        case 613:
                        case 556:
                            //_______________________________________________________
                            // 
                            // 556 ERROR_LOAD_SENSOR_CONFLICT_AFTER_PICKUP
                            // 613 WARNING_TARGET_APPROACH_ERROR
                            // Nie wykryto palety 
                            // ______________________________________________________

                            #region 556 613
                            string agvName = "";
                            //
                            #region AGV Names create 

                            switch (machineID){
                                case 1:
                                    agvName = "A-Mate 1";
                                        break;
                                case 2:
                                    agvName = "A-Mate 2";
                                        break;
                                case 3:
                                    agvName = "A-Mate 3"; 
                                        break;
                            }
                            #endregion
                            //
                            if (agv_1_loadSensorError == false && agv_2_loadSensorError == false && agv_3_loadSensorError == false)
                            {
                                //Console.WriteLine("Error : Błąd wykrycia palety - " + item.Machine);
                                //
                                SendTask_pozmda02_body body = new SendTask_pozmda02_body()
                                {
                                    Name = agvName,
                                    Details = "Brak palety w punkcie",
                                    MachineNumber = "AGV"
                                };
                                SendTask_pozmda02.POST(body);
                            }
                            stateLoadSensor = true;
                            switch (machineID)
                            {
                                case 1:
                                    agv_1_loadSensorError = true;
                                    break;
                                case 2:
                                    agv_2_loadSensorError = true;
                                    break;
                                case 3:
                                    agv_3_loadSensorError = true;
                                    break;
                            }

                            break;
                        #endregion
                        //
                        default:
                            bool stateReset = false;
                            int idToDelete = 0;
                            bool warning_622 = false;
                            // Sprawdzenie czy jest to ostatni element tablicy
                            if(item == agvMachineAlarmsList[agvMachineAlarmsList.Count - 1])
                            {
                                //Reset stanu wykrycia nowego przypadku zaistnienia problemu z wykryciem palety
                                if (! stateLoadSensor)
                                {
                                    agv_1_loadSensorError = false;
                                    agv_2_loadSensorError = false;
                                    agv_3_loadSensorError = false;
                                    //
                                }
                                // Reset czasu wystąpienia awari jeśli na liście nie ma zadania 622.
                                foreach (var agv in listMachineAGV)
                                {

                                    if (agv.MachineID == 1 && agv_1_obstacleActive == false)
                                    {
                                        
                                        warning_622 = true;
                                        if (warning_622 == true && (!(agv.AlarmOccuredTime.Year == 1)) && (!(agv.State == 9)))
                                        {
                                            await AGV_UpdateAlarm.Get(agv.MachineID, false);
                                            agv_1_ObstacleDetected = false;
                                            //
                                        }
                                    }
                                    if (agv.MachineID == 2 && agv_2_obstacleActive == false)
                                    {
                                        
                                        warning_622 = true;
                                        if (warning_622 == true && (!(agv.AlarmOccuredTime.Year == 1)) && (!(agv.State == 9)))
                                        {
                                            await AGV_UpdateAlarm.Get(agv.MachineID, false);
                                            agv_2_ObstacleDetected = false;
                                            //
                                        }
                                    }
                                    if (agv.MachineID == 3 && agv_3_obstacleActive == false)
                                    {
                                       
                                        warning_622 = true;
                                        if (warning_622 == true && (!(agv.AlarmOccuredTime.Year == 1)) && (!(agv.State == 9)))
                                        {
                                            await AGV_UpdateAlarm.Get(agv.MachineID, false);
                                            agv_3_ObstacleDetected = false;
                                            //
                                        }
                                    }
                                    // RESET zadania przeszkoda na drodze w DUNITASK
                                    if (warning_622 == true && (!(agv.AlarmOccuredTime.Year == 1)))
                                    {
                                        //
                                        foreach (var task in tasksPozmda02)
                                        {
                                            if (task.name == agv.MachineName && task.status == 0 && task.details == "Przeszkoda na drodze")
                                            {
                                                stateReset = true;
                                                idToDelete = task.id;
                                            }
                                            if ((stateReset == true) && task == tasksPozmda02[tasksPozmda02.Count - 1])
                                            {
                                                await DeleteDuniTask_pozmda02.DeleteTask(idToDelete);
                                                stateReset = false;
                                            }
                                        }
                                        taskObstacleDetectionSended = false;

                                    }
                                    // RESET zadania z brakiem palety pod maszyna w DUNITASK
                                    if (!stateLoadSensor)
                                    {
                                        foreach (var task in tasksPozmda02)
                                        {
                                            if (task.name == agv.MachineName && task.status == 0 && task.details == "Brak palety w punkcie")
                                            {
                                                stateReset = true;
                                                idToDelete = task.id;
                                            }
                                            if ((stateReset == true) && task == tasksPozmda02[tasksPozmda02.Count - 1])
                                            {
                                                await DeleteDuniTask_pozmda02.DeleteTask(idToDelete);
                                                stateReset = false;
                                            }
                                        }
                                    }

                                    warning_622 = false;
                                }
                                }
                            break;
                    }
                }
            }
        }
        }
       
}

