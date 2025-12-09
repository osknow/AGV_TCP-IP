using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AGV_TcpIp_ConsoleApp.Mapper;
using AGV_TcpIp_ConsoleApp.Model;
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
        public static int SQL_UpdateTime =5000;
        public static List<ID_309> ListobjId309public { get; set; } = new List<ID_309>();

        public static List<ID_310> ListobjId310public { get; } = new()
        {
            new ID_310 { MachineName = "A-Mate 1" },
            new ID_310 { MachineName = "A-Mate 2" },
            new ID_310 { MachineName = "A-Mate 3" }
        };

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
        static Thread updateDatainSQL_Thread = new Thread(new ThreadStart(UpdateDatainSQL));
        // Update Alarms AGV Frqame 56
        static Thread updateAgvAlarms_56_Thread = new Thread(new ThreadStart(UpdateVarialeToSendFrame_56));
        //
        static bool AgvAlarmsFrameSend_State = false;

        // To TESTS Only 
        //static Thread t = new Thread(new ThreadStart(TEMP_Empty));
        // Temporary block
        //static Thread ElokonTask = new Thread(new ThreadStart(WriteDataToTXT));
        //
        // TESTY LOKALNE 
        //
        //public static string HttpSerwerURI { get; set; } = "http://localhost:81";
        public static string HttpSerwerURI { get; set; } = "https://pozmda02.duni.org";
        //public static string HttpSerwerURI { get; set; } = "https://pozmda02.duni.org:82";

        static bool TcpStatusConnection;
        //public NetworkStream networkStream=new NetworkStream();
        static   async Task Main()
        {
#if !DEBUG
            Console.SetOut(new MyLoger("W:\\BackgroundTasks\\AGV_TCP_IP_v2\\logs_TEMP"));
#else
            Console.SetOut(new MyLoger("D:\\AGV_TCP_IP_v2\\logs"));
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
                Thread.Sleep(5000);
                updateDatainSQL_Thread.Interrupt();
                updateAgvAlarms_56_Thread.Interrupt();
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
            ushort senderId = 1005;
            ushort receiverId = 1000;
            byte messageType = 2;
            byte dataLength = 10;

            // Komenda
            byte errorType = 0;
            byte errorLevel = 0;
            UInt32 entityID = 0;
            UInt32 options = 0;


            // Utworzenie tablicy bajtów zawierającej ramkę
            Byte[] sendFrame = new byte[19];

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
            DateTime startTime = DateTime.Now;
            Console.WriteLine(startTime);
            //Task uruchaminay co 5 min więć while trwa max 4min50sec = 290sec
            while (DateTime.Now < startTime.AddSeconds(290))
            {
#if !DEBUG
            Console.SetOut(new MyLoger("W:\\BackgroundTasks\\AGV_TCP_IP_v2\\logs_TEMP"));
#else
            Console.SetOut(new MyLoger("D:\\AGV_TCP_IP_v2\\logs"));
#endif
                Console.WriteLine("_______________________________________________________________________________________________________________________________________________________________");
                Console.WriteLine("Łączenie z Serverem TCP/IP z [pozagv02] ...");
                Console.WriteLine("_______________________________________________________________________________________________________________________________________________________________");
                using TcpClient client = new TcpClient();
                await client.ConnectAsync(hostAdress, 8015);
                await using NetworkStream networkStream = client.GetStream();
                DateTime start = DateTime.Now;
                //
                byte[] headerBuffer = new byte[9];
                while (client.Connected)
                {
#if !DEBUG
            Console.SetOut(new MyLoger("W:\\BackgroundTasks\\AGV_TCP_IP_v2\\logs_TEMP"));
#else
                    Console.SetOut(new MyLoger("D:\\AGV_TCP_IP_v2\\logs"));
#endif
                    try
                    { 
                        TcpStatusConnection = true;
                        // Sleep z powodu zbyt częstego odpytywania serwera ponoć. Twierdzenie ELOKONu. 
                        // Navitec zapisuje logi do pliku gdzie po prostu logują nasze zapytania o ramkę  56 która przychodzi często chcąc mieć najrzetelniejsze dane.
                        //Thread.Sleep(200);
                        //
                        Byte[] receiveBuffer = new byte[1024];
                        int readTotal;
                        //
                        // Odczytaj nagłówek
                        int read = await networkStream.ReadAsync(headerBuffer, 0, headerBuffer.Length);
                        if (read < headerBuffer.Length)
                        {
                            Console.WriteLine("Niepełny nagłówek, przerywam.");
                            break;
                        }
                        //
                        // Parsowanie nagłówka
                        int iD = System.BitConverter.ToInt16(headerBuffer, 0);
                        int senderID = System.BitConverter.ToInt16(headerBuffer, 2);
                        int receiverID = System.BitConverter.ToInt16(headerBuffer, 4);
                        byte messagEType = headerBuffer[6];
                        int dataLengthTEMP = System.BitConverter.ToInt16(headerBuffer, 7);
                        if (senderID == 1000)
                        {
                            Console.WriteLine($"ID: {iD}, Sender: {senderID}, Receiver: {receiverID}, Type: {messagEType}, DataLength: {dataLengthTEMP}");
                        }
                        //
                        // Odczytaj dane ramki jeśli są
                        byte[] dataBuffer = new byte[dataLengthTEMP];
                        if (dataLengthTEMP > 0)
                        {
                            int bytesRead = await networkStream.ReadAsync(dataBuffer, 0, dataLengthTEMP);
                            //Console.WriteLine($"Dane: {BitConverter.ToString(dataBuffer)}");
                        }
                        //
                        #region 310
                        // AGVStatus (ID = 310)
                        //Periodic status message from a single machine in the system. Sending interval can be changed from Navithor Server
                        //parameters: Interval_To_Send_AGV_Status_To_MES_When_AGV_Enabled and
                        //Interval_To_Send_AGV_Status_To_MES_When_AGV_Disabled.Parameter
                        //MES_Disable_Outbound_Message_Groups_Rules can be used to disable message completely.
                        if (iD == 310)
                        {
#if !DEBUG
                            Console.SetOut(new MyLoger("W:\\BackgroundTasks\\AGV_TCP_IP_v2\\Logs_MachineAGV_TEMP"));
#else
                            Console.SetOut(new MyLoger("D:\\AGV_TCP_IP_v2\\Logs_MachineAGV"));
#endif
                            DateTime czas = DateTime.Now;
                            //Console.WriteLine("ID = 310 | czas: " + czas);
                            int begine = 0;
                            int begine_swap = begine;
                                objId310.MachineID = System.BitConverter.ToUInt16(dataBuffer, begine_swap);
                                begine_swap += 2;
                                objId310.X = System.BitConverter.ToDouble(dataBuffer, begine_swap);
                                begine_swap += 8;
                                objId310.Y = System.BitConverter.ToDouble(dataBuffer, begine_swap);
                                begine_swap += 8;
                                objId310.H = System.BitConverter.ToDouble(dataBuffer, begine_swap);
                                begine_swap += 8;
                                objId310.Poziom = System.BitConverter.ToInt16(dataBuffer, begine_swap);
                                begine_swap += 2;
                                objId310.PositionConfidence = dataBuffer[begine_swap];
                                begine_swap += 1;
                                objId310.SpeedNavigationPoint = System.BitConverter.ToDouble(dataBuffer, begine_swap);
                                begine_swap += 8;
                                objId310.State = dataBuffer[begine_swap];
                                begine_swap += 1;
                                objId310.BatteryLeve = System.BitConverter.ToDouble(dataBuffer, begine_swap);
                                begine_swap += 8;
                                objId310.AutoOrManual = dataBuffer[begine_swap];
                                begine_swap += 1;
                                objId310.PositionInitialized = dataBuffer[begine_swap];
                                begine_swap += 1;
                                objId310.LastSymbolPoint = System.BitConverter.ToInt32(dataBuffer, begine_swap);
                                begine_swap += 4;
                                objId310.MachineAtLastSymbolPoint = dataBuffer[begine_swap];
                                begine_swap += 1;
                                objId310.TargetSymbolPoint = System.BitConverter.ToInt32(dataBuffer, begine_swap);
                                begine_swap += 4;
                                objId310.MachineAtTarget = dataBuffer[begine_swap];
                                begine_swap += 1;
                                objId310.Operational = dataBuffer[begine_swap];
                                begine_swap += 1;
                                objId310.InProduction = dataBuffer[begine_swap];
                                begine_swap += 1;
                                objId310.LoadStatus = dataBuffer[begine_swap];
                                begine_swap += 1;
                                objId310.BatteryVoltage = System.BitConverter.ToDouble(dataBuffer, begine_swap);
                                begine_swap += 8;
                                objId310.ChargingStatus = receiveBuffer[begine_swap];
                                begine_swap += 1;
                                objId310.DistanceToTarget = System.BitConverter.ToSingle(dataBuffer, begine_swap);
                                begine_swap += 4;
                                objId310.CurrentDriveThroughPoint = System.BitConverter.ToInt32(dataBuffer, begine_swap);
                                begine_swap += 4;
                                objId310.NextLeveChangePointId = System.BitConverter.ToInt32(dataBuffer, begine_swap);
                                begine_swap += 4;
                                objId310.DistanceToNextLeveelChange = System.BitConverter.ToSingle(dataBuffer, begine_swap);
                                begine_swap += 4;
                                objId310.LastSymbolPointDrivenOver = System.BitConverter.ToInt32(dataBuffer, begine_swap);
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


                            var currentMachine = ListobjId310public.Find(x => x.MachineName == objId310.MachineName);
                            ListobjId310public.Remove(currentMachine);
                            if (currentMachine != null)
                            {
                                currentMachine.MachineID = objId310.MachineID;
                                currentMachine.X = objId310.X;
                                currentMachine.Y = objId310.Y;
                                currentMachine.H = objId310.H;
                                currentMachine.Poziom = objId310.Poziom;
                                currentMachine.PositionConfidence = objId310.PositionConfidence;
                                currentMachine.SpeedNavigationPoint = objId310.SpeedNavigationPoint;
                                currentMachine.State = objId310.State;
                                currentMachine.BatteryLeve = objId310.BatteryLeve;
                                currentMachine.AutoOrManual = objId310.AutoOrManual;
                                currentMachine.PositionInitialized = objId310.PositionInitialized;
                                currentMachine.LastSymbolPoint = objId310.LastSymbolPoint;
                                currentMachine.MachineAtLastSymbolPoint = objId310.MachineAtLastSymbolPoint;
                                currentMachine.TargetSymbolPoint = objId310.TargetSymbolPoint;
                                currentMachine.MachineAtTarget = objId310.MachineAtTarget;
                                currentMachine.Operational = objId310.Operational;
                                currentMachine.InProduction = objId310.InProduction;
                                currentMachine.LoadStatus = objId310.LoadStatus;
                                currentMachine.BatteryVoltage = objId310.BatteryVoltage;
                                currentMachine.ChargingStatus = objId310.ChargingStatus;
                                currentMachine.DistanceToTarget = objId310.DistanceToTarget;
                                currentMachine.CurrentDriveThroughPoint = objId310.CurrentDriveThroughPoint;
                                currentMachine.NextLeveChangePointId = objId310.NextLeveChangePointId;
                                currentMachine.DistanceToNextLeveelChange = objId310.DistanceToNextLeveelChange;
                                currentMachine.LastSymbolPointDrivenOver = objId310.LastSymbolPointDrivenOver;
                                currentMachine.UpdateTime = objId310.UpdateTime;
                            }
                            ListobjId310public.Add(currentMachine);
                            //ListobjId310public.Add(objId310);
                            //Console.WriteLine($" MachineID: {objId310.MachineID} , MachineName: {objId310.MachineName} , X: {objId310.X} , Y:  {objId310.Y} , Level : {objId310.Poziom} , PositionConfidence : {objId310.PositionConfidence} , State : {objId310.State} , BatteryLeve : {objId310.BatteryLeve} , LastSymbolPoint : {objId310.LastSymbolPoint} , UpdateTime : {objId310.UpdateTime} , ") ;


                            //State
                            string stateStringValue = "";

                            if (Enum.GetName(typeof(StateEnum), objId310.State) is not null)
                            {
                                stateStringValue = Enum.GetName(typeof(StateEnum), objId310.State);
                            }
                            else
                            {
                                stateStringValue = $"{objId310.State}";
                            }
                            //
                            Console.WriteLine(
                                $" MachineId: {objId310.MachineID}" +
                                $" ; X: {objId310.X}" +
                                $" ; Y: {objId310.Y}" +
                                $" ; H: {objId310.H}" +
                                $" ; Level: {objId310.Poziom}" +
                                $" ; PositionConfidence: {objId310.PositionConfidence}" +
                                $" ; SpeedNavigationPoint: {objId310.SpeedNavigationPoint}" +
                                $" ; State: {stateStringValue}" +
                                $" ; BatteryLevel: {objId310.BatteryLeve}" +
                                $" ; AutoOrManual: {Enum.GetName(typeof(AutoOrManualEnum),objId310.AutoOrManual)}" +
                                $" ; PositionInitialized: {objId310.PositionInitialized}" +
                                $" ; LastSymbolPoint: {objId310.LastSymbolPoint}" +
                                $" ; MachineAtLastSymbolPoint: {objId310.MachineAtLastSymbolPoint}" +
                                $" ; TargetSymbolPoint: {objId310.TargetSymbolPoint}" +
                                $" ; MachineAtTarget: {objId310.MachineAtTarget}" +
                                $" ; Operational: {Enum.GetName(typeof(OperationalEnum), objId310.Operational)}" +
                                $" ; InProduction:{Enum.GetName(typeof(InProductionEnum),objId310.InProduction)}" +
                                $" ; LoadStatus: {Enum.GetName(typeof(LoadStatusEnum), objId310.LoadStatus)}" +
                                $" ; BatteryVoltage: {objId310.BatteryVoltage}" +   // z "Battery voltage"
                                $" ; ChargingStatus: {Enum.GetName(typeof(ChargingStatusEnum), objId310.ChargingStatus)}" +
                                $" ; DistanceToTarget: {objId310.DistanceToTarget}" +
                                $" ; CurrentDriveThroughPoint: {objId310.CurrentDriveThroughPoint}" +
                                $" ; NextLevelChangePointId: {objId310.NextLeveChangePointId}" +
                                $" ; DistanceToNextLevelChange: {objId310.DistanceToNextLeveelChange}"+
                                $" ; LastSymbolPointDrivenOver: {objId310.LastSymbolPointDrivenOver}"+
                                ";"
                             );
                        }
                        #endregion
                        //
                        #region 349
                        // ErrorsV2 (ID = 349)
                        //This message is sent as a response to ErrorsV2 request (ID = 56).
                        if (iD == 349)
                        {

                            //
                            ListobjId349public.Clear();
                            DateTime czas = DateTime.Now;
                            //Console.WriteLine("ID = 309 | czas: " + czas);
                            //
                            //Sprawdzenie ilości przychodzących alarmów 
                            UInt16 NumberOfErrors = System.BitConverter.ToUInt16(dataBuffer, 0);
                            Console.WriteLine("Ilość elementów w liście Alarmów AGV : "+NumberOfErrors);
                            //8.1. Frame
                            //Every message has a frame that consists of following data. Sender ID and Receiver ID are specified as follow:
                            //Navithor: ID = 1000, MES clients: ID = 1001, 1002, ... For example when sending a message from MES to Navithor
                            //Sender ID = 1001 and Receiver ID = 1000.
                            //int begine = 11;
                            int begine = 2;
                            int begine_swap = begine;
                            UInt16 i = 0;
                            //
#if !DEBUG
                            Console.SetOut(new MyLoger("W:\\BackgroundTasks\\AGV_TCP_IP_v2\\Logs_ElokonFunction_TEMP"));
#else
                            Console.SetOut(new MyLoger("D:\\AGV_TCP_IP_v2\\Logs_ElokonFunction"));
#endif
                            Console.WriteLine("");
                            while (i < NumberOfErrors)
                            {
                                ID_349 objId349 = new ID_349();
                                //
                                // Object
                                objId349.ErrorId = System.BitConverter.ToUInt32(dataBuffer, begine_swap);
                                begine_swap += 4;
                                objId349.NameStringLength = System.BitConverter.ToUInt16(dataBuffer, begine_swap);
                                begine_swap += 2;
                                objId349.Name = System.Text.Encoding.ASCII.GetString(dataBuffer, begine_swap, objId349.NameStringLength);
                                begine_swap += objId349.NameStringLength;
                                objId349.ErrorType = (EnumErrorType)System.BitConverter.ToUInt16(dataBuffer, begine_swap);
                                begine_swap += 2;
                                objId349.EntityID = System.BitConverter.ToUInt32(dataBuffer, begine_swap);
                                begine_swap += 4;
                                objId349.Source = (EnumSource)dataBuffer[begine_swap];
                                begine_swap += 1;
                                objId349.Level = dataBuffer[begine_swap];
                                begine_swap += 1;
                                objId349.Value = System.BitConverter.ToUInt16(dataBuffer, begine_swap);
                                begine_swap += 2;
                                objId349.Priority = System.BitConverter.ToInt16(dataBuffer, begine_swap);
                                begine_swap += 2;
                                //
                                ListobjId349public.Add(objId349);
                                i += 1;
                                //
                                Console.WriteLine($"LOCAL : Id: {objId349.ErrorId} | Alarm: {objId349.Name} | Maszyna AGV:  {objId349.EntityID} | Źródło : {objId349.Source}");
                            }
                            Console.WriteLine("");
                        }



                        //
                        // Request po Errors_v2
                        if (AgvAlarmsFrameSend_State)
                        {
                            networkStream.Write(sendFrame, 0, 19);
                            //
#if !DEBUG
                            Console.SetOut(new MyLoger("W:\\BackgroundTasks\\AGV_TCP_IP_v2\\logs_TEMP"));
#else
                            Console.SetOut(new MyLoger("D:\\AGV_TCP_IP_v2\\logs"));
#endif
                            Console.WriteLine("Ramka ID:56 wysłana ...");
                        }
                        AgvAlarmsFrameSend_State = false;
                        //
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine($"Error Exception ; {ex}");
                    }
                //_______________________________________________
                //
                //THREADs
                //
                //_______________________________________________
                //
                int a = updateDatainSQL_Thread.ThreadState.GetHashCode();
                    //ThreadState.U
                    if (a == 12 || a == 8)
                    {
                        updateDatainSQL_Thread.IsBackground = true;
                        updateDatainSQL_Thread.Start();
                    }
                    int b = updateAgvAlarms_56_Thread.ThreadState.GetHashCode();
                    //ThreadState.U
                    if (b == 12 || b == 8)
                    {
                        updateAgvAlarms_56_Thread.IsBackground = true;
                        updateAgvAlarms_56_Thread.Start();
                    }
                }
                //
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
                Console.WriteLine(DateTime.Now);
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

                            //Update alarms to pozmda02 but only changes

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
                            IEnumerable<ID_349> toDelete = Enumerable.Empty<ID_349>(); 
                            //
                            if (! (ListobjId349publicLastUpdated == null)) 
                            {
                                toDelete = ListobjId349publicLastUpdated.ExceptBy(ListobjId349public, x => new { x.EntityID, x.ErrorId, x.ErrorType, x.Level, x.Name, x.NameStringLength, x.Priority, x.Source, x.Value });
                                //
                            }
                            //
                            AGV_AlarmsUpdateRequest requestData = new AGV_AlarmsUpdateRequest
                            {
                                DataToAdd = toAdd.ToList(),
                                DataToDelete = toDelete.ToList()
                            };
                            response310 = new HttpResponseMessage(HttpStatusCode.OK);
                            response349 = new HttpResponseMessage(HttpStatusCode.OK);
                            if (requestData.DataToAdd.Count > 0 || requestData.DataToDelete.Count > 0)
                            {
                                if (requestData.DataToAdd.Count > 0) { 
                                ListobjId349publicLastUpdated.AddRange(toAdd);
                                }
                                if (requestData.DataToDelete.Count > 0)
                                {
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
                                }
                                response349 = new HttpResponseMessage(HttpStatusCode.BadRequest);
                                response349 = await client.PostAsJsonAsync($"{HttpSerwerURI}/api/Agv/AGV_AlarmsUpdate_v2.1.0/", requestData);
                                //Console.WriteLine("Aktualizacja ALARMÓW AGV do DB");
                            }
                            else
                            {
                                if (ListobjId349public.Count > 0)
                                {
                                    //Console.WriteLine("Brak ALARMÓW do wysłania --> zgodnosć między ramką a DB");
                                }
                                else
                                {
                                    //Console.WriteLine("Brak ALARMÓW do wysłania - BRAK alarmów w ramce");
                                }


                            }
                            #endregion

                            #region Update MACHINES AGV
                            if (ListobjId310public.Count > 0)
                            {
                                if (ListobjId310public.Count > 1 && ListobjId310publicLastUpdated[1].MachineName == "A-Mate2")
                                {
                                    ListobjId310publicLastUpdated[1].MachineName = "A-Mate 2";
                                }
                                ID_310 object310LastUpdated = ListobjId310publicLastUpdated.FirstOrDefault(s => s.MachineName == ListobjId310public[0].MachineName);
                                //
                                //
                                // Porównanie obiektów i sprawdzenie czy są rówżnice w obiekcie który przyszedł względem obiektu który
                                // przechowuje ostatnio zaktualizowane maszyny AGV.
                                //
                                bool toUpdate = Extensions.Excepts(ListobjId310public[0], ListobjId310publicLastUpdated, x => new
                                {
                                    x.MachineName,
                                    x.X,
                                    x.Y,
                                    x.H,
                                    x.Poziom,
                                    x.PositionConfidence,
                                    x.SpeedNavigationPoint,
                                    x.State,
                                    x.BatteryLeve,
                                    x.AutoOrManual,
                                    x.PositionInitialized,
                                    x.LastSymbolPoint,
                                    x.MachineAtLastSymbolPoint,
                                    x.TargetSymbolPoint,
                                    x.MachineAtTarget,
                                    x.Operational,
                                    x.InProduction,
                                    x.LoadStatus,
                                    x.BatteryVoltage,
                                    x.ChargingStatus,
                                    x.DistanceToTarget,
                                    x.CurrentDriveThroughPoint,
                                    x.NextLeveChangePointId,
                                    x.DistanceToNextLeveelChange,
                                    x.LastSymbolPointDrivenOver
                                });
                                // false gdy jest zgodnosc obiektów    - NIE UAKTUALNIAMY DANYCH 
                                // true  gdy jest niezgodnosc obiektów - UAKTUALNIAMY DANE
                                if (toUpdate)
                                {
                                    response310 = new HttpResponseMessage(HttpStatusCode.BadRequest);
                                    response310 = await client.PostAsJsonAsync($"{HttpSerwerURI}/api/Agv/AGV_MachinesStatusUpdate/", ListobjId310public);
                                    //
                                    // Aktualizacja obiektu w liście MASZYN AGV który przychodzi do aktualizacji.
                                    //
                                    #region Updated data in List model ID_310
                                    ListobjId310publicLastUpdated = ListobjId310publicLastUpdated.Select(obj =>
                                        {
                                            if (obj.MachineName == ListobjId310public[0].MachineName)
                                            {
                                                obj = Mappers.dataID_310_toDto(ListobjId310public[0]);
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
                                    //Console.WriteLine("Aktualizacja MASZYNY dla " + ListobjId310public.Count + " maszyny. Dla maszyny: " + machineListString);
                                }
                                else
                                {
                                    //Console.WriteLine("Dane MASZYN AGV nie wysłane z uwagi na tę samą zawartość");
                                }
                            }
                            else
                            {
                                //Console.WriteLine("Brak danych z MASZYN AGV do wysłania Ilość = 0 ");
                            }

                            #endregion
                            if (response310.IsSuccessStatusCode && response349.IsSuccessStatusCode)
                            {
                                //ListobjId310public.Clear();
                                DateTime czas = DateTime.Now;
                                //Console.WriteLine("Work work work ...");
                            }
                            else
                            {
                                ListobjId310publicLastUpdated.Clear();
                                ListobjId349publicLastUpdated.Clear();
                                //Console.WriteLine("Error Occurred  sending machine status: "); ;
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

        //
        static void UpdateVarialeToSendFrame_56() 
        {
            while (true)
            {
                Thread.Sleep(1000);
                AgvAlarmsFrameSend_State = true;
            }
        }
            
        private static readonly object _lock349 = new object();
        private static readonly object _lock310 = new object();
        //
        static void WriteDataToTXT()
        {
            while (true)
            {
#if !DEBUG
                Console.SetOut(new MyLoger("W:\\BackgroundTasks\\AGV_TCP_IP_v2\\Logs_ElokonFunction_TEMP"));
#else
                Console.SetOut(new MyLoger("D:\\AGV_TCP_IP_v2\\Logs_ElokonFunction"));
#endif
                Console.WriteLine("");
                //
                try
                {
                    List<ID_349> localData349;
                    List<ID_310> localData310;

                    lock (_lock349)
                    {
                        localData349 = new List<ID_349>(ListobjId349public);
                    }

                    lock (_lock310)
                    {
                        localData310 = new List<ID_310>(ListobjId310public);
                    }

                    //
                    if (!(localData349.Count > 0))
                    {
                        Console.WriteLine("Brak danych o alarmach AGV do wysłania ...");
                    }
                    else
                    {
                        foreach (var record in localData349)
                        {
                            Console.WriteLine($" Id: {record.ErrorId} | Alarm: {record.Name} | Maszyna AGV:  {record.EntityID} | Źródło : {record.Source}");
                        }
                    }
                    Console.WriteLine("");
                    //
#if !DEBUG
                Console.SetOut(new MyLoger("W:\\BackgroundTasks\\AGV_TCP_IP_v2\\Logs_MachineAGV_TEMP"));
#else 
                    Console.SetOut(new MyLoger("D:\\AGV_TCP_IP_v2\\Logs_MachineAGV"));
#endif
                    Console.WriteLine("");
                    if (!(localData310.Count > 0))
                    {
                        Console.WriteLine("Brak danych maszyn AGV do wysłania ...");
                    }
                    else
                    {
                        foreach (var record in localData310)
                        {
                            Console.WriteLine($" MachineID: {record.MachineID} | MachineName: {record.MachineName} | X: {record.X} | Y:  {record.Y} | PositionConfidence : {record.PositionConfidence} | State : {record.State} | BatteryLeve : {record.BatteryLeve} | LastSymbolPoint : {record.LastSymbolPoint} | UpdateTime : {record.UpdateTime}");
                        }
                    }
                    Console.WriteLine("");
                    Thread.Sleep(10000);
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error during write lgs to .txt");
                    Console.WriteLine(e);
                }
            }
        }

    }
}
