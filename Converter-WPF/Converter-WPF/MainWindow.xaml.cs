using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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


        public MainWindow()
        {
            InitializeComponent();

            cbox_srcCrnc.SelectedIndex = 0;
            cbox_trgCrnc.SelectedIndex = 2;

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

            string[] defaultCurrencies = new string[]
            {
                "USD", "EUR", "UAH", "AUD", "AZN",
                "ALL", "DZD", "XCD", "AOA", "ARS",
                "AWG", "AFN", "BSD", "BDT", "BBD",
                "BHD", "BYN", "XOF", "BOB", "BRL",
                "BIF", "BTN", "VUV", "GBP", "VES",
                "XAF", "VND", "GYD", "GHS", "GMD"
            };

            foreach (string crnc in defaultCurrencies)
                AddCurrency(crnc);

        }


        private bool isNumber(string str)
        {
            double varriable;
            return double.TryParse(str, out varriable);
        }

        private bool ValidateTextBoxInput()
        {
            bool isValid = true;

            TextBox[] tboxes = new TextBox[] { tbox_srcCrncAmount, tbox_trgCrncAmount, tbox_srcRate, tbox_trgRate };

            foreach (var tbox in tboxes)
            {
                if (isNumber(tbox.Text) && double.Parse(tbox.Text) > 0)  
                    tbox.Background = Brushes.White;
                else
                {
                    tbox.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFAFAF");
                    isValid = false;
                }
            }

            isConvAvaliable = isValid;
            return isValid;
        }

        private void AddCurrency(string code)
        {
            cbox_srcCrnc.Items.Add(code);
            cbox_trgCrnc.Items.Add(code);
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
    }
}
