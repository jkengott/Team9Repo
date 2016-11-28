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
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SongID,SongName,SongPrice,SongLength")] Song song)
        {
            if (ModelState.IsValid)
            {
                db.Entry(song).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
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
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
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
