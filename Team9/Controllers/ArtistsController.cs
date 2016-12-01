using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Team9.Models;
using Microsoft.AspNet.Identity;

namespace Team9.Controllers
{

    public class ArtistsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        //for search...
        //var query = from p in db.purchases
        //          join pi in db.purchaseitems on p.PurchaseID equals pi.purchase.purchaseID
        //          where blah blah blah
        //          select pi.PurchaseItemSong.SongID

        // GET: Artists
        public ActionResult Index(string SearchString)
        {
            var query = from a in db.Artists
                        select a;

            //create a view bag to store the number of selected customers
            ViewBag.TotalArtistCount = db.Artists.Count();

            //create a list of selected customers
            List<Artist> SelectedArtists = new List<Artist>();

            // create count of selected customers
            ViewBag.SelectedArtistCount = db.Artists.Count();


            if (SearchString == null || SearchString == "") //didnt select anything
            {
                SelectedArtists = query.ToList();
            }
            else //something was picked
            {
                //linq to display searched name
                SelectedArtists = query.Where(c => c.ArtistName.Contains(SearchString)).ToList();

                int SelectedArtistCount = SelectedArtists.Count();

                // create count of selected artists
                ViewBag.SelectedArtistCount = SelectedArtists.Count();

                //order by artists name then average rating
                //TODO: Order by avg rating when we figure that out
                SelectedArtists.OrderBy(c => c.ArtistName);
                //return view with selected artists
                //return View(SelectedArtists);
            }


            //Add average rating to index
            List<ArtistIndexViewModel> ArtistDisplay = new List<ArtistIndexViewModel>();

            foreach (Artist a in SelectedArtists)
            {
                ArtistIndexViewModel AVM = new ArtistIndexViewModel();

                AVM.Artist = a;

                AVM.ArtistRating = getAverageRating(a.ArtistID);

                ArtistDisplay.Add(AVM);

            }
            return View(ArtistDisplay);
        }

        public ActionResult ArtistDetailedSearch()
        {
            ViewBag.SelectedGenre = GetAllGenres();
            return View();
        }

        //Detailed search results
        public ActionResult ArtistSearchResults(string ArtistSearchString, string RatingString, SortOrder SelectedBounds, int[] SelectedGenre)
        {
            var query = from a in db.Artists
                        select a;


            if (ArtistSearchString == null || ArtistSearchString == "") //they didn't select anything
            {
                ViewBag.ArtistSearchString = "Search String was null";
            }
            else //they picked something up
            {
                ViewBag.ArtistSearchString = "The search string is" + ArtistSearchString;
                query = query.Where(a => a.ArtistName.Contains(ArtistSearchString));
            }

            if (SelectedGenre == null) //nothing was selected
            {
                ViewBag.SelectedGenre = "No genres were selected";
            }
            else
            {
                String strSelectedGenre = "The selected genre(s) is/are: ";

                //get list of genres
                ViewBag.AllGenres = GetAllGenres();

                foreach (int GenreID in SelectedGenre)
                {
                    query = query.Where(s => s.ArtistGenre.Any(g => g.GenreID == GenreID));
                   
                }
                ViewBag.SelectedGenre = strSelectedGenre;
            }


            if (RatingString != "")
            //make sure string is a valid number
            {
                Decimal decRating;
                try
                {
                    decRating = Convert.ToDecimal(RatingString);

                }
                catch // this code will disolay when something is wrong
                {
                    //Add a message for the viewbag
                    ViewBag.Message = RatingString + "is not valid number. Please try again";

                    //send user back to homepage
                    return View("ArtistDetailedSearch");
                }


                List<ArtistIndexViewModel> ArtistsDisplay_descend = new List<ArtistIndexViewModel>();
                List<ArtistIndexViewModel> ArtistsDisplay_ascend = new List<ArtistIndexViewModel>();
                foreach (Artist a in query)
                {
                    Decimal d = getAverageRating(a.ArtistID);
                    if (d >= decRating)
                    {
                        ArtistIndexViewModel ab = new ArtistIndexViewModel();
                        ab.Artist = a;
                        ab.ArtistRating = d;
                        ArtistsDisplay_ascend.Add(ab);
                    }
                    else
                    {
                        ArtistIndexViewModel ab = new ArtistIndexViewModel();
                        ab.Artist = a;
                        ab.ArtistRating = d;
                        ArtistsDisplay_descend.Add(ab);
                    }
                }
                IEnumerable<ArtistIndexViewModel> new_list_artists = ArtistsDisplay_ascend;
                IEnumerable<ArtistIndexViewModel> new_list_artists_lt = ArtistsDisplay_descend;



                if (SelectedBounds == SortOrder.ascending)
                {
                    ViewBag.SelectedSortOrder = "The records should be sorted in ascending order";
                    return View("Index", new_list_artists);
                }
                else
                {
                    ViewBag.SelecredSortOrder = "The records should be sored in descending order";
                    return View("Index", new_list_artists_lt);
                }
            }

            List<ArtistIndexViewModel> ArtistsList = new List<ArtistIndexViewModel>();
            foreach (Artist a in query)
            {
                Decimal d = getAverageRating(a.ArtistID);
                    ArtistIndexViewModel ab = new ArtistIndexViewModel();
                    ab.Artist = a;
                    ab.ArtistRating = d;
                    ArtistsList.Add(ab);
                }

                return View("Index", ArtistsList);

        }

        // GET: Artists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // find artist id
            Artist artist = db.Artists.Find(id);
            // viewbag for average artist rating
            ViewBag.AverageArtistRating = getAverageRating(id);
            if (artist == null)
            {
                return HttpNotFound();
            }
            return View(artist);
        }

        // GET: Artists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Artists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ArtistID,ArtistName")] Artist artist)
        {
            if (ModelState.IsValid)
            {
                db.Artists.Add(artist);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(artist);
        }

        // GET: Artists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artist artist = db.Artists.Find(id);
            if (artist == null)
            {
                return HttpNotFound();
            }

            ViewBag.AllSongs = GetAllSongs(@artist);
            ViewBag.AllGenres = GetAllGenres(@artist);
            ViewBag.AllAlbums = GetAllAlbums(@artist);

            return View(artist);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ArtistID,ArtistName")] Artist artist, int[] SelectedSongs, int[] SelectedAlbums, int[] SelectedGenres)
        {
            if (ModelState.IsValid) // NOT CATCHING AS VALID!!!!
            {
                Artist artistToChange = db.Artists.Find(artist.ArtistID);

                //clear current songs
                artistToChange.ArtistSongs.Clear();

                //if there are Songs to add then add them
                if (SelectedSongs != null)
                {
                    foreach (int SongID in SelectedSongs)
                    {
                        Song songToAdd = db.Songs.Find(SongID);
                        artistToChange.ArtistSongs.Add(songToAdd);
                    }
                }

                //clear current albums
                artistToChange.ArtistAlbums.Clear();

                //if there are Songs to add then add them
                if (SelectedAlbums != null)
                {
                    foreach (int AlbumID in SelectedAlbums)
                    {
                        Album albumToAdd = db.Albums.Find(AlbumID);
                        artistToChange.ArtistAlbums.Add(albumToAdd);
                    }
                }

                //change genres
                //remove any existing genre
                artistToChange.ArtistGenre.Clear();

                //if there are genres to add then add them
                if (SelectedGenres != null)
                {
                    foreach (int GenreID in SelectedGenres)
                    {
                        Genre genreToAdd = db.Genres.Find(GenreID);
                        artistToChange.ArtistGenre.Add(genreToAdd);
                    }
                }


                db.Entry(artistToChange).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AllSongs = GetAllSongs(@artist);
            ViewBag.AllGenres = GetAllGenres(@artist);
            ViewBag.AllAlbums = GetAllAlbums(@artist);

            return View(@artist);
        }

        // GET: Artists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artist artist = db.Artists.Find(id);
            if (artist == null)
            {
                return HttpNotFound();
            }
            return View(artist);
        }

        // POST: Artists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Artist artist = db.Artists.Find(id);
            db.Artists.Remove(artist);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Artists/ReviewArtist/5
        public ActionResult ReviewArtist(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artist artist = db.Artists.Find(id);
            Rating NewRating = new Rating();
            NewRating.RatingArtist = artist;
            if (artist == null)
            {
                return HttpNotFound();
            }
            return View(NewRating);
        }

        // POST: Artists/ReviewArtist/5
        //TODO: creat the post method
        //######################################################//
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReviewArtist([Bind(Include = "RatingID,RatingText,RatingValue,RatingArtist_ArtistID")]
                                        Rating rating, int? id)
        {
            if (ModelState.IsValid)
            {
                // get user id
                //TODO: fix this so that it actually gets the user id
                AppUser user = db.Users.Find(User.Identity.GetUserId());
                rating.User = user;
                // get artist id
                rating.RatingArtist = db.Artists.Find(id);
                db.Ratings.Add(rating);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rating);
        }

        // gets the average rating for an artist
        public Decimal getAverageRating(int? id)
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

        public MultiSelectList GetAllGenres()
        {
            var query = from g in db.Genres
                        orderby g.GenreName
                        select g;

            //convert to list
            List<Genre> GenreList = query.ToList();

            //Add in choice for not selecting a Genre
            //Genre NoChoice = new Genre() { GenreID = 0, GenreName = "All Genres" };
            //GenreList.Add(NoChoice);

            //convert to multiselect
            MultiSelectList AllGenres = new MultiSelectList(GenreList.OrderBy(g => g.GenreName), "GenreID", "GenreName");

            return AllGenres;
        }

        public MultiSelectList GetAllGenres(Artist @artist)
        {
            var query = from g in db.Genres
                        orderby g.GenreName
                        select g;

            //convert to list
            List<Genre> GenreList = query.ToList();

            //Add in choice for not selecting a Genre
            //Genre NoChoice = new Genre() { GenreID = 0, GenreName = "All Genres" };
            //GenreList.Add(NoChoice);

            //convert to multiselect
            MultiSelectList AllGenres = new MultiSelectList(GenreList, "GenreID", "GenreName");

            return AllGenres;
        }

        public MultiSelectList GetAllAlbums(Artist @artist)
        {
            var query = from g in db.Albums
                        orderby g.AlbumName
                        select g;

            //convert to list
            List<Album> AlbumList = query.ToList();


            //convert to Selectlist
            MultiSelectList AllAlbums = new MultiSelectList(AlbumList, "AlbumID", "AlbumName");

            return AllAlbums;
        }

        public MultiSelectList GetAllSongs(Artist @artist)
        {
            var query = from g in db.Songs
                        orderby g.SongName
                        select g;

            //convert to list
            List<Song> SongList = query.ToList();

            //convert to multiselect
            MultiSelectList AllSongs = new MultiSelectList(SongList, "SongID", "SongName");

            return AllSongs;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
