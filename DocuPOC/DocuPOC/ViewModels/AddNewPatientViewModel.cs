using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DocuPOC.Messages;
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

    public class AddNewPatientViewModel : ObservableObject
    {
        private ObservableCollection<PatientDataListEntry> patientList;
        public ObservableCollection<PatientDataListEntry> PatientList { get => patientList; set => SetProperty(ref patientList, value); }

        private bool dataLoading;
        public bool DataLoading { get => dataLoading; set => SetProperty(ref dataLoading, value); }

        private bool canCreateAdmissionEnabled;
        public bool CanCreateAdmissionEnabled { get => canCreateAdmissionEnabled; set => SetProperty(ref canCreateAdmissionEnabled, value); }

        private PatientDataListEntry selectedPatient;
        public PatientDataListEntry SelectedPatient
        {
            get => selectedPatient;
            set
            {
                SetProperty(ref selectedPatient, value);
                CreateAdmissionCommand.NotifyCanExecuteChanged();
                CanCreateAdmission();
            }
        }

        private PatientDataListEntry searchPatientData;
        public PatientDataListEntry SearchPatientData { get => searchPatientData; set => SetProperty(ref searchPatientData, value); }

        public DateTimeOffset MinDate { get; } = new DateTimeOffset(new DateTime(1900, 1, 1));

        public IRelayCommand AddPatientCommand { get; set; }

        public IRelayCommand ResetFieldsCommand { get; set; }

        public IRelayCommand SearchDataCommand { get; set; }

        public IRelayCommand CreateAdmissionCommand { get; set; }

        private RoomComboboxEntry selectedRoom;
        public RoomComboboxEntry SelectedRoom
        {
            get => selectedRoom;
            set
            {
                SetProperty(ref selectedRoom, value);
                CreateAdmissionCommand.NotifyCanExecuteChanged();
                CanCreateAdmission();
            }
        }

        private ObservableCollection<RoomComboboxEntry> roomList;
        public ObservableCollection<RoomComboboxEntry> RoomList { get => roomList; set => SetProperty(ref roomList, value); }

        private DateTimeOffset admissionDate = new DateTimeOffset(DateTime.Today);
        public DateTimeOffset AdmissionDate
        {
            get => admissionDate;
            set
            {
                SetProperty(ref admissionDate, value); 
                CreateAdmissionCommand.NotifyCanExecuteChanged();
                CanCreateAdmission();
            }
        }

        public AddNewPatientViewModel(Room r)
        {
            DataLoading = true;
            SearchPatientData = new PatientDataListEntry();
            SearchPatientData.PropertyChanged += SearchPatientData_PropertyChanged;
            AddPatientCommand = new RelayCommand(AddNewPatientAndSelect, CanAddData);
            SearchDataCommand = new RelayCommand(PerformSearch, CanSearchData);
            CreateAdmissionCommand = new RelayCommand(CreateAdmission, CanCreateAdmission);
            ResetFieldsCommand = new RelayCommand(() => { SearchPatientData = new PatientDataListEntry(); LoadDataAsync(); });

            PatientList = new ObservableCollection<PatientDataListEntry>();
            RoomList = new ObservableCollection<RoomComboboxEntry>();

            var db = new Database.DataContext();
            var rooms = db.Rooms.ToList();

            foreach (var room in rooms)
            {
                RoomList.Add(new RoomComboboxEntry(room));
            }

            SelectedRoom = RoomList.Where(i => i.Room.RoomId == r.RoomId).FirstOrDefault();

            //ToDo: Hook into global loading pipeline
            LoadDataAsync();
        }

        private bool CanCreateAdmission()
        {
            CanCreateAdmissionEnabled = SelectedPatient != null && AdmissionDate.Year > 1850 && SelectedRoom != null;
            return CanCreateAdmissionEnabled;
        }

        private void CreateAdmission()
        {
            DataLoading = true;

            var db = new Database.DataContext();

            var room = SelectedRoom.Room;
            var patient = SelectedPatient.Patient;

            db.Attach(room);
            db.Attach(patient);

            var admission = new Admission();

            admission.Room = room;
            admission.Patient = patient;
            admission.AdmissionDateTime = AdmissionDate.DateTime;

            db.Admissions.Add(admission);

            db.SaveChanges();

            WeakReferenceMessenger.Default.Send(new NewAdmissionMessage(admission));

            DataLoading = false;
        }

        private async void PerformSearch()
        {
            PatientList.Clear();
            DataLoading = true;

            var db = new Database.DataContext();
            IQueryable<Patient> patients = db.Patients;

            if (!String.IsNullOrWhiteSpace(SearchPatientData.Name))
            {
                patients = patients.Where(p => p.Name.Contains(SearchPatientData.Name));
            }

            if (SearchPatientData.DateOfBirth != null)
            {
                patients = patients.Where(p => p.Birthday.Date == ((DateTimeOffset)SearchPatientData.DateOfBirth).Date);
            }

            await patients.LoadAsync();

            foreach (var p in patients)
            {
                PatientList.Add(new PatientDataListEntry(p));
            }

            DataLoading = false;
        }

        private bool CanAddData()
        {
            return !String.IsNullOrWhiteSpace(SearchPatientData.Name) && SearchPatientData != null && SearchPatientData.DateOfBirth?.Year > 1850;
        }

        private bool CanSearchData()
        {
            return !String.IsNullOrWhiteSpace(SearchPatientData.Name) || (SearchPatientData != null && SearchPatientData.DateOfBirth?.Year > 1850);
        }

        private async void AddNewPatientAndSelect()
        {
            DataLoading = true;
            var db = new Database.DataContext();
            var newPatient = new Patient();
            newPatient.Name = SearchPatientData.Name;
            newPatient.Birthday = SearchPatientData.DateOfBirth != null ? ((DateTimeOffset)SearchPatientData.DateOfBirth).DateTime : throw new ArgumentException();
            db.Patients.Add(newPatient);
            await db.SaveChangesAsync();

            PatientList.Clear();
            var patients = db.Patients.Where(p => p.PatientId == newPatient.PatientId);

            foreach (var p in patients)
            {
                PatientList.Add(new PatientDataListEntry(p));
            }

            SelectedPatient = PatientList.Where(p => p.Id == newPatient.PatientId).FirstOrDefault();
            SearchPatientData = new PatientDataListEntry();

            DataLoading = false;
        }

        private void SearchPatientData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            AddPatientCommand.NotifyCanExecuteChanged();
            SearchDataCommand.NotifyCanExecuteChanged();
        }

        private async void LoadDataAsync()
        {
            PatientList.Clear();
            DataLoading = true;

            var db = new Database.DataContext();
            var patients = await db.Patients.Take(50).ToListAsync();

            foreach (var p in patients)
            {
                PatientList.Add(new PatientDataListEntry(p));
            }

            DataLoading = false;
        }

    }
}
