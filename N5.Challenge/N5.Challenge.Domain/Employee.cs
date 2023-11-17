namespace N5.Challenge.Domain
{
    public class Employee: BaseEntity
    {
        public string FullName { get; set; }
        public string DocumentNumber { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public ICollection<Permission> Permissions { get; set; }
    }
}
