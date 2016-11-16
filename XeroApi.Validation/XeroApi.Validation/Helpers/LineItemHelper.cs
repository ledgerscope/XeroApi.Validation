using System.Collections.Generic;
using System.Linq;
using Xero.Api.Core.Model;

namespace XeroApi.Validation.Helpers
{
    public static class LineItemHelper
    {
        public static decimal GetLineItemTotal(this LineItem li)
        {
            return GetLineItemSubTotal(li) + li.TaxAmount.GetValueOrDefault();
        }

        public static decimal GetLineItemSubTotal(this LineItem li)
        {
            if (li.LineAmount.HasValue)
            {
                return li.LineAmount.Value;
            }
            else
            {
                return li.UnitAmount.GetValueOrDefault() * li.Quantity.GetValueOrDefault();
            }
        }

        public static decimal GetLineItemSubTotal(this IEnumerable<LineItem> li)
        {
            return li.Sum(a => a.GetLineItemSubTotal());
        }

        public static decimal GetLineItemTotal(this IEnumerable<LineItem> li)
        {
            return li.Sum(a => a.GetLineItemTotal());
        }

        //Manual Journals

        public static decimal GetLineItemTotal(this Line li)
        {
            var calculatedGross = li.Amount + li.TaxAmount;
            if (calculatedGross != 0)
                return calculatedGross;
            return li.GrossAmount;
        }

        public static decimal GetLineItemTotal(this IEnumerable<Line> li)
        {
            return li.Sum(a => a.GetLineItemTotal());
        }
    }
}
