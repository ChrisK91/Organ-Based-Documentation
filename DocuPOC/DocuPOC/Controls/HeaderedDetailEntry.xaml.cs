using CommunityToolkit.Mvvm.Messaging;
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

namespace DocuPOC.Controls
{
    public sealed partial class HeaderedDetailEntry : UserControl
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(string),
            typeof(TextBox), null
            );
        public string Header
        {
            get { return HeaderTextblock.Text; }
            set { HeaderTextblock.Text = value; }
        }


        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(String),
            typeof(HeaderedDetailEntry), new PropertyMetadata(String.Empty, new PropertyChangedCallback(OnTextChanged))
            );

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeaderedDetailEntry control = d as HeaderedDetailEntry;
            control.EditBox.Text = e.NewValue.ToString();
        }

        public object Value
        {
            get
            {
                return GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public HeaderedDetailEntry()
        {
            this.InitializeComponent();

            WeakReferenceMessenger.Default.Register<FormatBold>(this, formatBold);
            WeakReferenceMessenger.Default.Register<FormatItalic>(this, formatItalic);

        }

        private void formatItalic(object recipient, FormatItalic message)
        {
            togglePadding("*");
        }

        private void formatBold(object recipient, FormatBold message)
        {
            togglePadding("**");
        }

        private void togglePadding(string v, string v2 = null)
        {
            if (EditBox.SelectionLength > 0)
            {
                if (EditBox.SelectedText.StartsWith(v) && EditBox.SelectedText.EndsWith(v2 ?? v))
                {
                    var text = EditBox.SelectedText.Remove(0, v.Length);
                    text = text.Remove(text.Length - (v2 ?? v).Length, (v2 ?? v).Length);

                    EditBox.SelectedText = text;
                }
                else
                {
                    EditBox.SelectedText = v + EditBox.SelectedText + (v2 ?? v);
                }
            }
        }

        private void EditBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Value = EditBox.Text;
        }
    }
}
