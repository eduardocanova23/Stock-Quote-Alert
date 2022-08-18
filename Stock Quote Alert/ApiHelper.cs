using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockQuoteAlert
{
    public class ApiHelper
    {
        private static ApiHelper _instance;
        public static ApiHelper Instance => _instance ?? (_instance = new ApiHelper());

        // Private constructor so no one else can instantiate Foo
        private ApiHelper() { }

        // Função que chama a API e retorna uma string com os dados obtidos
        public string CallWebAPI(string ativ, double min, double max)
        {
            using (var client = new HttpClient())
            {
                // Construção da URL de chamada da API
                string url = "https://api.hgbrasil.com/finance/stock_price?key=32016adc&symbol="+ ativ;


                // Definindo cliente http para o receber os dados em JSON fornecido pela api
                var myClient = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });


                var response = client.GetAsync(url).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    return responseContent.ReadAsStringAsync().GetAwaiter().GetResult();
                }
                else
                {
                    throw new InvalidAPICall("Algo que foi inserido na chamada da API não está certo");
                }


                //Console.WriteLine(httpResponse.StatusCode);
            }
        }

    }
}
