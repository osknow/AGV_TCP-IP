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
        static ID_310 objId310 = new ID_310();
        static Thread t = new Thread(new ThreadStart(UpdateDatainSQL));
        public static Thread ele_TaskAlarms = new Thread(new ThreadStart(TaskEleAlarms));
        static Thread ele_TaskWarnings = new Thread(new ThreadStart(  TaskEleWarnings));
        //public static string HttpSerwerURI { get; set; } = "https://localhost:44396";
        public static string HttpSerwerURI { get; set; } = "https://pozmda02.duni.org";

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
                    var output = await networkStream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length);
                    int iD = System.BitConverter.ToInt16(receiveBuffer, 0);
                    String responseData = String.Empty;

                    if (iD == 309)
                    {
                        ListobjId309public.Clear();
                        DateTime czas = DateTime.Now;
                        //Console.WriteLine("ID = 309 | czas: " + czas);

                        int begine = 11;
                        int begine_swap = begine;

                        while (output > begine)
                        {
                            ID_309 objId309 = new ID_309();
                            int NameLength = System.BitConverter.ToInt16(receiveBuffer, begine_swap);
                            begine_swap += 2;
                            begine_swap += NameLength;
                            objId309.NumberId = System.BitConverter.ToUInt16(receiveBuffer, begine_swap);
                            begine_swap += 2;
                            objId309.Value = System.BitConverter.ToUInt16(receiveBuffer, begine_swap);
                            begine_swap += 2;
                            begine += 2;

                            string[] words = System.Text.Encoding.ASCII.GetString(receiveBuffer, begine, NameLength).Split(".");

                            objId309.Name = words[2];
                            objId309.Machine = words[1];
                            DateTime Time = DateTime.Now;
                            objId309.UpdatedTime = Time;

                            ListobjId309public.Add(objId309);


                            begine += NameLength + 4;
                        }
                    }


                    if (iD == 310)
                    {


                        DateTime czas = DateTime.Now;
                        //Console.WriteLine("ID = 310 | czas: " + czas);
                        int begine = 9;
                        int begine_swap = begine;
                        ListobjId310public.Clear();
                        //99 Max Length of data Frame for this comand from documentation
                        while (99 > begine_swap)
                        {
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
                        }
                        ListobjId310public.Add(objId310);
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
                            HttpResponseMessage response = await client.PostAsJsonAsync($"{HttpSerwerURI}/api/Agv/AGV_AlarmsUpdate/", ListobjId309public);
                            //
                            if (response.IsSuccessStatusCode)
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
                                Console.WriteLine("Error Occurred  in sending alarm list: "); ;
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

        static async void TaskEleAlarms()
        {
            while (true)
            {
                // Zadania Awari na tablet elektryka
                SendTask_Logic.UpdateTasks_ForElectricals(ListobjId310public);
                //
                Thread.Sleep(millisecondsTimeout: 10000);
            }
        }
        static async void TaskEleWarnings()
        {
            while (true)
            {
                // Zadanie o przeszkodzie na drodze na tablet elektryka
                // ZADANIA NIE MOGĄ BYĆ ASYNCHRONICZNE !!!!
                SendTask_Logic.RecognizeAlarm_Warning(ListobjId309public);
                //
                Thread.Sleep(millisecondsTimeout: 10000);
            }
        }
    }
}
