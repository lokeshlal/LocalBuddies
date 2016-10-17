using Prism.Mvvm;
using System.Collections.ObjectModel;
using Prism.Services;
using Prism.Navigation;
using LocalBuddies.Mobile.UI.Network;
using System.Threading.Tasks;
using Xamarin.Forms;
using LocalBuddies.Mobile.UI.Models;
using System.Linq;
using LocalBuddies.Mobile.UI.Services;

namespace LocalBuddies.Mobile.UI.ViewModels
{
    public class ListViewModel : BindableBase
    {
        private string _title;
        private ObservableCollection<User> availableDevices;
        private User selectedUser;
        private IPageDialogService dialogService;
        private INavigationService navigationService;
        private IBackgroundService backgroundService;
        private string userDetails;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ObservableCollection<User> AvailableDevices
        {
            get
            {
                return availableDevices;
            }

            set
            {
                SetProperty(ref availableDevices, value);
            }
        }

        public User SelectedUser
        {
            get
            {
                return selectedUser;
            }

            set
            {
                SetProperty(ref selectedUser, value);
                if (value != null && !string.IsNullOrWhiteSpace(value.IPAddress))
                    InitiateCall(value.IPAddress);
            }
        }

        public string UserDetails
        {
            get
            {
                return userDetails;
            }

            set
            {
                userDetails = value;
            }
        }

        public IBackgroundService BackgroundService
        {
            get
            {
                if (backgroundService == null)
                {
                    backgroundService = Xamarin.Forms.DependencyService.Get<IBackgroundService>();
                    backgroundService.CallRecieved += BackgroundService_CallRecieved;
                    backgroundService.Start();
                }
                return backgroundService;
            }
        }

        private void BackgroundService_CallRecieved(object sender, User e)
        {
            // new UDP message recieved
        }

        private void InitiateCall(string remoteIpAddresss)
        {
            if (!string.IsNullOrWhiteSpace(remoteIpAddresss))
            {
                NetworkHandler.InitiateCall(remoteIpAddresss);
                SelectedUser = null;
            }
        }

        public void NavigateToSettings()
        {
            navigationService.NavigateAsync("Settings");
        }

        public void RefreshDevices()
        {
            AvailableDevices = new ObservableCollection<User>();
            Task.Factory.StartNew(() =>
            {
                NetworkHandler.DiscoverDevices();
            }, new System.Threading.CancellationToken(false),
            TaskCreationOptions.AttachedToParent,
            TaskScheduler.FromCurrentSynchronizationContext()); //.ConfigureAwait(false);
        }

        private void InitiateReciever()
        {
            Task.Run(() =>
            {
                NetworkHandler.InitiateReciever();
            }); //.ConfigureAwait(false);
        }

        private void CallBackForUserDetails(User user)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var record = AvailableDevices.Where(a => a.IPAddress == user.IPAddress);
                if (record.Any())
                {
                    AvailableDevices.Remove(record.First());
                }
                AvailableDevices.Add(new User() { IPAddress = user.IPAddress, Name = user.Name });
            });
        }

        private void CallBackForCallStatus(ConnectionState state)
        {
            if (state == ConnectionState.Connected)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    navigationService.NavigateAsync("Call");
                });
            }
            else
            {
                //navigationService.GoBackAsync();
                //navigationService.GoBackAsync();
            }
        }

        public ListViewModel(IPageDialogService dialogService, INavigationService navigationService)
        {

            var ipaddressService = Xamarin.Forms.DependencyService.Get<IIPAddressManager>();
            string ipaddress = ipaddressService.GetIPAddress();
            string[] gateWayIpAddress = ipaddressService.GetGateWayAddress();
            if (!string.IsNullOrEmpty(ipaddress))
                UserDetails = string.Format("IP Address: {0} - Gateway Address: {1}", ipaddress, string.Join(".", gateWayIpAddress));


            this.dialogService = dialogService;
            this.navigationService = navigationService;
            AvailableDevices = new ObservableCollection<User>();
            NetworkHandler.CallBackForView = CallBackForCallStatus;
            NetworkHandler.CallBackForUserDetails = CallBackForUserDetails;

            // start the device discovery and initialize the reciever
            Task.Factory.StartNew(() =>
            {
                NetworkHandler.DiscoverDevices();
            }, new System.Threading.CancellationToken(false),
            TaskCreationOptions.None,
            TaskScheduler.FromCurrentSynchronizationContext()); //.ConfigureAwait(false);

            // move initiate reciever to App.cs in a long running task, to keep listening for the incoming connections, even
            // when the app is in the background
            // this will initiate the reciever using an android service
            //BackgroundService.Start();
            InitiateReciever();
        }

    }
}
