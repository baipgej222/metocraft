﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MetoCraft
{
    /// <summary>
    /// KnownErrorReport.xaml 的互動邏輯
    /// </summary>
    public partial class KnownErrorReport : Window
    {
        public KnownErrorReport()
        {
            InitializeComponent();
        }
        public KnownErrorReport(string desc)
        {
            InitializeComponent();
            this.lblDesc.Content = desc;
        }
        public KnownErrorReport(string desc, string help)
        {
            InitializeComponent();
            this.lblDesc.Content = desc;
            //this.lblHelp.Text = help;
            string[] helps = System.Text.RegularExpressions.Regex.Split(help, "/n/");
            for (int i = 0; i < helps.Length; i++)
            {
                lblHelp.Inlines.Add(helps[i]);
                if (i != helps.Length - 1)
                {
                    lblHelp.Inlines.Add(new LineBreak());
                }
            }
        }

        private void butOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
