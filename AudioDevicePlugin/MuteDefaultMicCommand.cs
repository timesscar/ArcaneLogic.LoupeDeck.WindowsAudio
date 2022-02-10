namespace Loupedeck.AudioDevicePlugin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    using AudioSwitcher.AudioApi;
    using AudioSwitcher.AudioApi.CoreAudio;

    using Loupedeck.AudioDevicePlugin.Settings;

    public class MuteDefaultMicCommand : PluginDynamicCommand
    {
        readonly CoreAudioController controller;

        public static event EventHandler MicMuteStateChangeRequested;

        private readonly Dictionary<bool, string> imageCache;

        private readonly WindowsAudioPluginSettingsConfigurationSection config;

        public MuteDefaultMicCommand() : base("Mute Default Mic", "Toggles mute on the default communication device", "Audio")
        {
            this.controller = new CoreAudioController();

            var executingAssembly = Assembly.GetExecutingAssembly();
            var configPath = Path.Combine(executingAssembly.GetFilePath(), executingAssembly.GetFilePathName() + ".config");
            using (AppConfigRemapper.Change(configPath))
            {
                this.config = WindowsAudioPluginSettingsConfigurationSection.Current;
            }

            var currLoc = Assembly.GetExecutingAssembly().GetFilePath();
            this.imageCache = new Dictionary<bool, string>
            {
                { true, BitmapImage.FromFile(Path.Combine(currLoc, Constants.ImagesFolderName, this.config.MuteIcon)).ToBase64String() },
                { false, BitmapImage.FromFile(Path.Combine(currLoc, Constants.ImagesFolderName, this.config.UnmuteIcon)).ToBase64String() }
            };
        }

        /// <inheritdoc />
        protected override void RunCommand(String actionParameter)
        {
            var defaultMic = this.controller.GetDefaultDevice(DeviceType.Capture, Role.Communications);

            defaultMic.ToggleMute();

            MicMuteStateChangeRequested?.Invoke(this, new EventArgs());
            this.ActionImageChanged();
        }

        /// <inheritdoc />
        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var muteState = this.GetDefaultCommDeviceMuteState();
            var desiredImage = BitmapImage.FromBase64String(this.imageCache[muteState]);

            int dimensions = (int)(imageSize.GetSize() * .7);
            desiredImage.Resize(dimensions, dimensions);
            return desiredImage;
        }

        /// <summary>
        /// Gets the mute state of the current default communications device.
        /// </summary>
        /// <returns>A value indicating whether or not the device is muted.</returns>
        private bool GetDefaultCommDeviceMuteState()
        {
            var defaultMic = this.controller.GetDefaultDevice(DeviceType.Capture, Role.Communications);

            return defaultMic.IsMuted;
        }

    }
}
