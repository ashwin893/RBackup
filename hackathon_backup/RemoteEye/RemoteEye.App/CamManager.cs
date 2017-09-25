using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace RemoteEye.App
{
    public class CamManager
    {
        public CamManager()
        {
            Initialize();
        }

        #region Fields

        private DeviceInformationCollection cameras;

        #endregion

        private async void Initialize()
        {
            cameras = await GetAllCameras();
        }

        public async Task<MediaCapture> GetMediaCapture(string id, CaptureElement element)
        {
            var captureManager = new MediaCapture();
            await captureManager.InitializeAsync(new MediaCaptureInitializationSettings
            {
                StreamingCaptureMode = StreamingCaptureMode.Video,
                AudioDeviceId = string.Empty,
                VideoDeviceId = id
            });

            element.Source = captureManager;
            await captureManager.StartPreviewAsync();

            return captureManager;
        }

        public async Task<DeviceInformationCollection> GetAllCameras()
        {
            return await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
        }


        public async Task StopPreviewAsync(MediaCapture capture)
        {
           await capture.StopPreviewAsync();
        }
        public async Task<StorageFile> CaptureImage(MediaCapture capture)
        {
            var imgFormat = ImageEncodingProperties.CreateJpeg();

            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                    "Photo.jpg", CreationCollisionOption.ReplaceExisting);

            await capture.CapturePhotoToStorageFileAsync(imgFormat, file);

            return file;
        }
    }
}