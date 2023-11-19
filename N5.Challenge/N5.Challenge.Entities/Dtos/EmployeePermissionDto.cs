namespace N5.Challenge.Entities.Dtos
{
    public class EmployeePermissionDto
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public Guid PermissionTypeId { get; set; }
        public string PermissionTypeName { get; set; }
        public bool Enabled { get; set; }
    }
}
