using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DocuPOC.Messages;
using DocuPOC.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuPOC.ViewModels
{
    public class RoomViewViewModel : ObservableObject
    {
        public Room Room { get; private set; }

        private ObservableCollection<AdmissionViewModel> admissions;
        public ObservableCollection<AdmissionViewModel> Admissions
        {
            get => admissions;
            set => SetProperty(ref admissions, value);
        }

        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public IRelayCommand AddPatient { get; set; }

        public RoomViewViewModel(Room room)
        {
            this.Room = room;
            Name = room.Name;
            Admissions = new ObservableCollection<AdmissionViewModel>();

            room.Admissions.ForEach(a => Admissions.Add(new AdmissionViewModel(a)));

            AddPatient = new RelayCommand(ShowAddPatientDialog);
        }

        private void ShowAddPatientDialog()
        {
            WeakReferenceMessenger.Default.Send(new ShowNewPatientDialog(this.Room));
        }
    }
}
