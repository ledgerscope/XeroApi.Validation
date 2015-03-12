using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XeroApi.Model;

namespace XeroApi.Validation.Helpers
{
    public static class JournalLineHelper
    {
        public static decimal GetTotal(this JournalLine li)
        {
            return GetSubTotal(li) + li.TaxAmount;
        }

        public static decimal GetSubTotal(this JournalLine li)
        {
            return li.NetAmount;
        }

        public static decimal GetSubTotal(this IEnumerable<JournalLine> li)
        {
            return li.Sum(a => a.GetSubTotal());
        }

        public static decimal GetTotal(this IEnumerable<JournalLine> li)
        {
            return li.Sum(a => a.GetTotal());
        }
    }
}
