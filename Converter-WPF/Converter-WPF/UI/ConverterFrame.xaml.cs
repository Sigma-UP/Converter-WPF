using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StringExtension;

namespace Converter_WPF
{
	/// <summary>
	/// Логика взаимодействия для ShowConverter.xaml
	/// </summary>
	public partial class ConverterFrame : Page
	{
		private double srcAmount;
		private double trgAmount;
		private double srcRate;
		private double trgRate;

		private bool isConvAvaliable = true;
		private bool isNormalConvMode = true;

		public ConverterFrame()
		{
			InitializeComponent();

			//if(MainWindow.DataBaseCode == 1 || MainWindow.DataBaseCode == 2)
            //{
			//	tbox_srcRate.IsReadOnly = true;
			//	tbox_trgRate.IsReadOnly = true;
            //}

			srcAmount = double.Parse(tbox_srcCrncAmount.Text);
			trgAmount = double.Parse(tbox_trgCrncAmount.Text);
			srcRate = double.Parse(tbox_srcRate.Text);
			trgRate = double.Parse(tbox_trgRate.Text);
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

			foreach(Currency crnc in MainWindow.databaseAPI.Currencies)
            {
				cbox_srcCrnc.Items.Add(crnc.Name);
				cbox_trgCrnc.Items.Add(crnc.Name);
            }

			cbox_srcCrnc.SelectedIndex = 0;
			
			SelectionChangedEventHandler currencyRateSetter = delegate
			{
				string srcCrnc = cbox_srcCrnc.SelectedItem as string;
				string trgCrnc = cbox_trgCrnc.SelectedItem as string;
				BackgroundWorker worker = new BackgroundWorker();
			
				tbox_srcRate.Text = "...";
				tbox_trgRate.Text = "...";
			
				worker.DoWork += (sender, args) =>
				{
					args.Result = MainWindow.databaseAPI.GetExchangeRate(srcCrnc, trgCrnc);
				};
			
				worker.RunWorkerCompleted += (sender, args) =>
				{
					if ((double)args.Result != double.NaN)
					{
						tbox_srcRate.Text = string.Format("{0:0.00##}", 0);
						tbox_trgRate.Text = string.Format("{0:0.00##}", 0);
					}
					else
					{
						tbox_srcRate.Text = string.Format("{0:0.00##}", args.Result);
					}

				};
			
				worker.RunWorkerAsync();
			};
			cbox_srcCrnc.SelectionChanged += currencyRateSetter;
			cbox_trgCrnc.SelectionChanged += currencyRateSetter;
			
			cbox_trgCrnc.SelectedIndex = 1;

		}

		private void ValidateTextBoxInput()
		{
			isConvAvaliable = true;

			TextBox[] tboxes = new TextBox[] { tbox_srcCrncAmount, tbox_trgCrncAmount, tbox_srcRate, tbox_trgRate };
			Border[] borders = new Border[] { border_srcCrncAmount, border_trgCrncAmount, border_srcRate, border_trgRate };
			TextBox tbox_iHave = isNormalConvMode ? tbox_srcCrncAmount : tbox_trgCrncAmount;

			for (int i = 0; i < tboxes.Length; i++)
			{
				if (tboxes[i].Text.isNumber() && double.Parse(tboxes[i].Text) > 0)
					borders[i].Visibility = Visibility.Hidden;
				else
				{
					borders[i].Visibility = Visibility.Visible;

					if (i == 0 || i == 1)
					{
						if (!tbox_iHave.Text.isNumber())
							isConvAvaliable = false;
					}
					else
						isConvAvaliable = false;
				}

			}

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



		#endregion

		#region Currency ComboBox event handlers

		private string srcRemovedCrncCode;
		private int srcRemovedCrncPos;
		private void cbox_srcCrnc_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (trgRemovedCrncCode != null)
				cbox_trgCrnc.Items.Insert(trgRemovedCrncPos, trgRemovedCrncCode);

			if (srcRemovedCrncCode != null)
				cbox_srcCrnc.Items.Insert(srcRemovedCrncPos, srcRemovedCrncCode);

			trgRemovedCrncCode = cbox_srcCrnc.SelectedItem as string;
			trgRemovedCrncPos = cbox_srcCrnc.SelectedIndex;

			cbox_trgCrnc.Items.Remove(trgRemovedCrncCode);
			if (srcRemovedCrncCode != null)
				cbox_srcCrnc.Items.Remove(srcRemovedCrncCode);
		}

		private string trgRemovedCrncCode;
		private int trgRemovedCrncPos;
		private void cbox_trgCrnc_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (srcRemovedCrncCode != null)
				cbox_srcCrnc.Items.Insert(srcRemovedCrncPos, srcRemovedCrncCode);

			if (trgRemovedCrncCode != null)
				cbox_trgCrnc.Items.Insert(trgRemovedCrncPos, trgRemovedCrncCode);

			srcRemovedCrncCode = cbox_trgCrnc.SelectedItem as string;
			srcRemovedCrncPos = cbox_trgCrnc.SelectedIndex;

			cbox_srcCrnc.Items.Remove(srcRemovedCrncCode);

			if (trgRemovedCrncCode != null)
				cbox_trgCrnc.Items.Remove(trgRemovedCrncCode);
		}

		#endregion

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

			(e.OriginalSource as TextBox).CharacterCasing = CharacterCasing.Upper;

			if (!cbox.Items.Contains(cbox.Text))
			{
				string searchValue = cbox.Text.ToUpper();
				bool isItemFounded = false;
				bool isCursorPosSetted = false;

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

							if (!isCursorPosSetted)
							{
								TextBox tboxBase = e.OriginalSource as TextBox;
								string text = tboxBase.Text;

								cbox.SelectedIndex = i;
								cbox.Text = text;
								tboxBase.SelectionStart = cbox.Text.Length;

								isCursorPosSetted = true;
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
    }
}
