using Microsoft.Practices.EnterpriseLibrary.Validation;
using System;
using System.Linq;
using Xero.Api.Core.Model;
using XeroApi.Validation.Helpers;
using Unity;

namespace XeroApi.Validation
{
    public class ManualJournalValidator : Validator<ManualJournal>
    {
        Validator<Line> lineItemValidator = null;

        public ManualJournalValidator(Validator<Line> lineItemValidator)
            : base(null, null)
        {
            this.lineItemValidator = lineItemValidator;
        }

        public ManualJournalValidator()
            : base(null, null)
        {
            this.lineItemValidator = ValidationHelper.Container.Resolve<Validator<Line>>();
        }

        protected override void DoValidate(ManualJournal objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            if (string.IsNullOrEmpty(objectToValidate.Narration))
            {
                validationResults.AddResult(new ValidationResult("Document Narration must be specified.", currentTarget, key, "Narration", this));            
            }

            if (objectToValidate.Lines == null || !objectToValidate.Lines.Any())
            {
                validationResults.AddResult(new ValidationResult("The document has no LineItems", currentTarget, key, "JournalLines", this));
            }
            else
            {
                ValidationResults vr = new ValidationResults();
                foreach (var item in objectToValidate.Lines)
                {
                    lineItemValidator.Validate(item, vr);
                }

                if (vr.Any())
                {
                    validationResults.AddResult(new ValidationResult("Invalid LineItems", currentTarget, key, "LineItems", this, vr));
                }

                if (objectToValidate.Lines.GetLineItemTotal() != 0)
                {
                    validationResults.AddResult(new ValidationResult("LineItems don't balance", currentTarget, key, "LineItems", this, vr));
                }
            }
        }

        protected override string DefaultMessageTemplate
        {
            get { throw new NotImplementedException(); }
        }
    }
}
