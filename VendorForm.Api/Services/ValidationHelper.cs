using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VendorForm.Api.Models;

namespace VendorForm.Api.Services;

public static class ValidationHelper
{
    public static List<string> ValidateVendor(VendorInformation vendorInfo)
    {
        var errors = new List<string>();

        var context = new ValidationContext(vendorInfo);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(
            vendorInfo,
            context,
            results,
            validateAllProperties: true);

        foreach(var result in results)
        {
            errors.Add(result.ErrorMessage ?? $"{result.MemberNames.First()}: Validation error.");
        }

        if(vendorInfo.EntityType == "Business")
        {
            if (string.IsNullOrEmpty(vendorInfo.FederalTaxId))
            {
                errors.Add("Federal Tax Id: Validation error.");
            }
            
        } else if(vendorInfo.EntityType == "Individual")
        {
            if(string.IsNullOrEmpty(vendorInfo.SSN) && string.IsNullOrEmpty(vendorInfo.LSUId))
            {
                errors.Add("SSN or LSUId: Validation error.");
            }
        }else
        {
            errors.Add("EntityType: Validation error.");
        }

            return errors;
    }
}


