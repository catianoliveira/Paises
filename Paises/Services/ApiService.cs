using Newtonsoft.Json;
using Paises.Modelos;
using Paises.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Paises.Services
{
    public class ApiService
    {
        public async Task<Response> GetCountries(string urlBase, string apiPath)
        {
            try
            {
                var client = new HttpClient(); 
                client.BaseAddress = new Uri(urlBase); 

                var response = await client.GetAsync(apiPath); 
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,

                    };
                }

                var countries = JsonConvert.DeserializeObject<List<Country>>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                return new Response
                {
                    IsSuccess = true,
                    Result = countries
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
    }
}
