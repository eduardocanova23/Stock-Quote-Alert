

using Newtonsoft.Json;

namespace API_Library
{
    public class StockProcessor
    {
        public static async Task<StockModel> LoadStock(string ativ)
        {
            string url = "https://api.hgbrasil.com/finance/stock_price?key=32016adc&symbol="+ ativ;

            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    StockModel stock = JsonConvert.DeserializeObject<StockModel>(
                        await response.Content.ReadAsStringAsync());

                    return stock;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
