using Microsoft.AspNetCore.Mvc;
using WebApplicationList.Models;
using WebApplicationList.Models.Enitity;
using WebApplicationList.Models.MainSiteModels.ViewModels;

namespace WebApplicationList.Services
{
    public interface ISearch
    {
        Task<IEnumerable<ProjectViewModel>> GetProjects();
        Task<IEnumerable<string>> GetTypesProject();
        Task<IEnumerable<ProjectViewModel>> GetProjectsApplyFilters(SearchOptions searchOptions);
        Task<IEnumerable<ProfileUserViewModel>> GetUsersApplyFilters(SearchOptions searchOptions);
        Task<ProjectViewModel> GetProjectPresentation(string projectName);
        Task<List<ProjectComment>> GetComments(int count, int projectId);
        Task<bool> GetLikedProject(int projectId);
    }
}
