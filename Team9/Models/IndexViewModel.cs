using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team9.Models;

namespace Team9.Models
{
    public class AlbumIndexViewModel
    {

        public Album Album { get; set; }

        public Decimal AlbumRating { get; set; }

    }


    public class ArtistIndexViewModel
    {

        public Artist Artist { get; set; }

        public Decimal ArtistRating { get; set; }

    }

    public class SongIndexViewModel
    {

        public Song Song { get; set; }

        public Decimal SongRating { get; set; }

    }

    public class PurchaseItemViewModel
    {
        public PurchaseItem PurchaseItem { get; set; }

        public Decimal PurchaseItemRating { get; set; }
    }

    public class PurchaseViewModel
    {
        public Int32 PurchaseID { get; set; }
        
        public String subtotal { get; set; }

        public String taxTotal { get; set; }

        public String discountTotal { get; set; }

        public String grandTotal { get; set; }

        //Add validations here
        public String newCardNumber { get; set; }

        public String giftRecipient { get; set; }
    }

    public class PurchaseItemReportViewModel
    {
        public PurchaseItem PurchaseItem { get; set; }

        public Int32 purchaseCount { get; set; }

        public Decimal songRevenue { get; set; }

    }
}