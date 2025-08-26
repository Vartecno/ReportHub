
using Newtonsoft.Json;
using ReportHub.Common.DataHelpers.IDataHelpers;
using ReportHub.Objects.DTOs.HelpersViewModel;
using ReportHub.Objects.Enum;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using static ReportHub.Objects.DTOs.HelpersViewModel.WebClientViewModel;


namespace ReportHub.Common.DataHelpers
{
    public class Client : IClient
    {
        private readonly IHttpClientFactory _clientFactory;
        public Client(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        private void AddContentAndContentType(HttpRequestMessage Request, string Content)
        {
            Request.Headers.Add("Accept", "application/json");
            Request.Content = new StringContent(Content, Encoding.UTF8);
            Request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        private void AddContentAndContentType(HttpRequestMessage Request, MultipartFormDataContent content)
        {
            Request.Content = content;
            //Request.Headers.Add("Accept", "multipart/form-data;");
            //Request.Content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data;");
        }


        private async Task<string> MakeARequest(HttpMethod Method, GeneralRequestViewModel Model)
        {
            HttpClient Client = _clientFactory.CreateClient();
            if (!string.IsNullOrEmpty(Model.BaseUrl))
            {
                Client.BaseAddress = new Uri(Model.BaseUrl);
            }
            var Request = new HttpRequestMessage(Method, Model.Url);
            switch (Model.ContentType)
            {
                case EContentType.JSON:
                    AddContentAndContentType(Request, Model.SerializedContent);
                    break;
                case EContentType.FormData:
                    AddContentAndContentType(Request, Model.FormDataContent);
                    break;
            }
            if (Model.Headers != null)
            {
                foreach (var Header in Model.Headers)
                {

                    Request.Headers.Add(Header.Key, Header.Value);
                }
            }
            HttpResponseMessage Response = await Client.SendAsync(Request);
            Client.Dispose();
            var ResponseStringContent = await Response.Content.ReadAsStringAsync();
            var Error = JsonConvert.DeserializeObject<APIResultViewModel<dynamic>>(ResponseStringContent);
            if (!Response.IsSuccessStatusCode ||
                !string.IsNullOrEmpty(Error.ErrorMessage) ||
                !string.IsNullOrEmpty(Error.ErrorInnerMessage))
            {
                if (Error != null)
                {
                    var ErrorMessage = Error.Message;
                    if (string.IsNullOrEmpty(ErrorMessage))
                    {
                        throw new Exception(ResponseStringContent);
                    }
                    throw new Exception(ErrorMessage);
                }
                throw new Exception(Response.ReasonPhrase);
            }
            return ResponseStringContent;
        }

        #region GET 
        public async Task<string> Get(GetRequestViewModel Model)
        {
            GeneralRequestViewModel GeneralModel = new GeneralRequestViewModel()
            {
                SerializedContent = "",
                Url = Model.Url,
                Headers = Model.Headers,
                BaseUrl = Model.BaseUrl,
            };
            return await MakeARequest(HttpMethod.Get, GeneralModel);
        }


        public async Task<string> Get(string URL, string BaseUrl = "")
        {
            GeneralRequestViewModel GeneralModel = new GeneralRequestViewModel()
            {
                SerializedContent = "",
                Url = URL,
                BaseUrl = BaseUrl,
            };
            return await MakeARequest(HttpMethod.Get, GeneralModel);
        }

        public async Task<string> Get(RequestWithFormDataViewModel Model)
        {
            GeneralRequestViewModel GeneralModel = new GeneralRequestViewModel()
            {
                FormDataContent = Model.Content,
                ContentType = EContentType.FormData,
                Url = Model.Url,
                Headers = Model.Headers,
                BaseUrl = Model.BaseUrl
            };
            return await MakeARequest(HttpMethod.Get, GeneralModel);
        }

        #endregion

        #region Delete 
        public async Task<string> Delete(GetRequestViewModel Model)
        {
            GeneralRequestViewModel GeneralModel = new GeneralRequestViewModel()
            {
                SerializedContent = "",
                Url = Model.Url,
                Headers = Model.Headers,
                BaseUrl = Model.BaseUrl,
            };
            return await MakeARequest(HttpMethod.Delete, GeneralModel);
        }

        public async Task<string> Delete(string URL, string BaseUrl = "")
        {
            GeneralRequestViewModel GeneralModel = new GeneralRequestViewModel()
            {
                SerializedContent = "",
                Url = URL,
                BaseUrl = BaseUrl,
            };
            return await MakeARequest(HttpMethod.Delete, GeneralModel);
        }

        #endregion


        #region Post 
        public async Task<string> Post(PostRequestViewModel Model)
        {
            GeneralRequestViewModel GeneralModel = new GeneralRequestViewModel()
            {
                SerializedContent = Model.SerializedContent,
                Url = Model.Url,
                Headers = Model.Headers,
                BaseUrl = Model.BaseUrl
            };
            return await MakeARequest(HttpMethod.Post, GeneralModel);
        }


        public async Task<string> Post(RequestWithFormDataViewModel Model)
        {
            GeneralRequestViewModel GeneralModel = new GeneralRequestViewModel()
            {
                FormDataContent = Model.Content,
                ContentType = EContentType.FormData,
                Url = Model.Url,
                Headers = Model.Headers,
                BaseUrl = Model.BaseUrl
            };
            return await MakeARequest(HttpMethod.Post, GeneralModel);
        }
        #endregion

        #region Put

        public async Task<string> Put(PostRequestViewModel Model)
        {
            GeneralRequestViewModel GeneralModel = new GeneralRequestViewModel()
            {
                SerializedContent = Model.SerializedContent,
                Url = Model.Url,
                Headers = Model.Headers,
                BaseUrl = Model.BaseUrl
            };
            return await MakeARequest(HttpMethod.Put, GeneralModel);
        }
        #endregion

        #region FTPUpdload


        public async Task<FtpUploadResult> UploadFileToFtp(UploadToFtpViewModel model)
        {
            try
            {

                string ftpFullPath = $"{model.FtpServer}/{model.FtpPath.TrimStart('/')}/{model.FileName}";

                try
                {
                    using (var client = new WebClient())
                    {
                        client.Credentials = new NetworkCredential(model.FtpUsername, model.FtpPassword);
                        byte[] fileContents = await File.ReadAllBytesAsync(ftpFullPath);
                        await client.UploadDataTaskAsync(new Uri(ftpFullPath), fileContents);
                    }
                    return new FtpUploadResult { IsSuccess = true };
                }
                catch (Exception ex)
                {
                    return new FtpUploadResult { IsSuccess = false, ErrorMessage = ex.Message };
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

    }

}
