namespace Demo.Data.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Login { get; set; }
        public string Password {  get; set; }
        public string FullName { get; set; }
        public int roleId { get; set; }

        public Role? role { get; set; }
    }
}
