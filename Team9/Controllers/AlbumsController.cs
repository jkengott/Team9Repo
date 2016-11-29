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
    public enum SortOrder { ascending, descending }

    public class AlbumsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public bool hasPurchased(int id)
        {
            String CurrentUserId = User.Identity.GetUserId();
            //var query = from p in db.Purchases
            //            join pi in db.PurchaseItems on p.PurchaseID equals pi.Purchase.PurchaseID
            //            where p.isPurchased == false && p.PurchaseUser.Id == CurrentUserId
            //            select pi.PurchaseItemSong.SongID;

            var query2 = from p in db.Purchases
                         join pi in db.PurchaseItems on p.PurchaseID equals pi.Purchase.PurchaseID
                         where p.isPurchased == false && p.PurchaseUser.Id == CurrentUserId
                         select pi.PurchaseItemAlbum.AlbumID;

            //List<Int32> SongIDs = query.ToList();
            List<Int32> AlbumIDs = query2.ToList();
            //List<Int32> AlbumSongs = new List<Int32>();
            if (AlbumIDs.Contains(id))
            {
                return true;
            }
            return false;
        }

        public Decimal getAverageRating(int? id)
        {
            Decimal count = 0;
            Decimal total = 0;
            Decimal average;

            Album Album = db.Albums.Find(id);
            foreach (Rating r in Album.AlbumRatings)
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

        // GET: Albums
        public ActionResult Index(string AlbumString)
        {

            var query = from a in db.Albums
                        select a;

            // Create a list of selected albums
            List<Album> SelectedAlbums = new List<Album>();

            //Create a view bag to store the number of selected albums
            ViewBag.TotalAlbumCount = db.Albums.Count();

            //Create selected count of customers
            ViewBag.SelectedAlbumCount = db.Albums.Count();

            if (AlbumString == null || AlbumString == "") // they didn't select anything
            {
                SelectedAlbums = db.Albums.ToList();

            }
            else //they picked something
            {
                //use linq to display searched names
                SelectedAlbums = db.Albums.Where(a => a.AlbumName.Contains(AlbumString) || a.AlbumArtist.Any(r => r.ArtistName == AlbumString)).ToList();

                //Create selected count of customers
                ViewBag.SelectedAlbumCount = SelectedAlbums.Count();

                //order the record to display sorted by lastname, first name, average sales
                SelectedAlbums.OrderBy(a => a.AlbumName).ThenBy(a => a.AlbumPrice);
            }

            List<AlbumIndexViewModel> AlbumsDisplay = new List<AlbumIndexViewModel>();

            foreach (Album a in SelectedAlbums)
            {
                AlbumIndexViewModel AVM = new AlbumIndexViewModel();

                AVM.Album = a;

                AVM.AlbumRating = getAverageRating(a.AlbumID);

                AlbumsDisplay.Add(AVM);

            }
            return View(AlbumsDisplay);


        }


        // GET: Albums/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            ViewBag.AverageAlbumRating = getAverageRating(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }

        //GET: Add to cart
        // TODO: set up role requirements
        public ActionResult addAlbumToCart(int id)
        {
            String CurrentUserId = User.Identity.GetUserId();
            var query = from p in db.Purchases
                        where p.isPurchased == false && p.PurchaseUser.Id == CurrentUserId
                        select p;

            Purchase NewPurchase = new Purchase();
            Album album = db.Albums.Find(id);
            List<Purchase> PurchaseList = new List<Purchase>();
            PurchaseList = query.ToList();
            if (PurchaseList.Count() == 1)
            {
                NewPurchase = PurchaseList[0];
                Int32 AlbumCount = 0;
                foreach(PurchaseItem pi in NewPurchase.PurchaseItems)
                {
                    if (pi.isAlbum)
                    {
                        AlbumCount += 1;
                    }
                }


                //TODOXX: IF for discounted price
                //newItem.PurchaseItemPrice = song.SongPrice;
                //foreach (Song s in album.Songs)
                //{
                if (AlbumCount == 0)
                {
                    PurchaseItem newItem = new PurchaseItem();
                    //Check if there is a discount price
                    if (!album.isDiscounted)
                    {
                        newItem.PurchaseItemPrice = album.AlbumPrice;
                    }
                    else
                    {
                        newItem.PurchaseItemPrice = album.DiscountAlbumPrice;
                    }
                    newItem.PurchaseItemAlbum = album;
                    newItem.Purchase = NewPurchase;
                    newItem.isAlbum = true;
                    db.PurchaseItems.Add(newItem);
                    db.SaveChanges();
                }
                else if (hasPurchased(album.AlbumID) )
                {
                    //continue;
                    //TODO:Error message to not add song?
                    // use a next to add all other songs that have not been added?
                }
                else
                {
                    PurchaseItem newItem = new PurchaseItem();
                    //Check if there is a discount price
                    if (!album.isDiscounted)
                    {
                        newItem.PurchaseItemPrice = album.AlbumPrice;
                    }
                    else
                    {
                        newItem.PurchaseItemPrice = album.DiscountAlbumPrice;
                    }
                    newItem.PurchaseItemAlbum = album;
                    newItem.Purchase = NewPurchase;
                    newItem.isAlbum = true;
                    db.PurchaseItems.Add(newItem);
                    db.SaveChanges();

                }
                //}
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

                //foreach (Song s in album.Songs)
                //{
                if (hasPurchased(album.AlbumID))
                {
                    //TODO:Error message to not add song?
                    // use a next to add all other songs that have not been added?
                }
                else
                {
                    PurchaseItem newItem = new PurchaseItem();
                    //Check if discount price is null
                    if (!album.isDiscounted)
                    {
                        newItem.PurchaseItemPrice = album.AlbumPrice;
                    }
                    else
                    {
                        newItem.PurchaseItemPrice = album.DiscountAlbumPrice;
                    }
                    newItem.PurchaseItemAlbum = album;
                    newItem.Purchase = NewPurchase;
                    newItem.isAlbum = false;
                    db.PurchaseItems.Add(newItem);
                    db.SaveChanges();
                }
                //}
            }
            return RedirectToAction("Index", "Purchases");
        }


        // GET: Albums/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AlbumID,AlbumName,AlbumPrice")] Album album)
        {
            if (ModelState.IsValid)
            {
                db.Albums.Add(album);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(album);
        }

        // GET: Albums/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AlbumID,AlbumName,AlbumPrice")] Album album)
        {
            if (ModelState.IsValid)
            {
                db.Entry(album).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(album);
        }

        // GET: Albums/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Album album = db.Albums.Find(id);
            db.Albums.Remove(album);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Album/ReviewArtist/5
        public ActionResult ReviewAlbum(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult AlbumDetailedSearch()
        {
            ViewBag.SelectedGenre = GetAllGenres();
            return View();
        }

        public ActionResult AlbumSearchResults(string AlbumSearchString, string RatingString, SortOrder SelectedBounds, int[] SelectedGenre)
        {
            var query = from a in db.Albums
                        select a;



            if (AlbumSearchString == null || AlbumSearchString == "") //they didn't select anything
            {
                ViewBag.AlbumSearchString = "Search String was null";
            }
            else //they picked something up
            {
                ViewBag.AlbumSearchString = "The search string is" + AlbumSearchString;
                query = query.Where(a => a.AlbumName.Contains(AlbumSearchString) || a.AlbumArtist.Any(r => r.ArtistName == AlbumSearchString));
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
                    query = query.Where(s => s.AlbumGenre.Any(g => g.GenreID == GenreID));
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
                    return View("AlbumDetailedSearch");
                }


                List<AlbumIndexViewModel> AlbumsDisplay_descend = new List<AlbumIndexViewModel>();
                List<AlbumIndexViewModel> AlbumsDisplay_ascend = new List<AlbumIndexViewModel>();
                foreach (Album a in query)
                {
                    Decimal d = getAverageRating(a.AlbumID);
                    if (d >= decRating)
                    {
                        AlbumIndexViewModel ab = new AlbumIndexViewModel();
                        ab.Album = a;
                        ab.AlbumRating = d;
                        AlbumsDisplay_ascend.Add(ab);
                    }
                    else
                    {
                        AlbumIndexViewModel ab = new AlbumIndexViewModel();
                        ab.Album = a;
                        ab.AlbumRating = d;
                        AlbumsDisplay_descend.Add(ab);
                    }
                }
                IEnumerable<AlbumIndexViewModel> new_list_albums = AlbumsDisplay_ascend;
                IEnumerable<AlbumIndexViewModel> new_list_albums_lt = AlbumsDisplay_descend;



                if (SelectedBounds == SortOrder.ascending)
                {
                    ViewBag.SelectedSortOrder = "The records should be sorted in ascending order";
                    return View("Index", new_list_albums);
                }
                else
                {
                    ViewBag.SelecredSortOrder = "The records should be sored in descending order";
                    return View("Index", new_list_albums_lt);
                }
            }


            List<AlbumIndexViewModel> AlbumsList = new List<AlbumIndexViewModel>();
            foreach (Album a in query)
            {
                Decimal d = getAverageRating(a.AlbumID);
                AlbumIndexViewModel ab = new AlbumIndexViewModel();
                ab.Album = a;
                ab.AlbumRating = d;
                AlbumsList.Add(ab);
            }

            return View("Index", AlbumsList);


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
    }
}

