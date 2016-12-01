using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;




namespace Team9.Models
{
    public class Artist
    {
        [Key]
        public Int32 ArtistID { get; set; }

        [Required(ErrorMessage = "Please enter a valid name")]
        public String ArtistName { get; set; }

        public virtual List<Genre> ArtistGenre { get; set; }

        public virtual List<Rating> ArtistRatings { get; set; }

        public virtual List<Song> ArtistSongs { get; set; }

        public virtual List<Album> ArtistAlbums { get; set; }

    }
}