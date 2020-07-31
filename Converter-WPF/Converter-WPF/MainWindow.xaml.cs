﻿using System;
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
        public MainWindow()
        {
            InitializeComponent();

            string[] defaultCurrencies = new string[]
            {
                "USD", "EUR", "UAH"
            };

            foreach (string crnc in defaultCurrencies)
                AddCurrency(crnc);

            cbox_srcCrnc.SelectedIndex = 0;
            cbox_trgCrnc.SelectedIndex = 2;
        }


        void AddCurrency(string code)
        {
            cbox_srcCrnc.Items.Add(code);
            cbox_trgCrnc.Items.Add(code);
        }


        private void tbox_srcCrncAmount_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void tbox_trgCrncAmount_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        private void tbox_srcToTrgRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void tbox_trgToSrcRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

    }
}