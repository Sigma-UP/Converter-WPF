using Converter_WPF.Core;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Converter_WPF
{
    public class CurrconvAPI : IDatabaseAPI
    {
        private string baseUrl { get; } = @"https://free.currconv.com";
        private string apiVersion { get; } = "v7";

        public string ApiKey { get; set; }

        public List<Currency> Currencies { get { throw new NotImplementedException(); } }

        public List<string> GetCurrenciesList()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            return;
        }

        public CurrconvAPI(string apiKey)
        {
            ApiKey = apiKey;
        }

        public double GetExchangeRate(string srcCurrency, string trgCurrency)
        {
            //string jsonString = GetResponse($"{baseUrl}/api/{apiVersion}/convert?q={srcCurrency}_{trgCurrency}&compact=ultra&apiKey={ApiKey}");
            //string rateString = jsonString.Substring(jsonString.IndexOf(':') + 1);
            //rateString = rateString.Trim('}').Replace(".", ",");
            //
            //return double.Parse(rateString);

            Thread.Sleep(300);
            return new Random().NextDouble(); 
        }

        public async Task<double> GetExchangeRateAsync(string srcCurrency, string trgCurrency)
        {
            return await Task.Run(() => GetExchangeRate(srcCurrency, trgCurrency));
        }

        private static string GetResponse(string url)
        {
            return Task.Run(() => new WebClient().DownloadString(url)).Result;
        }
    }
}
