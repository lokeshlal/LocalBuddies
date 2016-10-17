using LocalBuddies.Mobile.UI.ViewModels;
using Microsoft.Practices.ServiceLocation;

using Xamarin.Forms;

namespace LocalBuddies.Mobile.UI.Views
{
    public partial class List : ContentPage
    {
        public List()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetTitleIcon(this, "phone_icon.png");
            ToolbarItem ti = new ToolbarItem("Settings", "settings.png", () =>
            {
                ((ListViewModel)this.BindingContext).NavigateToSettings();
            }, 0, 0);
            ToolbarItem ti1 = new ToolbarItem("Refresh", null, () =>
           {
               ((ListViewModel)this.BindingContext).RefreshDevices();
           }, 0, 0);
            ToolbarItems.Add(ti);
            ToolbarItems.Add(ti1);
            
            this.Title = " Local Buddies";
        }
    }
}
