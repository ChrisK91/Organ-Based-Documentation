﻿using CommunityToolkit.Mvvm.Messaging;
using DocuPOC.Messages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DocuPOC.Views
{
    public sealed partial class OverviewView : UserControl
    {
        public OverviewView()
        {
            this.InitializeComponent();

            WeakReferenceMessenger.Default.Register<OverviewView, UpdateRefreshIndicator>(this, (r, m)=>{ RotateRefreshIcon.Begin(); });
        }
    }
}
