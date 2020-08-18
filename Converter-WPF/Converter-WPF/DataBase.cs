using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

using StringExtension;

namespace DATABASE
{
	public class TXT_DB
	{
		public static void DB_Load(string db_path, ComboBox cb1, ComboBox cb2)
		{
			if (LoadDataBase(db_path, cb1, cb2))
				return;

			string[] defCrncs = new string[] {
			"USD", "EUR", "UAH", "AUD", "AZN",
			"ALL", "DZD", "XCD", "AOA", "ARS",
			"AWG", "AFN", "BSD", "BDT", "BBD",
			"BHD", "BYN", "XOF", "BOB", "BRL",
			"BIF", "BTN", "VUV", "GBP", "VES",
			"XAF", "VND", "GYD", "GHS", "GMD"
			};
			foreach (string crnc in defCrncs)
			{
				cb1.Items.Add(crnc);
				cb2.Items.Add(crnc);
			}
			SaveDataBase(db_path, cb1);
		}

		private static bool LoadDataBase(string path, ComboBox comboBox1, ComboBox comboBox2)
		{
			DB_Validate(path);
			StreamReader sr;

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
					comboBox1.Items.Add(line);
					comboBox2.Items.Add(line);
				}
			sr.Close();


			return true;
		}

		public static void SaveDataBase(string path, ComboBox cb)
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

			for (int i = 0; i < cb.Items.Count; i++)
				sw.WriteLine(cb.Items[i]);
			sw.Close();
		}

		public static void DB_Validate(string path)
		{
			bool isDatabaseBroken = false;
			string currentLine;
			List<string> originalData = new List<string>();
			List<string> editData = new List<string>();

			StreamReader reader;
            try
            {
				reader = new StreamReader(path);
			}
            catch
            {
				isDatabaseBroken = true;
				return;
            }
			
			if (!isDatabaseBroken)
			{
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
			}

			if(isDatabaseBroken)
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