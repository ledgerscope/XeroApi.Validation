﻿using System;
using System.Linq;
using Xero.Api.Core.Model;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using XeroApi.Validation.Helpers;
using XeroApi.Validation.Extensions;
using Unity;

namespace XeroApi.Validation
{
    public class BankTransactionValidator : Validator<BankTransaction>
    {
        Validator<LineItem> lineItemValidator = null;

        public BankTransactionValidator()
            : base(null, null)
        {
            this.lineItemValidator = ValidationHelper.Container.Resolve<Validator<LineItem>>();
        }

        protected override void DoValidate(BankTransaction objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            if (objectToValidate.Contact == null)
            {
                validationResults.AddResult(new ValidationResult("The document has no Contact", currentTarget, key, "Contact", this));
            }

            if (objectToValidate.LineItems == null || !objectToValidate.LineItems.Any())
            {
                validationResults.AddResult(new ValidationResult("The document has no LineItems", currentTarget, key, "LineItems", this));
            }
            else
            {
                ValidationResults vr = new ValidationResults();
                foreach (var item in objectToValidate.LineItems)
                {
                    var valResults = lineItemValidator.Validate(item).AsEnumerable();
                    if (objectToValidate.Type.IsOverpayment())
                        valResults = valResults.Where(a => a.Tag != LineItemValidator.AccountCode);

                    vr.AddAllResults(valResults);

                    bool? isValidTax = item.IsValidTax();
                    if (isValidTax.HasValue && !isValidTax.Value)
                    {
                        var msg = string.Format("Invalid Tax Amount ({0} expected: {1})", item.TaxAmount, item.CalculateTaxAmount());
                        vr.AddResult(new ValidationResult(msg, currentTarget, key, "TaxAmount", this));
                    }
                }
                if (vr.Any())
                {
                    validationResults.AddResult(new ValidationResult("Invalid LineItems", currentTarget, key, "LineItems", this, vr));
                }

                if (objectToValidate.LineItems.GetLineItemTotal() <= 0)
                {
                    validationResults.AddResult(new ValidationResult("The LineItems total must be greater than 0.", currentTarget, key, "LineItems", this));
                }
            }

            if (objectToValidate.Total.HasValue)
            {
                if (objectToValidate.Total.Value != objectToValidate.LineItems.Sum(a => a.GetLineItemTotal()))
                {
                    validationResults.AddResult(new ValidationResult("The document total does not equal the sum of the lines.", currentTarget, key, "Total", this));
                }
                if (objectToValidate.Total.Value <= 0)
                {
                    validationResults.AddResult(new ValidationResult("The document total must be greater than 0.", currentTarget, key, "Total", this));
                }
            }

            if (objectToValidate.SubTotal.HasValue)
            {
                if (objectToValidate.SubTotal.Value != objectToValidate.LineItems.GetLineItemSubTotal())
                {
                    validationResults.AddResult(new ValidationResult("The document subtotal does not equal the sum of the lines.", currentTarget, key, "SubTotal", this));
                }
                if (objectToValidate.SubTotal.Value <= 0)
                {
                    validationResults.AddResult(new ValidationResult("The document subtotal must be greater than 0.", currentTarget, key, "SubTotal", this));
                }
            }

            if (objectToValidate.TotalTax.HasValue)
            {
                if (objectToValidate.TotalTax.Value != objectToValidate.LineItems.Sum(a => a.TaxAmount))
                {
                    validationResults.AddResult(
                        new ValidationResult("The document totaltax does not equal the sum of the lines.", currentTarget,
                            key, "TotalTax", this));
                }
                if (objectToValidate.TotalTax.Value < 0)
                {
                    validationResults.AddResult(
                        new ValidationResult("The document totaltax must be greater than or equal to 0.", currentTarget,
                            key, "TotalTax", this));
                }
            }
        }

        protected override string DefaultMessageTemplate
        {
            get { throw new NotImplementedException(); }
        }
    }
}
