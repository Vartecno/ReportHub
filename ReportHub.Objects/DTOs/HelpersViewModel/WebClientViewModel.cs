

using ReportHub.Objects.Enum;

namespace ReportHub.Objects.DTOs.HelpersViewModel;

public interface WebClientViewModel
{
    public class GetRequestViewModel
    {

        public string Url { get; set; }

        public Dictionary<string, string> Headers { get; set; }
        public string BaseUrl { get; set; }

    }

    public class DeleteRequestViewModel : GetRequestViewModel
    {
    }


    public class PostRequestViewModel
    {
        public PostRequestViewModel()
        {
            Headers = new Dictionary<string, string>();
        }
        public string Url { get; set; }
        public string BaseUrl { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public string SerializedContent { get; set; }
    }

    public class RequestWithFormDataViewModel
    {
        public RequestWithFormDataViewModel()
        {
            Headers = new Dictionary<string, string>();
        }
        public string Url { get; set; }
        public string BaseUrl { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public MultipartFormDataContent Content { get; set; }
    }


    public class PutRequestViewModel : PostRequestViewModel
    {
    }




    public class GeneralRequestViewModel
    {
        public GeneralRequestViewModel()
        {
            Headers = new Dictionary<string, string>();
        }
        public string Url { get; set; }
        public string BaseUrl { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public string SerializedContent { get; set; } = String.Empty;
        public MultipartFormDataContent FormDataContent { get; set; } = null;
        public EContentType ContentType { get; set; } = EContentType.JSON;
    }
    public class UploadToFtpViewModel
    {
        public string FtpServer { get; set; }
        public string FtpUsername { get; set; }
        public string FtpPassword { get; set; }
        public string LocalFilePath { get; set; }
        public string FtpPath { get; set; }
        public string FileName { get; set; }
    }
}
