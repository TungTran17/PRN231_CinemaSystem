namespace BussinessObject.Models
{
    public partial class User
    {
        public User()
        {
            Tickets = new HashSet<Ticket>();
        }

        public enum Roles
        {
            User,
            Staff,
            Admin
        }

        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string AvatarUrl { get; set; } = null!;
        public double Balance { get; set; }
        public int Role { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
