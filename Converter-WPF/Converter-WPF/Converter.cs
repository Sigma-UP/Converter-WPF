using System.Net;

namespace Currency_Converter
{
    public class Converter
    {
        // apiKey: e31ff0cc7dbafe2d6e05

        private string baseUrl = @"https://free.currconv.com";
        private string apiVersion = "v7";

        private string ApiKey { get; }

        public Converter(string apiKey)
        {
            ApiKey = apiKey;
        }

        public double GetCurrencyRate(string srcCurrency, string trgCurrency)
        {
            string jsonString = GetResponse($"{baseUrl}/api/{apiVersion}/convert?q={srcCurrency}_{trgCurrency}&compact=ultra&apiKey={ApiKey}");
            string rateString = jsonString.Substring(jsonString.IndexOf(':') + 1);
            rateString = rateString.Trim('}').Replace(".", ",");

            return double.Parse(rateString);
        }

        public double Convert(double amount, string srcCurrency, string trgCurrency)
        {
            return amount * GetCurrencyRate(srcCurrency, trgCurrency);
        }


        private static string GetResponse(string url)
        {
            return new WebClient().DownloadString(url);
        }
    }
}
