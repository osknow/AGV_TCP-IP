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
    public class AGV_UpdateAlarm
    {
            public static async Task<HttpResponseMessage> Get(int id, bool state)
            {
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        string url = "https://pozmda02.duni.org/api/agv/AGV_AlarmOccur/"+id+"/"+state;
                        return await client.GetAsync(url);

                        //return response;
                    }
                    catch (HttpRequestException e)
                    {
                        throw e;
                    }
                }
            }
    }
}
