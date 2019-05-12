using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YenePaySdk
{
    public class CheckoutOptions
    {
        /// <summary>
        /// Gets or sets the seller code assigned by YenePay for the merchant.
        /// </summary>
        [Required]
        public string SellerCode { get; set; }
        /// <summary>
        /// Gets or sets the unique id assigned for the order being processed.
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// Gets or sets the URL on the merchant's site that will be used to redirect the buyer when the payment is successfully completed.
        /// </summary>
        public string SuccessReturn { get; set; }
        /// <summary>
        /// Gets or sets the URL on the merchant's site that will be used to redirect the buyer when the payment process fails.
        /// </summary>
        public string FailureReturn { get; set; }
        /// <summary>
        /// Gets or sets the URL on the merchant's site that will be used to redirect the buyer when the payment process is cancelled.
        /// </summary>
        public string CancelReturn { get; set; }
        /// <summary>
        /// Gets or sets the URL endpoint on the merchant's site that will be used to send Instant Payment Notifications
        /// </summary>
        public string IpnUrlReturn { get; set; }
        /// <summary>
        /// the number of minutes before an order expires for payment;
        /// this is an optional field
        /// </summary>
        public int? ExpiresAfter { get; set; }
        public decimal? TotalItemsDeliveryFee { get; set; }
        public decimal? TotalItemsTax1 { get; set; }
        public decimal? TotalItemsTax2 { get; set; }
        public decimal? TotalItemsDiscount { get; set; }
        public decimal? TotalItemsHandlingFee { get; set; }
        /// <summary>
        /// Gets or sets the switch used to determine if the operation should be done in the sandbox environment
        /// </summary>
        public bool UseSandbox { get; set; }
        /// <summary>
        /// Gets or sets the checkout process type
        /// </summary>
        public CheckoutType Process { get; set; }

        public CheckoutOptions()
        {
            UseSandbox = false;
            Process = CheckoutType.Express;
        }

        public CheckoutOptions(string sellerCode, bool useSandBox)
        {
            SellerCode = sellerCode;
            UseSandbox = useSandBox;
        }

        /// <summary>
        /// Creates a new instance of a CheckoutOptions object with the initial values specified.
        /// This object will be u
        /// </summary>
        /// <param name="sellerCode"></param>
        /// <param name="merchantOrderId"></param>
        /// <param name="process"></param>
        /// <param name="useSandBox"></param>
        /// <param name="expiresAfter"></param>
        /// <param name="successReturn"></param>
        /// <param name="cancelReturn"></param>
        /// <param name="ipnUrl"></param>
        /// <param name="failureUrl"></param>
        public CheckoutOptions(string sellerCode, string merchantOrderId = "", CheckoutType process = CheckoutType.Express, bool useSandBox = false, int? expiresAfter = null, string successReturn = "", string cancelReturn = "", string ipnUrl = "", string failureUrl = "")
        {
            UseSandbox = useSandBox;
            Process = process;
            SellerCode = sellerCode;
            SuccessReturn = successReturn;
            CancelReturn = cancelReturn;
            IpnUrlReturn = ipnUrl;
            FailureReturn = failureUrl;
            ExpiresAfter = expiresAfter;
            OrderId = merchantOrderId;
        }

        public void SetOrderFees(decimal? totalItemsDeliveryFee, decimal? totalItemsDiscount, decimal? totalItemsHandlingFee, decimal? totalItemsTax1, decimal? totalItemsTax2)
        {
            TotalItemsDeliveryFee = totalItemsDeliveryFee;
            TotalItemsDiscount = totalItemsDiscount;
            TotalItemsHandlingFee = totalItemsHandlingFee;
            TotalItemsTax1 = totalItemsTax1;
            TotalItemsTax2 = totalItemsTax2;
        }

        public Dictionary<string, string> GetAsKeyValue(bool forCart)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("MerchantId", SellerCode);
            dict.Add("MerchantOrderId", OrderId);
            dict.Add("SuccessUrl", SuccessReturn);
            dict.Add("CancelUrl", CancelReturn);
            dict.Add("IPNUrl", IpnUrlReturn);
            dict.Add("FailureUrl", FailureReturn);
            dict.Add("Process", Process.ToString());
            if (ExpiresAfter.HasValue)
                dict.Add("ExpiresAfter", ExpiresAfter.Value.ToString());
            if (forCart)
            {
                dict.Add("TotalItemsDeliveryFee", TotalItemsDeliveryFee._ToString());
                dict.Add("TotalItemsTax1", TotalItemsTax1._ToString());
                dict.Add("TotalItemsTax2", TotalItemsTax2._ToString());
                dict.Add("TotalItemsDiscount", TotalItemsDiscount._ToString());
                dict.Add("TotalItemsHandlingFee", TotalItemsHandlingFee._ToString());
            }
            return dict;
        }
    }

    static class DecimalExtensions
    {
        public static string _ToString(this decimal? data)
        {
            return data.HasValue ? data.Value.ToString("0.00") : string.Empty;
        }
    }

    public enum CheckoutType
    {
        Express,
        Cart
    }
}

