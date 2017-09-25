using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace RemoteEye.App
{
    public class CamAgent
    {
        public string Id { get; set; }
        private CaptureElement element;
        public CamAgent(string id, CaptureElement element)
        {
            this.Id = id;
            this.element = element;
        }

        #region Fields

        private DeviceInformation camera;
        private MediaCapture mediaCapture;

        #endregion

        private async Task Initialize()
        {
            camera = await GetCamera(Id);
            mediaCapture = await GetMediaCapture(element);
        }

        public async Task<MediaCapture> GetMediaCapture(CaptureElement element)
        {
            var captureManager = new MediaCapture();
            await captureManager.InitializeAsync(new MediaCaptureInitializationSettings
            {
                StreamingCaptureMode = StreamingCaptureMode.Video,
                AudioDeviceId = string.Empty,
                VideoDeviceId = camera.Id
            });

            element.Source = captureManager;
            await captureManager.StartPreviewAsync();

            return captureManager;
        }

        public async Task<DeviceInformation> GetCamera(string id)
        {
            var cams = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            return cams.FirstOrDefault(x => x.Id == id);
        }

        public static async Task<DeviceInformationCollection> GetAllCameras()
        {
            return await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
        }

        public async Task StartPreviewAsync()
        {
            await Initialize();
        }

        public async Task StopPreviewAsync()
        {
           await mediaCapture.StopPreviewAsync();
        }
        public async Task<StorageFile> CaptureImage()
        {
            var imgFormat = ImageEncodingProperties.CreateJpeg();
           

            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                    "capture.jpg", CreationCollisionOption.GenerateUniqueName);
            
            await mediaCapture.CapturePhotoToStorageFileAsync(imgFormat, file);

            return file;
        }

        public static async Task<string> ImageToBase64(StorageFile file)
        {
            Stream ms = await file.OpenStreamForReadAsync();
            byte[] imageBytes = new byte[(int)ms.Length];
            ms.Read(imageBytes, 0, (int)ms.Length);
            return Convert.ToBase64String(imageBytes);
        }
    }
}