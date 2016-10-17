namespace LocalBuddies.Mobile.UI.Models
{
    /// <summary>
    /// User object
    /// </summary>
    public class User
    {
        public string IPAddress { get; set; }
        public string Name { get; set; }

        public string DisplayName
        {
            get
            {
                return string.Format("{0} - {1}", Name, IPAddress);
            }
        }
    }
}
