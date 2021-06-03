using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
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
            PatientList = new ObservableCollection<PatientDataListEntryWithAdmissionDetails>();
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            DataLoading = true;
            PatientList.Clear();

            var db = new Database.DataContext();
            var patients = await db.Patients.Take(50).Include(p => p.Admissions).ToListAsync();

            foreach (var p in patients)
            {
                PatientList.Add(new PatientDataListEntryWithAdmissionDetails(p));
            }

            DataLoading = false;
        }
    }
}
