using System.ComponentModel.DataAnnotations;

namespace GreenBowl.Models
{
    public class Receipt
    {
        [Key]
        public string ID { get; set; }
        public int Lot { get; set; }
        public ProductName ProductName { get; set; }
        public string Client { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime SellingDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int Quantity { get; set; }

    }
}
