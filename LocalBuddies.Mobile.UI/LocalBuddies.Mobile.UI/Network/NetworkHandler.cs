namespace LocalBuddies.Mobile.UI.Network
{
    using System.Threading.Tasks;
    using Xamarin.Forms;
    using System.Collections.Concurrent;
    using System;
    using System.Text;
    using Models;
    using Storage;
    using Messages;
    using Audio;

    /// <summary>
    /// Enum for clients connection state
    /// </summary>
    public enum ConnectionState
    {
        NotConnected = 0,
        Connected
    }

    /// <summary>
    /// Class to store the clients current state
    /// </summary>
    public static class ConnectionLifeCycle
    {
        public static string CurrentConnected = string.Empty;

        public static ConnectionState CurrentConnectionState = ConnectionState.NotConnected;
    }

    /// <summary>
    /// Class to handle all network related stuff
    /// </summary>
    public class NetworkHandler
    {
        private static bool isListening = false;
        /// <summary>
        /// socket service
        /// </summary>
        static ISockets socketService;
        static IAudioHandler audioHandler;

        public static Action<ConnectionState> CallBackForView { get; internal set; }
        public static Action<User> CallBackForUserDetails { get; internal set; }

        static NetworkHandler()
        {
            socketService = DependencyService.Get<ISockets>();
            audioHandler = DependencyService.Get<IAudioHandler>(); //new Audio.AudioHandler();
        }



        /// <summary>
        /// Discover all connected devices
        /// </summary>
        /// <returns></returns>
        public static void DiscoverDevices()
        {
            ConcurrentDictionary<string, bool> ipAddresses = new ConcurrentDictionary<string, bool>();
            var ipaddressService = DependencyService.Get<IIPAddressManager>();
            string ipaddress = ipaddressService.GetIPAddress();
            string[] gateWayIpAddress = ipaddressService.GetGateWayAddress();

            if (gateWayIpAddress != null)
            {
                var array = gateWayIpAddress;
                Parallel.For(0, 256, new ParallelOptions() { MaxDegreeOfParallelism = 2 }, (index) =>
                {
                    string ping_var = array[0] + "." + array[1] + "." + array[2] + "." + index.ToString();
                    if (!(ipaddress == ping_var || string.Join(".", gateWayIpAddress) == ping_var))
                    {
                        NameRequestCommand(ping_var);
                    }
                });
            }
        }

        /// <summary>
        /// initiate reciever to listen to incomming messages
        /// </summary>
        public static void InitiateReciever()
        {
            if (!isListening)
            {
                socketService.InitiateReciever(callback);
                isListening = !isListening;
            }
        }


        #region call handlers
        public static void InitiateCall(string ipAddress)
        {
            ConnectCommand(ipAddress);
        }

        public static void DisconnectCall(string ipAddress)
        {
            DisconnectCommand(ipAddress);
            ConnectionLifeCycle.CurrentConnected = string.Empty;
            ConnectionLifeCycle.CurrentConnectionState = ConnectionState.NotConnected;
            audioHandler.StopRecording();
        }
        #endregion

        #region call callback handler
        /// <summary>
        /// call back handler for UDP calls
        /// </summary>
        /// <param name="obj">response object</param>
        /// <param name="remoteIpAddress">from IP address</param>
        private static void callback(object obj, string remoteIpAddress)
        {
            var responseBytes = obj as byte[];
            if (responseBytes.Length < 3)
            {
                string response = Encoding.UTF8.GetString(responseBytes, 0, responseBytes.Length);
                if (response == "C")
                {
                    if (ConnectionLifeCycle.CurrentConnectionState == ConnectionState.Connected)
                    {
                        // ignore this message
                        // already connected
                        RejectCommand(remoteIpAddress);
                    }
                    else
                    {
                        ConnectionLifeCycle.CurrentConnectionState = ConnectionState.Connected;
                        ConnectionLifeCycle.CurrentConnected = remoteIpAddress;
                        CallBackForView?.Invoke(ConnectionLifeCycle.CurrentConnectionState);
                        ConnectResponseCommand(remoteIpAddress);
                        audioHandler.StartRecordingAsync(SendAudioCallback);
                    }
                }
                else if (response == "CR")
                {
                    ConnectionLifeCycle.CurrentConnectionState = ConnectionState.Connected;
                    ConnectionLifeCycle.CurrentConnected = remoteIpAddress;
                    CallBackForView?.Invoke(ConnectionLifeCycle.CurrentConnectionState);
                    // connected successfully
                    audioHandler.StartRecordingAsync(SendAudioCallback);
                }
                else if (response == "D")
                {
                    audioHandler.StopRecording();
                    ConnectionLifeCycle.CurrentConnectionState = ConnectionState.NotConnected;
                    ConnectionLifeCycle.CurrentConnected = string.Empty;
                    MessagingCenter.Send(new DisconnectMessage(), "Disconnect");
                    //CallBackForView?.Invoke(ConnectionLifeCycle.CurrentConnectionState);
                    // disconnected successfully
                }
                else if (response == "R")
                {
                    ConnectionLifeCycle.CurrentConnectionState = ConnectionState.NotConnected;
                    ConnectionLifeCycle.CurrentConnected = string.Empty;
                }
                else if (response == "UR")
                {
                    string name = AppStorage.Name;
                    // send this name back to IP Address requested
                    if (name.Length < 3) name = string.Concat(name, "0000".Substring(0, 3 - name.Length));
                    NameResponseCommand(remoteIpAddress, name);
                }
            }
            // request name of the client and validating connection
            else if (responseBytes.Length < 20)
            {
                string response = Encoding.UTF8.GetString(responseBytes, 0, responseBytes.Length);
                if (response.StartsWith("NR-"))
                {
                    string remoteName = response.Split(new char[] { '-' })[1];
                    var ipaddressService = DependencyService.Get<IIPAddressManager>();
                    string ipaddress = ipaddressService.GetIPAddress();
                    if (remoteIpAddress != ipaddress)
                        CallBackForUserDetails?.Invoke(new User() { IPAddress = remoteIpAddress, Name = remoteName });
                }
            }
            else // voice message
            {
                audioHandler.PlayAudio(responseBytes);
            }
        }

        private static void SendAudioCallback(byte[] obj)
        {
            socketService.SendMessage(ConnectionLifeCycle.CurrentConnected, obj);
        }
        #endregion

        #region connection commands
        private static void ConnectCommand(string ipAddress)
        {
            string connect = "C";
            SendCommand(ipAddress, connect);
        }

        private static void NameResponseCommand(string ipAddress, string name)
        {
            name = string.Format("NR-{0}", name);
            SendCommand(ipAddress, name);
        }

        private static void NameRequestCommand(string ipAddress)
        {
            string connect = "UR";
            SendCommand(ipAddress, connect);
        }

        private static void RejectCommand(string ipAddress)
        {
            string connect = "R";
            SendCommand(ipAddress, connect);
        }

        private static void ConnectResponseCommand(string ipAddress)
        {
            string connect = "CR";
            SendCommand(ipAddress, connect);
        }

        private static void DisconnectCommand(string ipAddress)
        {
            string connect = "D";
            SendCommand(ipAddress, connect);
        }

        private static void SendCommand(string ipAddress, string command)
        {
            var bytes = Encoding.UTF8.GetBytes(command);
            socketService.SendMessage(ipAddress, bytes);
        }
        #endregion
    }

    /// <summary>
    /// IP address manager, will be resolved in platform specific project via Xamarin forms dependency service
    /// </summary>
    public interface IIPAddressManager
    {
        string GetIPAddress();
        string[] GetGateWayAddress();
    }

    /// <summary>
    /// Socket manager, will be resolved in platform specific project via Xamarin forms dependency service
    /// </summary>
    public interface ISockets
    {
        void InitiateReciever(Action<object, string> callback);
        void SendMessage(string ipAddress, byte[] encodedData);
    }
}
