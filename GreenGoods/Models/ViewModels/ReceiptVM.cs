using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GreenBowl.Models.ViewModels
{
    public class ReceiptVM
    {
        [Key]
        public string ID { get; set; } = string.Empty;
        public int Lot { get; set; }
        public ProductName ProductName { get; set; }
        public string Client { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime SellingDate { get; set; } = DateTime.Today;
        public DateTime DeliveryDate { get; set; } = DateTime.Today;
        public int Quantity { get; set; }
        public List<SelectListItem> LotOptions { get; set; } = new();
    }
}
