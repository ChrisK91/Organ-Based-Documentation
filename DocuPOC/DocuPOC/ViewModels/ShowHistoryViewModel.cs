using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DocuPOC.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        private AdmissionDetailViewModel parent;
        public ICommand SetContentOfParent { get; set; }

        public ShowHistoryListEntry(AdmissionDetailViewModel parent)
        {
            this.parent = parent;
            SetContentOfParent = new RelayCommand(UpdateContentOfParent);
        }

        private void UpdateContentOfParent()
        {
            switch (EntryType)
            {
                case EntryType.Diagnosis:
                    parent.Diagnosis = Value;
                    break;
                case EntryType.Neurologic:
                    parent.Neurology = Value;
                    break;
                case EntryType.Pulmonal:
                    parent.Pulmonal = Value;
                    break;
                case EntryType.Cardiology:
                    parent.Cardial = Value;
                    break;
                case EntryType.Renal:
                    parent.Renal = Value;
                    break;
                case EntryType.Abdominal:
                    parent.Abdominal = Value;
                    break;
                case EntryType.Infectiology:
                    parent.Infectiology = Value;
                    break;
                case EntryType.ToDo:
                    parent.ToDo = Value;
                    break;
                case EntryType.Procedere:
                    parent.Procedere = Value;
                    break;
                case EntryType.Notes:
                    parent.PatientNotes = Value;
                    break;
            }

            WeakReferenceMessenger.Default.Send(new Messages.ShowInfoMessage(new Tuple<string, int>("Text in aktuelle Aufnahme übernommen", 2000)));
        }
    }

    public class ShowHistoryViewModel : ObservableObject
    {
        private AdmissionDetailViewModel parent;
        private bool loading = false;
        public bool Loading { get => loading; set => SetProperty(ref loading, value); }

        private ObservableCollection<ShowHistoryListEntry> textEntries;
        public ObservableCollection<ShowHistoryListEntry> TextEntries { get => textEntries; set => SetProperty(ref textEntries, value); }


        private bool showAllAdmissions = false;
        public bool ShowAllAdmissions { get => showAllAdmissions; set => SetProperty(ref showAllAdmissions, value); }

        private bool showDiagnosis = true;
        public bool ShowDiagnosis { get => showDiagnosis; set => SetProperty(ref showDiagnosis, value); }

        private bool showNeurology = true;
        public bool ShowNeurology { get => showNeurology; set => SetProperty(ref showNeurology, value); }

        private bool showCardiology = true;
        public bool ShowCardiology { get => showCardiology; set => SetProperty(ref showCardiology, value); }

        private bool showPulmonal = true;
        public bool ShowPulmonal { get => showPulmonal; set => SetProperty(ref showPulmonal, value); }

        private bool showAbdominal = true;
        public bool ShowAbdominal { get => showAbdominal; set => SetProperty(ref showAbdominal, value); }

        private bool showRenal = true;
        public bool ShowRenal { get => showRenal; set => SetProperty(ref showRenal, value); }

        private bool showInfectiology = true;
        public bool ShowInfectiology { get => showInfectiology; set => SetProperty(ref showInfectiology, value); }

        private bool showNotes = true;
        public bool ShowNotes { get => showNotes; set => SetProperty(ref showNotes, value); }

        private bool showToDo = true;
        public bool ShowToDo { get => showToDo; set => SetProperty(ref showToDo, value); }

        private bool showProcedere = true;
        public bool ShowProcedere { get => showProcedere; set => SetProperty(ref showProcedere, value); }

        public ICommand RefreshData { get; set; }
        public ICommand ResetFilters { get; set; }

        public ShowHistoryViewModel(AdmissionDetailViewModel parent)
        {
            this.parent = parent;
            TextEntries = new ObservableCollection<ShowHistoryListEntry>();
            PropertyChanged += ShowHistoryViewModel_PropertyChanged;

            RefreshData = new RelayCommand(LoadDataAsync);
            ResetFilters = new RelayCommand(resetFilters);

            ShowAllAdmissions = false;
            resetFilters();
            LoadDataAsync();
        }

        private void resetFilters()
        {
            ShowDiagnosis = true;
            ShowAbdominal = true;
            ShowNeurology = true;
            ShowCardiology = true;
            ShowRenal = true;
            ShowPulmonal = true;
            ShowInfectiology = true;
            ShowNotes = true;
            ShowProcedere = true;
            ShowToDo = true;
        }

        private void ShowHistoryViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(ShowAllAdmissions): LoadDataAsync(); break;
            }
        }

        internal void SortDescending()
        {
            sort(false);
        }

        internal void SortAscending()
        {
            sort(true);
        }

        private async void sort(bool sortAscending)
        {
            Loading = true;

            var sorted = await Task.Run(() => {
                var tmp = TextEntries.ToList();

                var ordered = sortAscending ? tmp.OrderBy(a => a.Timestamp) : tmp.OrderByDescending(a => a.Timestamp);
                return new ObservableCollection<ShowHistoryListEntry>(ordered);
            });

            TextEntries = sorted;

            Loading = false;
        }

        private async void LoadDataAsync()
        {
            Loading = true;
            var db = new Database.DataContext();
            TextEntries.Clear();

            var tmpList = new List<ShowHistoryListEntry>();

            var filterTypes = new List<EntryType>();

            if (ShowDiagnosis) filterTypes.Add(EntryType.Diagnosis);
            if (ShowAbdominal) filterTypes.Add(EntryType.Abdominal);
            if (ShowNeurology) filterTypes.Add(EntryType.Neurologic);
            if (ShowCardiology) filterTypes.Add(EntryType.Cardiology);
            if (ShowRenal) filterTypes.Add(EntryType.Renal);
            if (ShowPulmonal) filterTypes.Add(EntryType.Pulmonal);
            if (ShowInfectiology) filterTypes.Add(EntryType.Infectiology);
            if (ShowNotes) filterTypes.Add(EntryType.Notes);
            if (ShowProcedere) filterTypes.Add(EntryType.Procedere);
            if (ShowToDo) filterTypes.Add(EntryType.ToDo);

           await Task.Run(async () =>
            {
                List<int> admissionsToSelect = new List<int>();

                if(ShowAllAdmissions)
                {
                    admissionsToSelect.AddRange(
                        await db.Admissions.Where(a => a.PatientId == parent.PatientId).Select(a => a.AdmissionId).ToListAsync()
                        );
                }
                else
                {
                    admissionsToSelect.Add(parent.AdmissionId); 
                }

                await db.VersionedStringEntries.Where(e =>
                    e.PatientId == parent.PatientId && e.PatientId != null && filterTypes.Contains(e.EntryType)
                    ).ForEachAsync(e =>
                tmpList.Add(new ShowHistoryListEntry(parent)
                {
                    EntryType = e.EntryType,
                    Timestamp = e.Timestamp,
                    Value = e.Value
                }));

                await db.VersionedStringEntries.Where(e =>
                    admissionsToSelect.Contains(e.AdmissionId.Value) && e.AdmissionId != null && filterTypes.Contains(e.EntryType)
                    ).ForEachAsync(e =>
                tmpList.Add(new ShowHistoryListEntry(parent)
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
