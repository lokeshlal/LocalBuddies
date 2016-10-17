using LocalBuddies.Mobile.UI.Storage;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System.Windows.Input;

namespace LocalBuddies.Mobile.UI.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private string name;
        private INavigationService navigationService;

        public ICommand SaveCommand { get; set; }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                SetProperty(ref name, value);
                AppStorage.Name = value;
            }
        }
        public SettingsViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            Name = AppStorage.Name;
            SaveCommand = new DelegateCommand(SaveCall);
        }

        private void SaveCall()
        {
            AppStorage.SaveData();
            navigationService.GoBackAsync();
        }

       
    }
}
