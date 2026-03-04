using FAP_API.Models;

namespace FAP_API.ViewModels
{
    public class ClassUser
    {
        public string ClassId { get; set; }
        public string ClassCode { get; set; }
        public List<User> Users { get; set; }
    }
}
