using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team9.Models
{
    public class PurchaseItem
    {
        public Int32 PurchaseItemID { get; set; }

        public Decimal PurchaseItemPrice { get; set; }

        public bool isAlbum { get; set; }

        public virtual Song PurchaseItemSong { get; set; }

        public virtual Album PurchaseItemAlbum {get; set; }

        public virtual Purchase Purchase { get; set; }

    }
}