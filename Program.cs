﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AGV_TcpIp_ConsoleApp.Model;
using AGV_TcpIp_ConsoleApp.SubProgramLogic;
using AGV_TcpIp_ConsoleApp.SubPrograms;




namespace AGV_TcpIp_ConsoleApp
{
    public static class Extensions
    {
        public static IEnumerable<T> ExceptBy<T, TKey>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, TKey> keySelector)
        {
            var secondKeys = new HashSet<TKey>(second.Select(keySelector));
            return first.Where(item => !secondKeys.Contains(keySelector(item)));
        }
        public static bool Excepts<T, TKey>(T first, IEnumerable<T> second, Func<T, TKey> keySelector)
        {
            var secondKeys = new HashSet<TKey>(second.Select(keySelector));
            return !secondKeys.Contains(keySelector(first));
        }

    }

    public class Program
    {

        // Czas cyklicznego odświeżenia danych w bazie danych 
        public static int SQL_UpdateTime = 5000;
        public static List<ID_309> ListobjId309public { get; set; } = new List<ID_309>();
        public static List<ID_310> ListobjId310public { get; set; } = new List<ID_310>();
        public static List<ID_310> ListobjId310publicLastUpdated { get; set; } = new List<ID_310>();
        public static List<ID_349> ListobjId349public { get; set; } = new List<ID_349>();
        public static List<ID_349> ListobjId349publicLastUpdated { get; set; } = new List<ID_349>();
        //
        public static HttpResponseMessage response349 = new HttpResponseMessage();
        public static HttpResponseMessage response310 = new HttpResponseMessage();
        public static HttpResponseMessage response309 = new HttpResponseMessage();

        static ID_310 objId310 = new ID_310();
        static ID_349 objId349 = new ID_349();
        //
        static Thread t = new Thread(new ThreadStart(UpdateDatainSQL));
        public static Thread ele_TaskAlarms = new Thread(new ThreadStart(TaskEleAlarms));
        static Thread ele_TaskWarnings = new Thread(new ThreadStart(  TaskEleWarnings));
        //
        // TESTY LOKALNE 
        //
        public static string HttpSerwerURI { get; set; } = "https://localhost:44396";
        //public static string HttpSerwerURI { get; set; } = "https://pozmda02.duni.org";
        //public static string HttpSerwerURI { get; set; } = "https://pozmda02.duni.org:82";

        static bool TcpStatusConnection;
        //public NetworkStream networkStream=new NetworkStream();
        static   async Task Main()
        {
#if !DEBUG
            Console.SetOut(new MyLoger("W:\\BackgroundTasks\\AGV_TCP_IP_v2_0\\logs"));
#endif
            try
            {
                ListobjId310publicLastUpdated = await GetMachinesAGV_pozmda02.Get();
                await TcpIp();
            }
            catch(Exception ex)
            {
                Console.WriteLine("_______________________________________________________________________________________________________________________________________________________________");
                Console.WriteLine("Reconnect to TCP Server ...");
                Console.WriteLine(ex.Message);
                Console.WriteLine("_______________________________________________________________________________________________________________________________________________________________");
                Thread.Sleep(10000);
                t.Interrupt();
                await Main();
            }
            //sec.Resume;
            //UpdateDatainSQL();
        }
        static async Task TcpIp ()
        { 
            string IP = "10.3.0.43";
            int port = 8015;


            #region sendBuffer
            // Nagłówek
            byte id = 56;
            ushort senderId = 1002;
            ushort receiverId = 1000;
            byte messageType = 2;
            byte dataLength = 10;

            // Komenda
            byte errorType = 0;
            byte errorLevel = 0;
            UInt32 entityID = 0;
            UInt32 options = 0;


            // Utworzenie tablicy bajtów zawierającej ramkę
            byte[] sendFrame = new byte[19];

            // Wypełnienie ramki
            BitConverter.GetBytes(id).CopyTo(sendFrame, 0);          // 2 bajty
            BitConverter.GetBytes(senderId).CopyTo(sendFrame, 2);    // 2 bajty
            BitConverter.GetBytes(receiverId).CopyTo(sendFrame, 4);  // 2 bajty
            sendFrame[6] = messageType;                              // 1 bajty
            BitConverter.GetBytes(dataLength).CopyTo(sendFrame, 7);  // 2 bajty
            BitConverter.GetBytes(errorType).CopyTo(sendFrame, 9);   // 1 bajty
            BitConverter.GetBytes(errorLevel).CopyTo(sendFrame, 10); // 1 bajty
            BitConverter.GetBytes(entityID).CopyTo(sendFrame, 11);   // 4 bajty
            BitConverter.GetBytes(options).CopyTo(sendFrame, 15);    // 4 bajty
                                                                     // 
                                                                     //_____________
                                                                     //  19 byte 
                                                                     // Wysłanie ramki
            #endregion

            IPAddress hostAdress = IPAddress.Parse("10.3.0.43");
            //int port = 8015;
            while (true)
            {
                //Console.WriteLine("_______________________________________________________________________________________________________________________________________________________________");
                //Console.WriteLine("Łączenie z Serverem TCP/IP z [pozagv02] ...");
                //Console.WriteLine("_______________________________________________________________________________________________________________________________________________________________");
                using TcpClient client = new TcpClient();
                await client.ConnectAsync(hostAdress, 8015);
                await using NetworkStream networkStream = client.GetStream();
                DateTime start = DateTime.Now;

                while (client.Connected)
                {
                    try { 
                        TcpStatusConnection = true;
                        // Console.WriteLine("Live state ....");

                        byte[] receiveBuffer = new byte[1024];
                        int readTotal;
                        //
                        networkStream.Write(sendFrame, 0, 19);
                        //
                        var output = networkStream.Read(receiveBuffer, 0, receiveBuffer.Length);
                        int iD = System.BitConverter.ToInt16(receiveBuffer, 0);
                        //
                        if(iD==310 || iD == 349)
                        {
                            int dataLengthTEMP = System.BitConverter.ToInt16(receiveBuffer, 7);
                            if (dataLengthTEMP > 1000)
                            {
                                Console.WriteLine("_______________________________________________________________________________________________________________________________________________________________");
                                Console.WriteLine("Długośc danych do odczytania to : "+ dataLengthTEMP);
                                Console.WriteLine("_______________________________________________________________________________________________________________________________________________________________");
                            }
                        }
                        String responseData = String.Empty;
                        //
                        //
                        //Errors (ID = 309)
                        // This message is sent periodically from Navithor to MES. Message contains current error status in the system.
                        #region 309 Temporary Bocked
                        //
                        //if (iD == 309)
                        //{
                        //    ListobjId309public.Clear();
                        //    DateTime czas = DateTime.Now;
                        //    //Console.WriteLine("ID = 309 | czas: " + czas);

                        //    int begine = 11;
                        //    int begine_swap = begine;

                        //    while (output > begine)
                        //    {
                        //        ID_309 objId309 = new ID_309();
                        //        int NameLength = System.BitConverter.ToInt16(receiveBuffer, begine_swap);
                        //        begine_swap += 2;
                        //        begine_swap += NameLength;
                        //        objId309.NumberId = System.BitConverter.ToUInt16(receiveBuffer, begine_swap);
                        //        begine_swap += 2;
                        //        objId309.Value = System.BitConverter.ToUInt16(receiveBuffer, begine_swap);
                        //        begine_swap += 2;
                        //        begine += 2;

                        //        string[] words = System.Text.Encoding.ASCII.GetString(receiveBuffer, begine, NameLength).Split(".");

                        //        objId309.Name = words[2];
                        //        objId309.Machine = words[1];
                        //        DateTime Time = DateTime.Now;
                        //        objId309.UpdatedTime = Time;

                        //        ListobjId309public.Add(objId309);


                        //        begine += NameLength + 4;
                        //    }
                        //}
                        #endregion

                        //
                        // AGVStatus (ID = 310)
                        //Periodic status message from a single machine in the system. Sending interval can be changed from Navithor Server
                        //parameters: Interval_To_Send_AGV_Status_To_MES_When_AGV_Enabled and
                        //Interval_To_Send_AGV_Status_To_MES_When_AGV_Disabled.Parameter
                        //MES_Disable_Outbound_Message_Groups_Rules can be used to disable message completely.
                        //
                        //
                        #region 310 
                        if (iD == 310)
                        {
                            DateTime czas = DateTime.Now;
                            //Console.WriteLine("ID = 310 | czas: " + czas);
                            int begine = 9;
                            int begine_swap = begine;
                            ListobjId310public.Clear();
                            //99 Max Length of data Frame for this comand from documentation
                            //while (99 > begine_swap)
                            //{
                                objId310.MachineID = System.BitConverter.ToUInt16(receiveBuffer, begine_swap);
                                begine_swap += 2;
                                objId310.X = System.BitConverter.ToDouble(receiveBuffer, begine_swap);
                                begine_swap += 8;
                                objId310.Y = System.BitConverter.ToDouble(receiveBuffer, begine_swap);
                                begine_swap += 8;
                                objId310.H = System.BitConverter.ToDouble(receiveBuffer, begine_swap);
                                begine_swap += 8;
                                objId310.Poziom = System.BitConverter.ToInt16(receiveBuffer, begine_swap);
                                begine_swap += 2;
                                objId310.PositionConfidence = receiveBuffer[begine_swap];
                                begine_swap += 1;
                                objId310.SpeedNavigationPoint = System.BitConverter.ToDouble(receiveBuffer, begine_swap);
                                begine_swap += 8;
                                objId310.State = receiveBuffer[begine_swap];
                                begine_swap += 1;
                                objId310.BatteryLeve = System.BitConverter.ToDouble(receiveBuffer, begine_swap);
                                begine_swap += 8;
                                objId310.AutoOrManual = receiveBuffer[begine_swap];
                                begine_swap += 1;
                                objId310.PositionInitialized = receiveBuffer[begine_swap];
                                begine_swap += 1;
                                objId310.LastSymbolPoint = System.BitConverter.ToInt32(receiveBuffer, begine_swap);
                                begine_swap += 4;
                                objId310.MachineAtLastSymbolPoint = receiveBuffer[begine_swap];
                                begine_swap += 1;
                                objId310.TargetSymbolPoint = System.BitConverter.ToInt32(receiveBuffer, begine_swap);
                                begine_swap += 4;
                                objId310.MachineAtTarget = receiveBuffer[begine_swap];
                                begine_swap += 1;
                                objId310.Operational = receiveBuffer[begine_swap];
                                begine_swap += 1;
                                objId310.InProduction = receiveBuffer[begine_swap];
                                begine_swap += 1;
                                objId310.LoadStatus = receiveBuffer[begine_swap];
                                begine_swap += 1;
                                objId310.BatteryVoltage = System.BitConverter.ToDouble(receiveBuffer, begine_swap);
                                begine_swap += 8;
                                objId310.ChargingStatus = receiveBuffer[begine_swap];
                                begine_swap += 1;
                                objId310.DistanceToTarget = System.BitConverter.ToSingle(receiveBuffer, begine_swap);
                                begine_swap += 4;
                                objId310.CurrentDriveThroughPoint = System.BitConverter.ToInt32(receiveBuffer, begine_swap);
                                begine_swap += 4;
                                objId310.NextLeveChangePointId = System.BitConverter.ToInt32(receiveBuffer, begine_swap);
                                begine_swap += 4;
                                objId310.DistanceToNextLeveelChange = System.BitConverter.ToSingle(receiveBuffer, begine_swap);
                                begine_swap += 4;
                                objId310.LastSymbolPointDrivenOver = System.BitConverter.ToInt32(receiveBuffer, begine_swap);
                                begine_swap += 4;

                                objId310.UpdateTime = DateTime.Now;
                                // Nadanie nazw robotom
                                if (objId310.MachineID == 1)
                                {
                                    objId310.MachineName = "A-Mate 1";
                                }
                                else if (objId310.MachineID == 2)
                                {
                                    objId310.MachineName = "A-Mate 2";
                                }
                                else if (objId310.MachineID == 3)
                                {
                                    objId310.MachineName = "A-Mate 3";
                                }
                            //}
                            ListobjId310public.Add(objId310);
                        }
                        #endregion
                        //
                        // ErrorsV2 (ID = 349)
                        //This message is sent as a response to ErrorsV2 request (ID = 56).
                        #region 349
                        if (iD == 349)
                        {
                            ListobjId349public.Clear();
                            DateTime czas = DateTime.Now;
                            //Console.WriteLine("ID = 309 | czas: " + czas);
                            //
                            //Sprawdzenie ilości przychodzących alarmów 
                            UInt16 NumberOfErrors = System.BitConverter.ToUInt16(receiveBuffer, 9);
                            //8.1. Frame
                            //Every message has a frame that consists of following data. Sender ID and Receiver ID are specified as follow:
                            //Navithor: ID = 1000, MES clients: ID = 1001, 1002, ... For example when sending a message from MES to Navithor
                            //Sender ID = 1001 and Receiver ID = 1000.
                            int begine = 11;
                            int begine_swap = begine;
                            UInt16 i = 0;
                            //
                            while (i < NumberOfErrors)
                            {
                                ID_349 objId349 = new ID_349();
                                //
                                // Object
                                objId349.ErrorId = System.BitConverter.ToUInt32(receiveBuffer, begine_swap);
                                begine_swap += 4;
                                objId349.NameStringLength = System.BitConverter.ToUInt16(receiveBuffer, begine_swap);
                                begine_swap += 2;
                                objId349.Name = System.Text.Encoding.ASCII.GetString(receiveBuffer, begine_swap, objId349.NameStringLength);
                                begine_swap += objId349.NameStringLength;
                                objId349.ErrorType = (EnumErrorType)System.BitConverter.ToUInt16(receiveBuffer, begine_swap);
                                begine_swap += 2;
                                objId349.EntityID = System.BitConverter.ToUInt32(receiveBuffer, begine_swap);
                                begine_swap += 4;
                                objId349.Source = (EnumSource)receiveBuffer[begine_swap];
                                begine_swap += 1;
                                objId349.Level = receiveBuffer[begine_swap];
                                begine_swap += 1;
                                objId349.Value = System.BitConverter.ToUInt16(receiveBuffer, begine_swap);
                                begine_swap += 2;
                                objId349.Priority = System.BitConverter.ToInt16(receiveBuffer, begine_swap);
                                begine_swap += 2;
                                //
                                ListobjId349public.Add(objId349);
                                i += 1;
                            }
                        }

                            #endregion
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("_______________________________________________________________________________________________________________________________________________________________");
                        Console.WriteLine("Error during reading/writing data via TCP/IP");
                        Console.WriteLine(e.Message);
                        networkStream.Close();
                        client.Close();
                        Console.WriteLine("Client TCP/IP disconnected");
                        Console.WriteLine("_______________________________________________________________________________________________________________________________________________________________");
                        TcpStatusConnection = false;
                    }

                    //_______________________________________________
                    //
                    //THREADs
                    //
                    //_______________________________________________

                    int a = t.ThreadState.GetHashCode();
                    //ThreadState.U
                    if (a == 12 || a == 8)
                    {
                        t.IsBackground = true;
                        t.Start();
                    }
                    int b = ele_TaskAlarms.ThreadState.GetHashCode();
                    //ThreadState.U
                    if (b == 12 || b == 8)
                    {
                        ele_TaskAlarms.IsBackground = true;
                        ele_TaskAlarms.Start();
                    }
                    int c = ele_TaskWarnings.ThreadState.GetHashCode();
                    //ThreadState.U
                    if (c == 12 || c == 8)
                    {
                        ele_TaskWarnings.IsBackground = true;
                        ele_TaskWarnings.Start();
                    }
                }
                if (client.Connected == false)
                {
                 Thread.Sleep(7000);

                }
            }
        }

        static async void UpdateDatainSQL()
        {
            //List<ID_349> previousData = new List<ID_349>();
            while (true)
            {
                if (TcpStatusConnection)
                {
                    List<ID_310> newListLast = ListobjId310publicLastUpdated;
                    Thread.Sleep(millisecondsTimeout: SQL_UpdateTime);
                    // Jeśli 
                    if (ListobjId349publicLastUpdated.Count == 0)
                    {
                        ListobjId349publicLastUpdated = await ReadAGV_Alarms_v2_pozmda02.Get();
                        //__________________________________________________________
                        // REFACTORING - if error to jakis reconect czy cos 
                        //__________________________________________________________
                    }

                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            //
                            // Update alarms to pozmda02 but only changes
                            //
                            #region Update ALARMS
                            // Do DODANIA
                            // Te których nie ma na starej liście.
                            // Metoda ExceptBy - zapewnia pominięcie w porównaniu obiektówpola Id oraz OccurTime
                            IEnumerable<ID_349> toAdd = ListobjId349public.ExceptBy(ListobjId349publicLastUpdated, x => new { x.EntityID, x.ErrorId, x.ErrorType, x.Level, x.Name, x.NameStringLength, x.Priority, x.Source, x.Value });
                            //
                            // Nadanie czasu wystąpienia danego alarmu
                            toAdd = toAdd.Select(obj => { obj.OccurTime = DateTime.Now; return obj; }).ToList();
                            // Do USUNIĘCIA
                            // Te które są na starej liśćie.
                            // Metoda ExceptBy - zapewnia pominięcie w porównaniu obiektówpola Id oraz OccurTime
                            IEnumerable<ID_349> toDelete = ListobjId349publicLastUpdated.ExceptBy(ListobjId349public, x => new { x.EntityID, x.ErrorId, x.ErrorType, x.Level, x.Name, x.NameStringLength, x.Priority, x.Source, x.Value });
                            //
                            AGV_AlarmsUpdateRequest requestData = new AGV_AlarmsUpdateRequest
                            {
                                DataToAdd = toAdd.ToList(),
                                DataToDelete = toDelete.ToList()
                            };

                            if (requestData.DataToAdd.Count > 0 || requestData.DataToDelete.Count > 0)
                            {
                                //
                                //if (!localData.SequenceEqual(previousData))
                                //{

                                //    previousData = new List<ID_349>(localData);

                                ListobjId349publicLastUpdated.AddRange(toAdd);
                                ListobjId349publicLastUpdated.RemoveAll(item => toDelete.Any(toDel =>
                                    toDel.EntityID == item.EntityID &&
                                    toDel.ErrorId == item.ErrorId &&
                                    toDel.ErrorType == item.ErrorType &&
                                    toDel.Level == item.Level &&
                                    toDel.Name == item.Name &&
                                    toDel.NameStringLength == item.NameStringLength &&
                                    toDel.Priority == item.Priority &&
                                    toDel.Source == item.Source &&
                                    toDel.Value == item.Value));

                                response349 = await client.PostAsJsonAsync($"{HttpSerwerURI}/api/Agv/AGV_AlarmsUpdate_v2.1.0/", requestData);
                                Console.WriteLine("Send update with Alarms  Data on: " + DateTime.Now);
                                //
                            }
                            #endregion
                            if (ListobjId310public.Count > 0)
                            {
                                //__________________________________________________________
                                // REFACTORING - Opracować brak update dla tych samych danych w kółko
                                //__________________________________________________________
                                //
                                if(ListobjId310publicLastUpdated[1].MachineName == "A-Mate2")
                                {
                                    ListobjId310publicLastUpdated[1].MachineName = "A-Mate 2";
                                }
                                ID_310 object310LastUpdated = ListobjId310publicLastUpdated.FirstOrDefault(s => s.MachineName == ListobjId310public[0].MachineName);
                                //
                                //
                                // Porównanie obiektów i sprawdzenie czy są rówżnice w obiekcie który przyszedł względem obiektu który
                                // przechowuje ostatnio zaktualizowane maszyny AGV.
                                //
                                bool toUpdate = Extensions.Excepts(ListobjId310public[0] , ListobjId310publicLastUpdated, x => new { 
                                    x.MachineName,x.X,x.Y,x.H,x.Poziom,x.PositionConfidence, 
                                    x.SpeedNavigationPoint, x.State, x.BatteryLeve, x.AutoOrManual, 
                                    x.PositionInitialized, x.LastSymbolPoint, x.MachineAtLastSymbolPoint, 
                                    x.TargetSymbolPoint, x.MachineAtTarget, x.Operational, x.InProduction, 
                                    x.LoadStatus, x.BatteryVoltage, x.ChargingStatus, x.DistanceToTarget, 
                                    x.CurrentDriveThroughPoint, x.NextLeveChangePointId, x.DistanceToNextLeveelChange, 
                                    x.LastSymbolPointDrivenOver
                                });
                                // false gdy jest zgodnosc obiektów    - NIE UAKTUALNIAMY DANYCH 
                                // true  gdy jest niezgodnosc obiektów - UAKTUALNIAMY DANE
                                if (toUpdate)
                                {
                                    response310 = await client.PostAsJsonAsync($"{HttpSerwerURI}/api/Agv/AGV_MachinesStatusUpdate/", ListobjId310public);
                                    //
                                    // Aktualizacja obiektu w liście MASZYN AGV który przychodzi do aktualizacji.
                                    //
                                    #region Updated data in List model ID_310
                                    ListobjId310publicLastUpdated = ListobjId310publicLastUpdated.Select(obj =>
                                        {
                                            if (obj.MachineName == ListobjId310public[0].MachineName)
                                            {
                                                obj.MachineID = ListobjId310public[0].MachineID;
                                                obj.MachineName = ListobjId310public[0].MachineName;
                                                obj.X = ListobjId310public[0].X;
                                                obj.Y = ListobjId310public[0].Y;
                                                obj.H = ListobjId310public[0].H;
                                                obj.Poziom = ListobjId310public[0].Poziom;
                                                obj.PositionConfidence = ListobjId310public[0].PositionConfidence;
                                                obj.SpeedNavigationPoint = ListobjId310public[0].SpeedNavigationPoint;
                                                obj.State = ListobjId310public[0].State;
                                                obj.BatteryLeve = ListobjId310public[0].BatteryLeve;
                                                obj.AutoOrManual = ListobjId310public[0].AutoOrManual;
                                                obj.PositionInitialized = ListobjId310public[0].PositionInitialized;
                                                obj.LastSymbolPoint = ListobjId310public[0].LastSymbolPoint;
                                                obj.MachineAtLastSymbolPoint = ListobjId310public[0].MachineAtLastSymbolPoint;
                                                obj.TargetSymbolPoint = ListobjId310public[0].TargetSymbolPoint;
                                                obj.MachineAtTarget = ListobjId310public[0].MachineAtTarget;
                                                obj.Operational = ListobjId310public[0].Operational;
                                                obj.InProduction = ListobjId310public[0].InProduction;
                                                obj.LoadStatus = ListobjId310public[0].LoadStatus;
                                                obj.BatteryVoltage = ListobjId310public[0].BatteryVoltage;
                                                obj.ChargingStatus = ListobjId310public[0].ChargingStatus;
                                                obj.DistanceToTarget = ListobjId310public[0].DistanceToTarget;
                                                obj.CurrentDriveThroughPoint = ListobjId310public[0].CurrentDriveThroughPoint;
                                                obj.NextLeveChangePointId = ListobjId310public[0].NextLeveChangePointId;
                                                obj.DistanceToNextLeveelChange = ListobjId310public[0].DistanceToNextLeveelChange;
                                                obj.LastSymbolPointDrivenOver = ListobjId310public[0].LastSymbolPointDrivenOver;
                                                obj.UpdateTime = ListobjId310public[0].UpdateTime;
                                            }
                                            return obj;
                                        }).ToList();

                                    #endregion

                                    //
                                    StringBuilder machineListString = new StringBuilder();
                                    foreach (var machine in ListobjId310public)
                                        {
                                        if (machineListString.Length > 0)
                                        {
                                            machineListString.Append(", ");
                                        }
                                        machineListString.Append(machine.MachineName);
                                        }
                                    Console.WriteLine("Send update with MACHINE for " + ListobjId310public.Count + " machines. Main for :" + machineListString + ". Data : " + DateTime.Now);

                                }
                            }
                            if (response310.IsSuccessStatusCode && response349.IsSuccessStatusCode)
                            {
                                ListobjId310public.Clear();
                                DateTime czas = DateTime.Now;
                                //Console.WriteLine("Work work work ...");
                            }
                            else
                            {
                                ListobjId310publicLastUpdated.Clear();
                                ListobjId349publicLastUpdated.Clear();
                                Console.WriteLine("Error Occurred  sending machine status: "); ;
                            }
                        }
                        //}
                    }
                    catch (Exception e)
                    {
                        // Usunięcie listy ostatnio aktuaizowanych danych by przy odzyskaniu połączenia pobrać dane z bazy danych na nowo.
                        ListobjId349publicLastUpdated.Clear();
                        //
                        Console.WriteLine("_______________________________________________________________________________________________________________________________________________________________");
                        Console.WriteLine("Error Occurred  in sending alarm list: " + e); ;
                        Console.WriteLine("_______________________________________________________________________________________________________________________________________________________________");
                    }

                }
            }
        }

        //________________________________________________________________________________________________________________________________________________________________________

        //
        // ZABLOKOWANIE WSYŁĄNIA ZADAŃ !!! - TESTY nowej ramki danych z alarmami.
        //
        static async void TaskEleAlarms()
        {
            while (false)
            {
                // Zadania Awari na tablet elektryka
                SendTask_Logic.RecognizeAlarm(ListobjId310public);
                //
                Thread.Sleep(millisecondsTimeout: 10000);
            }
        }
        static async void TaskEleWarnings()
        {
            while (false)
            {
                // Zadanie o przeszkodzie na drodze na tablet elektryka
                // ZADANIA NIE MOGĄ BYĆ ASYNCHRONICZNE !!!!
                SendTask_Logic.RecognizeWarning(ListobjId309public);
                //
                Thread.Sleep(millisecondsTimeout: 10000);
            }
        }


    }
}
