namespace WebApplicationList.Models.ProjectFormat
{
    public class StyleFormat : IFormatType
    {
        public FileFormatResult FormattingFile(List<string> fileLines,string userName,string projectName)
        {
            FileFormatResult fileResult = new FileFormatResult();
            List<string> changes = new List<string>();

            string pattern = $"UserProjects/{userName}/{projectName}/";
            string patternRepeat = $"UserProjects/{userName}";

            var styles = fileLines.Where(p => p.Contains("<link")&& p.Contains("css/")||p.Contains(".css")).ToList();

            foreach (var style in styles)
            {
                if (style.Contains("href=\""))
                {
                    int lineIndex = fileLines.FindIndex(s => s.Contains(style));
                    string road = style.Substring(style.IndexOf("href=\"") + "href=\"".Length);
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

                            string result = style.Replace(road, newRoad);
                            
                            fileLines[lineIndex] = result;
                            changes.Add(result.Substring(result.IndexOf("<")));
                        }
                        else
                        {
                            string result = style.Replace(road, pattern + road);
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
