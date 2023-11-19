namespace N5.Challenge.Entities.Models
{
    public class UpdatePermissionModel
    {
        public Guid EmployeeId { get; set; }
        public Guid PermissionTypeId { get; set; }
        public bool Enabled { get; set; }
    }
}
