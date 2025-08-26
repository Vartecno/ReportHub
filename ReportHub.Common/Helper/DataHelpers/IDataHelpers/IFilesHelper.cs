using Microsoft.AspNetCore.Http;
using static ReportHub.Common.DataHelpers.FilesHelper;

namespace ReportHub.Common.DataHelpers.IDataHelpers;

public interface IFilesHelper
{
    public Task<FileResponse> CheckFileTypeAndSize(IFormFile file);
    public Task<FileResponse> CheckListFileTypeAndSize(IEnumerable<IFormFile> files);
    public Task<(string, string)> SaveFile(IFormFile file, string path, bool WithPath = true);
    public Task<string> GetSavedFile(string ModuleName, string FolderName, string FilePath, string FileName);
    public Task<string> GetReportFileURL(string FolderName, string FileName);
    public Task<bool> IsUrlValid(string url);
    public Task<string> GetReportPath();
    public Task<(string fileName, string fileExt, Guid guid, string FullPath)> SaveAttachment(IFormFile file, string Path);
    public Task<bool> DeleteAttachment(string fullPath);
}
