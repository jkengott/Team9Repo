using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;



namespace Team9.Models
{
    public class Album
    {
        [Key]
        public Int32 AlbumID { get; set; }

        [Required(ErrorMessage = "Please enter a valid name")]
        public String AlbumName { get; set; }

        public bool isDiscounted { get; set; }

        public bool isFeatured { get; set; }

        [Required(ErrorMessage = "Please enter a valid price")]
        public Decimal AlbumPrice { get; set; }

        public Decimal DiscountAlbumPrice { get; set; }

        public virtual List<Genre> AlbumGenre { get; set; }

        public virtual List<Artist> AlbumArtist { get; set; }

        public virtual List<Song> Songs { get; set; }

        public virtual List<Rating> AlbumRatings { get; set; }


    }
}