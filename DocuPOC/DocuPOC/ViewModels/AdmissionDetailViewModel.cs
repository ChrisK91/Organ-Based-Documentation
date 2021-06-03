using DocuPOC.Messages;
using DocuPOC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DocuPOC.ViewModels
{
    public class AdmissionDetailViewModel : ObservableObject, ITabViewModel
    {
        private Admission admission;

        private string patientName;
        public string PatientName { get => patientName; set => SetProperty(ref patientName, value); }

        public string Header { get => PatientName; }
        public bool CanClose { get => true; }
        public bool CanDrag { get => true; }

        public Microsoft.UI.Xaml.Controls.Symbol Symbol { get => Microsoft.UI.Xaml.Controls.Symbol.Contact; }

        private int patientAgeInYears;
        public int PatientAgeInYears { get => patientAgeInYears; set => SetProperty(ref patientAgeInYears, value); }

        private DateTimeOffset patientDob;
        public DateTimeOffset PatientDob
        {
            get => patientDob; set
            {
                SetProperty(ref patientDob, value);
                PatientAgeInYears = value.YearsDifference();
            }
        }

        private int admissionTimeInDays;
        public int AdmissionTimeInDays { get => admissionTimeInDays; set => SetProperty(ref admissionTimeInDays, value); }

        private DateTimeOffset admissionDate;
        public DateTimeOffset AdmissionDate
        {
            get => admissionDate; set
            {
                SetProperty(ref admissionDate, value);
                AdmissionTimeInDays = value.DaysDifference();
            }
        }

        private string patientNotes;
        public string PatientNotes { get => patientNotes; set => SetProperty(ref patientNotes, value); }

        private string diagnosis;
        public string Diagnosis { get => diagnosis; set => SetProperty(ref diagnosis, value); }

        private string neurology;
        public string Neurology { get => neurology; set => SetProperty(ref neurology, value); }

        private string renal;
        public string Renal { get => renal; set => SetProperty(ref renal, value); }

        private string pulmonal;
        public string Pulmonal { get => pulmonal; set => SetProperty(ref pulmonal, value); }

        private string abdominal;
        public string Abdominal { get => abdominal; set => SetProperty(ref abdominal, value); }

        private string cardial;
        public string Cardial { get => cardial; set => SetProperty(ref cardial, value); }

        private string infectiology;
        public string Infectiology { get => infectiology; set => SetProperty(ref infectiology, value); }

        private string todo;
        public string ToDo { get => todo; set => SetProperty(ref todo, value); }


        private string procedere;
        public string Procedere { get => procedere; set => SetProperty(ref procedere, value); }


        public int AdmissionId { get => admission.AdmissionId; }

        public IRelayCommand SaveChanges { get; set; }

        public IRelayCommand PrintAdmission { get; set; }

        public AdmissionDetailViewModel(Admission admission)
        {
            this.admission = admission;
            PatientName = admission.Patient.Name;
            PatientAgeInYears = admission.Patient.AgeInYears;
            AdmissionTimeInDays = admission.AdmissionTimeInDays;
            PatientNotes = admission.Patient.Notes;
            Diagnosis = admission.Diagnosis;
            PatientDob = admission.Patient.Birthday;
            AdmissionDate = admission.AdmissionDateTime;
            Pulmonal = admission.Pulmonal;
            Abdominal = admission.Abdominal;
            Cardial = admission.Cardiology;
            Renal = admission.Renal;
            Neurology = admission.Neurologic;
            Infectiology = admission.Infectiology;

            SaveChanges = new RelayCommand(SavePatientChanges);
            PrintAdmission = new RelayCommand(() => { WeakReferenceMessenger.Default.Send(new PrintAdmissionMessage(this.admission)); });
        }

        private void SavePatientChanges()
        {
            var patient = this.admission.Patient;

            var db = new Database.DataContext();
            db.Attach(patient);
            patient.Notes = PatientNotes;
            patient.Birthday = PatientDob.DateTime;

            db.Attach(this.admission);
            this.admission.Diagnosis = Diagnosis;
            this.admission.AdmissionDateTime = AdmissionDate.DateTime;
            this.admission.Pulmonal = Pulmonal;
            this.admission.Abdominal = Abdominal;
            this.admission.Cardiology = Cardial;
            this.admission.Renal = Renal;
            this.admission.Neurologic = Neurology;
            this.admission.Infectiology = Infectiology;
            this.admission.ToDo = ToDo;
            this.admission.Procedere = Procedere;

            db.SaveChanges();



            WeakReferenceMessenger.Default.Send(new PatientUpdatedMessage(patient));
            WeakReferenceMessenger.Default.Send(new ShowInfoMessage(new Tuple<string, int>("Daten gespeichert", 1500)));
        }
    }
}
