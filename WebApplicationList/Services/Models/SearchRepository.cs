using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationList.ApplicationDataBase;
using WebApplicationList.Models;
using WebApplicationList.Models.MainSiteModels.ProjectModels;
using WebApplicationList.Models.MainSiteModels.ViewModels;

namespace WebApplicationList.Services.Models
{
    public class SearchRepository:ISearch
    {
        private readonly ApplicationDb _context;
        private readonly IProfileUser _profileUser;
        public SearchRepository(ApplicationDb context, IProfileUser profileUser)
        {
            _context = context;
            _profileUser = profileUser;
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

            List<string>? types = new List<string>();

            foreach(var item in projects)
            {
                types = types.Concat(item.Type!.Split(',')).ToList();
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
        public async Task<IEnumerable<ProfileUserViewModel>> GetUsersApplyFilters(SearchOptions searchOptions)
        {
            if (string.IsNullOrEmpty(searchOptions.SortType))
            {
                return Enumerable.Empty<ProfileUserViewModel>();
            }

            if (searchOptions.PageIndex == 0)
            {
                searchOptions.PageIndex = 20;
            }

            var users = searchOptions.SortType switch
            {
                "name" => await _context.Users.Where(p=>EF.Functions.Like(p.UserName,$"%{searchOptions.Text}%")).OrderBy(p => p.UserName).Take(searchOptions.PageIndex).ToListAsync(),
                "date"=>await _context.Users.Where(p => EF.Functions.Like(p.UserName, $"%{searchOptions.Text}%")).OrderBy(p=>p.DateRegistraition).Take(searchOptions.PageIndex).ToListAsync(),
                _=> await _context.Users.Where(p => EF.Functions.Like(p.UserName, $"%{searchOptions.Text}%")).OrderBy(p=>p.UserName).Take(searchOptions.PageIndex).ToListAsync(),

            };

            List<ProfileUserViewModel> usersView = new List<ProfileUserViewModel>();

            foreach (var item in users)
            {
                usersView.Add(await _profileUser.GetUserViewModelAsync(item));
            }

            return usersView;
        }
        public async Task<ProjectViewModel> GetProjectPresentation(string projectName)
        {
            var project = await _context.userProjects!.Where(p => p.Name == projectName).FirstOrDefaultAsync();

            if(project is null)
            {
                throw new Exception("Не найдено");
            }
            
            return new ProjectViewModel()
            {
                user = await _context.Users.Where(p => p.Id == project.User_Id).FirstOrDefaultAsync(),
                userInfo = await _context.profileUserInfo!.Where(p => p.UserId == project.User_Id).FirstOrDefaultAsync(),
                project = project,
                likes = await _context.projectLikes!.Where(p=>p.project!.Id==project.Id).CountAsync(),
                liked = await GetLikedProject(project.Id),
                projectComments = await _context.projectComments!.Where(p => p.project!.Id == project.Id)
                    .OrderByDescending(p=>p.date)
                    .Include(p => p.project)
                    .Include(p => p.user).Take(20).ToListAsync(),
            };  
        }
        public async Task<List<ProjectComment>> GetComments(int count,int projectId)
        {
            if(count == 0)
            {
                count = 20;
            }

            return await _context.projectComments!.Where(p=>p.project!.Id == projectId)
                .OrderByDescending(p=>p.date)
                .Include(p=>p.project)
                .Include(p=>p.user)
                .Take(count).ToListAsync();
        }
        public async Task<bool> GetLikedProject(int projectId)
        {
            var user = await _profileUser.GetUserAsync();

            if(user is null)
            {
                throw new Exception("Пользователь не найден");
            }

            var result = await _context.projectLikes!.Where(p => p.project!.Id == projectId && p.user!.Id == user.Id).CountAsync();

            if (result > 0)
                return true;

            return false;
        }
    }
}
