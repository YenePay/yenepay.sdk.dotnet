using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YenePaySdk
{
    public class IPNModel
    {
        /// <summary>
        /// Gets or sets the total amount.
        /// </summary>
        /// <value>The total amount.</value>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// Gets or sets the buyer identifier.
        /// </summary>
        /// <value>The buyer identifier.</value>
        public Guid BuyerId { get; set; }
        /// <summary>
        /// Gets or sets the merchant order identifier.
        /// </summary>
        /// <value>The merchant order identifier.</value>
        public string MerchantOrderId { get; set; }
        /// <summary>
        /// Gets or sets the merchant identifier.
        /// </summary>
        /// <value>The merchant identifier.</value>
        public Guid MerchantId { get; set; }
        /// <summary>
        /// Gets or sets the merchant code.
        /// </summary>
        /// <value>The merchant code.</value>
        public string MerchantCode { get; set; }
        /// <summary>
        /// Gets or sets the transaction identifier.
        /// </summary>
        /// <value>The transaction identifier.</value>
        public Guid TransactionId { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string Status { get; set; }
        /// <summary>
        /// description of the payment status
        /// </summary>
        public string TransactionCode { get; set; }
        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        /// <value>The currency.</value>
        public string Currency { get; set; }
        /// <summary>
        /// encrypted signature 
        /// </summary>
        public string Signature { get; set; }

        public bool UseSandbox { get; set; }

        public Dictionary<string, string> GetIPNDictionary()
        {
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            keyValues.Add("TotalAmount", this.TotalAmount.ToString());
            keyValues.Add("BuyerId", this.BuyerId.ToString());
            keyValues.Add("MerchantOrderId", this.MerchantOrderId.ToString());
            keyValues.Add("MerchantId", this.MerchantId.ToString());
            keyValues.Add("MerchantCode", this.MerchantCode);
            keyValues.Add("TransactionId", this.TransactionId.ToString());
            keyValues.Add("Status", this.Status);
            keyValues.Add("TransactionCode", this.TransactionCode);
            keyValues.Add("Currency", this.Currency);
            keyValues.Add("Signature", this.Signature);
            return keyValues;
        }
    }
}
