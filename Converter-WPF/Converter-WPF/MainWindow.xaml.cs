using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System;

namespace Converter_WPF
{
	public partial class MainWindow : Window
	{
		public static List<Currency> currencies = new List<Currency>();
		
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

		public MainWindow()
		{
			InitializeComponent();

			cbox_Database.ItemsSource = databases;
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
			MainContent.NavigationService.Navigate(new Uri("ConverterFrame.xaml", UriKind.Relative));
		}
		private void btn_ChooseManage_Click(object sender, RoutedEventArgs e)
		{
			MainContent.NavigationService.Navigate(new Uri("DataBaseFrame.xaml", UriKind.Relative));
		}
        #endregion

        private void cbox_Database_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			DataBaseCode = cbox_Database.SelectedIndex;

			if(MainContent.Source.ToString() == "ConverterFrame.xaml")
            {
				//ConverterFrame.cbox_FullFill();
            }
			else if(MainContent.Source.ToString() == "DataBaseFrame.xaml")
            {

            }

			MainContent.Refresh();
        }
    }
}

