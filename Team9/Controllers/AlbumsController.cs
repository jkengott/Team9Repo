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

    public class AlbumsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public bool hasAlbums(int? id)
        {
            Purchase CurrentPurchase = db.Purchases.Find(id);
            foreach(PurchaseItem pi in CurrentPurchase.PurchaseItems)
            {
                if(pi.isAlbum)
                {
                    return true;
                }
            }
            return false;
        }

        public void checkDuplicates(int? id)
        {
            String CurrentUserId = User.Identity.GetUserId();
            var query = from p in db.Purchases
                        where p.isPurchased == false && p.PurchaseUser.Id == CurrentUserId
                        select p;

            Album currentAlbum = db.Albums.Find(id);
            List<Purchase> CartList = query.ToList();
            Purchase currentCart = CartList[0];
            foreach(PurchaseItem pi in currentCart.PurchaseItems)
            {
                if (!pi.isAlbum)
                {
                    if (currentAlbum.Songs.Contains(pi.PurchaseItemSong))
                    {
                        db.PurchaseItems.Remove(pi);
                        db.SaveChanges();
                    }

                }
            }

        }

        public bool hasPurchased(int id)
        {
            String CurrentUserId = User.Identity.GetUserId();

            var query2 = from p in db.Purchases
                         join pi in db.PurchaseItems on p.PurchaseID equals pi.Purchase.PurchaseID
                         where p.isPurchased == false && p.PurchaseUser.Id == CurrentUserId
                         select pi.PurchaseItemAlbum.AlbumID;

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
                return View(db.Albums.ToList());
            }
            else //they picked something
            {
                //use linq to display searched names
                SelectedAlbums = db.Albums.Where(a => a.AlbumName.Contains(AlbumString) || a.AlbumArtist.Any(r => r.ArtistName == AlbumString)).ToList();

                //Create selected count of customers
                ViewBag.SelectedAlbumCount = SelectedAlbums.Count();

                //order the record to display sorted by lastname, first name, average sales
                SelectedAlbums.OrderBy(a => a.AlbumName).ThenBy(a => a.AlbumPrice);
                return View(SelectedAlbums);
            }

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

                //TODOXX: IF for discounted price
                //newItem.PurchaseItemPrice = song.SongPrice;
                //foreach (Song s in album.Songs)
                //{
                if (hasAlbums(NewPurchase.PurchaseID))
                {
                    if (hasPurchased(album.AlbumID))
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
                        //checkDuplicates(album.AlbumID);
                        newItem.PurchaseItemAlbum = album;
                        newItem.isAlbum = true;
                        newItem.Purchase = NewPurchase;
                        db.PurchaseItems.Add(newItem);
                        db.SaveChanges();
                    }
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
                    //checkDuplicates(album.AlbumID);
                    newItem.PurchaseItemAlbum = album;
                    newItem.isAlbum = true;
                    newItem.Purchase = NewPurchase;
                    db.PurchaseItems.Add(newItem);
                    db.SaveChanges();
                }
            }

            //}
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
                newItem.isAlbum = true;
                db.PurchaseItems.Add(newItem);
                db.SaveChanges();
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
            return View();
        }

    }
}
