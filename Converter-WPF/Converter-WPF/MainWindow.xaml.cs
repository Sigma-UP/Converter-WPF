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


        public MainWindow()
        {
            InitializeComponent();

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

            cbox_srcCrnc.SelectedIndex = 0;
            cbox_trgCrnc.SelectedIndex = 2;

            srcAmount = double.Parse(tbox_srcCrncAmount.Text);
            trgAmount = double.Parse(tbox_trgCrncAmount.Text);
            srcRate = double.Parse(tbox_srcRate.Text);
            trgRate = double.Parse(tbox_trgRate.Text);
        }


        void AddCurrency(string code)
        {
            cbox_srcCrnc.Items.Add(code);
            cbox_trgCrnc.Items.Add(code);
        }


        private void ConvertSrcAmount()
        {
            if (tbox_trgCrncAmount == null || tbox_srcRate == null)
                return;

            tbox_trgCrncAmount.Text = string.Format("{0:0.00}", trgAmount = srcAmount * srcRate);
        }

        private void ConvertTrgAmount()
        {
            if (tbox_srcCrncAmount == null || tbox_trgRate == null)
                return;

            tbox_srcCrncAmount.Text = string.Format("{0:0.00}", srcAmount = trgAmount * trgRate);
        }


        private void tbox_srcCrncAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbox_trgCrncAmount != null && tbox_srcRate != null)
                if (double.TryParse(tbox_srcCrncAmount.Text, out srcAmount) & double.TryParse(tbox_srcRate.Text, out srcRate))
                    if (!tbox_trgCrncAmount.IsFocused)
                        ConvertSrcAmount();

        }

        private void tbox_trgCrncAmount_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (tbox_srcCrncAmount != null && tbox_trgRate != null)
                if (double.TryParse(tbox_trgCrncAmount.Text, out trgAmount) & double.TryParse(tbox_trgRate.Text, out trgRate))
                    if (!tbox_srcCrncAmount.IsFocused)
                        ConvertTrgAmount();
        }


        private void tbox_srcRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbox_trgRate != null)
                if (double.TryParse(tbox_srcRate.Text, out srcRate))
                    if(!tbox_trgRate.IsFocused)
                        tbox_trgRate.Text = string.Format("{0:0.00##}", 1.0 / srcRate);
        }

        private void tbox_trgRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbox_srcRate != null)
                if (double.TryParse(tbox_trgRate.Text, out trgRate))
                    if (!tbox_srcRate.IsFocused)
                        tbox_srcRate.Text = string.Format("{0:0.00##}", 1.0 / trgRate);
        }

    }
}
