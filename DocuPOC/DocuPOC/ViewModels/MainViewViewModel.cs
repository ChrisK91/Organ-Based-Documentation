using DocuPOC.Messages;
using DocuPOC.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DocuPOC.ViewModels
{
    public class MainViewViewModel : ObservableRecipient
    {
        private ObservableCollection<ITabViewModel> tabViewViewModels;

        public ObservableCollection<ITabViewModel> TabViewViewModels
        {
            get => tabViewViewModels;
            set => SetProperty(ref tabViewViewModels, value);
        }

        private int selectedTab = 0;
        public int SelectedTab { get => selectedTab; set => SetProperty(ref selectedTab, value); }

        private string source;
        public string Source { get => source; set => SetProperty(ref source, value); }


        public bool CanCreateAdmissionEnabled { get; } = false;
        public IRelayCommand CreateAdmissionCommand { get; } = new RelayCommand(() => { });

        public MainViewViewModel()
        {
            WeakReferenceMessenger.Default.Register<MainViewViewModel, OpenAdmissionDetailsMessage>(this, (r, m) =>
            {
                // check if a tab already exists with the viewmodel that should be opened
                var index = TabViewViewModels.IndexOf(
                    TabViewViewModels.FirstOrDefault(o => (o is AdmissionDetailViewModel) && (o as AdmissionDetailViewModel).AdmissionId == m.Value.AdmissionId
                    ));

                if (index < 0)
                {
                    // if not open a new tab and navigate to it
                    r.TabViewViewModels.Add(new AdmissionDetailViewModel(m.Value));
                    r.SelectedTab = r.TabViewViewModels.Count - 1;
                }
                else
                {
                    // otherwise navigate to the already open tab
                    r.SelectedTab = index;
                }
            });

            WeakReferenceMessenger.Default.Register<MainViewViewModel, CloseAdmissionDetailsMessage>(this, (r, m) =>
            {
                if (r.SelectedTab > 1)
                {
                    r.SelectedTab--;
                }

                r.TabViewViewModels.Remove(m.Value);
            });

            WeakReferenceMessenger.Default.Register<MainViewViewModel, ShowPdfMessage>(this, (r, m) =>
            {
                r.Source = m.Value;
            });

            WeakReferenceMessenger.Default.Register<MainViewViewModel, AdmissionDischargedMessage>(this, (r, m) =>
            {
                var viewModelOfAdmission = r.TabViewViewModels
                    .Select(t => t is AdmissionDetailViewModel ? (AdmissionDetailViewModel)t : null).Where(vm => vm != null)
                    .Where(t => t.AdmissionId == m.Value.AdmissionId).FirstOrDefault();

                if (viewModelOfAdmission != null)
                {
                    r.TabViewViewModels.Remove(viewModelOfAdmission);
                }
            });

            WeakReferenceMessenger.Default.Register<MainViewViewModel, OpenPatientArchiveMessage>(this, (r, _) =>
            {
                r.TabViewViewModels.Add(new PatientArchiveViewModel());
                r.SelectedTab = r.TabViewViewModels.Count - 1;
            });

            WeakReferenceMessenger.Default.Register<MainViewViewModel, CloseGenericTabMessage>(this, (r, m) =>
            {
                r.TabViewViewModels.Remove(m.Value);
                r.SelectedTab = r.TabViewViewModels.Count - 1;
            });

            TabViewViewModels = new ObservableCollection<ITabViewModel>();
            TabViewViewModels.Add(new OverviewViewModel());
        }
    }
}
