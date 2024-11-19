using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AGV_TcpIp_ConsoleApp.SubPrograms
{
    class DeleteDuniTask_pozmda02
    {
        public static async Task DeleteTask(int id)
        {
            var url = $"https://pozmda02.duni.org/api/DuniTasks/endTaskByAIP/{id}";
            // LINK:
            //pozmda01.duni.org:81//api/DuniTasks/changeTaskStatusByAGV/{status}/{duniTaskDetails}"

            using (var client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.PostAsync(url, null);


                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(response.StatusCode + " | " + "Zadanie o id : " + id + " skasowane.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR | Błąd podczas kasowania zadania o id : " + id + ".");
                      
                }
            }
        }
    }
}
