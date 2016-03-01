using System.Collections.Generic;
using System.Linq;
using Xero.Api.Core.Model;

namespace XeroApi.Validation.Helpers
{
    public static class JournalLineHelper
    {
        public static decimal GetJournalTotal(this LineItem li)
        {
            return GetJournalSubTotal(li) + li.TaxAmount.GetValueOrDefault();
        }

        public static decimal GetJournalSubTotal(this LineItem li)
        {
            return li.LineAmount.GetValueOrDefault();
        }

        public static decimal GetJournalSubTotal(this IEnumerable<LineItem> li)
        {
            return li.Sum(a => a.GetSubTotal());
        }

        public static decimal GetJournalTotal(this IEnumerable<LineItem> li)
        {
            return li.Sum(a => a.GetTotal());
        }
    }
}
