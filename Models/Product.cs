using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityDemo
{
    [Table("Product")]
    public class Product
    {
        [Key]
        public int ProductId {set; get;}

        [Required]
        [StringLength(50)]
        public string Name {set; get;}

        [Column(TypeName="Money")]
        public decimal Price {set; get;}
        public int CategoryId {set; get;} 

        // Sinh FK (CategoryID ~ Cateogry.CategoryID) ràng buộc đến PK key của Category
        public virtual Category Category {set; get;}
    }
}