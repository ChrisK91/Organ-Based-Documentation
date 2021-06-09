using CommunityToolkit.Mvvm.ComponentModel;
using DocuPOC.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuPOC.ViewModels
{
    public class ShowHistoryListEntry : ObservableObject
    {
        private DateTime timestamp;
        public DateTime Timestamp { get => timestamp; set => SetProperty(ref timestamp, value); }

        private EntryType entryType;
        public EntryType EntryType { get => entryType; set => SetProperty(ref entryType, value); }

        private string value;
        public string Value { get => value; set => SetProperty(ref this.value, value); }
    }

    public class ShowHistoryViewModel : ObservableObject
    {
        private AdmissionDetailViewModel parent;
        private bool loading = true;
        public bool Loading { get => loading; set => SetProperty(ref loading, value); }

        private ObservableCollection<ShowHistoryListEntry> textEntries;
        public ObservableCollection<ShowHistoryListEntry> TextEntries { get => textEntries; set => SetProperty(ref textEntries, value); }

        public ShowHistoryViewModel(AdmissionDetailViewModel parent)
        {
            this.parent = parent;
            TextEntries = new ObservableCollection<ShowHistoryListEntry>();
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            Loading = true;
            var db = new Database.DataContext();
            TextEntries.Clear();

            var tmpList = new List<ShowHistoryListEntry>();

            await Task.Run(async () =>
            {
                await db.VersionedStringEntries.Where(e =>
                    e.PatientId == parent.PatientId && e.PatientId != null
                    ).ForEachAsync(e =>
                tmpList.Add(new ShowHistoryListEntry()
                {
                    EntryType = e.EntryType,
                    Timestamp = e.Timestamp,
                    Value = e.Value
                }));

                await db.VersionedStringEntries.Where(e =>
                    e.AdmissionId == parent.AdmissionId && e.AdmissionId != null
                    ).ForEachAsync(e =>
                tmpList.Add(new ShowHistoryListEntry()
                {
                    EntryType = e.EntryType,
                    Timestamp = e.Timestamp,
                    Value = e.Value
                }));
            });

            TextEntries = new ObservableCollection<ShowHistoryListEntry>(tmpList);

            Loading = false;
        }
    }
}
