using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YenePaySdk
{
    public class CheckoutHelper
    {
        private const string checkoutBaseUrlProd = "https://www.yenepay.com/checkout/Home/Process/";
        private const string checkoutBaseUrlSandbox = "https://test.yenepay.com/Home/Process/";
        private const string ipnVerifyUrlProd = "https://endpoints.yenepay.com/api/verify/ipn/";
        private const string ipnVerifyUrlSandbox = "https://testapi.yenepay.com/api/verify/ipn/";
        private const string pdtUrlProd = "https://endpoints.yenepay.com/api/verify/pdt/";
        private const string pdtUrlSandbox = "https://testapi.yenepay.com/api/verify/pdt/";

        private static HttpClient client = new HttpClient();

        public static string GetCheckoutUrl(CheckoutOptions options, CheckoutItem item)
        {
            try
            {
                var dict = options.GetAsKeyValue(false);
                item.GetAsKeyValue(dict);
                var checkoutUrl = string.Format(string.Concat(checkoutBaseUrlProd, "?{0}"), dict.ConvertToUriParamString());
                if (options.UseSandbox)
                    checkoutUrl = string.Format(string.Concat(checkoutBaseUrlSandbox, "?{0}"), dict.ConvertToUriParamString());
                return checkoutUrl;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetCheckoutUrl(CheckoutOptions options, List<CheckoutItem> items)
        {
            try
            {
                var dict = options.GetAsKeyValue(true);
                for (int i = 0; i < items.Count; i++)
                {
                    var itemDict = items[i].GetAsKeyValue(null);
                    foreach (var keyValue in itemDict)
                    {
                        dict.Add(string.Format("Items[{0}].{1}", i, keyValue.Key), keyValue.Value);
                    }
                }
                var checkoutUrl = string.Format(string.Concat(checkoutBaseUrlProd, "?{0}"), dict.ConvertToUriParamString());
                if (options.UseSandbox)
                    checkoutUrl = string.Format(string.Concat(checkoutBaseUrlSandbox, "?{0}"), dict.ConvertToUriParamString());
                return checkoutUrl;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<bool> IsIPNAuthentic(IPNModel ipnModel)
        {
            var keyValues = ipnModel.GetIPNDictionary();
            var content = new FormUrlEncodedContent(keyValues);
            var ipnVerifyUrl = ipnModel.UseSandbox ? ipnVerifyUrlSandbox : ipnVerifyUrlProd;
            var response = await client.PostAsync(ipnVerifyUrl, content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public static async Task<Dictionary<string, string>> RequestPDT(PDTRequestModel pdtRequest)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            pdtRequest.RequestType = "PDT";
            var keyValues = pdtRequest.GetPDTDictionary();
            var content = new FormUrlEncodedContent(keyValues);
            var pdtUrl = pdtRequest.UseSandbox ? pdtUrlSandbox : pdtUrlProd;
            var response = await client.PostAsync(pdtUrl, content);
            if (response.IsSuccessStatusCode)
            {
                string pdtString = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(pdtString))
                {
                    pdtString = pdtString.Trim();
                    //the PDT string with this format: "result=SUCCESS&TotalAmount=total_amount&BuyerId=buyer_yenepay_account_id&BuyerName=buyer_name&TransactionFee=service_charge_fee_amount&MerchantOrderId=order_id_set_by_merchant&MerchantId=merchant_yenepay_account_id&MerchantCode=merchant_yenepay_seller_code&TransactionId=transaction_order_id&Status=current_status_of_the_order&StatusDescription=current_status_description&Currency=currency_used_for_transaction"
                    var arr = pdtString.Trim('"').Split('&');
                    foreach (var item in arr)
                    {
                        var keyValue = item.Split('=');
                        result.Add(keyValue[0], keyValue[1]);
                    }
                }
            }
            return result;
        }
    }

    public static class HelperExtentions
    {
        public static IDictionary<TKey, TValue> Clone<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            Dictionary<TKey, TValue> clone = new Dictionary<TKey, TValue>();
            foreach (var keyValue in dict)
            {
                clone.Add(keyValue.Key, keyValue.Value);
            }
            return clone;
        }

        public static void CleanQueryParams(this IDictionary<string, string> dict)
        {
            if (dict.Count > 0)
            {
                var invalidKeys = dict.Keys.Where(k => k.Contains("?")).ToArray();
                foreach (var invalidKey in invalidKeys)
                {
                    string validKey = invalidKey.Replace("?", string.Empty);
                    string value = dict[invalidKey];
                    dict.Remove(invalidKey);
                    dict.Add(validKey, value);
                }
            }
        }

        public static string ConvertToUriParamString(this IDictionary<string, string> dict, bool cloneFirst = false, bool cleanFirst = false)
        {
            IDictionary<string, string> resultDict = cloneFirst ? dict.Clone() : dict;
            if (cleanFirst)
            {
                resultDict.CleanQueryParams();
            }
            List<string> paramList = new List<string>();
            foreach (var keyValue in resultDict)
            {
                paramList.Add(string.Format("{0}={1}", keyValue.Key, keyValue.Value));
            }
            if (paramList.Count > 0)
            {
                return string.Join("&", paramList.ToArray());
            }
            return string.Empty;
        }
    }
}

