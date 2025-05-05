using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AGV_TcpIp_ConsoleApp.SubPrograms
{
    class ReadAGV_Alarms_v2_pozmda02
    {
        public static async Task<List<ID_349>> Get()
        {
            using (HttpClient client = new HttpClient())
            {
                List<ID_349> empty = new List<ID_349>();
                try
                {
                    string url = "https://pozmda02.duni.org/api/Agv/AGV_Alarms_v2";
                    return await client.GetFromJsonAsync<List<ID_349>>(url);

                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("ERROR | Bład podczas odczytywania zadań z pozmda02: " + e);
                    return empty;
                }
            }
        }
    }
}
