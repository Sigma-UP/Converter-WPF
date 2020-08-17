using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using DATABASE;
using String_Advanced;

namespace Converter_WPF
{
	public partial class MainWindow : Window
	{
		private double srcAmount;
		private double trgAmount;
		private double srcRate;
		private double trgRate;

		private bool isConvAvaliable;
		private bool isNormalConvMode;

		private string DB_path = "DB.txt";

		public MainWindow()
		{
			InitializeComponent();

			srcAmount = double.Parse(tbox_srcCrncAmount.Text);
			trgAmount = double.Parse(tbox_trgCrncAmount.Text);
			srcRate = double.Parse(tbox_srcRate.Text);
			trgRate = double.Parse(tbox_trgRate.Text);
			isConvAvaliable = true;
			isNormalConvMode = true;
			tbox_srcCrncAmount.TextChanged += tbox_srcCrncAmount_TextChanged;
			tbox_trgCrncAmount.TextChanged += tbox_trgCrncAmount_TextChanged;
			tbox_srcRate.TextChanged += tbox_srcRate_TextChanged;
			tbox_trgRate.TextChanged += tbox_trgRate_TextChanged;

			TextChangedEventHandler validate = delegate { ValidateTextBoxInput(); };
			tbox_srcCrncAmount.TextChanged += validate;
			tbox_trgCrncAmount.TextChanged += validate;
			tbox_srcRate.TextChanged += validate;
			tbox_trgRate.TextChanged += validate;

			cbox_srcCrnc.SelectionChanged += cbox_SelectionChanged;
			cbox_trgCrnc.SelectionChanged += cbox_SelectionChanged;
			cbox_srcCrnc.SelectionChanged += cbox_srcCrnc_SelectionChanged;
			cbox_trgCrnc.SelectionChanged += cbox_trgCrnc_SelectionChanged;
			cbox_srcCrnc.GotFocus += cbox_GotFocus;
			cbox_trgCrnc.GotFocus += cbox_GotFocus;
			cbox_srcCrnc.LostFocus += cbox_LostFocus;
			cbox_trgCrnc.LostFocus += cbox_LostFocus;

			TXT_DB.DB_Load(DB_path, cbox_srcCrnc, cbox_trgCrnc);

			cbox_srcCrnc.SelectedIndex = 0;
			cbox_trgCrnc.SelectedIndex = 1;
		}

		private bool ValidateTextBoxInput()
		{
			bool isValid = true;

			TextBox[] tboxes = new TextBox[] { tbox_srcCrncAmount, tbox_trgCrncAmount, tbox_srcRate, tbox_trgRate };

			foreach (var tbox in tboxes)
			{
				if (StringOPS.isNumber(tbox.Text) && double.Parse(tbox.Text) > 0)
					tbox.Background = Brushes.LightGray;
				else
				{
					tbox.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFAFAF");
					isValid = false;
				}
			}

			isConvAvaliable = isValid;
			return isValid;
		}
		private void btn_CurrAdd_Click(object sender, RoutedEventArgs e)
		{
			cbox_trgCrnc.Items.Add(tbox_NewCurrency.Text);
			cbox_srcCrnc.Items.Add(tbox_NewCurrency.Text);
			TXT_DB.SaveDataBase(DB_path, cbox_trgCrnc);
			tbox_NewCurrency.Text = "added!";

			var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
			dispatcherTimer.Tick += delegate { tbox_NewCurrency.Text = ""; dispatcherTimer.Stop(); };
			dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
			dispatcherTimer.Start();
		}

		private void Convert()
		{
			if (!isConvAvaliable)
				return;

			if (isNormalConvMode)
			{
				tbox_trgCrncAmount.TextChanged -= tbox_trgCrncAmount_TextChanged;
				tbox_trgCrncAmount.Text = string.Format("{0:0.00}", trgAmount = srcAmount * srcRate);
				tbox_trgCrncAmount.TextChanged += tbox_trgCrncAmount_TextChanged;
			}
			else
			{
				tbox_srcCrncAmount.TextChanged -= tbox_srcCrncAmount_TextChanged;
				tbox_srcCrncAmount.Text = string.Format("{0:0.00}", srcAmount = trgAmount * trgRate);
				tbox_srcCrncAmount.TextChanged += tbox_srcCrncAmount_TextChanged;
			}
		}


		#region TextChanged event handlers

		private void tbox_srcCrncAmount_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (double.TryParse(tbox_srcCrncAmount.Text, out srcAmount))
				if (!tbox_trgCrncAmount.IsFocused)
				{
					isNormalConvMode = true;
					Convert();
				}
		}

		private void tbox_trgCrncAmount_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (double.TryParse(tbox_trgCrncAmount.Text, out trgAmount))
				if (!tbox_srcCrncAmount.IsFocused)
				{
					isNormalConvMode = false;
					Convert();
				}
		}


		private void tbox_srcRate_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (double.TryParse(tbox_srcRate.Text, out srcRate) && srcRate > 0)
			{
				if (!tbox_trgRate.IsFocused)
					tbox_trgRate.Text = string.Format("{0:0.00##}", 1.0 / srcRate);
				Convert();
			}
		}

		private void tbox_trgRate_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (double.TryParse(tbox_trgRate.Text, out trgRate) && trgRate > 0)
			{
				if (!tbox_srcRate.IsFocused)
					tbox_srcRate.Text = string.Format("{0:0.00##}", 1.0 / trgRate);
				Convert();
			}
		}
		private void tbox_newCurrency_TextChanged(object sender, TextChangedEventArgs e)
		{
			bool isValid = false;
			btn_CurrAdd.IsEnabled = false;

			if ((StringOPS.isLetter(tbox_NewCurrency.Text) && tbox_NewCurrency.Text.Length == 3) || tbox_NewCurrency.Text.Length == 0)
			{
				tbox_NewCurrency.Background = Brushes.LightGray;
				if (tbox_NewCurrency.Text.Length == 3)
					isValid = true;
			}
			else
			{
				tbox_NewCurrency.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFAFAF");
				isValid = false;
			}

			if (isValid)
				btn_CurrAdd.IsEnabled = true;
		}

		#endregion

		private void btn_close_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void grid_header_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DragMove();
		}
	

        #region Currency ComboBox event handlers

        private string srcDeletedCrnc;
		private int srcDeletedIndex;
        private void cbox_srcCrnc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			if (trgDeletedCrnc != null)
				cbox_trgCrnc.Items.Insert(trgDeletedIndex, trgDeletedCrnc);

			trgDeletedCrnc = cbox_srcCrnc.SelectedItem as string;
			trgDeletedIndex = cbox_srcCrnc.SelectedIndex;
			cbox_trgCrnc.Items.Remove(trgDeletedCrnc);
		}

		private string trgDeletedCrnc;
		private int trgDeletedIndex;
		private void cbox_trgCrnc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			if (srcDeletedCrnc != null)
				cbox_srcCrnc.Items.Insert(srcDeletedIndex, srcDeletedCrnc);

			srcDeletedCrnc = cbox_trgCrnc.SelectedItem as string;
			srcDeletedIndex = cbox_trgCrnc.SelectedIndex;
			cbox_srcCrnc.Items.Remove(srcDeletedCrnc);
		}


		#region ComboBox Search

		private void cbox_ShowAllItems(ComboBox cbox)
        {
			for (int i = 0; i < cbox.Items.Count; i++)
			{
				ComboBoxItem cbItem = (ComboBoxItem)cbox.ItemContainerGenerator.ContainerFromIndex(i);
				if (cbItem != null)
					cbItem.Visibility = Visibility.Visible;
			}
		}

		private void cbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox cbox = e.Source as ComboBox;

			if (cbox.SelectedIndex == -1)
				cbox.SelectedIndex = 0;
			
			if (!cbox.IsDropDownOpen)
				cbox_ShowAllItems(cbox);
		}

		private void cbox_GotFocus(object sender, RoutedEventArgs e)
		{
			ComboBox cbox = e.Source as ComboBox;
			cbox_ShowAllItems(cbox);
		}

		private void cbox_LostFocus(object sender, RoutedEventArgs e)
		{
			ComboBox cbox = e.Source as ComboBox;
			cbox.Text = cbox.SelectedItem as string;
			cbox_ShowAllItems(cbox);
		}

		private void cbox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ComboBox cbox = e.Source as ComboBox;
			if (cbox.Items.Count <= 0)
				return;

			if (!cbox.Items.Contains(cbox.Text))
			{
				string searchValue = cbox.Text.ToUpper();
				bool isItemFounded = false;
				bool isCoursosPosSetted = false;

				for (int i = 0; i < cbox.Items.Count; i++)
				{
					ComboBoxItem cbItem = (ComboBoxItem)cbox.ItemContainerGenerator.ContainerFromIndex(i);
					string item = cbox.Items[i] as string;

					if (cbItem != null)
					{
						if (item.ToUpper().Contains(searchValue))
						{
							cbItem.Visibility = Visibility.Visible;
							isItemFounded = true;

							if (!isCoursosPosSetted)
							{
								isCoursosPosSetted = true;
								int textLength = cbox.Text.Length;
								cbox.SelectedIndex = i;
								(e.OriginalSource as TextBox).SelectionStart = textLength;
							}
						}
						else
							cbItem.Visibility = Visibility.Collapsed;
					}
				}

				if (isItemFounded)
					cbox.IsDropDownOpen = true;
				else
					cbox.IsDropDownOpen = false;
			}

		}

		#endregion


		#endregion
	}
}

