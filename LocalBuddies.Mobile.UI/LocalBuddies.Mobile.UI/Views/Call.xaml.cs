
using LocalBuddies.Mobile.UI.ViewModels;
using Xamarin.Forms;

namespace LocalBuddies.Mobile.UI.Views
{
    public partial class Call : ContentPage
    {
        public Call()
        {
            InitializeComponent();
            // hide navigation bar on this page
            NavigationPage.SetHasNavigationBar(this, false);
        }
        protected override bool OnBackButtonPressed()
        {
            ((CallViewModel)this.BindingContext).DisconnectCall();
            return base.OnBackButtonPressed();
        }
    }
}
