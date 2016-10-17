using LocalBuddies.Mobile.UI.Messages;
using LocalBuddies.Mobile.UI.Network;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System.Windows.Input;
using Xamarin.Forms;

namespace LocalBuddies.Mobile.UI.ViewModels
{
    public class CallViewModel : BindableBase, INavigationAware
    {
        private string remoteAddress;
        private INavigationService navigationService;
        private IPageDialogService dialogService;
        private bool isNavigated = false;
        private bool isCallDisconnected = false;
        public ICommand DisconnectCommand { get; set; }

        public string RemoteAddress
        {
            get
            {
                return remoteAddress;
            }

            set
            {
                remoteAddress = value;
            }
        }

        public CallViewModel(INavigationService navigationService, IPageDialogService dialogService)
        {
            MessagingCenter.Subscribe<DisconnectMessage>(this, "Disconnect", (sender) =>
            {
                CancelCall();
            });
            this.navigationService = navigationService;
            this.dialogService = dialogService;
            RemoteAddress = ConnectionLifeCycle.CurrentConnected;
            DisconnectCommand = new DelegateCommand(CancelCall);
        }

        public void DisconnectCall()
        {
            if (isCallDisconnected) return;
            isCallDisconnected = true;
            NetworkHandler.DisconnectCall(RemoteAddress);
        }

        public void CancelCall()
        {
            if (isNavigated) return;
            isNavigated = true;
            Device.BeginInvokeOnMainThread(() =>
            {
                DisconnectCall();
                navigationService.NavigateAsync("List");
                //dialogService.DisplayAlertAsync("3", "3", "ok");
            });
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            isCallDisconnected = false;
            isNavigated = false;
            RemoteAddress = ConnectionLifeCycle.CurrentConnected;
        }
    }
}
