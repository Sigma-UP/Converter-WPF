using Converter_WPF.Core;
using System;
using System.IO;
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

        public bool isNameEditAllowed { get; } = false;
        public bool isRateEditAllowed { get; } = false;

        private List<Currency> currencies;
        public List<Currency> Currencies { get { return currencies; } }


        public bool GetInfo()
        {
            string jsonString;

            try
            {
                jsonString = GetResponse($"{baseUrl}/api/{apiVersion}/currencies?apiKey={ApiKey}");
            }
            catch
            {
                return false;
            }

            jsonString = jsonString.Replace("\"results\":", "");
            jsonString = jsonString.Replace("\"", "");
            jsonString = jsonString.Replace("{", "");
            jsonString = jsonString.Replace("}", "");

            string crncList = string.Empty;
            foreach (string str in jsonString.Split(','))
                if (str.Contains("id:"))
                    crncList += str + '\n';
            crncList = crncList.Replace("id:", "");

            StringReader stringReader = new StringReader(crncList);
            currencies = new List<Currency>();

            while (true)
            {
                string str = stringReader.ReadLine();

                if (str != null)
                    currencies.Add(new Currency(str, 0));
                else
                    break;
            }

            return true;
        }

        public List<string> GetCurrenciesList()
        {
            List<string> result = new List<string>();

            foreach (Currency crnc in currencies)
                result.Add(crnc.Name);
            return result;
        }

        public double GetExchangeRate(string srcCurrency, string trgCurrency)
        {
            string jsonString;

            try
            {
                jsonString = GetResponse($"{baseUrl}/api/{apiVersion}/convert?q={srcCurrency}_{trgCurrency}&compact=ultra&apiKey={ApiKey}");
            }
            catch
            {
                return 0;
            }

            string rateString = jsonString.Substring(jsonString.IndexOf(':') + 1);
            rateString = rateString.Trim('}').Replace(".", ",");

            return double.Parse(rateString);

            //Thread.Sleep(300);
            //return new Random().NextDouble(); 
        }

        private static string GetResponse(string url)
        {
            return new WebClient().DownloadString(url);
        }

        public void Rewrite(List<Currency> currencies)
        {
            return;
        }

        public CurrconvAPI(string apiKey)
        {
            ApiKey = apiKey;
        }
    }
}
