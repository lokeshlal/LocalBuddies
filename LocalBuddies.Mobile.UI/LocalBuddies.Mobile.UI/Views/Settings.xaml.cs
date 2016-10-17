using Xamarin.Forms;

namespace LocalBuddies.Mobile.UI.Views
{
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            InitializeComponent();
            NavigationPage.SetTitleIcon(this, "phone_icon.png");
            this.Title = " Settings";

            userName.TextChanged += (sender, args) =>
            {
                string _text = userName.Text;      //Get Current Text
                if (_text.Length > 15)       //If it is more than your character restriction
                {
                    _text = _text.Remove(_text.Length - 1);  // Remove Last character
                    userName.Text = _text;        //Set the Old value
                }
            };
        }
    }
}
