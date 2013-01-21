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
using System.Windows.Navigation;
using System.Windows.Shapes;
using FuzzyRain.Model;

namespace FuzzyRain
{
    /// <summary>
    /// Interaction logic for MonthTab.xaml
    /// </summary>
    public partial class ValuesDisplayer : UserControl
    {
        public string Title
        {
            set { contentTitle.Text = value; }
        }

        public ValuesDisplayer()
        {
            InitializeComponent();
        }

        public void SetDataMonths(Distribution distribution)
        {
            double avg = distribution.Average;
            double desv = distribution.Std_Desv;

            lwValues.ItemsSource = distribution.ValuesInOrderOfAppearance;
            txtAvg.Text = avg.ToString("#0.00");
            txtDesv.Text = desv.ToString("#0.00");
        }

        public void SetConvergenceData(double avg, double desv, int eventNumberOfConvergence)
        {
            txtValueConv.Text = eventNumberOfConvergence.ToString();
            txtAvgConv.Text = avg.ToString("#0.00");
            txtDesvConv.Text = desv.ToString("#0.00");

            valueConvPanel.Visibility = System.Windows.Visibility.Visible;
            avgConvPanel.Visibility = System.Windows.Visibility.Visible;
            desvConvPanel.Visibility = System.Windows.Visibility.Visible;
        }
    }
}