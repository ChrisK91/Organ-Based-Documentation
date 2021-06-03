using DocuPOC.Models;
using System.Collections.ObjectModel;

namespace DocuPOC.ViewModels
{
    public class PatientDataListEntryWithAdmissionDetails : PatientDataListEntry
    {
        public ObservableCollection<AdmissionListEntryViewModel> admissionEntries;
        public ObservableCollection<AdmissionListEntryViewModel> AdmissionEntries { get => admissionEntries; set => SetProperty(ref admissionEntries, value); }

        public PatientDataListEntryWithAdmissionDetails(Patient p) : base(p)
        {
            AdmissionEntries = new ObservableCollection<AdmissionListEntryViewModel>();
            p.Admissions?.ForEach(a => AdmissionEntries.Add(new AdmissionListEntryViewModel(a)));
        }
    }
}
