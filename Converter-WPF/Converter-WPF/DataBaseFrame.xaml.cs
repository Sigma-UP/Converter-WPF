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
		public DataBaseFrame()
        {
			InitializeComponent();
            tbox_NewCurrency_Code.TextChanged += tbox_newCurrency_Code_TextChanged;
			DataShow();
			
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
			TXT_DB dB = new TXT_DB("DB.txt");

			btn_CurrAdd.IsEnabled = false;
			string addStatus = "ADDED";
			bool isAddAvailable = true;
			Border border = new Border();
		
			for (int i = 0; i < MainWindow.currencies.Count - 1; i++)
				if (tbox_NewCurrency_Code.Text == MainWindow.currencies[i].Name)
				{
					isAddAvailable = false;
					border.Visibility = Visibility.Visible;
					addStatus = "ERROR";
					break;
				}
			if (isAddAvailable)
			{
				Currency currentCurrency = new Currency();
				
				currentCurrency.ID = MainWindow.currencies.Count;
				currentCurrency.Name = tbox_NewCurrency_Code.Text;
				currentCurrency.Rate = Convert.ToDouble(tbox_NewCurrency_Rate.Text);

				object lockObj = new object();
				BindingOperations.EnableCollectionSynchronization(MainWindow.currencies, lockObj);

				MainWindow.currencies.Add(currentCurrency);

				CollectionViewSource.GetDefaultView(currencyDataGrid.ItemsSource).Refresh();

				dB.Save(MainWindow.currencies);
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
		
		#region DatabaseShow
        private void DataShow()
        {
			MainWindow.currencies.Clear();
			
            if (MainWindow.DataBaseCode == 0)
				SHOW_txt();
			else if (MainWindow.DataBaseCode == 1)
				SHOW_MySql();
			else if (MainWindow.DataBaseCode == 2)
				SHOW_API();

			currencyDataGrid.ItemsSource = MainWindow.currencies;
		}
		private void SHOW_API()
        {
			//заполнение MainWindow.currencies с АПИ. Поля для заполнения:
			//ID  - int
			//Name - string
			//Rate - double
        }
		private void SHOW_txt()
        {
			TXT_DB dB = new TXT_DB("DB.txt");
			dB.GetInfo(MainWindow.currencies);
		}
		private void SHOW_MySql()
		{
			MySQL_DB dB = new MySQL_DB("localhost", 3306, "conv_db", "root", "Brotherhood13");
			dB.GetInfo(MainWindow.currencies);
		}

        private void btn_dbSave_Click(object sender, RoutedEventArgs e)
        {
			switch (MainWindow.DataBaseCode)
			{
				case 0:
					break;
				case 1:
					break;
				case 2:
					break;

                default:
                    break;
            }
        }

        #endregion



        //непонятная ебала
        //private void currencyDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        //{
        //	int selectedColumn = currencyDataGrid.CurrentCell.Column.DisplayIndex;
        //	var selectedCell = currencyDataGrid.SelectedCells[selectedColumn];
        //	var cellContent = selectedCell.Column.GetCellContent(selectedCell.Item);
        //	if (cellContent is TextBlock)
        //	{
        //		MessageBox.Show((cellContent as TextBlock).Text);
        //	}
        //}
    }
}