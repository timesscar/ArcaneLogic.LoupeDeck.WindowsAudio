namespace Loupedeck.AudioDevicePlugin
{
    using System;

    public class AudioDevicePlugin : Plugin
    {
        public override Boolean HasNoApplication => true;

        public override void Load()
        {
        }

        public override void Unload()
        {
        }

        private void OnApplicationStarted(Object sender, EventArgs e)
        {
        }

        private void OnApplicationStopped(Object sender, EventArgs e)
        {
        }

        public override void RunCommand(String commandName, String parameter)
        {
        }

        public override void ApplyAdjustment(String adjustmentName, String parameter, Int32 diff)
        {
        }
    }
}
