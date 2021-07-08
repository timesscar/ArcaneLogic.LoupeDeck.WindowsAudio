namespace Loupedeck.AudioDevicePlugin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using AudioSwitcher.AudioApi;
    using AudioSwitcher.AudioApi.CoreAudio;

    public class MuteDefaultMicCommand : PluginDynamicCommand
    {
        readonly CoreAudioController controller;

        public static event EventHandler MicMuteStateChangeRequested;

        public MuteDefaultMicCommand() : base("Mute Default Mic", "Toggles mute on the default communication device", "Audio")
        {
            this.controller = new CoreAudioController();
        }

        protected override void RunCommand(String actionParameter)
        {
            var defaultMic = this.controller.GetDefaultDevice(DeviceType.Capture, Role.Communications);

            defaultMic.ToggleMute();

            MicMuteStateChangeRequested?.Invoke(this, new EventArgs());
        }
    }
}
