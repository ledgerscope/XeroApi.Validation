using Microsoft.Practices.EnterpriseLibrary.Validation;
using System;
using Xero.Api.Core.Model;
using XeroApi.Validation.Helpers;

namespace XeroApi.Validation
{
    public class ManualJournalLineValidator : Validator<Line>
    {
        public ManualJournalLineValidator()
            : base(null, null)
        { }

        protected override void DoValidate(Line objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            if (objectToValidate.AccountCode.IsNullOrWhiteSpace())
            {
                validationResults.AddResult(new ValidationResult("No AccountCode Specified", currentTarget, key, "AccountCode", this));
            }

            if (objectToValidate.Amount == 0)
            {
                validationResults.AddResult(new ValidationResult("LineAmount must be not equal to 0", currentTarget, key, "LineAmount", this));
            }
        }

        protected override string DefaultMessageTemplate
        {
            get { throw new NotImplementedException(); }
        }
    }
}
