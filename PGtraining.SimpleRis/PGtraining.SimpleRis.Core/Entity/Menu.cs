using System.ComponentModel.DataAnnotations.Schema;

namespace PGtraining.SimpleRis.Core.Entity
{
    [Table("Menu")]
    public class Menu
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string OrderNo { get; set; }
        public string MenuCode { get; set; }
        public string MenuName { get; set; }

        public Menu()
        {
        }
    }
}