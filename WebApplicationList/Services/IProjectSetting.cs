using WebApplicationList.Models.Enitity;
using WebApplicationList.Models.ViewModels;

namespace WebApplicationList.Services
{
    public interface IProjectSetting
    {
        Task<string> GetHtmlProject(string nameProject,string page);
        Task<ExplorerViewModel> GetExplorer(Stream fileStream,string username);
        Task<ExplorerViewModel> GetExplorerItem(string directoryPath);
        Task<FileItem> GetContentFileAsync(string filePath);
        Task<bool> ChangeContentFile(FileItem fileItem);
        Task<bool> GetValidationProjectName(string projectName);
        IEnumerable<FileItem> GetPagesProject(string username,string? projectName);
        List<string> FormattingFile(string path, string projectName, string userName);
        Task<bool> SaveProject(ProjectSettingsViewModel projectSettings,User user);
        Task<bool> AddComment(int projectId, User user, string text);
        Task<bool> SelectLike(int projectId, User user);
        Task<bool> DeleteLike(int projectId, User user);
        Task SetView(string projectName, User user);
        Task<bool> DeleteProject(string projectName, User user);
        string GetPathProject(string projectName, string userName);
        Task<ProjectOptions> GetProjectOptions(string projectName, User user);
    }
}
