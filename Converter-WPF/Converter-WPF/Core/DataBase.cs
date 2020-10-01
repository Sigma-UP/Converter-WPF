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

		public List<Currency> currency;
		public List<Currency> Currencies { get { return currency; } }

		public double GetExchangeRate(string srcCrnc, string trgCrnc)
        {
			return double.NaN;
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
				currency.Add(new Currency(i, defCrncs[i], 1.0));
			}

			Save();
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

					currentCurrency.ID = i;
					currentCurrency.Name = line;
					currentCurrency.Rate = 1.0;

					currency.Add(currentCurrency);
				}
			sr.Close();


			return true;
		}

		public void Save()
		{
			StreamWriter sw;

			try
			{
				sw = new StreamWriter(path, false);
			}
			catch
			{
				return;
			}

			for (int i = 0; i < currency.Count; i++)
			{
				sw.WriteLine(currency[i].Name);
				currency[i].ID = i;
				currency[i].Rate = 1.0;
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

	public class MySQL_DB : IDatabaseAPI
	{
		int port;
		string host;
		string database;
		string username;
		string password;

		public List<Currency> currencies;
		public List<Currency> Currencies { get { return currencies; } }

		public double GetExchangeRate(string srcCrnc, string trgCrnc)
        {
			throw new NotImplementedException();
        }

		public List<string> GetCurrenciesList()
        {
			List<string> result = new List<string>();
			foreach (Currency crnc in currencies)
				result.Add(crnc.Name);

			return result;
		}

		public void Save()
        {
			throw new NotImplementedException();
        }

		public MySQL_DB(string host, int port, string database, string username, string password)
		{
			this.host = host;
			this.port = port;
			this.database = database;
			this.username = username;
			this.password = password;

			currencies = new List<Currency>();
		}

		//getting connection
		public MySqlConnection GetDBConnection()
		{
			// Connection String
			String connString = "Server=" + this.host + ";Database=" + this.database
				+ ";port=" + this.port + ";User Id=" + this.username + ";password=" + this.password;

			MySqlConnection conn = new MySqlConnection(connString);

			return conn;
		}
		

		//getting FULL INFO from DATABASE by QueryCurrency
		public void GetInfo()
		{
            try
            {
				GetDBConnection().Open();
            }
            catch (Exception)
            {
				MessageBox.Show("Failed to connect. Retry later.");
				return;
            }

			MySqlConnection conn = GetDBConnection();
			conn.Open();

			try
			{
				QuerySelect(currencies, conn); 
			}
			catch (Exception e)
			{
				MessageBox.Show("Failed to query. Retry later.");
				return;
			}
			finally
			{
				// Закрыть соединение.
				conn.Close();
				// Уничтожить объект, освободить ресурс.
				conn.Dispose();
			}
		}
		
		//A Query that fullfill Combobox;
		private void QuerySelect(List<Currency> outputList, MySqlConnection conn)
		{
			string sql = "SELECT * FROM currencies;";

			// Создать объект Command.
			MySqlCommand cmd = new MySqlCommand();

			// Сочетать Command с Connection.
			cmd.Connection = conn;
			cmd.CommandText = sql;

			using (DbDataReader reader = cmd.ExecuteReader()) //using для контролируемого удаления reader за границами
			{
				if (reader.HasRows)
				{
					while (reader.Read())
					{
						Currency currentCurrency = new Currency();
						currentCurrency.ID = reader.GetInt32(reader.GetOrdinal("ID"));
						currentCurrency.Name = reader.GetString(reader.GetOrdinal("CURR_NAME"));
						currentCurrency.Rate = reader.GetDouble(reader.GetOrdinal("CURR_RARE"));
						outputList.Add(currentCurrency);
					}
				}
			}
		}
	} 
}