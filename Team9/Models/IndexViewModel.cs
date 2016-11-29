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
}