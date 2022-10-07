﻿using WebApplicationList.Models.MainSiteModels.ProjectModels;

namespace WebApplicationList.Models
{
    public class UserProject
    {

        public UserProject()
        {
            AddedTime = DateTime.Now;
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? UrlImage { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public DateTime AddedTime { get; set; }

        public string? User_Id { get; set; }
        public List<ProjectComment> projectComments { get; set; } = new List<ProjectComment>();

    }
}
