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
    public class SendTask_pozmda02
    {
        public static async Task<HttpResponseMessage> POST(SendTask_pozmda02_body body)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = "https://pozmda02.duni.org/api/DuniTasks/electrical";
                    return await client.PostAsJsonAsync<SendTask_pozmda02_body>(url, body);

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
