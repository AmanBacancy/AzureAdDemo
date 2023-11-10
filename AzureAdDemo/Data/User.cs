using System.ComponentModel.DataAnnotations.Schema;

namespace AzureAdDemo.Data
{
    public class User
    {
        public int Id { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; }
        [Column(name: "Azure_User_Id")]
        public Guid? AzureUserId { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? Username { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string? Password { get; set; }

        [Column(name: "Is_Active")]
        public bool IsActive { get; set; } = true;
        [Column(name: "Create_Date")]
        public DateTime CreateDate { get; set; }
    }
}
