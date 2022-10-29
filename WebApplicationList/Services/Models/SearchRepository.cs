
using Microsoft.EntityFrameworkCore;
using WebApplicationList.ApplicationDataBase;
using WebApplicationList.Models.Enitity;
using WebApplicationList.Models.ViewModels;

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
                .Include(p=>p.projectComments)
                .Select(p => new ProjectViewModel
                {
                    userName = p.user!.UserName,
                    linkAvatar = p.user!.LinkAvatar,
                    projectName = p.Name,
                    projectUrl = p.Url,
                    projectUrlImage = p.UrlImage,
                    addedTime = p.AddedTime,
                    likes = p.projectLikes.Count,
                    views = p.projectViews.Count,
                    commentsValue = p.projectComments.Count,

                }).AsSplitQuery().AsNoTracking().Take(20).ToListAsync();

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
                .Include(p => p.user)
                .Include(p=>p.projectLikes)
                .Include(p=>p.projectViews)
                .Select(p => new ProjectViewModel
                {
                    userName = p.user!.UserName,
                    linkAvatar = p.user!.LinkAvatar,
                    projectName = p.Name,
                    projectUrl = p.Url,
                    projectUrlImage = p.UrlImage,
                    addedTime = p.AddedTime,
                    likes = p.projectLikes.Count(),
                    views = p.projectViews.Count(),

                }).AsNoTracking().AsSplitQuery();

            return searchOptions.SortType switch 
            {
                "name"=> await request.OrderBy(p=>p.projectName)
                    .Take(searchOptions.PageIndex).ToListAsync(),
                   
                "date"=> await request.OrderByDescending(p=>p.addedTime)
                    .Take(searchOptions.PageIndex).ToListAsync(),

                "popular" => await request.OrderByDescending(p=>p.likes)
                    .Take(searchOptions.PageIndex).ToListAsync(),

                "views" => await request.OrderByDescending(p=>p.views)
                    .Take(searchOptions.PageIndex).ToListAsync(),

                _ => await request.OrderByDescending(p => p.projectId)
                    .Take(searchOptions.PageIndex).ToListAsync(),
            };

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

            string userNameOwner = _profileUser.GetUserName();

            var users = await _context.Users.Where(p => EF.Functions.Like(p.UserName, $"%{searchOptions.Text}%")&&p.UserName!=userNameOwner)
                    .Include(p => p.profileUserInfo)
                    .Include(p => p.projects).ThenInclude(p=>p.projectLikes)             
                    .Include(p => p.usersProfile)
                    .Select(p => new ProfileUserViewModel
                    {
                        UserName = p.UserName,
                        Surname = p.profileUserInfo!.Surname,
                        LinkAvatar = p.LinkAvatar,
                        Profession = p.profileUserInfo!.Profession,
                        DateRegistration = p.DateRegistraition,
                        countProjects = p.projects.Count(),
                        countSubscriber = p.subscribes.Count(),   
                        countLikes = p.projects.Select(p=>p.projectLikes.Count).ToList(),
                        countViews = p.projects.Select(p => p.projectViews.Count).ToList()
                    })
                    .AsNoTracking().AsSplitQuery().ToListAsync();

            

            var result = searchOptions.SortType switch
            {
                "name" => users.OrderBy(p=>p.UserName)
                    .Take(searchOptions.PageIndex).ToList(),

                "date"=> users.OrderByDescending(p=>p.DateRegistration)
                    .Take(searchOptions.PageIndex).ToList(),

                "popular" => users.OrderByDescending(p=>p.countLikes!.Sum())
                    .Take(searchOptions.PageIndex).ToList(),

                "views" => users.OrderByDescending(p=>p.countViews!.Sum())
                    .Take(searchOptions.PageIndex).ToList(),

                _ => users.OrderBy(p=>p.UserName)
                    .Take(searchOptions.PageIndex).ToList(),
            };

            foreach(var item in result)
            {
                item.signed = await GetSignedUser(item.UserName!);
            }
        
            return result;
        }
        public async Task<ProjectViewModel> GetProjectPresentation(string projectName)
        {
            var project = await _context.userProjects!.Where(p => p.Name == projectName)
                .Include(p => p.user).ThenInclude(p => p!.profileUserInfo)
                .Include(p => p.projectViews)
                .Include(p => p.projectLikes)
                .Include(p => p.projectComments).ThenInclude(p=>p.user)
                .Select(p => new ProjectViewModel
                {
                    userName = p.user!.UserName,
                    linkAvatar = p.user!.LinkAvatar,
                    profession = p.user!.profileUserInfo!.Profession,
                    projectId = p.Id,
                    projectName = p.Name,
                    projectUrl = p.Url,
                    projectUrlImage = p.UrlImage,
                    projectDescription = p.Description,
                    addedTime = p.AddedTime,
                    projectTypes = p.Type,
                    projectComments = p.projectComments.OrderByDescending(p=>p.date).ToList(),
                    likes = p.projectLikes.Count(),
                    views = p.projectViews.Count()

                }).AsNoTracking().AsSplitQuery().FirstOrDefaultAsync();

            project!.liked = await GetLikedProject(project.projectId);
            project!.signed = await GetSignedUser(project.userName!);

            return project;
        }
        public async Task<List<ProjectComment>> GetComments(int count,int projectId)
        {
            if(count == 0)
            {
                count = 20;
            }

            var comments = await _context.projectComments!.Where(p=>p.project!.Id == projectId)
                .OrderByDescending(p=>p.date)
                .Include(p=>p.project)
                .Include(p=>p.user)
                .Take(count).AsSplitQuery().AsNoTracking().ToListAsync();

            return comments;
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
        public async Task<bool> GetSignedUser(string userName)
        {
            var user = await _profileUser.GetUserAsync();

            if (user is null)
            {
                return false;
            }

            var result = await _context.subscribeUsers!
                .Where(p => p.user == user && p.subscribe!.UserName == userName).CountAsync();

            if (result > 0)
                return true;

            return false;
        }

    }
}
