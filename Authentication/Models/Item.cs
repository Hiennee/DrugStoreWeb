using System.ComponentModel.DataAnnotations;
using System.Security.Permissions;

namespace Authentication.Models
{
    public class Item
    {
        public int Id { get; set; }
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        [Display(Name = "Loại sản phẩm")]
        public string Category { get; set; }
        public int Quantity { get; set; }
        [Display(Name = "Giá")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }
        public double Discount { get; set; }
        public double NewPrice { get { return Price - Price * Discount; } }
        public double TotalPrice { get { return NewPrice * Quantity; } set { } }
    }
}
