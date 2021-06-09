using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DocuPOC.Messages;
using DocuPOC.Models;
using DotLiquid.Util;
using System;

namespace DocuPOC.ViewModels
{
    public class AdmissionListEntryViewModel : ObservableObject
    {
        private Admission admission;


        private DateTimeOffset? dischargeDate;
        public DateTimeOffset? DischargeDate { get => dischargeDate; set => SetProperty(ref dischargeDate, value); }

        private DateTimeOffset admissionDate;
        public DateTimeOffset AdmissionDate { get => admissionDate; set => SetProperty(ref admissionDate, value); }

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

        public IRelayCommand RemoveDischargeDate { get; private set; }

        public AdmissionListEntryViewModel(Admission a)
        {
            admission = a;

            Diagnosis = a.Diagnosis;

            if (a.DischargeDateTime is DateTime && a.DischargeDateTime?.Year <= 1) // TODO: convert to try-catch
            {
                // invalid Date-Time has been set, Setting it to a value ensures that a delete is possible
                DischargeDate = DateTimeOffset.MinValue;
            }
            else
            {
                DischargeDate = a.DischargeDateTime;
            }
            AdmissionDate = a.AdmissionDateTime;

            PatientNotes = admission.Patient.Notes;
            Diagnosis = admission.Diagnosis;
            AdmissionDate = admission.AdmissionDateTime;
            Pulmonal = admission.Pulmonal;
            Abdominal = admission.Abdominal;
            Cardial = admission.Cardiology;
            Renal = admission.Renal;
            Neurology = admission.Neurologic;
            Infectiology = admission.Infectiology;

            RemoveDischargeDate = new RelayCommand(ConvertToActiveAdmission);
        }

        private void ConvertToActiveAdmission()
        {
            var db = new Database.DataContext();
            db.Attach(admission);
            admission.DischargeDateTime = null;
            DischargeDate = null;
            db.SaveChanges();

            WeakReferenceMessenger.Default.Send(new NewAdmissionMessage(admission));
        }
    }
}
