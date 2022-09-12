namespace WebApplicationList.IdentityApplication.ViewModels
{
    public class ViewAuthorizationModel
    {
        public ViewAuthorizationModel()
        {
            Errors = new List<string>();
        }
        public bool Success { get; set; }
        public List<string>? Errors { get; set; }
        public string? Type { get; set; }




        public void IsSuccess() => Success = true;

        public void NotSuccess() => Success = false;
    }
}
