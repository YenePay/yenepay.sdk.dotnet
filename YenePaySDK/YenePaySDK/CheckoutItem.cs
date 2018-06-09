using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YenePaySdk
{
    public class CheckoutItem
    {
        public string ItemId { get; set; }
        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        /// <value>The name of the item.</value>
        [Required]
        public string ItemName { get; set; }
        /// <summary>
        /// Gets or sets the unit price for the item
        /// </summary>
        /// <value>The unit price.</value>
        [Required]
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// Gets or sets the quantity of the item
        /// </summary>
        /// <value>The quantity.</value>
        [Required]
        public int Quantity { get; set; }
        /// <summary>
        /// Gets or sets the DeliveryFee
        /// </summary>
        /// <value>The DeliveryFee.</value>
        public decimal? DeliveryFee { get; set; }
        /// <summary>
        /// Gets or sets the 15% VAT amount (if available)
        /// </summary>
        /// <value>The tax1.</value>
        public decimal? Tax1 { get; set; }
        /// <summary>
        /// Gets or sets the 10% TOT amount (if available)
        /// </summary>
        /// <value>The tax2.</value>
        public decimal? Tax2 { get; set; }
        /// <summary>
        /// Gets or sets the discount amount on the item (if available)
        /// </summary>
        /// <value>The discount.</value>
        public decimal? Discount { get; set; }
        /// <summary>
        /// Gets or sets the handling fee for shipping the item (if available)
        /// </summary>
        /// <value>The handling fee.</value>
        public decimal? HandlingFee { get; set; }
        /// <summary>
        /// Gets the total price 
        /// </summary>
        /// <value>The total price.</value>
        public decimal TotalPrice
        {
            get
            {
                decimal price = UnitPrice < 0 ? decimal.Zero : UnitPrice;
                int quantity = Quantity < 1 ? 1 : Quantity;
                return price * quantity;
            }
        }

        public CheckoutItem(string itemId, string itemName, decimal price, int quantity = 1, decimal? tax1 = null, decimal? tax2 = null, decimal? discount = null, decimal? handlingFee = null, decimal? deliveryFee = null)
        {
            this.Discount = discount;
            this.HandlingFee = handlingFee;
            this.ItemId = itemId;
            this.ItemName = itemName;
            this.UnitPrice = price;
            this.Quantity = quantity;
            this.DeliveryFee = deliveryFee;
            this.Tax1 = tax1;
            this.Tax2 = tax2;
        }

        public CheckoutItem()
        {
            this.Quantity = 1;
        }

        public CheckoutItem(string itemName, decimal price, int quantity = 1)
        {
            this.ItemName = itemName;
            this.UnitPrice = price;
            this.Quantity = quantity;
        }

        public Dictionary<string, string> GetAsKeyValue(Dictionary<string, string> dict)
        {
            if (dict == null)
                dict = new Dictionary<string, string>();

            dict.Add("ItemId", ItemId);
            dict.Add("ItemName", ItemName);
            dict.Add("UnitPrice", UnitPrice.ToString());
            dict.Add("Quantity", Quantity.ToString());

            if (Discount.HasValue)
                dict.Add("Discount", Discount.Value.ToString());
            if (HandlingFee.HasValue)
                dict.Add("HandlingFee", HandlingFee.Value.ToString());
            if (DeliveryFee.HasValue)
                dict.Add("DeliveryFee", DeliveryFee.Value.ToString());
            if (Tax1.HasValue)
                dict.Add("Tax1", Tax1.Value.ToString());
            if (Tax2.HasValue)
                dict.Add("Tax2", Tax2.Value.ToString());

            return dict;
        }
    }
}
