
using Android.App;
using Android.Content.PM;
using Android.OS;
using Prism.Unity;
using Microsoft.Practices.Unity;
using LocalBuddies.Mobile.UI;

namespace LocalBuddies.Mobile.UI.Droid
{
    [Activity(Label = "LocalBuddies.Mobile.UI", Icon = "@drawable/icon", 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            //LoadApplication(new App(new UwpInitializer()));
            LoadApplication(new App());
        }

    }

    public class UwpInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IUnityContainer container)
        {

        }
    }
}

