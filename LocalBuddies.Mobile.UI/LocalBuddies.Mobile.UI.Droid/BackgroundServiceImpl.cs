using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using LocalBuddies.Mobile.UI.Network;

namespace LocalBuddies.Mobile.UI.Droid
{
    [Service]
    class BackgroundServiceImpl : Service
    {
        IBinder binder;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Task.Run(() =>
            {
                // background self should be defined
                if (BackgroundService.Self != null)
                {
                    NetworkHandler.InitiateReciever();
                }
                // start the monitoring for the UDP packets

                StopSelf();
            });
            return StartCommandResult.Sticky; //base.OnStartCommand(intent, flags, startId);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}