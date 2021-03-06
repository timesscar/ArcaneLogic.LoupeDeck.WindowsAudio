
namespace Loupedeck.AudioDevicePlugin.Settings
{
    using System;
    using System.Configuration;
    using System.Reflection;

    /// <summary>
    /// Configuration manager custom section for windows audio settings
    /// </summary>
    public class WindowsAudioPluginSettingsConfigurationSection : ConfigurationSection
    {
        public static string DefaultSectionName = "windowsAudioPluginSettings";

        /// <summary>
        /// Gets the default config section.
        /// </summary>
        public static WindowsAudioPluginSettingsConfigurationSection Current
        {
            get
            {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

                try
                {
                    return ConfigurationManager.GetSection(DefaultSectionName) as WindowsAudioPluginSettingsConfigurationSection;
                }
                finally
                {
                    AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
                }
            }
        }

        /// <summary>
        /// Gets the cooldown for volume icon changes
        /// </summary>
        [ConfigurationProperty(
            nameof(VolumeChangeCooldown),
            IsRequired = true)]
        public int VolumeChangeCooldown
        {
            get
            {
                return (int)this[nameof(this.VolumeChangeCooldown)];
            }
        }

        [ConfigurationProperty(
            nameof(MuteIcon),
            IsRequired = true)]
        public string MuteIcon
        {
            get
            {
                return (string)this[nameof(this.MuteIcon)];
            }
        }

        [ConfigurationProperty(
            nameof(UnmuteIcon),
            IsRequired = true)]
        public string UnmuteIcon
        {
            get
            {
                return (string)this[nameof(this.UnmuteIcon)];
            }
        }

        /// <summary>
        /// Gets a collection of custom image elements
        /// </summary>
        [ConfigurationProperty(nameof(CustomImages))]
        public CustomImageElementCollection CustomImages
        {
            get
            {
                return (CustomImageElementCollection)this[nameof(this.CustomImages)];
            }
        }

        private static Assembly CurrentDomain_AssemblyResolve(Object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("AudioDevicePlugin"))
            {
                return Assembly.GetExecutingAssembly();
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName == args.Name)
                {
                    return assembly;
                }
            }

            return null;
        }
    }
}
