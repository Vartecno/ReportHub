using System.Runtime.Serialization;

namespace ReportHub.Objects.DTOs.HelpersViewModel;

public class APIResultViewModel<T> where T : class
{
    public string Code { get; set; }
    public string Message { get; set; }
    public string ErrorMessage { get; set; }
    public string ErrorInnerMessage { get; set; }
    public T Content { get; set; }
}
public class FtpUploadResult
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
}

[DataContract]
[Serializable]
public class Response
{
    [DataMember]
    public int ErrorCode { get; set; }

    [DataMember]
    public string ErrorMessage { get; set; }

    [DataMember]
    public bool IsScusses { get; set; }

    [DataMember]
    public object ResponseDetails { get; set; }
}
