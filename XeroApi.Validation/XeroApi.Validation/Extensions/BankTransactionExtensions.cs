using Xero.Api.Core.Model.Types;

namespace XeroApi.Validation.Extensions
{
    public static class BankTransactionExtensions
    {
        public static bool IsOverpayment(this BankTransactionType bankTransactionType)
        {
            return bankTransactionType == BankTransactionType.ReceiveOverpayment || 
                   bankTransactionType == BankTransactionType.SpendOverpayment;
        }
    }
}
