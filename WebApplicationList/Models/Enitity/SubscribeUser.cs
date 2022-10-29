using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationList.Models.Enitity
{
    [Table("SubscribersUsers")]
    public class SubscribeUser
    {
        [Key]
        public int Id { get; set; }
        public User? user { get; set; }
        public User? subscribe { get; set; }
    }
}
