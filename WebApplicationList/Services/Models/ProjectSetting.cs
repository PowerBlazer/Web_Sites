using WebApplicationList.ApplicationDataBase;
using Microsoft.EntityFrameworkCore;
using System.Text;
using WebApplicationList.Models.MainSiteModels.ViewModels;
using WebApplicationList.Models.MainSiteModels.ProjectFormat;
using WebApplicationList.Models;
using System.IO.Compression;
using EncodingText;
using WebApplicationList.Models.MainSiteModels.ProjectModels;

namespace WebApplicationList.Services.Models
{
    public class ProjectSetting : IProjectSetting
    {
        private readonly ApplicationDb _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProjectSetting(ApplicationDb applicationDb, IWebHostEnvironment webHostEnvironment)
        {
            _context = applicationDb;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<bool> ChangeContentFile(FileItem fileItem)
        {
            if (fileItem is null)
            {
                return await Task.FromResult(false);
            }

            fileItem.Path = CustomEncoding.EncodeDecrypt(fileItem.Path!);

            using (StreamWriter writer = new StreamWriter(fileItem.Path!, false, Encoding.UTF8))
            {
                await writer.WriteLineAsync(fileItem.Content);
            }

            return await Task.FromResult(true);      
        }
        public async Task<FileItem> GetContentFileAsync(string filePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            FileItem? fileItem = ( await GetFilesAsync(new List<string> { filePath! })).FirstOrDefault();

            if (!File.Exists(CustomEncoding.EncodeDecrypt(filePath)))
            {
                throw new Exception("Такого файла нет");
            }

            using (StreamReader reader = new StreamReader(CustomEncoding.EncodeDecrypt(filePath), Encoding.UTF8))
            {
                fileItem!.Content = await reader.ReadToEndAsync();
                fileItem.Path = filePath;
            }

            return fileItem;
        }
        public async Task<ExplorerViewModel> GetExplorer(Stream fileStream,string userName)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (fileStream is null || string.IsNullOrWhiteSpace(userName))
            {
                return null;
            }

            string projectPath = Path.Combine(_webHostEnvironment.WebRootPath,"UserProjects",$"{userName}","temporary");

            if(Directory.Exists(projectPath))
            {
                Directory.Delete(projectPath, true);
            }

            Directory.CreateDirectory(projectPath);
                      
            //Распаковка файлов      
            using (ZipArchive zipFile = new ZipArchive(fileStream, ZipArchiveMode.Read, false, Encoding.GetEncoding(866)))
            {
                zipFile.ExtractToDirectory(projectPath);
            }
            
            if(IsDirectoryEmpty(projectPath))
            {
                return null;
            }

            var explorerResult = new ExplorerViewModel()
            {
                Files = await GetFilesAsync(Directory.EnumerateFiles(projectPath)),
                Folders = await GetFilesAsync(Directory.EnumerateDirectories(projectPath)),
                ProjectName = "temporary",   
                CurrentPath = projectPath,
            };

            return explorerResult;
        }
        public async Task<ExplorerViewModel> GetExplorerItem(string directoryPath)
        {
            directoryPath = CustomEncoding.EncodeDecrypt(directoryPath);

            var explorerResult = new ExplorerViewModel()
            {
                Files = await GetFilesAsync(Directory.EnumerateFiles(directoryPath)),
                Folders = await GetFilesAsync(Directory.EnumerateDirectories(directoryPath)),
                ReturnPath = GetReturnPath(directoryPath), 
                CurrentPath = directoryPath,
            };

            return explorerResult;
        }
        public async Task<string> GetHtmlProject(string nameProject, string page)
        {
            if (string.IsNullOrEmpty(nameProject))
                return string.Empty;

            var project = await _context.userProjects!.Where(p => p.Name == nameProject)
                .Include(p=>p.user).FirstOrDefaultAsync();

            if (project is null)
                return string.Empty;

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "UserProjects");
            var listUserNames = Directory.GetDirectories(filePath).ToList();
            string? pathUserProjects = listUserNames.Where(p => p.EndsWith(project.user!.UserName)).FirstOrDefault();

            if (string.IsNullOrEmpty(pathUserProjects))
                return string.Empty;

            var listUserProjects = Directory.GetDirectories(pathUserProjects!).ToList();
            string? pathProject = listUserProjects.Where(p => p.EndsWith(nameProject)).FirstOrDefault();

            if (string.IsNullOrEmpty(pathProject))
                return string.Empty;

            var projectfiles = Directory.GetFiles(pathProject!, "*.html").ToList();

            if (string.IsNullOrEmpty(page))
                return string.Empty;

            string? indexPath = projectfiles.Where(p => p.Contains(page)).FirstOrDefault();

            if (string.IsNullOrEmpty(indexPath))
                return string.Empty;

            string? htmlcode;

            using (StreamReader streamReader = new StreamReader(indexPath!))
            {
                htmlcode = await streamReader.ReadToEndAsync();
            }

            if (string.IsNullOrEmpty(htmlcode))
                return string.Empty;

            return htmlcode;

        }
        public IEnumerable<FileItem> GetPagesProject(string username)
        {
            string pathProject = Path.Combine(_webHostEnvironment.WebRootPath, "UserProjects",username,"temporary");

            var projectHtmlDocs = Directory.GetFiles(pathProject!, "*.html").ToList();

            List<FileItem> fileItems = new List<FileItem>();
            for(int i = 0; i < projectHtmlDocs.Count(); i++)
            {
                fileItems.Add(new FileItem
                {
                    Name = Path.GetFileName(projectHtmlDocs[i]),
                    Path = CustomEncoding.EncodeDecrypt(projectHtmlDocs[i]),
                });
            }

            return fileItems;
        }
        public async Task<bool> GetValidationProjectName(string projectName)
        {
            if(string.IsNullOrWhiteSpace(projectName)||projectName=="temporary")
            {
                return false;
            }

            var countResult = await _context.userProjects!.Where(p => p.Name == projectName).CountAsync();

            if (countResult > 0)
            {
                return false;
            }

            return true;
        }
        public List<string> FormattingFile(string path,string projectName,string userName)
        {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(projectName)
                || string.IsNullOrWhiteSpace(userName))
            {
                throw new Exception("Пустые значения");
            }

            path = CustomEncoding.EncodeDecrypt(path);

            string pattern = $"UserProjects/{userName}/{projectName}/";
            string patternRepeat = $"UserProjects/{userName}";
            
            if(!File.Exists(path))
            {
                throw new ArgumentNullException(path);
            }

            var fileLines = File.ReadAllLines(path).ToList();

            IEnumerable<IFormatType> formatTypes = new List<IFormatType>() 
            {
                new ImageFormat(), 
                new ScriptFormat(),
                new StyleFormat(),
            };

            List<string> changesLines = new List<string>();

            foreach(var item in formatTypes)
            {
               var result = item.FormattingFile(fileLines!,pattern,patternRepeat,userName);
               fileLines = result.Result;
               changesLines = changesLines.Concat(result.Changes!).ToList();
            }

            File.WriteAllLines(path,fileLines);

            return changesLines;
        }
        public async Task SetViewProject(string projectName, User user)
        {
            var project = await _context.userProjects!.Where(p => p.Name == projectName).FirstOrDefaultAsync();

            if(project is null)
            {
                return;
            }

            var count = await _context.projectViews!
                .Where(p => p.project!.Name == projectName && p.user!.Id == user.Id).CountAsync();

            if (count == 0)
            {
                await _context.projectViews!.AddAsync(new ProjectView
                {
                    project = project,
                    user = user,
                });

                await _context.SaveChangesAsync();
            }

        }
        public async Task<bool> AddComment(int projectId, User user, string text)
        {
            if(user is null|| string.IsNullOrEmpty(text))
            {
                return false;
            }

            var project = await _context.userProjects!.Where(p => p.Id == projectId).FirstOrDefaultAsync();

            if(project is null)
            {
                throw new Exception("Проект не найден");
            }

            await _context.AddAsync(new ProjectComment
            {
                user = user,
                project = project,
                Text = text,
            });

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> SelectLike(int projectId,User user)
        {
            if(user is null)
            {
                return false;
            }

            var project = await _context.userProjects!.Where(p => p.Id == projectId).FirstOrDefaultAsync();

            if (project is null)
            {
                throw new Exception("Проект не найден");
            }

            await _context.AddAsync(new ProjectLike
            {
                user = user,
                project = project,
            });

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            

            return true;
        }
        public async Task<bool> DeleteLike(int projectId,User user)
        {
            var like = await _context.projectLikes!.Where(p => p.project!.Id == projectId && p.user!.Id == user.Id).FirstOrDefaultAsync();

            if(like is null)
            {
                return false;
            }

            try
            {
                _context.projectLikes!.Remove(like);

                await _context.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }
        public async Task<bool> SaveProject(ProjectSettingsViewModel projectSettings,User user)
        {
            if(await _context.userProjects!.Where(p=>p.Name==projectSettings.Name).CountAsync()!=0||
              string.IsNullOrWhiteSpace(projectSettings.Name)||string.IsNullOrWhiteSpace(projectSettings.ImageByte)
              ||string.IsNullOrWhiteSpace(projectSettings.SelectedPage))
            {
                 return false;
            }
         
            if (!CheckFormatImage(projectSettings.ImageByte!))
            {
                 return false;
            }

           

            string pathProjectImage = Path.Combine(_webHostEnvironment.WebRootPath, "UserProjectsImage",$"{projectSettings.Name}.jpg");
            byte[] byteImage = Convert.FromBase64String(projectSettings.ImageByte);

            if(byteImage.Length> 40971520)
            {
                 return false;
            }

            string projectPath = Path.Combine(_webHostEnvironment.WebRootPath, "UserProjects", $"{user.UserName}", "temporary");
            string newProjectPath = Path.Combine(_webHostEnvironment.WebRootPath, "UserProjects", $"{user.UserName}", $"{projectSettings.Name}");

            var files = Directory.GetFiles(projectPath, "*.html").ToList();
            int checkCount = files.Where(p => p.Contains(projectSettings.SelectedPage)).Count();

            if(checkCount==0)
            {
                throw new Exception($"Не существует {projectSettings.SelectedPage}");
            }

            if(!Directory.Exists(projectPath))
            {
                throw new Exception("Папки нету");
            }

            using (FileStream fileStream = new FileStream(pathProjectImage, FileMode.Create, FileAccess.ReadWrite))
            {
                await fileStream.WriteAsync(byteImage, 0, byteImage.Length);
            }

            if(Directory.Exists(newProjectPath))
            {
                Directory.Delete(newProjectPath, true);
            }

            Directory.Move(projectPath, newProjectPath);

            await _context.userProjects!.AddAsync(new UserProject
            {
                Name = projectSettings.Name,
                Description = projectSettings.Description,
                Type= projectSettings.Types,
                user = user, 
                UrlImage = $"UserProjectsImage/{projectSettings.Name}.jpg",
                Url = $"?Site={projectSettings.Name}&page={projectSettings.SelectedPage}"
            });

            await _context.SaveChangesAsync();

            return true;
        }
       

        private async Task<List<FileItem>> GetFilesAsync(IEnumerable<string> paths)
        {
            var files = new List<FileItem>();
            await Task.Run(() =>
            {
                foreach (var item in paths)
                {
                    var path = item;

                    if (!item.Contains("UserProjects"))
                    {
                        path = CustomEncoding.EncodeDecrypt(item);
                    }
             
                    files.Add(new FileItem
                    {
                        Name = Path.GetFileName(path),
                        Path = CustomEncoding.EncodeDecrypt(path),
                        Size = GetSizeFile(path)
                    });
                }
            });
            
            return files;
        }
        private string GetReturnPath(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            return CustomEncoding.EncodeDecrypt(directoryInfo.Parent!.ToString());
        }
        private string GetSizeFile(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            try
            {
                if (fileInfo.Length < 1024000)
                {
                    return Math.Round((double)fileInfo.Length / 1024, 2) + "KB";
                }
                else
                {
                    return Math.Round((double)fileInfo.Length / 1024 / 1024, 2) + "MB";
                }
            }
            catch
            {
                return string.Empty;
            }
        }
        private bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }
        private bool CheckFormatImage(string byteImage)
        {
            string JpgMagicValue = "/9j/4AAQSkZJRgA";
            string PngMagicValue = "iVBORw0KGgoAAA";

            if(byteImage.Contains(JpgMagicValue)||byteImage.Contains(PngMagicValue))
            {
                return true;
            }

            return false;
        }
    }
}
