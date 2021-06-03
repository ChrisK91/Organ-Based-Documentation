using DocuPOC.Messages;
using DocuPOC.Models;
using DotLiquid;
using DotLiquid.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;

namespace DocuPOC.ViewModels
{
    public class RoomComboboxEntry
    {
        public string Name { get; private set; }
        public Room Room { get; private set; }

        public RoomComboboxEntry(Room room)
        {
            Name = room.Name;
            Room = room;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class AdmissionViewModel : ObservableObject
    {
        private Admission admission;

        private string patientName;
        public string PatientName { get => patientName; set => SetProperty(ref patientName, value); }

        public string Header { get => PatientName; }

        private int patientAgeInYears;
        public int PatientAgeInYears { get => patientAgeInYears; set => SetProperty(ref patientAgeInYears, value); }

        private int admissionTimeInDays;
        public int AdmissionTimeInDays { get => admissionTimeInDays; set => SetProperty(ref admissionTimeInDays, value); }

        private string patientNotes;
        public string PatientNotes { get => patientNotes; set => SetProperty(ref patientNotes, value); }

        private string diagnosis;
        public string Diagnosis { get => diagnosis; set => SetProperty(ref diagnosis, value); }

        private DateTimeOffset dischargeDate = new DateTimeOffset(DateTime.Today);
        public DateTimeOffset DischargeDate { get => dischargeDate; set => SetProperty(ref dischargeDate, value); }

        public IRelayCommand MovePatientCommand { get; }

        public IRelayCommand OpenAdmissionCommand { get; }

        public IRelayCommand PrintAdmissionCommand { get; }

        public IRelayCommand DischargeAdmissionCommand { get; }

        public AdmissionViewModel(Admission admission)
        {
            this.admission = admission;
            SetDataByAdmission(admission);

            MovePatientCommand = new RelayCommand(MovePatient, CanMovePatientExecute);
            OpenAdmissionCommand = new RelayCommand(OpenAdmission);
            PrintAdmissionCommand = new RelayCommand(PrintAdmission);
            DischargeAdmissionCommand = new RelayCommand(DischargeAdmission);

            WeakReferenceMessenger.Default.Register<PatientUpdatedMessage>(this, (r, m)=>
            {
                if(m.Value.PatientId == this.admission.PatientId)
                {
                    UpdateData();
                }
            });

            UpdatePossibleRooms();
        }

        private void DischargeAdmission()
        {
            var db = new Database.DataContext();
            db.Attach(admission);
            admission.DischargeDateTime = DischargeDate.DateTime;
            admission.Room.Admissions.Remove(admission);
            db.SaveChanges();

            WeakReferenceMessenger.Default.Send(new AdmissionDischargedMessage(admission));

        }

        private void PrintAdmission()
        {
            WeakReferenceMessenger.Default.Send(new PrintAdmissionMessage(admission));
        }

        private void UpdateData()
        {
            var db = new Database.DataContext();
            var admission = db.Admissions.Include(a => a.Patient).Where(a => a.AdmissionId == this.admission.AdmissionId).First();
            SetDataByAdmission(admission);
        }

        private void SetDataByAdmission(Admission admission)
        {
            PatientName = admission.Patient.Name;
            PatientAgeInYears = admission.Patient.AgeInYears;
            AdmissionTimeInDays = admission.AdmissionTimeInDays;
            PatientNotes = admission.Patient.Notes;
            Diagnosis = admission.Diagnosis;
        }

        private void MovePatient()
        {
            // Todo: factor out
            var db = new Database.DataContext();
            db.Attach(admission);
            admission.Room = SelectedRoom.Room;
            db.SaveChanges();

            WeakReferenceMessenger.Default.Send(new AdmissionMovedMessage(admission));
        }

        private bool CanMovePatientExecute()
        {
            return SelectedRoom != null;
        }

        private void OpenAdmission()
        {
            WeakReferenceMessenger.Default.Send(new OpenAdmissionDetailsMessage(admission));
        }

        private async void UpdatePossibleRooms()
        {
            possibleRooms.Clear();
            var db = new Database.DataContext();
            var rooms = db.Rooms.Where(r => r != this.admission.Room);
            await rooms.ForEachAsync(r => possibleRooms.Add(new RoomComboboxEntry(r)));
        }

        private ObservableCollection<RoomComboboxEntry> possibleRooms = new ObservableCollection<RoomComboboxEntry>();
        public ObservableCollection<RoomComboboxEntry> PossibleRooms
        {
            get => possibleRooms;
            set => SetProperty(ref possibleRooms, value);
        }

        private RoomComboboxEntry selectedRoom;
        public RoomComboboxEntry SelectedRoom
        {
            get => selectedRoom; set
            {
                SetProperty(ref selectedRoom, value);
                MovePatientCommand.NotifyCanExecuteChanged();
            }
        }
    }
}
