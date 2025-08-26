

using ReportHub.Objects.DTOs.HelpersViewModel;
using static ReportHub.Objects.DTOs.HelpersViewModel.WebClientViewModel;

namespace ReportHub.Common.DataHelpers.IDataHelpers;

public interface IClient
{
    public Task<string> Get(GetRequestViewModel Model);
    public Task<string> Get(string URL, string BaseUrl = "");
    public Task<string> Delete(GetRequestViewModel Model);
    public Task<string> Delete(string URL, string BaseUrl = "");
    public Task<string> Post(PostRequestViewModel Model);
    public Task<string> Post(RequestWithFormDataViewModel Model);
    public Task<string> Get(RequestWithFormDataViewModel Model);
    public Task<string> Put(PostRequestViewModel Model);
    public Task<FtpUploadResult> UploadFileToFtp(UploadToFtpViewModel model);

}
