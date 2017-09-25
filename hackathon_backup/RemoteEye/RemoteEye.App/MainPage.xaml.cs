using Microsoft.AspNet.SignalR.Client;
using RemoteEye.App.ErmServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RemoteEye.App
{

    public class CamTimer : DispatcherTimer
    {
        public string Id { get; set; }
    }


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Queue<Action> JobMediaQueue = new Queue<Action>();
        public List<CamAgent> Cameras { get; set; }
        public List<CamTimer> Timers { get; set; }
        private DispatcherTimer queueTimer;
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            this.Cameras = new List<CamAgent>();
            this.Timers = new List<CamTimer>();
            queueTimer = new DispatcherTimer();
            queueTimer.Tick += QueueTimer_Tick;
            queueTimer.Interval = new TimeSpan(0,0,5);
            queueTimer.Start();
        }

        private void QueueTimer_Tick(object sender, object e)
        {
            while (JobMediaQueue.Any())
            {
                Action action = JobMediaQueue.Dequeue();
                action();
            }
        }

        private CoreDispatcher _dispatcher;
        private MediaCapture capture;

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            var cams = await CamAgent.GetAllCameras();
            foreach (var camera in cams)
            {
                this.Cameras.Add(new CamAgent(camera.Id, PhotoPreview));
                this.Timers.Add(GetTimer(camera.Id));
            }

            await RegisterOnSignalR();
            //var client = new ErmIoTServiceClient();
            //await client.UploadFileAsync(string.Empty);

            RegisterCamera(@"\\?\USB#VID_046D&PID_082B&MI_00#6&ff670fd&0&0000#{e5323777-f976-4f5b-9b55-b94699c46e44}\GLOBAL", 10);
        }

        private CamTimer GetTimer(string id)
        {
            var timer = new CamTimer();
            timer.Id = id;
            timer.Tick += Timer_Tick;
            return timer;
        }

        private async void Timer_Tick(object sender, object e)
        {
            JobMediaQueue.Enqueue(async () =>
            {
                var id = (sender as CamTimer).Id;
                this.messageText.Text = "Capturing image...";
                var camera = this.Cameras.FirstOrDefault(x => x.Id == id);
                await camera.StartPreviewAsync();
                var file = await camera.CaptureImage();
                await camera.StopPreviewAsync();
                this.messageText.Text = "Uploading image...";
                await Task.Delay(1000);

                var base64img = await CamAgent.ImageToBase64(file);
                var client = new ErmIoTServiceClient();
                ((BasicHttpBinding)client.Endpoint.Binding).MaxReceivedMessageSize = int.MaxValue;
                ((BasicHttpBinding)client.Endpoint.Binding).MaxBufferSize = int.MaxValue;
                ((BasicHttpBinding)client.Endpoint.Binding).MaxBufferPoolSize = long.MaxValue;

                await client.UploadFileAsync(base64img);
                this.messageText.Text = "Upload successful.";
                await Task.Delay(1000);
            });

           
        }

        private async Task RegisterOnSignalR()
        {
            string url = "http://192.168.31.73:5301/signalr";
            var connection = new HubConnection(url);
            var hubProxy = connection.CreateHubProxy("NotificationHub");
            await connection.Start();
            hubProxy.On<string, int>("GetJobStatus", (camId, interval) => { RegisterCamera(camId, interval); });
        }

        public string Uptime { get; set; }

        private async void RegisterCamera(string camId, int interval)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    var timer = Timers.FirstOrDefault(x => x.Id == camId);
                    timer.Interval = new TimeSpan(0, 0, interval);
                    timer.Start();
                });
        }

        //public static async Task<byte[]> GetBytesAsync(StorageFile file)
        //{
        //    byte[] fileBytes = null;
        //    if (file == null) return null;
        //    using (var stream = await file.OpenReadAsync())
        //    {
        //        fileBytes = new byte[stream.Size];
        //        using (var reader = new DataReader(stream))
        //        {
        //            await reader.LoadAsync((uint)stream.Size);
        //            reader.ReadBytes(fileBytes);
        //        }
        //    }
        //    return fileBytes;
        //}
    }
}
