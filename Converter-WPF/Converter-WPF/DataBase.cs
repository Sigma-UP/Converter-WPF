using System.IO;
using System.Collections.Generic;

using StringExtension;

namespace DATABASE
{
	public class TXT_DB
	{
		public delegate void AddNewCrncMethod(string currencyCode);


		public static void DB_Load(string db_path, AddNewCrncMethod AddCrnc)
		{
			if (LoadDataBase(db_path, AddCrnc))
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
			foreach (string crnc in defCrncs)
				AddCrnc(crnc);

			SaveDataBase(db_path, new List<string>(defCrncs));
		}

		private static bool LoadDataBase(string path, AddNewCrncMethod AddCrnc)
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
					AddCrnc(line);

			sr.Close();


			return true;
		}

		public static void SaveDataBase(string path, List<string> currencies)
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

			for (int i = 0; i < currencies.Count; i++)
				sw.WriteLine(currencies[i]);
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