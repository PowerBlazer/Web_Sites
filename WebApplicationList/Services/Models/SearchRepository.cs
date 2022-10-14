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
            var projects = await _context.userProjects!.OrderBy(p=>p.Name)
                .Include(p=>p.user)
                .Include(p=>p.projectLikes)
                .Include(p=>p.projectViews)
                .Select(p => new ProjectViewModel
                {
                    userInfo = p.user!.ProfileUserInfo,
                    project = p,
                    likes = p.projectLikes.Count(),
                    views = p.projectViews.Count(),
                }).AsSplitQuery().Take(20).ToListAsync();

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

            var request = _context.userProjects!
                                 .Where(p => EF.Functions.Like(p.Name!, $"%{searchOptions.Text}%"))
                                 .Where(p => EF.Functions.Like(p.Type!, $"%{searchOptions.Type!}%"))
                                 .Include(p => p.user).ThenInclude(p => p!.ProfileUserInfo)
                                 .Select(p => new ProjectViewModel
                                 {
                                     userInfo = p.user!.ProfileUserInfo,
                                     project = p,
                                 }).Take(searchOptions.PageIndex);

            var result = searchOptions.SortType switch 
            {
                "name"=> await request.OrderBy(p=>p.project!.Name).ToListAsync(),
                   
                "date"=> await request.OrderByDescending(p=>p.project!.AddedTime).ToListAsync(),

                "popular" => await request.OrderBy(p=>p.project!.Id).ToListAsync(),

                _ => await request.OrderBy(p => p.project!.Id).ToListAsync(),
            };

            return result; 
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

            var request = _context.Users.Where(p => EF.Functions.Like(p.UserName, $"%{searchOptions.Text}%"))
                    .Include(p => p.ProfileUserInfo).ThenInclude(p => p!.user)
                    .Include(p => p.projects)
                    .Include(p => p.projectLikes)
                    .Include(p=>p.usersProfile)
                    .Select(p => new ProfileUserViewModel
                    {
                        userInfo = p.ProfileUserInfo,
                        countLikes = p.projectLikes.Count(),
                        countProjects = p.projects.Count(),
                        countSubscriber = p.subscribes.Count(),
                    })
                    .Take(searchOptions.PageIndex).AsSplitQuery();

            var result = searchOptions.SortType switch
            {
                "name" => await request.OrderBy(p=>p.userInfo!.user!.UserName).ToListAsync(),
                "date"=> await request.OrderBy(p=>p.userInfo!.user!.DateRegistraition).ToListAsync(),
                _ => await request.OrderBy(p=>p.userInfo!.Id).ToListAsync(),
            };
        
            return result;
        }
        public async Task<ProjectViewModel> GetProjectPresentation(string projectName)
        {
            var project = await _context.userProjects!.Where(p => p.Name == projectName)
                .Include(p=>p.user).FirstOrDefaultAsync();

            if(project is null)
            {
                throw new Exception("Не найдено");
            }

            var result = new ProjectViewModel()
            {
                userInfo = await _context.profileUserInfo!.Where(p => p.user!.Id == project.user!.Id)
                .Include(p => p.user).FirstOrDefaultAsync(),
                project = project,
                likes = await _context.projectLikes!.Where(p => p.project!.Id == project.Id).CountAsync(),
                liked = await GetLikedProject(project.Id),
                views = await _context.projectViews!.Where(p => p.project!.Id == project.Id).CountAsync(),
                signed = await GetSignedUser(project.user!),
                projectComments = await _context.projectComments!.Where(p => p.project!.Id == project.Id)
                    .OrderByDescending(p=>p.date)
                    .Include(p => p.project)
                    .Include(p => p.user).Take(20).ToListAsync(),
            };

            return result;
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
                .Take(count).AsSplitQuery().ToListAsync();
        }
        public async Task<bool> GetLikedProject(int projectId)
        {
            var user = await _profileUser.GetUserAsync();

            if(user is null)
            {
                return false;
            }

            var result = await _context.projectLikes!.Where(p => p.project!.Id == projectId && p.user!.Id == user.Id).CountAsync();

            if (result > 0)
                return true;

            return false;
        }
        public async Task<bool> GetSignedUser(User projectOwner)
        {
            var user = await _profileUser.GetUserAsync();

            if (user is null)
            {
                return false;
            }

            var result = await _context.subscribeUsers!.Where(p => p.user == user && p.subscribe == projectOwner).CountAsync();

            if (result > 0)
                return true;

            return false;
        }
        public async Task<int> GetNumberSubscribers(User user)
        {
           return await _context.subscribeUsers!.Where(p => p.user == user).CountAsync();
        }




    }
}
