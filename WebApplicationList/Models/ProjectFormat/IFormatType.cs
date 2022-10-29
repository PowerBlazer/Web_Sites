namespace WebApplicationList.Models.ProjectFormat
{
    public interface IFormatType
    {
        FileFormatResult FormattingFile(List<string> fileLines,string userName,string projectName);
    }
}
