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
    /// ACLogin.xaml 的互動邏輯
    /// </summary>
    public partial class ACLogin : Window
    {
        public string UN;
        public string PW;
        public KMCCC.Authentication.IAuthenticator auth;
        public ACLogin()
        {
            InitializeComponent();
        }

        private void butOffline_Click(object sender, RoutedEventArgs e)
        {
            gridMenu.Visibility = Visibility.Hidden;
            gridOff.Visibility = Visibility.Visible;
        }

        private void butMojang_Click(object sender, RoutedEventArgs e)
        {
            gridMenu.Visibility = Visibility.Hidden;
            gridMojang.Visibility = Visibility.Visible;
        }

        private void butBackOff_Click(object sender, RoutedEventArgs e)
        {
            gridOff.Visibility = Visibility.Hidden;
            gridMenu.Visibility = Visibility.Visible;
        }

        private void butLoginOff_Click(object sender, RoutedEventArgs e)
        {
            auth = new KMCCC.Authentication.OfflineAuthenticator(txtBoxUN.Text);
            Close();
        }

        private void butBackM_Click(object sender, RoutedEventArgs e)
        {
            gridMojang.Visibility = Visibility.Hidden;
            gridMenu.Visibility = Visibility.Visible;
        }

        private void butLoginM_Click(object sender, RoutedEventArgs e)
        {
            auth = new KMCCC.Authentication.YggdrasilLogin(txtBoxUNE.Text, pwbox.Password, (bool)checkTwitch.IsChecked);
            Close();
        }

        private void butClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

/*        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (gridOff.Margin != new Thickness(0)) {
                gridOff.Margin = new Thickness(0, 0, 0, gridMain.ActualHeight);
            }
            if (gridMojang.Margin != new Thickness(0))
            {
                gridMojang.Margin = new Thickness(0, 0, 0, gridMain.ActualHeight);
            }
        }*/
    }
}