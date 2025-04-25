using System.ComponentModel.DataAnnotations;
using TechWave_Electronics.Models;

namespace TechWave_Electronics.Controllers
{
    public class ViewModel : MyUser
    {
        [Key]
        public new int Id { get; set; }
        public string Name { get; set; }
    }

}