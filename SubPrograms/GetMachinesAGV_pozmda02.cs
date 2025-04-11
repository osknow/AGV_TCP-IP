using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using AGV_TcpIp_ConsoleApp.Model;

namespace AGV_TcpIp_ConsoleApp.SubPrograms
{
    class GetMachinesAGV_pozmda02
    {
        public static async Task<List<ID_310>> Get()
        {
            List <ID_310> empty = new List<ID_310>();
            string HttpSerwerURI = "https://pozmda02.duni.org:82/api/Agv/AGV_Machines";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    return  await client.GetFromJsonAsync<List<ID_310>>(HttpSerwerURI);

                     
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR | Błąd podczas odczytywania maszyn  z pozmda02: " + e);
                return empty;
            }
        }
    }
}
