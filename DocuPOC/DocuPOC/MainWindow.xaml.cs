﻿using DocuPOC.Messages;
using DocuPOC.ViewModels;
using Microsoft.Toolkit.Mvvm.Messaging;
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

namespace DocuPOC
{

    public class MainWindowTabDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OverviewTemplate { get; set; }
        public DataTemplate AdmissionTemplate { get; set; }
        public DataTemplate EmptyTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch (item)
            {
                case OverviewViewModel: return OverviewTemplate;
                case AdmissionDetailViewModel: return AdmissionTemplate;
                default: return EmptyTemplate;
            }
        }
    }

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            WeakReferenceMessenger.Default.Register<ShowInfoMessage>(this, (r, m) =>
            {
                this.InAppNotification.Show(m.Value.Item1, m.Value.Item2);
            });

            WeakReferenceMessenger.Default.Register<ShowNewPatientDialog>(this, async (r, m) =>
            {
                AddNewPatient.DataContext = new AddNewPatientViewModel(m.Value);
                var result = await AddNewPatient.ShowAsync();
            });

            WeakReferenceMessenger.Default.Register<ShowPdfMessage>(this, async (r, m) =>
            {
                PrintDialog.Hide();
                await PrintDialog.ShowAsync();
            });
        }

        private void TabViewItem_CloseRequested(TabViewItem sender, TabViewTabCloseRequestedEventArgs args)
        {
            if (sender.DataContext != null && sender.DataContext is AdmissionDetailViewModel)
            {

                WeakReferenceMessenger.Default.Send(new CloseAdmissionDetailsMessage(sender.DataContext as AdmissionDetailViewModel));
            }
        }

        private void CloseSelectedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var InvokedTabView = (args.Element as TabView);

            if(InvokedTabView.SelectedItem is AdmissionDetailViewModel)
            {
                WeakReferenceMessenger.Default.Send(new CloseAdmissionDetailsMessage(InvokedTabView.SelectedItem as AdmissionDetailViewModel));
            }

            args.Handled = true;
        }

        private void NavigateToNumberedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var InvokedTabView = (args.Element as TabView);

            int tabToSelect = 0;

            switch (sender.Key)
            {
                case Windows.System.VirtualKey.Number1:
                    tabToSelect = 0;
                    break;
                case Windows.System.VirtualKey.Number2:
                    tabToSelect = 1;
                    break;
                case Windows.System.VirtualKey.Number3:
                    tabToSelect = 2;
                    break;
                case Windows.System.VirtualKey.Number4:
                    tabToSelect = 3;
                    break;
                case Windows.System.VirtualKey.Number5:
                    tabToSelect = 4;
                    break;
                case Windows.System.VirtualKey.Number6:
                    tabToSelect = 5;
                    break;
                case Windows.System.VirtualKey.Number7:
                    tabToSelect = 6;
                    break;
                case Windows.System.VirtualKey.Number8:
                    tabToSelect = 7;
                    break;
                case Windows.System.VirtualKey.Number9:
                    // Select the last tab
                    tabToSelect = InvokedTabView.TabItems.Count - 1;
                    break;
            }

            // Only select the tab if it is in the list
            if (tabToSelect < InvokedTabView.TabItems.Count)
            {
                InvokedTabView.SelectedIndex = tabToSelect;
            }

            args.Handled = true;
        }


        private async void WebViewControl_NavigationCompleted(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            await WebViewControl.CoreWebView2.ExecuteScriptAsync("window.print();");
        }
    }
}