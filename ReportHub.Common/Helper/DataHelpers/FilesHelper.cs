 
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ReportHub.Common.DataHelpers.IDataHelpers;
using ReportHub.Objects.DTOs.HelpersViewModel;
using System.IO;


namespace ReportHub.Common.DataHelpers;

public class FilesHelper : IFilesHelper
{

    public readonly IConfiguration _configuration;
    public readonly IClient _IClient;
    private bool IsDMS;

    public FilesHelper(IConfiguration configuration, IClient iClient)
    {
        _configuration = configuration;
        IsDMS = Convert.ToBoolean(_configuration.GetSection("FileConfigration:IsDMS").Value);
        _IClient = iClient;
    }

    public async Task<FileResponse> CheckFileTypeAndSize(IFormFile file)
    {
        var model = new FileResponse();
        try
        {
            if (file == null || file.Length == 0)
            {
                model.Message = "File is Empty";
                model.Type = "error";
                model.IsSuccess = false;
                return model;
            }


            var contentTypesRequired = _configuration["AllowedFileMIME"]?.Split(',').Select(s => s.Trim()).ToArray() ?? Array.Empty<string>();
            var fileExtensionsRequired = _configuration["AllowedFileExtensions"]?.Split(',').Select(s => s.Trim()).ToArray() ?? Array.Empty<string>();
            var fileSize = int.TryParse(_configuration["AllowedFileSize"], out int size) ? size : 0;

            if (file.Length > fileSize * 1024 * 1024)
            {
                model.Message = "FileIsTooLarge";
                model.Type = "error";
                model.IsSuccess = false;
                return model;
            }

            if (!contentTypesRequired.Contains(file.ContentType))
            {
                model.Message = string.Format("{0} - {1} {2}", "Filetypeiswrong", "Filetypesallowedare", ",", contentTypesRequired);
                model.Type = "error";
                model.IsSuccess = false;
                return model;
            }

            // Check if file extension is allowed 
            var fileExtension = Path.GetExtension(file.FileName)?.TrimStart('.').ToLowerInvariant();
            if (!fileExtensionsRequired.Contains(fileExtension) && file.FileName != "blob")
            {
                model.Message = string.Format("{0} - {1} {2}", "Filetypeiswrong", "Filetypesallowedare", ",", fileExtensionsRequired);
                model.Type = "error";
                model.IsSuccess = false;
                return model;
            }

            return model;
        }
        catch
        {
            model.Type = "error";
            model.Message = "ErrorHappened";
            model.IsSuccess = false;
            return model;
        }
    }

    public async Task<FileResponse> CheckListFileTypeAndSize(IEnumerable<IFormFile> files)
    {
        var model = new FileResponse();
        try
        {
            foreach (var file in files)
            {
                model = await CheckFileTypeAndSize(file);
                if (!model.IsSuccess)
                {
                    return model;
                }
            }
            return model;
        }
        catch
        {
            model.Type = "error";
            model.Message = "ErrorHappened";
            model.IsSuccess = false;
            return model;
        }
    }

    public async Task<string> GetSavedFile(string ModuleName, string FolderName, string FilePath, string FileName)
    {
        try
        {
            string _URL = "APPs:" + ModuleName + ":APPURL"; 

            var URL = _configuration["APPs:" + ModuleName + ":APPURL"];
            var directoryPath = "";
            if (ModuleName == "Vehicle" && string.IsNullOrEmpty(FileName))
            {
                directoryPath = _configuration["APPs:" + ModuleName + ":DirectoryPathLogo"];
            }
            else
            {
                directoryPath = _configuration["APPs:" + ModuleName + ":DirectoryPath"];
            }

            var path = Path.Combine(URL, directoryPath, FolderName);
            string finalPath;
            if (string.IsNullOrEmpty(FileName))
            {
                finalPath = Path.Combine(path, FilePath);
            }
            else
            {
                finalPath = Path.Combine(path, FilePath, FileName);
            }

            return finalPath;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<string> GetReportFileURL(string FolderName, string FileName)
    {
        try
        {

            var URL = _configuration["FileConfigration:APPURL"];

            var directoryPath = _configuration["FileConfigration:GenerateFinancialStatement"];

            string finalPath;
            if (!string.IsNullOrEmpty(FolderName))
            {
                finalPath = Path.Combine(URL, directoryPath, FolderName, FileName);
            }
            else
            {
                finalPath = Path.Combine(URL, directoryPath, FileName);
            }
            finalPath = finalPath.Replace("\\", "/");
            if (await IsUrlValid(finalPath))
            {
                return finalPath;

            }
            else
            {
                return "/images/default_car_260.jpeg";
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<bool> IsUrlValid(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

    public async Task<(string, string)> SaveFile(IFormFile file, string path, bool WithPath = true)
    {
        try
        {
            string root = _configuration["FileConfigration:RootPath"];
            if (file == null || path == null)
                return (null, null);

            if (!IsDMS)
            {
                if (string.IsNullOrEmpty(root))
                {
                    root = Environment.CurrentDirectory;
                }

                string FullPath;
                string guid = Guid.NewGuid().ToString();

                if (WithPath)
                {
                    FullPath = Path.Combine(@root, path.TrimStart('\\'), guid);
                }
                else
                {
                    FullPath = Path.Combine(@root, path.TrimStart('\\'));
                }
                if (!Directory.Exists(FullPath))
                {
                    Directory.CreateDirectory(FullPath);
                }

                var filename = $"{DateTime.Now.Ticks}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(FullPath, filename);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                if (File.Exists(filePath))
                {
                    if (WithPath)
                    {
                        return (filename, guid);
                    }
                    else
                    {
                        return (filename, null);
                    }
                }
                else
                {
                    return (null, null);
                }
            }
            else
            {
                //string FtpServer = _configuration.GetSection("FileConfigration:DMSConfig:FtpServer").Value;
                //string FtpUsername = _configuration.GetSection("FileConfigration:DMSConfig:FtpUsername").Value;
                //string FtpPassword = _configuration.GetSection("FileConfigration:DMSConfig:FtpPassword").Value;

                string guid = Guid.NewGuid().ToString();
                string filename = $"{DateTime.Now.Ticks}{Path.GetExtension(file.FileName)}";
                string ftpPath = WithPath ? $"{path.TrimStart('/')}/{guid}" : $"{path.TrimStart('/')}";

                string localFilePath;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    localFilePath = Path.Combine(Path.GetTempPath(), filename);
                    await File.WriteAllBytesAsync(localFilePath, memoryStream.ToArray());
                }

                var uploadModel = new Objects.DTOs.HelpersViewModel.WebClientViewModel.UploadToFtpViewModel
                {
                    FtpServer = _configuration.GetSection("FileConfigration:DMSConfig:FtpServer").Value,
                    FtpUsername = _configuration.GetSection("FileConfigration:DMSConfig:FtpUsername").Value,
                    FtpPassword = _configuration.GetSection("FileConfigration:DMSConfig:FtpPassword").Value,
                    LocalFilePath = localFilePath,
                    FtpPath = ftpPath,
                    FileName = filename
                };

                FtpUploadResult result = await _IClient.UploadFileToFtp(uploadModel);

                if (result.IsSuccess)
                {
                    if (WithPath)
                    {
                        return (filename, guid);
                    }
                    else
                    {
                        return (filename, null);
                    }
                }
                else
                {
                    throw new Exception($"FTP upload failed: {result.ErrorMessage}");
                }
            }

        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while saving the file: {ex.Message}", ex);
        }
    }
    public async Task<string> GetReportPath()
    {
        try
        {
            var RootPath = _configuration.GetSection("FileConfigration:RootPath").Value;
            var GenerateFinancialStatement = _configuration.GetSection("FileConfigration:GenerateFinancialStatement").Value;
            var FullPath = Path.Combine(RootPath, GenerateFinancialStatement);

            return FullPath;
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public class FileResponse
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
    public async Task<(string fileName, string fileExt, Guid guid, string FullPath)> SaveAttachment(IFormFile file, string path)
    {
        try
        {
            int allowedFileSizeInMB =Convert.ToInt16( _configuration["AllowedFileSize"]);
            var allowedFileSizeInBytes = allowedFileSizeInMB * 1024 * 1024;
            string root = _configuration["FileConfigration:RootPath"];
            if (file.Length > allowedFileSizeInBytes)
                return (null, null, Guid.Empty, null);
            if (string.IsNullOrEmpty(root))
            {
                root = Environment.CurrentDirectory;
            }

            if (file == null || path == null)
                return (null, null, Guid.Empty, null);

            string FullPath;
            Guid guid = Guid.NewGuid();
            FullPath = Path.Combine(root, path);

            if (!Directory.Exists(FullPath))
            {
                Directory.CreateDirectory(FullPath);
            }
            string extension = Path.GetExtension(file.FileName);
            string fileNameWithGuid = $"{guid}{extension}";
            string filePath = Path.Combine(FullPath, fileNameWithGuid);
            string FileName = file.FileName.Replace(extension, "");
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return (FileName, extension, guid, filePath);
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while saving the file: {ex.Message}", ex);
        }

    }

    public async Task<bool> DeleteAttachment(string fullPath)
    {
        try
        {
            string root = _configuration["FileConfigration:RootPath"];
            fullPath = fullPath.Replace("/", Path.DirectorySeparatorChar.ToString()).TrimStart(Path.DirectorySeparatorChar, '/');

            string AttPath;
            AttPath = Path.Combine(root, fullPath);
            if (!File.Exists(AttPath))
                return false;
            File.Delete(AttPath);
            return true;

        }
        catch (Exception)
        {
            return false;
        }
    }
}
