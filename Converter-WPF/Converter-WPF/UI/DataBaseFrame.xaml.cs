using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using System;

using DATABASE;
using StringExtension;

//ВЗАИМОДЕЙСТВИЕ ТОЛЬКО С БАЗОЙ ДАННЫХ

//tastks:
//-- add or remove edit data in datagrid;

namespace Converter_WPF
{
    /// <summary>
    /// Логика взаимодействия для ShowDataBase.xaml
    /// </summary>
    public partial class DataBaseFrame : Page
    {
		private List<Currency> dataGridList;

		public DataBaseFrame()
        {
			InitializeComponent();
            tbox_NewCurrency_Code.TextChanged += tbox_newCurrency_Code_TextChanged;

			dataGridList = new List<Currency>(MainWindow.databaseAPI.Currencies);
			currencyDataGrid.ItemsSource = dataGridList;

			currencyDataGrid_NameCollumn.IsReadOnly = !MainWindow.databaseAPI.isNameEditAllowed;
			currencyDataGrid_RateCollumn.IsReadOnly = !MainWindow.databaseAPI.isRateEditAllowed;

			btn_CurrAdd.IsEnabled = false;
        }

		#region NewCurrencyHandlers

        bool isValid_Code = false;
		bool isValid_Rate = false;
		private void tbox_newCurrency_Code_TextChanged(object sender, TextChangedEventArgs e)
		{
			if ((tbox_NewCurrency_Code.Text.isLetter() && tbox_NewCurrency_Code.Text.Length == 3) || tbox_NewCurrency_Code.Text.Length == 0)
			{
				border_newCrnc.Visibility = Visibility.Hidden;
				if (tbox_NewCurrency_Code.Text.Length == 3)
					isValid_Code = true;
			}
			else
			{
				border_newCrnc.Visibility = Visibility.Visible;
				isValid_Code = false;
			}

			if (isValid_Code && isValid_Rate)
				btn_CurrAdd.IsEnabled = true;
			else
				btn_CurrAdd.IsEnabled = false;
		}
		private void tbox_newCurrency_Rate_TextChanged(object sender, TextChangedEventArgs e)
        {
			isValid_Rate = tbox_NewCurrency_Rate.Text.isNumber() ? true : false;

			if (isValid_Code && isValid_Rate)
				btn_CurrAdd.IsEnabled = true;
			else
				btn_CurrAdd.IsEnabled = false;
		}

		private void btn_CurrAdd_Click(object sender, RoutedEventArgs e)
		{
			btn_CurrAdd.IsEnabled = false;
			string addStatus = "ADDED";
			bool isAddAvailable = true;
			Border border = new Border();
		
			for (int i = 0; i < MainWindow.databaseAPI.Currencies.Count - 1; i++)
				if (tbox_NewCurrency_Code.Text == MainWindow.databaseAPI.Currencies[i].Name)
				{
					isAddAvailable = false;
					border.Visibility = Visibility.Visible;
					addStatus = "ERROR";
					break;
				}
			if (isAddAvailable)
			{
				Currency currentCurrency = new Currency();
				
				currentCurrency.Name = tbox_NewCurrency_Code.Text;
				currentCurrency.Rate = Convert.ToDouble(tbox_NewCurrency_Rate.Text);

				object lockObj = new object();
				BindingOperations.EnableCollectionSynchronization(MainWindow.databaseAPI.Currencies, lockObj);

				MainWindow.databaseAPI.Currencies.Add(currentCurrency);

				CollectionViewSource.GetDefaultView(currencyDataGrid.ItemsSource).Refresh();

				MainWindow.databaseAPI.Save();
			}
			else
				border.Visibility = Visibility.Visible;

			tbox_NewCurrency_Code.TextChanged -= tbox_newCurrency_Code_TextChanged;
			tbox_NewCurrency_Rate.TextChanged -= tbox_newCurrency_Rate_TextChanged;
			tbox_NewCurrency_Code.Text = "CURRENCY";
			tbox_NewCurrency_Rate.Text = addStatus;



			var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
		
			dispatcherTimer.Tick += delegate
			{
				tbox_NewCurrency_Code.TextChanged += tbox_newCurrency_Code_TextChanged;
				tbox_NewCurrency_Code.Text = "";

				tbox_NewCurrency_Rate.TextChanged += tbox_newCurrency_Rate_TextChanged;
				tbox_NewCurrency_Rate.Text = "";
				dispatcherTimer.Stop();
			};
		
			dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
			dispatcherTimer.Start();
		}

        #endregion
		
        private void btn_dbSave_Click(object sender, RoutedEventArgs e)
        {
			// dataGridList Validation
			// Update MainWindow.databaseAPI.Currencies from validated dataGridList

			MainWindow.databaseAPI.Save();
        }

    }
}