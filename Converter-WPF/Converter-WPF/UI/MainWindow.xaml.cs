using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System;
using Converter_WPF.Core;
using DATABASE;

namespace Converter_WPF
{
	public partial class MainWindow : Window
	{
		public static IDatabaseAPI databaseAPI;

		private static TXT_DB txtDB;
		private static MySQL_DB sqlDB;
		private static CurrconvAPI currconvAPI;

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
		
		public static int DataBaseCode = 0;

		static MainWindow()
        {
			txtDB = new TXT_DB("DB.txt");
			txtDB.GetInfo();

			sqlDB = new MySQL_DB("localhost", 3306, "conv_db", "root", "Brotherhood13");
			sqlDB.GetInfo();

			currconvAPI = new CurrconvAPI("5f1965af1bceb7361177");
		}

		public MainWindow()
		{
			InitializeComponent();

			cbox_Database.ItemsSource = databases;
			cbox_Database.SelectedIndex = 0;
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
		}
		private void btn_ChooseManage_Click(object sender, RoutedEventArgs e)
		{
			MainContent.NavigationService.Navigate(new Uri("UI/DataBaseFrame.xaml", UriKind.Relative));
		}
        #endregion

        private void cbox_Database_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			DataBaseCode = cbox_Database.SelectedIndex;

			switch (cbox_Database.SelectedIndex)
            {
				case 0:
					databaseAPI = txtDB;
					break;

				case 1:
					databaseAPI = sqlDB;
					break;

				case 2:
					databaseAPI = currconvAPI;
					break;
            }

			MainContent.Refresh();
        }
    }
}

