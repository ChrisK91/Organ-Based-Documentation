using CommunityToolkit.WinUI.UI.Controls;
using DocuPOC.ViewModels;
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
    public sealed partial class ShowHistoryView : UserControl
    {
        public ShowHistoryView()
        {
            this.InitializeComponent();
        }

        private void DataGrid_Sorting(object sender, CommunityToolkit.WinUI.UI.Controls.DataGridColumnEventArgs e)
        {
            if (e.Column.Tag.ToString() == "Date")
            {
                //Implement sort on the column "Date" using LINQ
                if (this.DataContext is ShowHistoryViewModel)
                {
                    var ctx = this.DataContext as ShowHistoryViewModel;

                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                    {
                        ctx.SortDescending();
                        e.Column.SortDirection = DataGridSortDirection.Ascending;
                    }
                    else
                    {
                        ctx.SortAscending();
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
                }

            }
        }
    }
}
