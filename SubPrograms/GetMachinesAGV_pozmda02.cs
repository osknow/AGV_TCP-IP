﻿using System;
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
        public static async Task<List<AGV_Machine>> Get()
        {
            string HttpSerwerURI = "https://pozmda02.duni.org/api/Agv/AGV_Machines";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    return  await client.GetFromJsonAsync<List<AGV_Machine>>(HttpSerwerURI);

                     
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}