using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Hosting;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using Devart.Data.Oracle;
using System.Data;

namespace RemoteEye.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service2" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service2.svc or Service2.svc.cs at the Solution Explorer and start debugging.
    public class ErmIoTService : IErmIoTService
    {
        //public async void DoWork()
        //{
        //    string url = "http://192.168.31.73:5301/signalr";
        //    var connection = new HubConnection(url);
        //    var hubProxy = connection.CreateHubProxy("MyHub");
        //    await connection.Start();
        //    await hubProxy.Invoke("JobStatusNotification", "CameOne", 5);
        //}

        //public async void UploadJobMedia(string camId, string imageId, string base64Image, string timestamp)
        //{
        //    string url = "http://192.168.31.73:5301/signalr";
        //    var connection = new HubConnection(url);
        //    var hubProxy = connection.CreateHubProxy("MyHub");
        //    await connection.Start();
        //    await hubProxy.Invoke("JobStatusNotification", "CameOne", 5);
        //}

        //public async void FileUpload(Stream fileStream)
        //{
        //    string url = "http://192.168.31.73:5301/signalr";
        //    var connection = new HubConnection(url);
        //    var hubProxy = connection.CreateHubProxy("MyHub");
        //    await connection.Start();
        //    await hubProxy.Invoke("JobStatusNotification", "CameOne", 5);

        //    //FileStream fileToupload = new FileStream("D:\\FileUpload\\" + fileName, FileMode.Create);

        //    //byte[] bytearray = new byte[10000];
        //    //int bytesRead, totalBytesRead = 0;
        //    //do
        //    //{
        //    //    bytesRead = fileStream.Read(bytearray, 0, bytearray.Length);
        //    //    totalBytesRead += bytesRead;
        //    //} while (bytesRead > 0);

        //    //fileToupload.Write(bytearray, 0, bytearray.Length);
        //    //fileToupload.Close();
        //    //fileToupload.Dispose();

        //}

        public async void UploadFile(string name)
        {
            //File.WriteAllText(@"C:\Users\Ravikanth.Kaja\Documents\New folder\michael.txt",name);
            var bytes = Convert.FromBase64String(name);

            InsertJobMedia(@"\\?\USB#VID_046D&PID_082B&MI_00#6&ff670fd&0&0000#{e5323777-f976-4f5b-9b55-b94699c46e44}\GLOBAL", Guid.NewGuid().ToString() + ".jpg", name, DateTime.Now.ToLongDateString());
            //using (var imageFile = new FileStream(@"C:\Users\Ravikanth.Kaja\Documents\New folder\"+ Guid.NewGuid().ToString() +".jpg", FileMode.Create))
            //{
            //    imageFile.Write(bytes, 0, bytes.Length);
            //    imageFile.Flush();
            //}

            //string url = "http://192.168.31.73:5301/signalr";
            //var connection = new HubConnection(url);
            //var hubProxy = connection.CreateHubProxy("NotificationHub");
            //await connection.Start();
            //await hubProxy.Invoke("JobStatusNotification", @"\\?\USB#VID_046D&PID_082B&MI_00#6&ff670fd&0&0000#{e5323777-f976-4f5b-9b55-b94699c46e44}\GLOBAL", 5);


            //FileStream targetStream = null;
            ////Stream sourceStream = request.FileByteStream;

            //string uploadFolder = @"C:\Users\Ravikanth.Kaja\Documents\New folder";

            //string filePath = Path.Combine(uploadFolder, name);

            //using (targetStream = new FileStream(filePath, FileMode.Create,
            //                      FileAccess.Write, FileShare.None))
            //{
            //    //read from the input stream in 65000 byte chunks

            //    const int bufferLen = 65000;
            //    byte[] buffer = new byte[bufferLen];
            //    int count = 0;
            //    while ((count = sourceStream.Read(buffer, 0, bufferLen)) > 0)
            //    {
            //        // save to output stream
            //        targetStream.Write(buffer, 0, count);
            //    }
            //    targetStream.Close();
            //    sourceStream.Close();
            //}

        }

        public void InsertJobMedia(string camId, string imageId, string image, string timeStamp)
        {
            const string connectionString = "User ID=MNSTD_38828; password=MNSTD_38828; Data Source=ERM12;";
            var conn = new OracleConnection(connectionString);

            try
            {

                conn.Open();

                OracleCommand command = new OracleCommand
                {
                    Connection = conn,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "image_biz.insert_images"
                };
                var param1 = new OracleParameter("p_cam_id", OracleDbType.VarChar, camId, ParameterDirection.Input);
                var param2 = new OracleParameter("p_image_id", OracleDbType.VarChar, imageId, ParameterDirection.Input);
                var param3 = new OracleParameter("p_image", OracleDbType.Blob, image, ParameterDirection.Input);
                var param4 = new OracleParameter("p_time_stamp", OracleDbType.VarChar, timeStamp, ParameterDirection.Input);
                command.Parameters.Add(param1);
                command.Parameters.Add(param2);
                command.Parameters.Add(param3);
                command.Parameters.Add(param4);
                command.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

    }
}
