using System.ComponentModel.DataAnnotations;

namespace VendorForm.Api.Models;

public class VendorInformation
{
    [Required(ErrorMessage = "Vendor Name is required.")]
    public string VendorName { get; set; } = string.Empty;

    public string PayeeName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required.")]
    public string Address { get; set; } = string.Empty;
    public string Address2 { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required.")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "State is required.")]
    public string? State { get; set; }

    [Required(ErrorMessage = "Zipcode is required.")]
    public string Zipcode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; } = string.Empty;

    public string EntityType { get; set; } = "Business";
    public string FederalTaxId { get; set; } = string.Empty;
    public string SSN { get; set; } = string.Empty;
    public string LSUId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Contact Name is required.")]
    public string ContactName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Contact Phone is required.")]
    public string ContactPhone { get; set; } = string.Empty;

    public string Ext { get; set; } = string.Empty;

    [Required(ErrorMessage = "Account Type is required.")]
    public string? AccountType { get; set; }

    [Required(ErrorMessage = "Bank Name is required.")]
    public string BankName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Routing Number is required.")]
    public string RoutingNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Account Number is required.")]
    public string AccountNumber { get; set; } = string.Empty;
}