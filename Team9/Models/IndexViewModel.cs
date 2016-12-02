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

        public String AlbumRating { get; set; }

    }


    public class ArtistIndexViewModel
    {

        public Artist Artist { get; set; }

        public String ArtistRating { get; set; }

    }

    public class SongIndexViewModel
    {

        public Song Song { get; set; }

        public String SongRating { get; set; }

    }

    public class PurchaseItemViewModel
    {
        public PurchaseItem PurchaseItem { get; set; }

        public String PurchaseItemRating { get; set; }
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

    public class SongReportViewModel
    {
        public Song Song { get; set; }

        public Int32 purchaseCount { get; set; }

        public Decimal songRevenue { get; set; }

    }

    public class AlbumReportViewModel
    {
        public Album Album { get; set; }

        public Int32 purchaseCount { get; set; }

        public Decimal AlbumRevenue { get; set; }

    }

    public class GenreReportViewModel
    {
        public Genre Genre { get; set; }

        public String topArtist { get; set; }

        public Int32 songCount { get; set; }

        public Int32 albumCount { get; set; }

        public String songRev { get; set;}

        public String albumRev { get; set; }

        public String totalRev { get; set; }
    }

    public class HomeIndexViewModel
    {
        public Song Song { get; set; }

        public bool isSong { get; set; }

        public Artist Artist { get; set; }

        public bool isArist { get; set; }

        public Album Album { get; set; }

        public bool isAlbum { get; set; }

        public String SongRating { get; set; }

        public String ArtistRating { get; set; }

        public String AlbumRating { get; set; }
    }
}