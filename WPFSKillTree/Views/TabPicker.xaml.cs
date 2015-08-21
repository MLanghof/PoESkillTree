﻿using MahApps.Metro.Controls;
using System;
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

namespace POESKillTree.Views
{
    /// <summary>
    /// Interaction logic for TabPicker.xaml
    /// </summary>
    public partial class TabPicker : MetroWindow
    {
        public TabPicker()
        {
            InitializeComponent();
        }



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TabPicker), new PropertyMetadata(""));



        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public bool Delete { get; private set; }


        // Using a DependencyProperty as the backing store for SelectedColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(TabPicker), new PropertyMetadata(Color.FromRgb(98,128,0)));


        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.SetValue(SelectedColorProperty, ((SolidColorBrush)((Rectangle)sender).Fill).Color);
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Delete = true;
            this.Close();
        }
    }
}
