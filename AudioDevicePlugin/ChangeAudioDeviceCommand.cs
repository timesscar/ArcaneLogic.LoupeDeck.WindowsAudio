namespace Loupedeck.AudioDevicePlugin
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using AudioSwitcher.AudioApi.CoreAudio;

    using Loupedeck.AudioDevicePlugin.Settings;

    public class ChangeAudioDeviceCommand : PluginDynamicCommand
    {
        private readonly List<CoreAudioDevice> deviceCache;

        private readonly CoreAudioController controller;

        private readonly WindowsAudioPluginSettingsConfigurationSection config;

        public ChangeAudioDeviceCommand() : base()
        {
            this.controller = new CoreAudioController();
            this.deviceCache = this.controller.GetDevices().ToList();

            foreach(var device in this.deviceCache.Where(c => c.IsPlaybackDevice))
            {
                this.AddParameter(device.FullName, device.FullName, "Set Playback Device");
            }

            foreach (var device in this.deviceCache.Where(c => c.IsCaptureDevice))
            {
                this.AddParameter(device.FullName, device.FullName, "Set Recording Device");
            }

            var executingAssembly = Assembly.GetExecutingAssembly();
            var configPath = Path.Combine(executingAssembly.GetFilePath(), executingAssembly.GetFilePathName() + ".config");
            using (AppConfigRemapper.Change(configPath))
            {
                this.config = WindowsAudioPluginSettingsConfigurationSection.Current;
            }
        }

        protected override void RunCommand(String actionParameter)
        {
            var matchingAudioDevice = this.deviceCache.Where(c => c.FullName == actionParameter).FirstOrDefault();

            if (matchingAudioDevice == null)
            {
                return;
            }

            this.controller.SetDefaultDevice(matchingAudioDevice);
        }

        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var matching = this.config.CustomImages.Where(c => c.DeviceName == actionParameter).FirstOrDefault();

            string fileName = "invalid.png";

            if (matching != null)
                fileName = matching.ImageName;

            var matchingAudioDevice = this.deviceCache.Where(c => c.FullName == actionParameter).FirstOrDefault();

            BitmapImage convertedImage;
            if(!matchingAudioDevice.IsDefaultDevice)
            {
                convertedImage = this.WashOutImage(fileName);
            }
            else
            {
                convertedImage = BitmapImage.FromFile(fileName);
            }

            convertedImage.Resize(imageSize.GetSize(), imageSize.GetSize());

            return convertedImage;
        }

        /// <summary>
        /// Loads an image and removes some of the color 
        /// </summary>
        /// <param name="fileName">The file name to load.</param>
        /// <returns>The adjusted image.</returns>
        private BitmapImage WashOutImage(string fileName)
        {
            BitmapImage convertedImage;
            using (var image = Bitmap.FromFile(fileName))
            {
                using (var graphics = Graphics.FromImage(image))
                {
                    float[][] colorMatrixElements = {
                       new float[] {.2f,  0,  0,  0, 0},        // red scaling factor of 2
                       new float[] {0,  .2f,  0,  0, 0},        // green scaling factor of 1
                       new float[] {0,  0,  .2f,  0, 0},        // blue scaling factor of 1
                       new float[] {0,  0,  0,  1, 0},        // alpha scaling factor of 1
                       new float[] { 0, 0, 0, 0, 1}};

                    ColorMatrix matrix = new ColorMatrix(colorMatrixElements);

                    //create image attributes  
                    ImageAttributes attributes = new ImageAttributes();

                    //set the color(opacity) of the image  
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    //now draw the image  
                    graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);

                    graphics.Flush();
                }

                byte[] result = null;

                using (var stream = new MemoryStream())
                {
                    image.Save(stream, ImageFormat.Png);
                    result = stream.ToArray();
                }

                convertedImage = new BitmapImage(result);
            }

            return convertedImage;
        }
    }
}
