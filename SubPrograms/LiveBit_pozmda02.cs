using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using AGV_TcpIp_ConsoleApp.Model;

namespace AGV_TcpIp_ConsoleApp.SubPrograms
{
    public class LiveBit_pozmda02
    {
        public static async Task<HttpResponseMessage> Get()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = new HttpResponseMessage();
                try
                {
                    string url = "https://pozmda02.duni.org/api/machine/LiveBitRequest";

                    response = await client.GetAsync(url);

                    return response;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("ERROR | Błąd podczas sprawdzenia stanu serwera pozmda02 - liveBit check. " + e);
                    return response;
                }
            }
        }
    }
}
