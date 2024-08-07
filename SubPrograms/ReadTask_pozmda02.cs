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
    public class ReadTask_pozmda02
    {
            public static async Task<List<ReadTask_pozmda02_body>> Get()
            {
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        string url = "https://pozmda02.duni.org/api/DuniTasks/GetCurrentTasks/electrical";
                        return await client.GetFromJsonAsync<List<ReadTask_pozmda02_body>>(url);

                        //return response;
                    }
                    catch (HttpRequestException e)
                    {
                        throw;
                    }
                }
            }
    }
}
