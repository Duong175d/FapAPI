namespace FAP_API.ViewModels
{
    public class AccountViewModel
    {
        public string Username { get; set; }
        public string Role { get; set; } 
        public string Status { get; set; }  
    }

    public class AccountDetail
    {
        public string Email { get; set; } 

        public string Role { get; set; }

        public string StudentCode { get; set; }

        public string FullName { get; set; } 

        public string? Status { get; set; }  
    } 
    
}
