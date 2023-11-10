using System.ComponentModel.DataAnnotations.Schema;

namespace AzureAdDemo.Data
{
    public class Role
    {
        public int Id { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; }

        [Column(name: "Is_Active")]
        public bool IsActive { get; set; } = true;
        [Column(name: "Create_Date")]
        public DateTime CreateDate { get; set; }
    }
}
