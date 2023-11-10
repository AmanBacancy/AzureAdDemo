using System.ComponentModel.DataAnnotations.Schema;

namespace AzureAdDemo.Data
{
    [Table("User_Role_Mapping")]
    public class UserRoleMapping
    {
        public int Id { get; set; }
        [Column(name: "User_Id")]
        public int UserId { get; set; }
        [Column(name: "Role_Id")]
        public int RoleId { get; set; }
        [Column(name: "Is_Active")]
        public bool IsActive { get; set; } = true;
        [Column(name: "Create_Date")]
        public DateTime CreateDate { get; set; }

        [ForeignKey("UserId")]
        public User UserNavigation { get;set; }
        [ForeignKey("RoleId")]
        public Role RoleNavigation { get;set; }
    }
}
