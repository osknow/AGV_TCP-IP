using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using AGV_TcpIp_ConsoleApp.Model;

namespace AGV_TcpIp_ConsoleApp.SubPrograms
{
    public class AGV_UpdateAlarm
    {
            public static async Task<HttpResponseMessage> Get(int id, bool state)
            {
            HttpResponseMessage output = new HttpResponseMessage();
            using (HttpClient client = new HttpClient())
                {
                if(!(id == 0))
                    { 
                    try
                    {
                        string url = "https://pozmda02.duni.org/api/agv/AGV_AlarmOccur/"+id+"/"+state;
                        output = await client.GetAsync(url);
                        return output;

                        //return response;
                    }
                    catch (HttpRequestException e)
                    {
                        Console.WriteLine("ERROR | Bład podczas aktualizacji czasu wystąpienia awari AGV: " + e);
                        return output;
                    }
                    }
                else
                {

                    return output;
                }
                }
            }
    }
}
