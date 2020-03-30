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
        public async Task<Response> GetCountries(string urlBase, string controller) //tarefa do metodo é devolver uma response - vai buscar taxas
        {
            try
            {
                var client = new HttpClient(); //faz a ligação externa via http
                client.BaseAddress = new Uri(urlBase); //endereço base, onde está a api

                var response = await client.GetAsync(controller); //controlador da api (/api/rates), await para app continuar a correr enquanto vai buscar taxas

                var result = await response.Content.ReadAsStringAsync(); //vai buscar conteudo do response e lê como uma string 

                if (!response.IsSuccessStatusCode)  //caso não se consiga ligar ou houver algum problema
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result
                    };
                }

                var countries = JsonConvert.DeserializeObject<List<Country>>(result);
                //usa o jsonconvert para converter numa lista do tipo rate

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
                    Message = ex.Message
                };
            }
        }
    }
}
