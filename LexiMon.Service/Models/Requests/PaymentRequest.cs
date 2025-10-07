using System.ComponentModel.DataAnnotations;

namespace LexiMon.Service.Models.Requests;

public class PaymentRequest
{
    [Required(ErrorMessage = "Vui lòng nhập OrderId!")]
    public Guid OrderId { get; set; }
}