using System;
using System.Collections.Generic;
using System.Windows;

namespace UI.Controls
{   
    public partial class AutoRepairWindow : Window
    {
        public bool CleanSmudges { get; set; } 
        public bool CleanScrates { get; set; }
        public bool CleanDust { get; set; }


        public AutoRepairWindow()
        {
            InitializeComponent();
            CleanSmudges = true;
            CleanScrates = true;
            CleanDust = true;
        }
    }
}
