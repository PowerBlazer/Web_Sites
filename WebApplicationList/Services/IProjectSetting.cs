using WebApplicationList.Models;
using WebApplicationList.Models.MainSiteModels.ViewModels;

namespace WebApplicationList.Services
{
    public interface IProjectSetting
    {
        Task<string> GetHtmlProject(string nameProject,string page);
        Task<ExplorerViewModel> GetExplorer(Stream fileStream,string username);
        Task<ExplorerViewModel> GetExplorerItem(string directoryPath);
        Task<FileItem> GetContentFileAsync(string filePath);
        Task<bool> ChangeContentFile(FileItem fileItem);
        string GetProjectPath(string username);
        Task<bool> GetValidationProjectName(string projectName);
        IEnumerable<FileItem> GetPagesProject(string username);
        List<string> FormattingFile(string path, string projectName, string userName);
        Task<bool> SaveProject(ProjectSettingsViewModel projectSettings,User user);  
    }
}
