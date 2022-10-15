namespace WebApplicationList.Models.MainSiteModels.ProjectFormat
{
    public interface IFormatType
    {
        FileFormatResult FormattingFile(List<string> fileLines,string pattern,string patternRepeat,string userName);
    }
}
