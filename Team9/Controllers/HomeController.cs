using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team9.Models;
using Microsoft.AspNet.Identity;

//XXTODO: Change the namespace here to match your project's name
namespace Team9.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // Get: Featured Items
        public ActionResult Index()
        {
            // a list of the view models from each query (song,artist,album)
            List<HomeIndexViewModel> hivm = new List<HomeIndexViewModel>();

            // FEATURED SONGS
            var songQuery = from s in db.Songs
                            where s.isFeatured == true
                            select s;

            // query to list
            List<Song> Songlist = songQuery.ToList();
            foreach (Song s in Songlist)
            {
                // each song will be a view model
                HomeIndexViewModel songHIVM = new HomeIndexViewModel();
                // set the bool to true to make it that item for later
                songHIVM.isSong = true;
                songHIVM.Song = s;
                songHIVM.SongRating = getAverageSongRating(s.SongID).ToString("0.0");
                //add the song view model to the overall view model for all (song, artist, album)
                hivm.Add(songHIVM);
            }





            // FEATURED Artist
            var artistQuery = from a in db.Artists
                              where a.isFeatured == true
                              select a;

            List<Artist> Artistlist = artistQuery.ToList();
            foreach (Artist a in Artistlist)
            {
                HomeIndexViewModel artistHIVM = new HomeIndexViewModel();
                artistHIVM.isArist = true;
                artistHIVM.Artist = a;
                artistHIVM.ArtistRating = getAverageArtistRating(a.ArtistID).ToString("0.0");
                hivm.Add(artistHIVM);
            }





            // FEATURED ALBUM
            var albumQuery = from a in db.Albums
                             where a.isFeatured == true
                             select a;

            List<Album> Albumlist = albumQuery.ToList();
            foreach (Album a in Albumlist)
            {
                HomeIndexViewModel albumHIVM = new HomeIndexViewModel();
                albumHIVM.isAlbum = true;
                albumHIVM.Album = a;
                albumHIVM.AlbumRating = getAverageAlbumRating(a.AlbumID).ToString("0.0");
                hivm.Add(albumHIVM);
            }

            return View(hivm);

        }


        //####################################################################################//
        //     get average ratings functions                                                  //
        //####################################################################################//
        public Decimal getAverageAlbumRating(int? id)
        {
            Decimal count = 0;
            Decimal total = 0;
            Decimal average;

            Album album = db.Albums.Find(id);
            foreach (Rating r in album.AlbumRatings)
            {
                count += 1;
                total += r.RatingValue;
            }
            if (count == 0)
            {
                average = 0;
            }
            else
            {
                average = total / count;
            }

            return average;
        }

        public Decimal getAverageSongRating(int? id)
        {
            Decimal count = 0;
            Decimal total = 0;
            Decimal average;

            Song song = db.Songs.Find(id);
            foreach (Rating r in song.SongRatings)
            {
                count += 1;
                total += r.RatingValue;
            }
            if (count == 0)
            {
                average = 0;
            }
            else
            {
                average = total / count;
            }

            return average;
        }

        public Decimal getAverageArtistRating(int? id)
        {
            Decimal count = 0;
            Decimal total = 0;
            Decimal average;

            Artist artist = db.Artists.Find(id);
            foreach (Rating r in artist.ArtistRatings)
            {
                count += 1;
                total += r.RatingValue;
            }
            if (count == 0)
            {
                average = 0;
            }
            else
            {
                average = total / count;
            }

            return average;
        }
        //####################################################################################//
        //      end of average functions                                                      //
        //####################################################################################//
    }
}