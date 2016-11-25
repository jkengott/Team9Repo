using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;





namespace Team9.Models
{
    public class Rating
    {
        [Key]
        public Int32 RatingID { get; set; }

        public String RatingText { get; set; }

        public Int32  RatingValue { get; set; }

        public AppUser User { get; set; }

        public virtual Song RatingSong { get; set; }

        public virtual Artist RatingArtist { get; set; }

        public virtual Album RatingAlbum { get; set; }
    }

}