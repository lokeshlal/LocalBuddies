using Xamarin.Forms;

namespace LocalBuddies.Mobile.UI.Storage
{
    /// <summary>
    /// Manages local storage
    /// </summary>
    public class AppStorage
    {
        #region private constants
        /// <summary>
        /// Key for Name
        /// </summary>
        private const string NAME = "NAME";
        #endregion

        #region public properties
        /// <summary>
        /// Property to access Name storage
        /// </summary>
        public static string Name
        {
            get
            {
                return GetApplicationData(NAME);
            }
            set
            {
                SetApplicationData(NAME, value);
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// Save data in local storage
        /// </summary>
        public static void SaveData()
        {
            Application.Current.SavePropertiesAsync(); //.ConfigureAwait(false);
        }

        #endregion
        /// <summary>
        /// Get application local storage data
        /// </summary>
        /// <param name="v">Key</param>
        /// <returns>value</returns>
        private static string GetApplicationData(string v)
        {
            if (Application.Current.Properties.ContainsKey(v))
            {
                return Application.Current.Properties[v] as string;
            }
            return string.Empty;
        }

        /// <summary>
        /// Add or update the value to application local storage
        /// </summary>
        /// <param name="v">key</param>
        /// <param name="value">value</param>
        private static void SetApplicationData(string v, string value)
        {
            if (Application.Current.Properties.ContainsKey(v))
            {
                Application.Current.Properties[v] = value;
            }
            else
            {
                Application.Current.Properties.Add(v, value);
            }
        }
    }
}
