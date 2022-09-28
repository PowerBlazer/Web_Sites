using WebApplicationList.ApplicationDataBase;
using Microsoft.EntityFrameworkCore;
using System.Text;
using WebApplicationList.Models.MainSiteModels.ViewModels;
using WebApplicationList.Models.MainSiteModels.ProjectFormat;
using WebApplicationList.Models;
using System.IO.Compression;

namespace WebApplicationList.Services.Models
{
    public class ProjectSetting : IProjectSetting
    {
        private readonly ApplicationDb _applicationDb;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProjectSetting(ApplicationDb applicationDb, IWebHostEnvironment webHostEnvironment)
        {
            _applicationDb = applicationDb;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<bool> ChangeContentFile(FileItem fileItem)
        {
            if (fileItem is null)
            {
                return await Task.FromResult(false);
            }

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

            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
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

            var project = await _applicationDb.userProjects!.Where(p => p.Name == nameProject).FirstOrDefaultAsync();

            if (project is null)
                return string.Empty;

            var user = await _applicationDb.Users.Where(p => p.Id == project.User_Id).FirstOrDefaultAsync();

            if (user is null)
                return string.Empty;

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "UserProjects");
            var listUserNames = Directory.GetDirectories(filePath).ToList();
            string? pathUserProjects = listUserNames.Where(p => p.EndsWith(user!.UserName)).FirstOrDefault();

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
                    Path = projectHtmlDocs[i],
                });
            }

            return fileItems;
        }
        public string GetProjectPath(string username)
        {
            return Path.Combine(_webHostEnvironment.WebRootPath, "UserProjects", username, "temporary");
        }
        public async Task<bool> GetValidationProjectName(string projectName)
        {
            if(string.IsNullOrWhiteSpace(projectName)||projectName=="temporary")
            {
                return false;
            }

            var countResult = await _applicationDb.userProjects!.Where(p => p.Name == projectName).CountAsync();

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
        public async Task<bool> SaveProject(ProjectSettingsViewModel projectSettings,User user)
        {
            if(await _applicationDb.userProjects!.Where(p=>p.Name==projectSettings.Name).CountAsync()!=0||
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

            await _applicationDb.userProjects!.AddAsync(new UserProject
            {
                Name = projectSettings.Name,
                Description = projectSettings.Description,
                Type= projectSettings.Types,
                User_Id = user.Id,
                UrlImage = $"UserProjectsImage/{projectSettings.Name}.jpg",
                Url = $"?Site={projectSettings.Name}&page={projectSettings.SelectedPage}"
            });

            await _applicationDb.SaveChangesAsync();

            return true;
        }


        private async Task<List<FileItem>> GetFilesAsync(IEnumerable<string> paths)
        {
            var files = new List<FileItem>();
            await Task.Run(() =>
            {
                foreach (var item in paths)
                {
                    files.Add(new FileItem
                    {
                        Name = Path.GetFileName(item),
                        Path = item,
                        Size = GetSizeFile(item)
                    });
                }
            });
            
            return files;
        }
        private string GetReturnPath(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            return directoryInfo.Parent!.ToString();
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
                return String.Empty;
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
