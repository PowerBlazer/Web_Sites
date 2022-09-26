using Microsoft.EntityFrameworkCore;
using WebApplicationList.ApplicationDataBase;
using WebApplicationList.Models;
using WebApplicationList.Models.MainSiteModels.ViewModels;

namespace WebApplicationList.Services.Models
{
    public class SearchRepository:ISearch
    {
        private readonly ApplicationDb _context;
        public SearchRepository(ApplicationDb context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectViewModel>> GetProjects()
        {
            var userProjects = await _context.userProjects!.OrderBy(p=>p.Name).Take(20).ToListAsync();

            if (userProjects.Count == 0)
            {
                return Enumerable.Empty<ProjectViewModel>();
            }

            List<ProjectViewModel> projects = new List<ProjectViewModel>();

            foreach (var project in userProjects)
            {
                projects.Add(new ProjectViewModel()
                {
                    project = project,
                    user = await _context.Users.Where(p=>p.Id == project.User_Id).FirstOrDefaultAsync(),
                });
            }

            return projects;
        }

        public async Task<IEnumerable<string>> GetTypesProject()
        {
            var projects = await _context.userProjects!.ToListAsync();

            IEnumerable<string>? types = new List<string>();

            foreach(var item in projects)
            {
                types = types.Concat(item.Type!.Split(',').AsEnumerable());
            }

            var result = types!.GroupBy(p => p).Select(p => p.FirstOrDefault()).OrderBy(p=>p);

            return result!;
        } 
        public async Task<IEnumerable<ProjectViewModel>> GetProjectsApplyFilters(SearchOptions searchOptions)
        {
            if (string.IsNullOrEmpty(searchOptions.SortType))
            {
                return Enumerable.Empty<ProjectViewModel>();
            }

            if (searchOptions.PageIndex == 0)
            {
                searchOptions.PageIndex = 20;
            }

            List<UserProject> projects = searchOptions.SortType switch 
            {
                "name"=> await _context.userProjects!
                                 .Where(p => EF.Functions.Like(p.Name!, $"%{searchOptions.Text}%"))
                                 .Where(p => EF.Functions.Like(p.Type, $"%{searchOptions.Type!}%"))
                                 .OrderBy(p => p.Name).Take(searchOptions.PageIndex).ToListAsync(),
                   
                "date"=> await _context.userProjects!
                                .Where(p => EF.Functions.Like(p.Name!, $"%{searchOptions.Text}%"))
                                .Where(p => EF.Functions.Like(p.Type, $"%{searchOptions.Type!}%"))
                                .OrderBy(p => p.AddedTime).Take(searchOptions.PageIndex).ToListAsync(),
                    
                "popular"=> await _context.userProjects!
                                .Where(p => EF.Functions.Like(p.Name!, $"%{searchOptions.Text}%"))
                                .Where(p => EF.Functions.Like(p.Type, $"%{searchOptions.Type!}%"))
                                .OrderBy(p => p.Id).Take(searchOptions.PageIndex).ToListAsync(),
                   
                 _ => await _context.userProjects!.OrderBy(p => p.Id).Take(searchOptions.PageIndex).ToListAsync(),
            };

            if (projects.Count == 0)
            {
                return Enumerable.Empty<ProjectViewModel>();
            }

            var projectsView = new List<ProjectViewModel>();

            foreach(var item in projects)
            {
                projectsView.Add(new ProjectViewModel
                {
                    project = item,
                    user = await _context.Users.Where(p => p.Id == item.User_Id).FirstOrDefaultAsync(),
                });
            }

            return projectsView;
           
        }

    }
}
