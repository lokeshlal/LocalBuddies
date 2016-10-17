using System;
using Xamarin.Forms;
using LocalBuddies.Mobile.UI.Droid;
using System.Net;
using System.Net.Sockets;
using LocalBuddies.Mobile.UI.Network;

[assembly: Dependency(typeof(Sockets))]
namespace LocalBuddies.Mobile.UI.Droid
{
    public class Sockets : ISockets
    {
        public void InitiateReciever(Action<object, string> callback)
        {
            byte[] data = new byte[1024];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9050);
            UdpClient newsock = new UdpClient(ipep);
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                data = newsock.Receive(ref sender);
                callback(data, sender.Address.ToString());
            }
        }

        public void SendMessage(string ipAddress, byte[] encodedData)
        {
            var client = new UdpClient(ipAddress, 9050);
            client.Send(encodedData, encodedData.Length);
            client.Close();
        }
    }
}