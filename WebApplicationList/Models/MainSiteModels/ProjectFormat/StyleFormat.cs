namespace WebApplicationList.Models.MainSiteModels.ProjectFormat
{
    public class StyleFormat : IFormatType
    {
        public FileFormatResult FormattingFile(List<string> fileLines, string pattern,string patternRepeat,string userName)
        {
            FileFormatResult fileResult = new FileFormatResult();
            List<string> changes = new List<string>();

            var images = fileLines.Where(p => p.Contains("<link")&& p.Contains("css/")||p.Contains(".css")).ToList();

            foreach (var image in images)
            {
                if (image.Contains("href=\""))
                {
                    int lineIndex = fileLines.FindIndex(s => s.Contains(image));
                    string road = image.Substring(image.IndexOf("href=\"") + "href=\"".Length);
                    road = road.Remove(road.IndexOf("\""));

                    if (!road.Contains(pattern))
                    {
                        if (road.Contains(patternRepeat))
                        {
                            string roadProjectName = road.Substring(road.IndexOf($"{userName}/") + $"{userName}/".Length);
                            roadProjectName = roadProjectName.Remove(roadProjectName.IndexOf("/"));

                            string newRoadProjectName = pattern.Substring(road.IndexOf($"{userName}/") + $"{userName}/".Length);
                            newRoadProjectName = newRoadProjectName.Remove(newRoadProjectName.IndexOf("/"));

                            string newRoad = road.Replace(roadProjectName, newRoadProjectName);

                            string result = image.Replace(road, newRoad);
                            
                            fileLines[lineIndex] = result;
                            changes.Add(result.Substring(result.IndexOf("<")));
                        }
                        else
                        {
                            string result = image.Replace(road, pattern + road);
                            fileLines[lineIndex] = result;
                            changes.Add(result.Substring(result.IndexOf("<")));
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
