using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace StockQuoteAlert
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bem-Vindo ao monitoramento de ativos Stock Quote Alert");
            Console.WriteLine("------------------------------------------------------");

            // Primeira interação com o usuário, pedir os dados e armazená-los
            Console.WriteLine("Digite o código do ativo a ser monitorado, o preço de referência para venda e o preço de referência para compra");

            string parameters = Console.ReadLine();

            string[] parameters_list = parameters.Split(" ");

            string ativ = parameters_list[0];
            double min = double.Parse(parameters_list[1], CultureInfo.InvariantCulture);
            double max = double.Parse(parameters_list[2], CultureInfo.InvariantCulture);


            Console.WriteLine("Vou monitorar o ativo {0} e te mandar um email se a cotação desse ativo ficar abaixo de {1} reais," +
                " ou se ele ficar acima de {2} reais", ativ, min, max);

            Console.WriteLine("Por favor, insira o email que receberá os avisos");
            string endEmail = Console.ReadLine();

            while (true)
            {
                CallWebAPIAsync(ativ, min, max, endEmail)
                .Wait();
                Thread.Sleep(180000);
            }
        }

        // Função assíncrona que busca o preço do ativo desejado e 
        static async Task CallWebAPIAsync(string ativ, double min, double max, string endEmail)
        {
            using (var client = new HttpClient())
            {   
                // Construção da URL de chamada da API
                string url = "https://api.hgbrasil.com/finance/stock_price?key=32016adc&symbol="+ ativ;

                var myClient = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
                var response = await myClient.GetAsync(url);
                string jstring = await response.Content.ReadAsStringAsync();

                JObject json = JObject.Parse(jstring);

                // Extração do preço do objeto json que foi retornado pela API
                double price = Convert.ToDouble(json["results"][ativ.ToUpper()]["price"].ToString());

                // Comparar os valores dos preços e tomar a decisão de qual email enviar
                if (price < min)
                {
                    SendWarnEmail(false, ativ, min, max, endEmail);
                }
                if (price > max)
                {
                    SendWarnEmail(true, ativ, min, max, endEmail);
                }
                //Console.WriteLine(httpResponse.StatusCode);
            }
        }

        // Função de enviar email que ainda será feita
        static async Task SendWarnEmail(bool maior, string ativ, double min, double max, string endEmail)
        {
                Console.WriteLine("mandei o email teoricamente");
            
        }

    }
}