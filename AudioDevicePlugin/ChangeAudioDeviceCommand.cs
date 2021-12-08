namespace Loupedeck.AudioDevicePlugin
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using AudioSwitcher.AudioApi.CoreAudio;

    using Loupedeck.AudioDevicePlugin.Settings;

    /// <summary>
    /// Plugin command that changes the default audio device.
    /// </summary>
    public class ChangeAudioDeviceCommand : PluginDynamicCommand
    {
        private readonly List<CoreAudioDevice> deviceCache;

        private readonly CoreAudioController controller;

        private readonly WindowsAudioPluginSettingsConfigurationSection config;

        private readonly Dictionary<string, string> imageCache = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeAudioDeviceCommand"/> class.
        /// </summary>
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

            foreach(CustomImageElement item in this.config.CustomImages)
            {
                if (!this.imageCache.ContainsKey(item.ImageName))
                    this.imageCache.Add(item.ImageName, this.GetBitmapString(item.ImageName));
            }

            this.imageCache.Add("invalid.png", this.GetBitmapString("invalid.png"));
        }

        /// <inheritdoc />
        protected override void RunCommand(String actionParameter)
        {
            var matchingAudioDevice = this.deviceCache.Where(c => c.FullName == actionParameter).FirstOrDefault();

            if (matchingAudioDevice == null)
            {
                return;
            }

            this.controller.SetDefaultDevice(matchingAudioDevice);
        }

        /// <inheritdoc />
        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var matching = this.config.CustomImages.Where(c => c.DeviceName == actionParameter).FirstOrDefault();

            var fileName = "invalid.png";

            if (matching != null)
                fileName = matching.ImageName;

            var matchingAudioDevice = this.deviceCache.Where(c => c.FullName == actionParameter).FirstOrDefault();

            bool washOutImage = !(matchingAudioDevice != null && matchingAudioDevice.IsDefaultDevice);

            var convertedImage = !washOutImage
                ? BitmapImage.FromBase64String(this.imageCache[fileName])
                : this.WashOutImage(this.imageCache[fileName]);

            convertedImage.Resize(imageSize.GetSize(), imageSize.GetSize());

            return convertedImage;
        }

        /// <summary>
        /// Loads an image and removes some of the color 
        /// </summary>
        /// <param name="base64Image">The base64 encoded image</param>
        /// <returns>The adjusted image.</returns>
        private BitmapImage WashOutImage(string base64Image)
        {
            BitmapImage convertedImage;

            var bytes = Convert.FromBase64String(base64Image);

            using (var inStream = new MemoryStream(bytes))
            {
                using (var image = Bitmap.FromStream(inStream))
                {
                    using (var graphics = Graphics.FromImage(image))
                    {
                        // Apply a .2 multiplier to the rgb color channels
                        float[][] colorMatrixElements = {
                           new float[] {.2f,  0,  0,  0, 0},
                           new float[] {0,  .2f,  0,  0, 0},
                           new float[] {0,  0,  .2f,  0, 0},
                           new float[] {0,  0,  0,  1, 0},
                           new float[] { 0, 0, 0, 0, 1}};

                        var matrix = new ColorMatrix(colorMatrixElements);

                        var attributes = new ImageAttributes();
                        attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                        graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);

                        graphics.Flush();
                    }

                    byte[] result = null;

                    using (var outStream = new MemoryStream())
                    {
                        image.Save(outStream, ImageFormat.Png);
                        result = outStream.ToArray();
                    }

                    convertedImage = new BitmapImage(result);
                }
            }

            return convertedImage;
        }

        /// <summary>
        /// Loads a bitmap image from disk and converts it to a base64 string
        /// </summary>
        /// <param name="imageName">The image name.  The image is loaded from <see cref="Constants.ImagesFolderName"/></param>
        /// <returns>The image encoded as a base64 string.</returns>
        private string GetBitmapString(string imageName)
        {
            var currLoc = Assembly.GetExecutingAssembly().GetFilePath();

            byte[] result = null;

            using (var image = Bitmap.FromFile(Path.Combine(currLoc, Constants.ImagesFolderName, imageName)))
            {
                using (var stream = new MemoryStream())
                {
                    image.Save(stream, ImageFormat.Png);
                    result = stream.ToArray();
                }
            }

            return Convert.ToBase64String(result);
        }
    }
}
