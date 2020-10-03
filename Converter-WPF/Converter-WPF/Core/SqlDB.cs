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
	public class MySQL_DB : IDatabaseAPI
	{
		int port;
		string host;
		string database;
		string username;
		string password;

		public bool isNameEditAllowed { get; } = true;
		public bool isRateEditAllowed { get; } = true;

		public List<Currency> currencies;
		public List<Currency> Currencies { get { return currencies; } }

		public double GetExchangeRate(string srcCrnc, string trgCrnc)
		{
			return 0;
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
		public bool GetInfo()
		{
			try
			{
				GetDBConnection().Open();
			}
			catch (Exception)
			{
				return false;
			}

			MySqlConnection conn = GetDBConnection();
			conn.Open();

			try
			{
				QuerySelect(currencies, conn);
			}
			catch (Exception e)
			{
				return false;
			}
			finally
			{
				// Закрыть соединение.
				conn.Close();
				// Уничтожить объект, освободить ресурс.
				conn.Dispose();
			}

			return true;
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
						currentCurrency.Name = reader.GetString(reader.GetOrdinal("CURR_NAME"));
						currentCurrency.Rate = reader.GetDouble(reader.GetOrdinal("CURR_RARE"));
						outputList.Add(currentCurrency);
					}
				}
			}
		}
	}
}
