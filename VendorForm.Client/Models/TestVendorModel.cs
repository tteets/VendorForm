using System.ComponentModel.DataAnnotations;

namespace VendorForm.Client.Models;

public class TestVendorModel : VendorInformation
{
    public TestVendorModel()
    {
        VendorName = "Toadstool Tavern";
        PayeeName = "Tony Teets";
        Address = "123 Main St";
        Address2 = string.Empty;
        City = "Baton Rouge";
        State = "Louisiana";
        Zipcode = "70820";
        Email = "test@gmail.com";
        EntityType = "Business";
        FederalTaxId = "121230123";
        SSN = "1234567890";
        LSUId = "0987654321";
        ContactName = "Tony Teets";
        ContactPhone = "1231231230";
        Ext = string.Empty;
        AccountType = "Checking";
        BankName = "Test Bank";
        RoutingNumber = "123456789";
        AccountNumber = "12345678910";
    }
}
