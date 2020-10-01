using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data.Common;
using StringExtension;
using System.Windows;
using Converter_WPF;
using System.IO;
using System;

namespace DATABASE
{
	public class TXT_DB
	{
		private string path;

        public TXT_DB(string path)
        {
			path = this.path;
        }

		public void GetInfo(List<Currency> currency)
		{
			if (Load(currency))
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

			for (int i = 0; i < defCrncs.Length - 1; i++) 
			{
				Currency currentCurrency = new Currency();

				currentCurrency.ID = i;
				currentCurrency.Name = defCrncs[i];
				currentCurrency.Rate = 1.0;

				currency.Add(currentCurrency);
			}

			Save(currency);
		}

		private bool Load(List<Currency> currency)
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

		public void Save(List<Currency> currency)
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

	public class MySQL_DB
	{
		string host;
		int port;
		string database;
		string username;
		string password;

		public MySQL_DB(string host, int port, string database, string username, string password)
		{
			this.host = host;
			this.port = port;
			this.database = database;
			this.username = username;
			this.password = password;
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
		public void GetInfo(List<Currency> outputList)
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
				QuerySelect(outputList, conn); 
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