using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace RemoteEye.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService2" in both code and config file together.
    [ServiceContract]
    public interface IErmIoTService
    {
        //    [OperationContract]
        //    void DoWork();

        //    [OperationContract]
        //    //[WebInvoke(Method = "POST", UriTemplate = "MediaUpload/{fileName}")]

        //   // [WebInvoke(UriTemplate = "UploadJobMedia/{camId}/{imageId}/{base64Image}/{timestamp}")]
        //    [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        //    //double Plus(double a, double b);
        //    void UploadJobMedia(string camId, string imageId, string base64Image, string timestamp);

        //    [OperationContract]
        //    [WebInvoke(Method = "POST", UriTemplate = "FileUpload/{fileStream}")]
        //    void FileUpload(Stream fileStream);

        [OperationContract]
        void UploadFile(string request);
    }

[MessageContract]
public class RemoteFileInfo : IDisposable
{
    [MessageHeader(MustUnderstand = true)]
    public string FileName;

    [MessageHeader(MustUnderstand = true)]
    public long Length;

    [MessageBodyMember(Order = 1)]
    public System.IO.Stream FileByteStream;

    public void Dispose()
    {
        if (FileByteStream != null)
        {
            FileByteStream.Close();
            FileByteStream = null;
        }
    }
}
}
