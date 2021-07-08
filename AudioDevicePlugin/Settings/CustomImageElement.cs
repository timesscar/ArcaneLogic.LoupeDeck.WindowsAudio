
namespace Loupedeck.AudioDevicePlugin.Settings
{
    using System.Configuration;

    /// <summary>
    /// Configuration manager custom section for jit groups.
    /// </summary>
    public class CustomImageElement : ConfigurationSection
    {

        /// <summary>
        /// Gets the account type.
        /// </summary>
        [ConfigurationProperty(nameof(DeviceName), IsRequired = true)]
        public string DeviceName
        {
            get
            {
                return (string)this[nameof(this.DeviceName)];
            }
        }

        /// <summary>
        /// Gets the account type.
        /// </summary>
        [ConfigurationProperty(nameof(ImageName), IsRequired = true)]
        public string ImageName
        {
            get
            {
                return (string)this[nameof(this.ImageName)];
            }
        }
    }
}
