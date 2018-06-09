using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using YenePaySdk;

namespace SampleShopApplication.Controllers
{
    public class HomeController : Controller
    {
        private CheckoutOptions checkoutoptions;
        private string pdtToken = "YOUR_PDT_KEY_HERE";

        public HomeController()
        {
            string sellerCode = "YOUR_YENEPAY_SELLER_CODE";
            string successUrlReturn = "http://localhost:5525/Home/PaymentSuccessReturnUrl"; //"YOUR_SUCCESS_URL";
            string ipnUrlReturn = "http://localhost:5525/Home/IPNDestination"; //"YOUR_IPN_URL";
            string cancelUrlReturn = "http://localhost:5525/Home/PaymentCancelReturnUrl"; //"YOUR_CANCEL_URL";
            string failureUrlReturn = ""; //"YOUR_FAILURE_URL";
            bool useSandBox = true;
            checkoutoptions = new CheckoutOptions(sellerCode, string.Empty, CheckoutType.Express, useSandBox, null, successUrlReturn, cancelUrlReturn, ipnUrlReturn, failureUrlReturn);
        }

        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Handles the Express checkout type which allows buying a single item from your site or online store
        /// </summary>
        [HttpPost]
        public void CheckoutExpress()
        {
            checkoutoptions.Process = CheckoutType.Express;
            var itemId = Request.Form["ItemId"];
            var itemName = Request.Form["ItemName"];
            var unitPrice = decimal.Parse(Request.Form["UnitPrice"]);
            var quantity = int.Parse(Request.Form["Quantity"]);
            var discount = decimal.Parse(Request.Form["Discount"]);
            var deliveryFee = decimal.Parse(Request.Form["DeliveryFee"]);
            var handlingFee = decimal.Parse(Request.Form["HandlingFee"]);
            var tax1 = decimal.Parse(Request.Form["Tax1"]);
            var tax2 = decimal.Parse(Request.Form["Tax2"]);

            CheckoutItem checkoutitem = new CheckoutItem(itemId, itemName, unitPrice, quantity, tax1, tax2, discount, handlingFee, deliveryFee);
            checkoutoptions.OrderId = "12-34"; //"YOUR_UNIQUE_ID_FOR_THIS_ORDER";  //can also be set null
            checkoutoptions.ExpiresAfter = 2880; //"NUMBER_OF_MINUTES_BEFORE_THE_ORDER_EXPIRES"; //setting null means it never expires
            var url = CheckoutHelper.GetCheckoutUrl(checkoutoptions, checkoutitem);
            Response.Redirect(url);
        }

        [HttpGet]
        public ActionResult Cart()
        {
            return View();
        }

        /// <summary>
        /// Handles the Cart checkout type which allows buying multiple items at once from your site or online store
        /// </summary>
        /// <param name="Items">A list of Items selected by your customer</param>
        /// <returns>A JSON object containing the checkout payment URL which you can use to redirect a customer to YenePay's payment processing site</returns>
        [HttpPost]
        public ActionResult CheckoutCart(List<CheckoutItem> Items)
        {
            checkoutoptions.Process = CheckoutType.Cart;
            decimal? totalItemsDeliveryFee = 50;
            decimal? totalItemsDiscount = 30;
            decimal? totalItemsHandlingFee = 10;
            decimal? totalItemsTax1 = Items.Sum(i => (i.UnitPrice * i.Quantity)) * (decimal)0.15;
            decimal? totalItemsTax2 = Items.Sum(i => (i.UnitPrice * i.Quantity)) * (decimal)0.02;
            checkoutoptions.SetOrderFees(totalItemsDeliveryFee, totalItemsDiscount, totalItemsHandlingFee, totalItemsTax1, totalItemsTax2);

            checkoutoptions.OrderId = "AB-CD"; //"YOUR_UNIQUE_ID_FOR_THIS_ORDER";  //can also be set null
            checkoutoptions.ExpiresAfter = 2880; //"NUMBER_OF_MINUTES_BEFORE_THE_ORDER_EXPIRES"; //setting null means it never expires

            var url = CheckoutHelper.GetCheckoutUrl(checkoutoptions, Items);
            return Json(new { redirectUrl = url });

        }

        [HttpPost]
        public async Task<string> IPNDestination(IPNModel ipnModel)
        {
            var result = string.Empty;
            ipnModel.UseSandbox = checkoutoptions.UseSandbox;
            if (ipnModel != null)
            {
                var isIPNValid = await CheckIPN(ipnModel);

                if (isIPNValid)
                {
                    //This means the payment is completed
                    //You can now mark the order as "Paid" or "Completed" here and start the delivery process
                }
            }
            return result;
        }

        public async Task<ActionResult> PaymentSuccessReturnUrl(IPNModel ipnModel)
        {
            PDTRequestModel model = new PDTRequestModel(pdtToken, ipnModel.TransactionId, ipnModel.MerchantOrderId);
            var pdtResponse = await CheckoutHelper.RequestPDT(model);
            if (pdtResponse.Count() > 0)
            {
                if (pdtResponse["Status"] == "Paid")
                {
                    //This means the payment is completed. 
                    //You can extract more information of the transaction from the pdtResponse dictionary
                    //You can now mark the order as "Paid" or "Completed" here and start the delivery process
                }
            }
            else
            {
                //This means the pdt request has failed.
                //possible reasons are 
                //1. the TransactionId is not valid
                //2. the PDT_Key is incorrect
            }
            return Redirect("/"); ;
        }

        public async Task<string> PaymentCancelReturnUrl(IPNModel ipnModel)
        {
            PDTRequestModel model = new PDTRequestModel(pdtToken, ipnModel.TransactionId, ipnModel.MerchantOrderId);
            var pdtResponse = await CheckoutHelper.RequestPDT(model);
            if (pdtResponse.Count() > 0)
            {
                if (pdtResponse["Status"] == "Canceled")
                {
                    //This means the payment is canceled. 
                    //You can extract more information of the transaction from the pdtResponse dictionary
                    //You can now mark the order as "Canceled" here.
                }
            }
            else
            {
                //This means the pdt request has failed.
                //possible reasons are 
                //1. the TransactionId is not valid
                //2. the PDT_Key is incorrect
            }
            return string.Empty;
        }

        private async Task<bool> CheckIPN(IPNModel model)
        {
            return await CheckoutHelper.IsIPNAuthentic(model);
        }

    }
}
