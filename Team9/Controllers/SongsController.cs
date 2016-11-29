﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Team9.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace Team9.Controllers
{
    public class SongsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public bool hasSongs(int? id)
        {
            Purchase CurrentPurchase = db.Purchases.Find(id);
            if(CurrentPurchase.PurchaseItems.Count() == 0)
            {
                return false;
            }
            foreach (PurchaseItem pi in CurrentPurchase.PurchaseItems)
            {
                if (!pi.isAlbum)
                {
                    return true;
                }
            }
            return false;
        }

        public bool hasPurchased(int id)
        {
            String CurrentUserId = User.Identity.GetUserId();
            var query = from p in db.Purchases
                        join pi in db.PurchaseItems on p.PurchaseID equals pi.Purchase.PurchaseID
                        where p.isPurchased == false && p.PurchaseUser.Id == CurrentUserId
                        select p;

            List<Purchase> PurchaseList = query.ToList();
            Purchase currentPurchase = PurchaseList[0];
            foreach(PurchaseItem pi in currentPurchase.PurchaseItems)
            {
                if (!pi.isAlbum)
                {
                    if(pi.PurchaseItemSong.SongID == id)
                    {
                        return true;
                    }
                }
                else
                {
                    foreach(Song s in pi.PurchaseItemAlbum.Songs)
                    {
                        if(s.SongID == id)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public Decimal getAverageRating(int? id)
        {
            Decimal count = 0;
            Decimal total = 0;
            Decimal average;

            Song song = db.Songs.Find(id);
            foreach(Rating r in song.SongRatings)
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

        // GET: Songs
        public ActionResult Index(string SongString)
        {
            var query = from a in db.Songs
                        select a;

            // Create a list of selected albums
            List<Song> SelectedSongs = new List<Song>();

            //Create a view bag to store the number of selected albums
            ViewBag.TotalSongCount = db.Songs.Count();

            //Create selected count of customers
            ViewBag.SelectedSongCount = db.Songs.Count();

            if (SongString == null || SongString == "") // they didn't select anything
            {
                SelectedSongs = db.Songs.ToList();

            }
            else //they picked something
            {
                //use linq to display searched names
                SelectedSongs = db.Songs.Where(a => a.SongName.Contains(SongString) || a.SongArtist.Any(r => r.ArtistName == SongString)).ToList();

                //Create selected count of customers
                ViewBag.SelectedSongCount = SelectedSongs.Count();

                //order the record to display sorted by lastname, first name, average sales
                SelectedSongs.OrderBy(a => a.SongName).ThenBy(a => a.SongPrice);
            }

            List<SongIndexViewModel> SongsDisplay = new List<SongIndexViewModel>();

            foreach (Song a in SelectedSongs)
            {
                SongIndexViewModel AVM = new SongIndexViewModel();

                AVM.Song = a;

                AVM.SongRating = getAverageRating(a.SongID);

                SongsDisplay.Add(AVM);

            }

            return View(SongsDisplay);
        }

        // GET: Songs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            ViewBag.AverageSongRating = getAverageRating(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
        }

        // POST: AddToCart
        //[HttpPost, ActionName("addToCart")]
        //[ValidateAntiForgeryToken]
        //TODO: Add role validation
        public ActionResult addSongToCart(int id)
        {
            String CurrentUserId = User.Identity.GetUserId();
            var query = from p in db.Purchases
                        where p.isPurchased == false && p.PurchaseUser.Id == CurrentUserId
                        select p;

            Purchase NewPurchase = new Purchase();
            Song song = db.Songs.Find(id);
            List<Purchase> PurchaseList = new List<Purchase>();
            PurchaseItem newItem = new PurchaseItem();
            PurchaseList = query.ToList();

            //Check if theyve purchased before

            if (PurchaseList.Count() == 1)
            {
                NewPurchase = PurchaseList[0];

                //TODOXX: IF for discounted price
                //newItem.PurchaseItemPrice = song.SongPrice;
                if (hasSongs(NewPurchase.PurchaseID))
                {
                        if (hasPurchased(id))
                        {
                            //TODO: Add error Message?
                        }
                        else
                        {
                            if (!song.isDiscoutned)
                            {
                                newItem.PurchaseItemPrice = song.SongPrice;
                            }
                            else
                            {
                                newItem.PurchaseItemPrice = song.DiscountPrice;
                            }
                            newItem.PurchaseItemSong = song;
                            newItem.Purchase = NewPurchase;
                            db.PurchaseItems.Add(newItem);
                            db.SaveChanges();
                        }
                }
                else
                {
                    if (!song.isDiscoutned)
                    {
                        newItem.PurchaseItemPrice = song.SongPrice;
                    }
                    else
                    {
                        newItem.PurchaseItemPrice = song.DiscountPrice;
                    }
                    newItem.PurchaseItemSong = song;
                    newItem.Purchase = NewPurchase;
                    db.PurchaseItems.Add(newItem);
                    db.SaveChanges();
                }
            }
            else
            {
                NewPurchase.PurchaseUser = db.Users.Find(CurrentUserId);
                NewPurchase.isPurchased = false;
                db.Purchases.Add(NewPurchase);
                db.SaveChanges();
                PurchaseList = query.ToList();
                NewPurchase = PurchaseList[0];

                //TODOXX: IF for discounted price
                if (!song.isDiscoutned)
                {
                    newItem.PurchaseItemPrice = song.SongPrice;
                }
                else
                {
                    newItem.PurchaseItemPrice = song.DiscountPrice;
                }
                newItem.PurchaseItemSong = song;
                newItem.Purchase = NewPurchase;
                db.PurchaseItems.Add(newItem);
                db.SaveChanges();
            }
            
                return RedirectToAction("Index", "Purchases");
            
        }

        // GET: Songs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SongID,SongName,SongPrice,SongLength")] Song song)
        {
            if (ModelState.IsValid)
            {
                db.Songs.Add(song);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(song);
        }

        // GET: Songs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }

            ViewBag.AllAlbums = GetAllAlbums(@song);

            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SongID,SongName,SongPrice,SongLength")] Song song, int SelectedAlbums)
        {
            if (ModelState.IsValid)

            {
                //    //find associated member
                //    Song SongToChange = db.Songs.Find(@song.SongID);

                //    SongToChange.SongAlbum.Clear();

                //    //if there are events to add then add them
                //    if (SelectedAlbums != null)
                //    {
                //            Album AlbumToAdd = db.Albums.Find(@song.SongAlbum.AlbumID);
                //            SongToChange.SongAlbum.Add(AlbumToAdd); 
                //    }

                //    db.Entry(song).State = EntityState.Modified;
                //    db.SaveChanges();
                //    return RedirectToAction("Index");
                //}

                //find associated member
                Song songToChange = db.Songs.Find(@song.SongID);
                Album albumToAdd = db.Albums.Find(@song.SongID);
                songToChange.SongAlbum = (albumToAdd);
                songToChange.SongName = song.SongName;
                songToChange.SongPrice = song.SongPrice;
                songToChange.SongLength = song.SongLength;
                songToChange.SongRatings = song.SongRatings;
                songToChange.SongGenre = song.SongGenre;

                db.Entry(songToChange).State = EntityState.Modified;
                db.SaveChanges();

            }
            return View(song);
        }

        // GET: Songs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Song song = db.Songs.Find(id);
            db.Songs.Remove(song);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Songs/ReviewSong/5
        public ActionResult ReviewSong(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Song song = db.Songs.Find(id);
            Rating NewRating = new Rating();
            NewRating.RatingSong = song;
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(NewRating);
        }
        // POST: Songs/ReviewSong/5
        //######################################################//
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReviewSong([Bind(Include = "RatingID,RatingText,RatingValue,RatingSong_SongID")]
                                        Rating rating, int? id)
        {
            if (ModelState.IsValid)
            {
                // get user id
                //TODO: fix this so that it actually gets the user id
                AppUser user = db.Users.Find(User.Identity.GetUserId());
                rating.User = user;
                // get song id
                rating.RatingSong = db.Songs.Find(id);
                db.Ratings.Add(rating);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rating);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult SongDetailedSearch()
        {
            ViewBag.SelectedGenre = GetAllGenres();
            return View();
        }

        public ActionResult SongSearchResults(string SongSearchString, string RatingString, SortOrder SelectedBounds, int[] SelectedGenre)
        {
            var query = from a in db.Songs
                        select a;



            if (SongSearchString == null || SongSearchString == "") //they didn't select anything
            {
                ViewBag.SongSearchString = "Search String was null";
            }
            else //they picked something up
            {
                ViewBag.SongSearchString = "The search string is" + SongSearchString;
                query = query.Where(a => a.SongName.Contains(SongSearchString) || a.SongArtist.Any(r => r.ArtistName == SongSearchString));
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
                    query = query.Where(s => s.SongGenre.Any(g => g.GenreID == GenreID));
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
                    return View("SongDetailedSearch");
                }


                List<SongIndexViewModel> SongsDisplay_descend = new List<SongIndexViewModel>();
                List<SongIndexViewModel> SongsDisplay_ascend = new List<SongIndexViewModel>();
                foreach (Song a in query)
                {
                    Decimal d = getAverageRating(a.SongID);
                    if (d >= decRating)
                    {
                        SongIndexViewModel ab = new SongIndexViewModel();
                        ab.Song = a;
                        ab.SongRating = d;
                        SongsDisplay_ascend.Add(ab);
                    }
                    else
                    {
                        SongIndexViewModel ab = new SongIndexViewModel();
                        ab.Song = a;
                        ab.SongRating = d;
                        SongsDisplay_descend.Add(ab);
                    }
                }
                IEnumerable<SongIndexViewModel> new_list_songs = SongsDisplay_ascend;
                IEnumerable<SongIndexViewModel> new_list_songs_lt = SongsDisplay_descend;



                if (SelectedBounds == SortOrder.ascending)
                {
                    ViewBag.SelectedSortOrder = "The records should be sorted in ascending order";
                    return View("Index", new_list_songs);
                }
                else
                {
                    ViewBag.SelectedSortOrder = "The records should be sored in descending order";
                    return View("Index", new_list_songs_lt);
                }
            }


            List<SongIndexViewModel> SongsList = new List<SongIndexViewModel>();
            foreach (Song a in query)
            {
                Decimal d = getAverageRating(a.SongID);
                SongIndexViewModel ab = new SongIndexViewModel();
                ab.Song = a;
                ab.SongRating = d;
                SongsList.Add(ab);
            }

            return View("Index", SongsList);

        }

        public MultiSelectList GetAllGenres()
        {
            var query = from g in db.Genres
                        orderby g.GenreName
                        select g;

            //convert to list
            List<Genre> GenreList = query.ToList();

            //Add in choice for not selecting a frequency
            Genre NoChoice = new Genre() { GenreID = 0, GenreName = "All Genres" };
            GenreList.Add(NoChoice);

            //convert to multiselect
            MultiSelectList AllGenres = new MultiSelectList(GenreList.OrderBy(g => g.GenreName), "GenreID", "GenreName");

            return AllGenres;
        }

        public SelectList GetAllAlbums(Song @song)
        {
            var query = from g in db.Albums
                        orderby g.AlbumName
                        select g;

            //convert to list
            List<Album> AlbumList = query.ToList();


            //convert to multiselect
            SelectList AllAlbums = new SelectList(AlbumList.OrderBy(g => g.AlbumName), "AlbumID", "AlbumName");

            return AllAlbums;
        }

    }

}
