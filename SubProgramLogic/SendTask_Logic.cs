﻿using System;
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
        static bool agv_1_forkliftFailureError = false;
        static bool agv_2_forkliftFailureError = false;
        static bool agv_3_forkliftFailureError = false;
        //
        static bool taskObstacleDetectionSended = false;
        //
        public static async Task RecognizeAlarm(List<ID_310> agvMachineStatus)
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
        public static async Task RecognizeWarning(List<ID_309> agvMachineAlarmsList)
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
                //
                bool stateLoadSensor = false;
                bool stateForkliftFailure = false;
                //
                int machineID = 0;
                string agvName;
                AGV_Machine agv_machine;
                bool liveBit;
                HttpResponseMessage responsLiveBit_2;

                foreach (var item in agvMachineAlarmsList) {
                    //
                    #region AGV_Ids
                    switch (item.Machine)
                    {
                        case "AGV1":
                        case "AGV 1":
                            machineID = 1;
                            break;
                        //
                        case "AGV2":
                        case "AGV 2":
                            machineID = 2;
                            break;
                        //
                        case "AGV3":
                        case "AGV 3":
                            machineID = 3;
                            break;
                    }
                    #endregion
                    //Switch po ID alarmów które widizmy na stronie : https://pozmda02.duni.org/Agv/AGV_AlarmListView
                    switch (item.NumberId) { 
                    //
                        case 622:
                        case 620:
                        case 323:
                        case 322:
                        case 324:
                        case 607:

                            //_______________________________________________________
                            // 
                            // 622 WARNING_OBSTACLE_DETECTED
                            // 620 WARNING_SYMBOL_POINT_OCCUPIED
                            // 323 WARNING_SAFETY_SCANNER_WARNING_ZONE1
                            // 322 WARNING_SAFETY_SCANNER_PROTECTION_ZONE
                            // 324 WARNING_SAFETY_SCANNER_WARNING_ZONE2
                            // 607 WARNING_SAFETY_SCANNER_OBSTACLE
                            //
                            // Przeszkoda na drodze
                            // ______________________________________________________

                            #region Warnings 622 620
                            // Nadanie Id dla maszyn AGV niezbędne do wysałania requestu o zadaniu. 
                            switch (machineID)
                            {
                                case 1:
                                    agv_1_obstacleActive = true;
                                    break;
                                //
                                case 2:
                                    agv_2_obstacleActive = true;
                                    break;
                                //
                                case 3:
                                    agv_3_obstacleActive = true;
                                    break;
                                //default:
                                //    agv_1_obstacleActive = true;
                                //    agv_2_obstacleActive = true;
                                //    agv_3_obstacleActive = true;
                                //    break;
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
                                    if(tasksPozmda02.Count == 0)
                                    {
                                        //
                                        //Sprawdzenie czy serwer odpowiada jeśli nie to zablokowanie wysłania zadania dla elektryków.
                                        HttpResponseMessage responseLiveBit = await LiveBit_pozmda02.Get();
                                        if(! responseLiveBit.IsSuccessStatusCode)
                                        {
                                            state = true;
                                        }
                                    }
                                    else { 
                                        foreach (var task in tasksPozmda02)
                                        {
                                            if (task.name == agv.MachineName)
                                            {
                                                state = true;
                                            }
                                        }
                                    }
                                    // Tworzenie zadania
                                    if (((!(agv.State == 9)) && (timeNow.AddMinutes(-2) >= agv.AlarmOccuredTime)) && (state == false) && (! agv.AGV_SerwviceWork) && ((agv_1_ObstacleDetected == true && machineID == 1) || (agv_2_ObstacleDetected == true && machineID == 2) || (agv_3_ObstacleDetected == true && machineID == 3)))
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
                            //
                            // Brak palety w punkcie
                            // ______________________________________________________

                            #region Warnings 556 613
                            agvName = "";
                            //
                            #region AGV Names create 
                            agv_machine = new AGV_Machine();
                            switch (machineID){
                                case 1:
                                    agvName = "A-Mate 1";
                                    agv_machine = listMachineAGV.Find(x => x.MachineName == agvName);
                                    break;
                                case 2:
                                    agvName = "A-Mate 2";
                                    agv_machine = listMachineAGV.Find(x => x.MachineName == agvName);
                                    break;
                                case 3:
                                    agvName = "A-Mate 3";
                                    agv_machine = listMachineAGV.Find(x => x.MachineName == agvName);
                                    break;
                            }
                            #endregion
                            //
                            //Sprawdzenie czy serwer pozmda02 odpowiada jeśli nie to zablokowanie wysłania zadania dla elektryków.
                            liveBit = true;
                            responsLiveBit_2 = await LiveBit_pozmda02.Get();
                            if (!responsLiveBit_2.IsSuccessStatusCode)
                            {
                                liveBit = false;
                            }
                            //
                            if (((agv_1_loadSensorError == false && machineID == 1) || (agv_2_loadSensorError == false && machineID == 2) || (agv_3_loadSensorError == false && machineID == 3)) && (! agv_machine.AGV_SerwviceWork) && liveBit)
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
                            //
                            stateLoadSensor = true;
                            //
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
                        case 644:
                        case 647:
                        case 201:
                            //_______________________________________________________
                            // 
                            // 644 WARNING_CHANGING_LOAD_OR_UNLOAD_HEIGHT
                            // 647 WARNING_DECREASING_FORK_TO_ROUTE_LEVEL
                            // 201 ERROR_VEH_INTE_MSG_PARSE
                            //
                            // Bład wideł
                            // ______________________________________________________

                            #region Warnings 644 647 201
                            agvName = "";
                            //
                            #region AGV Names create 
                            agv_machine = new AGV_Machine();
                            switch (machineID)
                            {
                                case 1:
                                    agvName = "A-Mate 1";
                                    agv_machine = listMachineAGV.Find(x => x.MachineName == agvName);
                                    break;
                                case 2:
                                    agvName = "A-Mate 2";
                                    agv_machine = listMachineAGV.Find(x => x.MachineName == agvName);
                                    break;
                                case 3:
                                    agvName = "A-Mate 3";
                                    agv_machine = listMachineAGV.Find(x => x.MachineName == agvName);
                                    break;
                            }
                            #endregion
                            //
                            //Sprawdzenie czy serwer pozmda02 odpowiada jeśli nie to zablokowanie wysłania zadania dla elektryków.
                            liveBit = true;
                            responsLiveBit_2 = await LiveBit_pozmda02.Get();
                            if (!responsLiveBit_2.IsSuccessStatusCode)
                            {
                                liveBit = false;
                            }
                            //
                            if (((agv_1_forkliftFailureError == false && machineID == 1) || (agv_2_forkliftFailureError == false && machineID == 2) || (agv_3_forkliftFailureError == false && machineID == 3)) && (!agv_machine.AGV_SerwviceWork) && liveBit)
                            {
                                //Console.WriteLine("Error : Błąd wykrycia palety - " + item.Machine);
                                //
                                SendTask_pozmda02_body body = new SendTask_pozmda02_body()
                                {
                                    Name = agvName,
                                    Details = "Błąd wideł",
                                    MachineNumber = "AGV"
                                };
                                await SendTask_pozmda02.POST(body);
                            }
                            //
                            stateForkliftFailure = true;
                            //
                            switch (machineID)
                            {
                                case 1:
                                    agv_1_forkliftFailureError = true;
                                    break;
                                case 2:
                                    agv_2_forkliftFailureError = true;
                                    break;
                                case 3:
                                    agv_3_forkliftFailureError = true;
                                    break;
                            }
                            break;

                        #endregion

                        
                        default:
                            bool stateReset = false;
                            int idToDelete = 0;
                            bool warning_622 = false;
                            //
                            // Sprawdzenie czy jest to ostatni element tablicy
                            if(item == agvMachineAlarmsList[agvMachineAlarmsList.Count - 1])
                            {
                                //Reset stanu wykrycia nowego przypadku zaistnienia problemu z wykryciem palety
                                #region Reset LoadSensorError
                                //
                                if (stateLoadSensor == false)
                                {
                                    agv_1_loadSensorError = false;
                                    agv_2_loadSensorError = false;
                                    agv_3_loadSensorError = false;
                                }
                                if (stateForkliftFailure == false)
                                {
                                    agv_1_forkliftFailureError = false;
                                    agv_2_forkliftFailureError = false;
                                    agv_3_forkliftFailureError = false;
                                }
                                


                                #endregion
                                // Reset czasu wystąpienia awari jeśli na liście nie ma zadania 622.
                                foreach (var agv in listMachineAGV)
                                {
                                    //
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
                                    //
                                    // RESET zadania z brakiem palety pod maszyna w DUNITASK
                                    if (!stateForkliftFailure)
                                    {
                                        foreach (var task in tasksPozmda02)
                                        {
                                            if (task.name == agv.MachineName && task.status == 0 && task.details == "Błąd wideł")
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
                                    //

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

