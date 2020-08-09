using System;

namespace String_Advanced
{
	public static class StringOPS
	{
		public static bool isNumber(string str)
		{
			double varriable;
			return double.TryParse(str, out varriable);
		}

		public static bool isLetter(string str)
		{
			for (int i = 0; i < str.Length; i++)
				if (!Char.IsLetter(str[i]))
					return false;

			return true;
		}

		public static bool isUpper(string str)
		{
			for (int i = 0; i < str.Length; i++)
				if (!Char.IsUpper(str[i]))
					return false;

			return true;
		}
	}
}
