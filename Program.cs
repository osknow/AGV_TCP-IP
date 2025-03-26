using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using AGV_TcpIp_ConsoleApp.Model;
using AGV_TcpIp_ConsoleApp.SubProgramLogic;
using AGV_TcpIp_ConsoleApp.SubPrograms;

namespace AGV_TcpIp_ConsoleApp
{
    public class Program
    {

        // Czas cyklicznego odświeżenia danych w bazie danych 
        public static int SQL_UpdateTime = 5000;
        public static List<ID_309> ListobjId309public { get; set; } = new List<ID_309>();
        public static List<ID_310> ListobjId310public { get; set; } = new List<ID_310>();
        public static List<ID_349> ListobjId349public { get; set; } = new List<ID_349>();
        //
        static ID_310 objId310 = new ID_310();
        static ID_349 objId349 = new ID_349();
        //
        static Thread t = new Thread(new ThreadStart(UpdateDatainSQL));
        public static Thread ele_TaskAlarms = new Thread(new ThreadStart(TaskEleAlarms));
        static Thread ele_TaskWarnings = new Thread(new ThreadStart(  TaskEleWarnings));
        //static Thread send_TCP_ID_56 = new Thread(new ThreadStart(  SendTCP_ID_56));

        // TESTY LOKALNE 
        public static string HttpSerwerURI { get; set; } = "https://localhost:44396";
        //public static string HttpSerwerURI { get; set; } = "https://pozmda02.duni.org";

        static bool TcpStatusConnection;
        //public NetworkStream networkStream=new NetworkStream();
        static   async Task Main()
        {
#if !DEBUG
            Console.SetOut(new MyLoger("W:\\BackgroundTasks\\AGV_TCP_IP\\logs"));
#endif
            try
            {
                await TcpIp();
            }
            catch(Exception ex)
            {
                    Console.WriteLine("Reconnect to TCP Server ..."+DateTime.Now);
                    Console.WriteLine(ex.Message);
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
                using TcpClient client = new TcpClient();
                await client.ConnectAsync(hostAdress, 8015);
                await using NetworkStream networkStream = client.GetStream();
                DateTime start = DateTime.Now;

                while (client.Connected)
                {
                    TcpStatusConnection = true;
                    // Console.WriteLine("Live state ....");
                    byte[] receiveBuffer = new byte[1024];
                    int readTotal;
                    //
                    networkStream.Write(sendFrame, 0, 19);
                    //
                    var output = networkStream.Read(receiveBuffer, 0, receiveBuffer.Length);
                    int iD = System.BitConverter.ToInt16(receiveBuffer, 0);
                    String responseData = String.Empty;
                    //
                    //
                    #region 309 Temporary Bocked
                    //Errors (ID = 309)
                    // This message is sent periodically from Navithor to MES. Message contains current error status in the system.
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
                    #region 310 Temprary Blocked
                    //if (iD == 310)
                    //{


                    //    DateTime czas = DateTime.Now;
                    //    //Console.WriteLine("ID = 310 | czas: " + czas);
                    //    int begine = 9;
                    //    int begine_swap = begine;
                    //    ListobjId310public.Clear();
                    //    //99 Max Length of data Frame for this comand from documentation
                    //    while (99 > begine_swap)
                    //    {
                    //        objId310.MachineID = System.BitConverter.ToUInt16(receiveBuffer, begine_swap);
                    //        begine_swap += 2;
                    //        objId310.X = System.BitConverter.ToDouble(receiveBuffer, begine_swap);
                    //        begine_swap += 8;
                    //        objId310.Y = System.BitConverter.ToDouble(receiveBuffer, begine_swap);
                    //        begine_swap += 8;
                    //        objId310.H = System.BitConverter.ToDouble(receiveBuffer, begine_swap);
                    //        begine_swap += 8;
                    //        objId310.Poziom = System.BitConverter.ToInt16(receiveBuffer, begine_swap);
                    //        begine_swap += 2;
                    //        objId310.PositionConfidence = receiveBuffer[begine_swap];
                    //        begine_swap += 1;
                    //        objId310.SpeedNavigationPoint = System.BitConverter.ToDouble(receiveBuffer, begine_swap);
                    //        begine_swap += 8;
                    //        objId310.State = receiveBuffer[begine_swap];
                    //        begine_swap += 1;
                    //        objId310.BatteryLeve = System.BitConverter.ToDouble(receiveBuffer, begine_swap);
                    //        begine_swap += 8;
                    //        objId310.AutoOrManual = receiveBuffer[begine_swap];
                    //        begine_swap += 1;
                    //        objId310.PositionInitialized = receiveBuffer[begine_swap];
                    //        begine_swap += 1;
                    //        objId310.LastSymbolPoint = System.BitConverter.ToInt32(receiveBuffer, begine_swap);
                    //        begine_swap += 4;
                    //        objId310.MachineAtLastSymbolPoint = receiveBuffer[begine_swap];
                    //        begine_swap += 1;
                    //        objId310.TargetSymbolPoint = System.BitConverter.ToInt32(receiveBuffer, begine_swap);
                    //        begine_swap += 4;
                    //        objId310.MachineAtTarget = receiveBuffer[begine_swap];
                    //        begine_swap += 1;
                    //        objId310.Operational = receiveBuffer[begine_swap];
                    //        begine_swap += 1;
                    //        objId310.InProduction = receiveBuffer[begine_swap];
                    //        begine_swap += 1;
                    //        objId310.LoadStatus = receiveBuffer[begine_swap];
                    //        begine_swap += 1;
                    //        objId310.BatteryVoltage = System.BitConverter.ToDouble(receiveBuffer, begine_swap);
                    //        begine_swap += 8;
                    //        objId310.ChargingStatus = receiveBuffer[begine_swap];
                    //        begine_swap += 1;
                    //        objId310.DistanceToTarget = System.BitConverter.ToSingle(receiveBuffer, begine_swap);
                    //        begine_swap += 4;
                    //        objId310.CurrentDriveThroughPoint = System.BitConverter.ToInt32(receiveBuffer, begine_swap);
                    //        begine_swap += 4;
                    //        objId310.NextLeveChangePointId = System.BitConverter.ToInt32(receiveBuffer, begine_swap);
                    //        begine_swap += 4;
                    //        objId310.DistanceToNextLeveelChange = System.BitConverter.ToSingle(receiveBuffer, begine_swap);
                    //        begine_swap += 4;
                    //        objId310.LastSymbolPointDrivenOver = System.BitConverter.ToInt32(receiveBuffer, begine_swap);
                    //        begine_swap += 4;

                    //        objId310.UpdateTime = DateTime.Now;
                    //        // Nadanie nazw robotom
                    //        if (objId310.MachineID == 1)
                    //        {
                    //            objId310.MachineName = "A-Mate 1";
                    //        }
                    //        else if (objId310.MachineID == 2)
                    //        {
                    //            objId310.MachineName = "A-Mate 2";
                    //        }
                    //        else if (objId310.MachineID == 3)
                    //        {
                    //            objId310.MachineName = "A-Mate 3";
                    //        }
                    //    }
                    //    ListobjId310public.Add(objId310);
                    //}
                    #endregion
                    //
                    // ErrorsV2 (ID = 349)
                    //This message is sent as a response to ErrorsV2 request (ID = 56).
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
                    //
                    //Reconnect
                    if (output == 0)
                    {
                        Console.WriteLine("Connection lost ...");
                        networkStream.Close();
                        client.Close();
                        Console.WriteLine("Client disconnected");
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
                    //int d = send_TCP_ID_56.ThreadState.GetHashCode();
                    ////ThreadState.U
                    //if (d == 12 || d == 8)
                    //{
                    //    send_TCP_ID_56.IsBackground = true;
                    //    send_TCP_ID_56.Start();
                    //}
                    

                }
                if (client.Connected == false)
                {
                 Thread.Sleep(7000);

                }
            }
        }

        static async  void  UpdateDatainSQL()
        {
            while(true)
            {
                if (TcpStatusConnection)
                {
                    Thread.Sleep(millisecondsTimeout: SQL_UpdateTime);


                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            //
                            if (ListobjId349public.Count > 0) { 
                                HttpResponseMessage response = await client.PostAsJsonAsync($"{HttpSerwerURI}/api/Agv/AGV_AlarmsUpdate_v2/", ListobjId349public);
                                //
                                // TESTY BEZ UPDATU MASZYN
                                //
                                if (response.IsSuccessStatusCode && false)
                                {
                                 
                                    //
                                    HttpResponseMessage response310 = await client.PostAsJsonAsync($"{HttpSerwerURI}/api/Agv/AGV_MachinesStatusUpdate/", ListobjId310public);
                                    //
                                    if (response310.IsSuccessStatusCode)
                                    {
                                        DateTime czas = DateTime.Now;
                                        Console.WriteLine("Updated DataBase SQL ...");

                                    }
                                    else
                                    {
                                        Console.WriteLine("Error Occurred  sending machine status: "); ;
                                    }
                                }
                                else
                                {
                                    //Console.WriteLine("Error Occurred  in sending alarm list: "); ;
                                    Console.WriteLine("TESTY- Update SQL"); ;
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
        static async void SendTCP_ID_56()
        {
            // Adres IP i port docelowy
            string ipAddress = "10.3.0.43";
            int port = 8015;
         
            //
            try 
            {
                using (TcpClient client = new TcpClient(ipAddress, port))
                {
                    NetworkStream stream = client.GetStream();

                    // Wysyłanie ramki do serwera
                    //stream.Write(sendFrame, 0, 19);
                    Console.WriteLine("Ramka wysłana.");
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd: " + ex.Message);
            }
            Thread.Sleep(millisecondsTimeout: 5000);
        }
    }
}
