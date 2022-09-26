using WebApplicationList.Models.MainSiteModels.ViewModels;

namespace WebApplicationList.Services
{
    public interface ISearch
    {
        Task<IEnumerable<ProjectViewModel>> GetProjects();
        Task<IEnumerable<string>> GetTypesProject();
        Task<IEnumerable<ProjectViewModel>> GetProjectsApplyFilters(SearchOptions searchOptions);
    }
}
