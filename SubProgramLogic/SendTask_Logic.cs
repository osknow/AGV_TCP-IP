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
                                if (! (machineAGV.AlarmOccuredTime.Year == 1))
                                {
                                    AGV_UpdateAlarm.Get(machineAGV.MachineID, false);
                                }
                                //
                                foreach (var task in tasksPozmda02)
                                {
                                    if (task.name == machine.MachineName && task.status == 0)
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
        }
       
}

