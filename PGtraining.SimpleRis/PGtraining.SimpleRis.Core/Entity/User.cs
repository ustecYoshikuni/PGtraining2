using System.ComponentModel.DataAnnotations.Schema;

namespace PGtraining.SimpleRis.Core.Entity
{
    [Table("Users")]
    internal class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string UserId { get; set; }
        public string Name { get; set; }
        public string PassWord { get; set; }
        public string ExpirationDate { get; set; }

        public User()
        {
        }
    }
}