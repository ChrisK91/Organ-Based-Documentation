using DocuPOC.Messages;
using DocuPOC.Models;
using DotLiquid;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Automation.Peers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;

namespace DocuPOC.Services
{
    public static class TextFilter
    {
        public static string NewlineToBrExtended(string input)
        {
            input = string.IsNullOrWhiteSpace(input) ? input : Regex.Replace(input, @"(\r?\n)", "<br />$1", RegexOptions.None, Template.RegexTimeOut);
            input = string.IsNullOrWhiteSpace(input) ? input : Regex.Replace(input, @"(\r)", "<br />$1", RegexOptions.None, Template.RegexTimeOut);
            return input;
        }

        public static string Markdown(string input)
        {
            input = string.IsNullOrWhiteSpace(input) ? input : Markdig.Markdown.ToHtml(input);
            return input;
        }
    }

    public interface IPrintService
    {
        void RegisterPrintMessaging();
    }
    public class PrintService : IPrintService
    {
        public PrintService()
        {
            Template.RegisterFilter(typeof(TextFilter));
        }

        public void RegisterPrintMessaging()
        {
            WeakReferenceMessenger.Default.Register<PrintService, PrintAdmissionMessage>(this, (r, m) =>
            {
                printAdmission(m.Value);
            });

            WeakReferenceMessenger.Default.Register<PrintService, PrintOverview>(this, (r, m) =>
            {
                printOverview(m.Value);
            });
        }

        private async void printOverview(List<Room> roomList)
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Templates/OverviewTemplate.html"));

            string outputDestination = await createTemplatedFileAsync(roomList, file);

            WeakReferenceMessenger.Default.Send(new ShowPdfMessage(outputDestination));
        }

        private async void printAdmission(Admission admission)
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Templates/PatientPrintoutTemplate.html"));

            string outputDestination = await createTemplatedFileAsync(admission, file);

            WeakReferenceMessenger.Default.Send(new ShowPdfMessage(outputDestination));
        }

        private static async Task<string> createTemplatedFileAsync(object documentModel, StorageFile template)
        {
            string tempDirectory = Ioc.Default.GetService<ISettingsService>().GetSettingWithDefault("output_destination", SettingsService.DefaultTemporaryDirectory);
            Directory.CreateDirectory(tempDirectory);

            var outputDestination = System.IO.Path.Combine(tempDirectory, Guid.NewGuid().ToString() + ".html");

            string templateSource = await Windows.Storage.FileIO.ReadTextAsync(template);

            Template t = Template.Parse(templateSource);
            var model = new { Model = documentModel };

            var content = t.Render(Hash.FromAnonymousObject(model));


            File.WriteAllText(outputDestination, content);
            return outputDestination;
        }
    }
}
