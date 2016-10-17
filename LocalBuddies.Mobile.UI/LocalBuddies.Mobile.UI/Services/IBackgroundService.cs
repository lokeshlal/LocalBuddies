using LocalBuddies.Mobile.UI.Models;
using System;

namespace LocalBuddies.Mobile.UI.Services
{
    public interface IBackgroundService
    {
        void Start();

        event EventHandler<User> CallRecieved;
    }
}
