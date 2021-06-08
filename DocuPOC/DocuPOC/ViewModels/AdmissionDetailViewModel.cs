using DocuPOC.Messages;
using DocuPOC.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;

namespace DocuPOC.ViewModels
{
    public class AdmissionDetailViewModel : ObservableObject, ITabViewModel
    {
        private Admission admission;

        private string patientName;
        public string PatientName { get => patientName; set => SetProperty(ref patientName, value); }

        private string header;
        public string Header { get => header; set => SetProperty(ref header, value); }
        public bool CanClose { get => true; }
        public bool CanDrag { get => true; }

        public int PatientId { get; private set; }

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

        private bool isDirty;
        public bool IsDirty { get => isDirty; set => SetProperty(ref isDirty, value); }


        public int AdmissionId { get => admission.AdmissionId; }

        public IRelayCommand SaveChanges { get; set; }

        public IRelayCommand PrintAdmission { get; set; }
        public IRelayCommand ShowHistory { get; set; }

        public AdmissionDetailViewModel(Admission admission)
        {
            this.admission = admission;
            PatientId = admission.Patient.PatientId;
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

            Header = admission.Patient.Name;

            this.PropertyChanged += MarkDirty;

            SaveChanges = new RelayCommand(SavePatientChanges);
            PrintAdmission = new RelayCommand(() => { WeakReferenceMessenger.Default.Send(new PrintAdmissionMessage(this.admission)); });
            ShowHistory = new RelayCommand(() => { WeakReferenceMessenger.Default.Send(new ShowHistory(this)); });
        }

        private void MarkDirty(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(PatientName):
                case nameof(PatientAgeInYears):
                case nameof(PatientNotes):
                case nameof(Diagnosis):
                case nameof(PatientDob):
                case nameof(AdmissionDate):
                case nameof(Pulmonal):
                case nameof(Abdominal):
                case nameof(Cardial):
                case nameof(Renal):
                case nameof(Neurology):
                case nameof(Infectiology):
                    IsDirty = true;
                    break;

                case nameof(IsDirty):
                    Header = IsDirty ? PatientName + " *" : PatientName;
                    break;
            }
        }

        private void SavePatientChanges()
        {
            var patient = this.admission.Patient;

            var db = new Database.DataContext();
            db.Attach(patient);
            db.UpdateNotes(patient, PatientNotes);
            patient.Birthday = PatientDob.DateTime;

            db.Attach(this.admission);

            db.UpdateDiagnosis(this.admission, Diagnosis);
            db.UpdatePulmonal(this.admission, Pulmonal);
            db.UpdateAbdominal(this.admission, Abdominal);
            db.UpdateCardiology(this.admission, Cardial);
            db.UpdateRenal(this.admission, Renal);
            db.UpdateNeurologic(this.admission, Neurology);
            db.UpdateInfectiology(this.admission, Infectiology);
            db.UpdateTodo(this.admission, ToDo);
            db.UpdateProcedere(this.admission, Procedere);

            this.admission.AdmissionDateTime = AdmissionDate.DateTime;


            db.SaveChanges();

            WeakReferenceMessenger.Default.Send(new PatientUpdatedMessage(patient));
            WeakReferenceMessenger.Default.Send(new ShowInfoMessage(new Tuple<string, int>("Daten gespeichert", 1500)));

            IsDirty = false;
        }
    }
}
