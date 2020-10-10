using Converter_WPF;
using Converter_WPF.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Windows;

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
				double srcRate = -1, trgRate = -1;
				foreach(Currency currency in Currencies)
				{
					if (srcRate != -1 && trgRate != -1)
						break;
					
					if (srcCrnc == currency.Name)
						srcRate = currency.Rate;
					if (trgCrnc == currency.Name)
						trgRate = currency.Rate;	
				}	
				return srcRate/trgRate;
			}

			public List<string> GetCurrenciesList()
			{
				List<string> result = new List<string>();
				foreach (Currency crnc in currencies)
					result.Add(crnc.Name);

				return result;
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
				MySqlConnection conn = GetDBConnection();
				try
				{
					// open connection
					conn.Open();
					QuerySelect(currencies, conn);
				}
				catch (Exception)
				{
					return false;
				}
				finally
				{
					// close connection
					conn.Close();
					// delete object
					conn.Dispose();
				}

				return true;
			}

			//add created rows to DATABASE by QuerySelect
			public void Rewrite(List<Currency> newItems)
			{
				MySqlConnection conn = GetDBConnection();
				try
				{
					conn.Open();
					MySqlCommand cmd = new MySqlCommand();
					cmd.Connection = conn;
					cmd.CommandText = "delete from currencies ";
					cmd.ExecuteNonQuery();

					QueryInsert(newItems, conn);
				}
				catch (Exception e)
				{
					MessageBox.Show("ERROR: " + e);
				}
				finally
				{
					// close connection
					conn.Close();
					// delete object
					conn.Dispose();
				}
			}

			//DB_SELECT * 
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
			//DB_INSERT
			private void QueryInsert(List<Currency> insertList, MySqlConnection conn)
			{
				string sql = "INSERT INTO currencies (CURR_NAME, CURR_RARE)" 
							+ " VALUES (@CODE, @RATE) ";

				try
				{
					foreach(Currency item in insertList)
					{
						MySqlCommand cmd = new MySqlCommand();

						cmd.Connection = conn;
						cmd.CommandText = sql;
					
						MySqlParameter CODE = new MySqlParameter("@CODE", MySqlDbType.String);
						MySqlParameter RATE = new MySqlParameter("@RATE", MySqlDbType.Double);
					
						CODE.Value = item.Name;
						RATE.Value = item.Rate;

						cmd.Parameters.Add(CODE);
						cmd.Parameters.Add(RATE);
						cmd.ExecuteNonQuery();
					}
				}
				catch (Exception e)
				{
					MessageBox.Show("ERROR: " + e);
				}
			}
		}
}
