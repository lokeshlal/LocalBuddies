using LocalBuddies.Mobile.UI.Droid;
using Xamarin.Forms;
using System.Net;
using System.Net.NetworkInformation;
using LocalBuddies.Mobile.UI.Network;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;
using System;
using Android.Net.Wifi;
using Android.Net;

[assembly: Dependency(typeof(IPAddressManager))]
namespace LocalBuddies.Mobile.UI.Droid
{
    public class IPAddressManager : IIPAddressManager
    {
        public string[] GetGateWayAddress()
        {
            WifiManager manager = (WifiManager)Forms.Context.GetSystemService(Android.Content.Context.WifiService);
            if (manager.IsWifiEnabled)
            {
                int gateWayIP = manager.DhcpInfo.Gateway;

                int intMyIp3 = gateWayIP / 0x1000000;
                int intMyIp3mod = gateWayIP % 0x1000000;
                int intMyIp2 = intMyIp3mod / 0x10000;
                int intMyIp2mod = intMyIp3mod % 0x10000;
                int intMyIp1 = intMyIp2mod / 0x100;
                int intMyIp0 = intMyIp2mod % 0x100;

                string gatewayAddress = Convert.ToString(intMyIp0)
                    + "." + Convert.ToString(intMyIp1)
                    + "." + Convert.ToString(intMyIp2)
                    + "." + Convert.ToString(intMyIp3);

                return gatewayAddress.Split('.');
            }
            return null;
        }

        public string GetIPAddress()
        {
            WifiManager manager = (WifiManager)Forms.Context.GetSystemService(Android.Content.Context.WifiService);
            if (manager.IsWifiEnabled)
            {
                int myIp = manager.ConnectionInfo.IpAddress;
                int intMyIp3 = myIp / 0x1000000;
                int intMyIp3mod = myIp % 0x1000000;
                int intMyIp2 = intMyIp3mod / 0x10000;
                int intMyIp2mod = intMyIp3mod % 0x10000;
                int intMyIp1 = intMyIp2mod / 0x100;
                int intMyIp0 = intMyIp2mod % 0x100;

                string ipAddress = Convert.ToString(intMyIp0)
                     + "." + Convert.ToString(intMyIp1)
                     + "." + Convert.ToString(intMyIp2)
                     + "." + Convert.ToString(intMyIp3);

                return ipAddress;
            }
            return string.Empty;
        }
    }
}