using Xamarin.Forms;
using LocalBuddies.Mobile.UI.Network;
using System;
using LocalBuddies.Mobile.UI.iOS;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Linq;

[assembly: Dependency(typeof(IPAddressManager))]
namespace LocalBuddies.Mobile.UI.iOS
{
    public class IPAddressManager : IIPAddressManager
    {
        public string[] GetGateWayAddress()
        {
            foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    return netInterface.GetIPProperties().GatewayAddresses.First().Address.ToString().Split(new char[] { '.' });
                }
            }

            return null;
        }

        public string GetIPAddress()
        {
            foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return addrInfo.Address.ToString();

                        }
                    }
                }
            }

            return null;
        }
    }
}