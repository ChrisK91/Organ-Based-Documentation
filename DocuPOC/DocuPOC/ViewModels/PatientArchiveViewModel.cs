using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DocuPOC.ViewModels
{
    public class PatientArchiveViewModel : ObservableObject, ITabViewModel
    {
        public string Header { get => "Patienten suchen"; }

        public Symbol Symbol { get => Symbol.Find; }

        public bool CanClose { get => true; }

        public bool CanDrag { get => true; }

        private bool dataLoading;
        public bool DataLoading { get => dataLoading; set => SetProperty(ref dataLoading, value); }

        private string searchName;
        public string SearchName { get => searchName; set { SetProperty(ref searchName, value); SearchDataCommand.NotifyCanExecuteChanged(); } }

        public DateTimeOffset MinDate { get; } = new DateTimeOffset(new DateTime(1900, 1, 1));

        private DateTimeOffset? searchBirthday;
        public DateTimeOffset? SearchBirthday { get => searchBirthday; set { SetProperty(ref searchBirthday, value); SearchDataCommand.NotifyCanExecuteChanged(); } }

        public IRelayCommand ResetFieldsCommand { get; set; }

        public IRelayCommand SearchDataCommand { get; set; }

        private ObservableCollection<PatientDataListEntryWithAdmissionDetails> patientList;
        public ObservableCollection<PatientDataListEntryWithAdmissionDetails> PatientList { get => patientList; set => SetProperty(ref patientList, value); }

        private AdmissionListEntryViewModel selectedAdmission;
        public AdmissionListEntryViewModel SelectedAdmission { get => selectedAdmission; set => SetProperty(ref selectedAdmission, value); }

        private PatientDataListEntryWithAdmissionDetails selectedPatient;
        public PatientDataListEntryWithAdmissionDetails SelectedPatient
        {
            get => selectedPatient;
            set
            {
                SetProperty(ref selectedPatient, value);
            }
        }

        public PatientArchiveViewModel()
        {
            ResetFieldsCommand = new RelayCommand(() =>
            {
                SearchBirthday = null;
                SearchName = null;
                LoadDataAsync();
            });

            SearchDataCommand = new RelayCommand(PerformSearch, CanSearch);

            PatientList = new ObservableCollection<PatientDataListEntryWithAdmissionDetails>();
            LoadDataAsync();

            //TODO: Return to safe state, when data changes in different forms
        }

        private bool CanSearch()
        {
            return !String.IsNullOrEmpty(SearchName) || SearchBirthday != null;
        }

        private async void PerformSearch()
        {
            DataLoading = true;
            PatientList.Clear();

            var db = new Database.DataContext();
            IQueryable<Models.Patient> patients = db.Patients;

            if (!String.IsNullOrEmpty(SearchName))
            {
                patients = patients.Where(p => EF.Functions.Like(SearchName, p.Name));
            }

            if(SearchBirthday != null)
            {
                patients = patients.Where(p => p.Birthday.Date == SearchBirthday.Value.Date);
            }

            await patients.Include(p => p.Admissions).ThenInclude(a => a.Diagnosis.OrderByDescending(v => v.Timestamp).Take(1)).ForEachAsync(p => PatientList.Add(new PatientDataListEntryWithAdmissionDetails(p)));

            SelectedAdmission = null;
            SelectedPatient = null;

            DataLoading = false;
        }

        private async void LoadDataAsync()
        {
            DataLoading = true;
            PatientList.Clear();

            var db = new Database.DataContext();
            var patients = await db.Patients.Take(50).Include(p => p.Admissions).ThenInclude(a => a.Diagnosis.OrderByDescending(v => v.Timestamp).Take(1)).ToListAsync();

            foreach (var p in patients)
            {
                PatientList.Add(new PatientDataListEntryWithAdmissionDetails(p));
            }

            SelectedPatient = null;
            SelectedAdmission = null;

            DataLoading = false;
        }
    }
}
