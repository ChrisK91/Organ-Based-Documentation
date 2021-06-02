using CommunityToolkit.WinUI.UI.Controls.TextToolbarSymbols;
using DocuPOC.Database;
using DocuPOC.Messages;
using DocuPOC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DocuPOC.ViewModels
{
    public class OverviewViewModel : ObservableRecipient
    {
        private ObservableCollection<RoomViewViewModel> rooms;
        public ObservableCollection<RoomViewViewModel> Rooms
        {
            get => rooms;
            set => SetProperty(ref rooms, value);
        }

        private int width = 450;
        public int Width
        {
            get => width;
            set => SetProperty(ref width, value);
        }

        public string Header { get; private set; } = "Übersicht";
        public bool CanClose { get => false; }

        private bool openNewAdmission = true;
        public bool OpenNewAdmission
        {
            get => openNewAdmission;
            set => SetProperty(ref openNewAdmission, value);
        }

        public Microsoft.UI.Xaml.Controls.Symbol Symbol { get => Microsoft.UI.Xaml.Controls.Symbol.Home; }

        public IRelayCommand PrintOverview { get; set; }

        public OverviewViewModel()
        {
            Rooms = new ObservableCollection<RoomViewViewModel>();
            PrintOverview = new RelayCommand(() =>
            {
                WeakReferenceMessenger.Default.Send(new PrintOverview(Rooms.Select(r => r.Room).ToList()));
            });

            LoadData();

            WeakReferenceMessenger.Default.Register<OverviewViewModel, AdmissionMovedMessage>(this, (r, m) =>
            {
                LoadData();
            });

            WeakReferenceMessenger.Default.Register<OverviewViewModel, NewAdmissionMessage>(this, (r, m) =>
            {
                LoadData();

                if (OpenNewAdmission)
                {
                    WeakReferenceMessenger.Default.Send(new OpenAdmissionDetailsMessage(m.Value));
                }
            });
        }

        private void LoadData()
        {
            Rooms.Clear();
            var db = new DataContext();
            db.Rooms.Include(r => r.Admissions).ThenInclude(a => a.Patient).ForEachAsync(r => Rooms.Add(new RoomViewViewModel(r)));
        }
    }
}
