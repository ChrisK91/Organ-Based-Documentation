using DocuPOC.Messages;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuPOC.ViewModels
{
    public class MainViewViewModel : ObservableRecipient
    {
        private ObservableCollection<ObservableObject> tabViewViewModels;

        public ObservableCollection<ObservableObject> TabViewViewModels
        {
            get => tabViewViewModels;
            set => SetProperty(ref tabViewViewModels, value);
        }

        private int selectedTab = 0;
        public int SelectedTab { get => selectedTab; set => SetProperty(ref selectedTab, value); }

        private string source;
        public string Source { get => source; set => SetProperty(ref source, value); }

        public bool CanCreateAdmissionEnabled { get; } = false;
        public IRelayCommand CreateAdmissionCommand { get; } = new RelayCommand(()=> { });

        public MainViewViewModel()
        {
            WeakReferenceMessenger.Default.Register<MainViewViewModel, OpenAdmissionDetailsMessage>(this, (r, m) =>
            {
                var index = TabViewViewModels.IndexOf(
                    TabViewViewModels.FirstOrDefault(o => (o is AdmissionDetailViewModel) && (o as AdmissionDetailViewModel).AdmissionId == m.Value.AdmissionId
                    ));

                if (index < 0)
                {
                    r.TabViewViewModels.Add(new AdmissionDetailViewModel(m.Value));
                    r.SelectedTab = r.TabViewViewModels.Count - 1;
                }
                else
                {
                    r.SelectedTab = index;
                }
            });

            WeakReferenceMessenger.Default.Register<MainViewViewModel, CloseAdmissionDetailsMessage>(this, (r, m) => {
                if(r.SelectedTab > 1)
                {
                    r.SelectedTab--;
                }

                r.TabViewViewModels.Remove(m.Value);
            });

            WeakReferenceMessenger.Default.Register<MainViewViewModel, ShowPdfMessage>(this, (r, m) =>
            {
                r.Source = m.Value;
            });

            TabViewViewModels = new ObservableCollection<ObservableObject>();
            TabViewViewModels.Add(new OverviewViewModel());
        }
    }
}
