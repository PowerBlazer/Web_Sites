

namespace WebApplicationList.Models.ProjectFormat
{
    public class LinksFormat : IFormatType
    {
        public FileFormatResult FormattingFile(List<string> fileLines,string userName,string projectName)
        {
            FileFormatResult fileResult = new FileFormatResult();
            List<string> changes = new List<string>();

            
            var links = fileLines.Where(p => p.Contains("<a")).ToList();
            
            

            foreach (var link in links)
            {
                if (!link.Contains("https://")|| !link.Contains("http://"))
                {
                    if (link.Contains("href=\""))
                    {
                        int lineIndex = fileLines.FindIndex(s => s.Contains(link));
                        if (link.Contains(".html"))
                        {
                            
                            string road = link.Substring(link.IndexOf("href=\"") + "href=\"".Length);
                            road = road.Remove(road.IndexOf("\""));

                            string pageName = road.Replace(".html", "");
                            string pattern = $"/UserProject?Site={projectName}&page={pageName}";

                            string result = link.Replace(road, pattern);

                            fileLines[lineIndex] = result;

                            changes.Add(result);

                        }

                        if (link.Contains("/UserProject?Site="))
                        {
                            string road = link.Substring(link.IndexOf("href=\"") + "href=\"".Length);
                            road = road.Remove(road.IndexOf("\""));

                            string pageName = road.Substring(road.IndexOf("page=") + "page=".Length);

                            string pattern = $"/UserProject?Site={projectName}&page={pageName}";

                            string result = link.Replace(road, pattern);


                            fileLines[lineIndex] = result;

                            changes.Add(result);
                        }

                        
                    }
                }

            }

            fileResult.Result = fileLines;
            fileResult.Changes = changes;

            return fileResult;
        }
    }
}
