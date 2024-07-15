using System.ComponentModel.DataAnnotations;

namespace Tranzaksyon_com.Order_Processing.Dtos
{
    public class OrderDto
    {
        [Required]
        public string Status { get; set; }
    }
}
