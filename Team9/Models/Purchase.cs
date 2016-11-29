using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;





namespace Team9.Models
{
    public class Purchase
    {
        [Key]
        public Int32 PurchaseID { get; set; }

        public bool isPurchased { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public bool isGift { get; set; }

        public virtual AppUser GiftUser { get; set; }

        public virtual CreditCard PurchaseCard {get;set;}

        public virtual List<PurchaseItem> PurchaseItems { get; set; }

        public virtual AppUser PurchaseUser { get; set; }



    }
}