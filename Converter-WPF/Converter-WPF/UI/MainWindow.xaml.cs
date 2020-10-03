using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System;
using Converter_WPF.Core;
using DATABASE;
using System.Runtime.CompilerServices;

namespace Converter_WPF
{
	public partial class MainWindow : Window
	{
		public static IDatabaseAPI databaseAPI;

		private static TXT_DB txtDB;
		private static MySQL_DB sqlDB;
		private static CurrconvAPI currconvAPI;

		private static bool isTxtDBInitialized = false;
		private static bool isSqlDBInitialized = false;
		private static bool isOuterDBInitialized = false;

		private static int defaultDBIndex = 0;

		//sosi

		//Code 0 = Work with .txt-file only - only CurrencyName available,
		//Code 1 = Work with MySQL-DB - currency name and rare available and can be edit;
		//Code 2 = Work with CC-DB - currency name and rare available, but can`t be edit

		public static List<string> databases = new List<string>()
		{
			"txt-DB",
			"User-DB",
			"Outer-DB"
		};

		static MainWindow()
        {
			txtDB = new TXT_DB("DB.txt");

			sqlDB = new MySQL_DB("localhost", 3306, "conv_db", "root", "Brotherhood13");

			// apiKey 1: e31ff0cc7dbafe2d6e05
			// apiKey 2: 5f1965af1bceb7361177
			currconvAPI = new CurrconvAPI("5f1965af1bceb7361177");
		}

		public MainWindow()
		{
			InitializeComponent();

			cbox_Database.ItemsSource = databases;
			cbox_Database.SelectedIndex = defaultDBIndex;
		}

        #region WindowManipulation
        private void grid_header_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DragMove();
		}
		private void btn_close_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
        #endregion

        #region MenuNavigation

        private void btn_ChooseConverter_Click(object sender, RoutedEventArgs e)
        {
			MainContent.NavigationService.Navigate(new Uri("UI/ConverterFrame.xaml", UriKind.Relative));

			Height = 200;
			Width = 800;
		}
		private void btn_ChooseManage_Click(object sender, RoutedEventArgs e)
		{
			MainContent.NavigationService.Navigate(new Uri("UI/DataBaseFrame.xaml", UriKind.Relative));

			Height = 300;
			Width = 600;
		}

        #endregion

        private void cbox_Database_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			switch (cbox_Database.SelectedIndex)
            {
				case 0:
					if(!isTxtDBInitialized)
                    {
						txtDB.GetInfo();
						isTxtDBInitialized = true;
					}

					databaseAPI = txtDB;
					break;

				case 1:
					if (!isSqlDBInitialized)
						if (sqlDB.GetInfo())
							isSqlDBInitialized = true;
						else
						{
							MessageBox.Show("Failed to connect. Retry later.");
							cbox_Database.SelectedIndex = defaultDBIndex;
							break;
						}

					databaseAPI = sqlDB;
					break;

				case 2:
					if (!isOuterDBInitialized)
						if (currconvAPI.GetInfo())
							isOuterDBInitialized = true;
						else
						{
							MessageBox.Show("Failed to connect. Retry later.");
							cbox_Database.SelectedIndex = defaultDBIndex;
							break;
						}

					databaseAPI = currconvAPI;
					break;
            }

			MainContent.Refresh();
        }
    }
}

