using System.ComponentModel.DataAnnotations;

namespace WebApplicationList.Models.Enitity
{
    public class SubscribeUser
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public User? user { get; set; }
        public string? SubscribeId { get; set; }
        public User? subscribe { get; set; }
    }
}
