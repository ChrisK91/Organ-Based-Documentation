using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.UI.Controls.TextToolbarSymbols;
using DocuPOC.Database;
using DocuPOC.Messages;
using DocuPOC.Models;
using DocuPOC.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
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
    public class OverviewViewModel : ObservableRecipient, ITabViewModel
    {
        private ObservableCollection<RoomViewViewModel> rooms;
        public ObservableCollection<RoomViewViewModel> Rooms
        {
            get => rooms;
            set => SetProperty(ref rooms, value);
        }

        private ObservableCollection<AdmissionViewModel> admissionsWithoutRooms;
        public ObservableCollection<AdmissionViewModel> AdmissionsWithoutRooms
        {
            get => admissionsWithoutRooms;
            set => SetProperty(ref admissionsWithoutRooms, value);
        }

        private int width = 450;
        public int Width
        {
            get => width;
            set => SetProperty(ref width, value);
        }


        public string DatabaseLocation { get => Ioc.Default.GetService<ISettingsService>().GetSettingWithDefault("DatabaseLocation", SettingsService.DefaultDatabaseLocation); }

        public string Header { get; private set; } = "Übersicht";
        public bool CanClose { get => false; }
        public bool CanDrag { get => false; }

        private bool openNewAdmission = true;
        public bool OpenNewAdmission
        {
            get => openNewAdmission;
            set => SetProperty(ref openNewAdmission, value);
        }

        public Microsoft.UI.Xaml.Controls.Symbol Symbol { get => Microsoft.UI.Xaml.Controls.Symbol.Home; }

        public IRelayCommand PrintOverview { get; set; }
        public IRelayCommand OpenHistoryTab { get; set; }
        public IRelayCommand RefreshData { get; set; }

        DispatcherTimer refreshTimer;

        private int progressTime = 0;
        public int ProgressTime { get => progressTime; set => SetProperty(ref progressTime, value); }

        public int RefreshInterval { get => 60; } // TODO: make configurable

        public OverviewViewModel()
        {
            Rooms = new ObservableCollection<RoomViewViewModel>();
            AdmissionsWithoutRooms = new ObservableCollection<AdmissionViewModel>();

            PrintOverview = new RelayCommand(() =>
            {
                WeakReferenceMessenger.Default.Send(new PrintOverview(Rooms.Select(r => r.Room).ToList()));
            });

            OpenHistoryTab = new RelayCommand(() =>
            {
                WeakReferenceMessenger.Default.Send(new OpenPatientArchiveMessage());
            });

            RefreshData = new RelayCommand(() =>
            {
                LoadData();
            });

            LoadData();

            WeakReferenceMessenger.Default.Register<OverviewViewModel, AdmissionMovedMessage>(this, (r, m) =>
            {
                LoadData();
            });

            WeakReferenceMessenger.Default.Register<OverviewViewModel, AdmissionDischargedMessage>(this, (r, m) =>
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

            refreshTimer = new DispatcherTimer();
            refreshTimer.Interval = new TimeSpan(0, 0, 1);
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, object e)
        {
            ProgressTime++;
            if (ProgressTime == RefreshInterval)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            WeakReferenceMessenger.Default.Send(new DisplayLoadingIndicator(null));
            Rooms.Clear();
            AdmissionsWithoutRooms.Clear();

            var db = new DataContext();
            db.Rooms.OrderBy(r => r.RoomId)
                .Include(r => r.Admissions.Where(a => a.DischargeDateTime == null)) // select Admission without a discharge date
                .ThenInclude(a => a.Patient)
                .ThenInclude(r => r.Admissions)
                .ForEachAsync(r => Rooms.Add(new RoomViewViewModel(r)));

            db.Admissions.Where(a => a.Room == null && a.DischargeDateTime == null).Include(a => a.Patient).ForEachAsync(a => AdmissionsWithoutRooms.Add(new AdmissionViewModel(a)));

            ProgressTime = 0;
        }
    }
}
