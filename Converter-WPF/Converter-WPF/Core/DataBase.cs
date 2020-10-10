using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data.Common;
using StringExtension;
using System.Windows;
using Converter_WPF;
using System.IO;
using System;
using Converter_WPF.Core;

namespace DATABASE
{
	public class TXT_DB : IDatabaseAPI
	{
		private string path;

		public bool isNameEditAllowed { get; } = true;
		public bool isRateEditAllowed { get; } = false;

		public List<Currency> currency;
		public List<Currency> Currencies { get { return currency; } }

		public double GetExchangeRate(string srcCrnc, string trgCrnc)
        {
			return 0;
        }

		public List<string> GetCurrenciesList()
        {
			List<string> result = new List<string>();
			foreach (Currency crnc in currency)
				result.Add(crnc.Name);

			return result;
        }


        public TXT_DB(string path)
        {
			currency = new List<Currency>();

			path = this.path;
        }

		public void GetInfo()
		{
			if (Load())
				return;

			string[] defCrncs = new string[]
			{
				"USD", "EUR", "UAH", "AUD", "AZN",
				"ALL", "DZD", "XCD", "AOA", "ARS",
				"AWG", "AFN", "BSD", "BDT", "BBD",
				"BHD", "BYN", "XOF", "BOB", "BRL",
				"BIF", "BTN", "VUV", "GBP", "VES",
				"XAF", "VND", "GYD", "GHS", "GMD"
			};

			for (int i = 0; i < defCrncs.Length; i++) 
			{
				currency.Add(new Currency(defCrncs[i], 0));
			}

			Rewrite(currency);
		}

		private bool Load()
		{
			Validate();
			StreamReader sr;
			int i = 0;

			try
			{
				sr = new StreamReader(path);
			}
			catch
			{
				return false;
			}

			string line;
			while ((line = sr.ReadLine()) != null)
				if (line.Length == 3)
				{
					Currency currentCurrency = new Currency();

					currentCurrency.Name = line;
					currentCurrency.Rate = 1.0;

					currency.Add(currentCurrency);
				}
			sr.Close();


			return true;
		}

		public void Rewrite(List<Currency> currencies)
		{
			StreamWriter sw;

			try
			{
				sw = new StreamWriter(path);
			}
			catch
			{
				return;
			}

			for (int i = 0; i < currencies.Count; i++)
			{
				sw.WriteLine(currencies[i].Name);
				currencies[i].Rate = 1.0;
			}
			sw.Close();
		}

		private void Validate()
		{
			bool isDatabaseBroken = false;
			string currentLine;
			List<string> originalData = new List<string>();
			List<string> editData = new List<string>();

			StreamReader reader;
			try
				{ reader = new StreamReader(path); }
			catch
			{
				isDatabaseBroken = true;
				return;
			}

			while (!reader.EndOfStream)
			{
				currentLine = reader.ReadLine();
				originalData.Add(currentLine);
			}

			for (int i = 0; i < originalData.Count; i++)
			{
				bool isFormatBroken = false;
				bool isRepeat = false;

				if (originalData[i].Length == 3 && originalData[i].isLetter() && originalData[i].isUpper())
				{
					foreach (string existCurrency in editData)
						if (originalData[i] == existCurrency)
						{
							isRepeat = true;
							break;
						}
				}
				else
					isFormatBroken = true;

				if (!isRepeat && !isFormatBroken)
					editData.Add(originalData[i]);
				else
					isDatabaseBroken = true;
			}
			

			if (isDatabaseBroken)
			{
				reader.Close();
				StreamWriter sw;
				try
				{
					sw = new StreamWriter(path, false);
				}
				catch
				{
					return;
				}

				for (int i = 0; i < editData.Count; i++)
					sw.WriteLine(editData[i]);
				sw.Close();
			}
		}
	}

}