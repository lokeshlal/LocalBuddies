using System;
using Android.Content;
using LocalBuddies.Mobile.UI.Models;
using LocalBuddies.Mobile.UI.Services;
using Xamarin.Forms;
using LocalBuddies.Mobile.UI.Droid;

[assembly: Dependency(typeof(BackgroundService))]
namespace LocalBuddies.Mobile.UI.Droid
{
    class BackgroundService : IBackgroundService
    {
        public static BackgroundService Self;

        public event EventHandler<User> CallRecieved;

        public void Start()
        {
            Self = this;
            Forms.Context.StartService(new Intent(Forms.Context, typeof(BackgroundServiceImpl)));
        }

        public void ReceivedNewCall(User user)
        {
            CallRecieved(this, user);
        }
    }
}