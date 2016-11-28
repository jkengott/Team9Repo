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
    public class PurchasesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        public bool checkDuplicates(List<Album> Albums)
        {
            String CurrentUserId = User.Identity.GetUserId();
            var query = from p in db.Purchases
                        where p.isPurchased == false && p.PurchaseUser.Id == CurrentUserId
                        select p;
            List<Purchase> ActiveCartList = query.ToList();
            Purchase ActiveCart = ActiveCartList[0];
            List<Song> AlbumSongs = new List<Song>();
            foreach(Album a in Albums)
            {
                foreach(Song s in a.Songs)
                {
                    AlbumSongs.Add(s);
                }
            }
            foreach(PurchaseItem pi in ActiveCart.PurchaseItems)
            {
                if (!pi.isAlbum)
                {
                    if (AlbumSongs.Contains(pi.PurchaseItemSong))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // GET: Purchases
        public ActionResult Index()
        {
            String CurrentUserId = User.Identity.GetUserId();
            var query = from p in db.Purchases
                        where p.isPurchased == false && p.PurchaseUser.Id == CurrentUserId
                        select p;

            List<Purchase> ActiveCartList = query.ToList();
            if (ActiveCartList.Count() == 1 )
            {
                Purchase ActiveCartPurchase = new Purchase();
                ActiveCartPurchase = ActiveCartList[0];
                List<Album> Albums = new List<Album>();
                foreach(PurchaseItem pi in ActiveCartPurchase.PurchaseItems)
                {
                    if (pi.isAlbum)
                    {
                        Albums.Add(pi.PurchaseItemAlbum);
                    }
                }
                if (checkDuplicates(Albums))
                {
                    ViewBag.DuplicateMessage = "Remove duplicate items before checking out";
                }
                //CalcSubtotals
                decimal subtotal = 0;
                decimal discountSubtotal = -1;
                decimal taxTotal = 0;
                decimal tax = .0825m;
                decimal grandTotal = 0;
                foreach (PurchaseItem pi in ActiveCartPurchase.PurchaseItems)
                {
                    if (pi.isAlbum)
                    {
                        subtotal += pi.PurchaseItemAlbum.AlbumPrice;
                        if (pi.PurchaseItemAlbum.isDiscounted)
                        {
                            discountSubtotal = pi.PurchaseItemAlbum.DiscountAlbumPrice;
                        }
                        else
                        {
                            discountSubtotal = pi.PurchaseItemAlbum.AlbumPrice;
                        }
                    }
                    else
                    {
                        subtotal += pi.PurchaseItemSong.SongPrice;
                        if (pi.PurchaseItemSong.isDiscoutned)
                        {
                            discountSubtotal = pi.PurchaseItemSong.DiscountPrice;
                        }
                        else
                        {
                            discountSubtotal = pi.PurchaseItemSong.SongPrice;
                        }
                    }
                }
                if (discountSubtotal > 0)
                {
                    taxTotal = tax * discountSubtotal;
                    grandTotal = discountSubtotal + taxTotal;
                }
                else
                {
                    taxTotal = tax * subtotal;
                    grandTotal = subtotal + taxTotal;
                }
                ViewBag.subtotal = subtotal.ToString("c");
                ViewBag.discountSubtotal = discountSubtotal.ToString("c");
                ViewBag.taxTotal = taxTotal.ToString("c");
                ViewBag.GrandTotal = grandTotal.ToString("c");
                //End Calc Subtotals
                return View(ActiveCartPurchase.PurchaseItems.ToList());
            }
            else
            {
                return View(db.PurchaseItems.ToList());
            }
        }

        // GET: Purchases/Details/5
        public ActionResult Details(int? id)
        {
            //STARTS HERE
            //ENDS HERE
            String CurrentUserId = User.Identity.GetUserId();
            Purchase ActiveCartPurchase = db.Purchases.Find(id);
            List<Album> Albums = new List<Album>();
                
                foreach (PurchaseItem pi in ActiveCartPurchase.PurchaseItems)
                {
                    if (pi.isAlbum)
                    {
                        Albums.Add(pi.PurchaseItemAlbum);
                    }
                } 
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Purchase purchase = db.Purchases.Find(id);
                if (purchase == null)
                {
                    return HttpNotFound();
                }
                if (checkDuplicates(Albums))
                {
                    return RedirectToAction("Index");
                }
            else
            {
                //CalcSubtotals
                decimal subtotal = 0;
                decimal discountSubtotal = -1;
                decimal taxTotal = 0;
                decimal tax = .0825m;
                decimal grandTotal = 0;
                foreach (PurchaseItem pi in ActiveCartPurchase.PurchaseItems)
                {
                    if (pi.isAlbum)
                    {
                        subtotal += pi.PurchaseItemAlbum.AlbumPrice;
                        if (pi.PurchaseItemAlbum.isDiscounted)
                        {
                            discountSubtotal = pi.PurchaseItemAlbum.DiscountAlbumPrice;
                        }
                        else
                        {
                            discountSubtotal = pi.PurchaseItemAlbum.AlbumPrice;
                        }
                    }
                    else
                    {
                        subtotal += pi.PurchaseItemSong.SongPrice;
                        if (pi.PurchaseItemSong.isDiscoutned)
                        {
                            discountSubtotal = pi.PurchaseItemSong.DiscountPrice;
                        }
                        else
                        {
                            discountSubtotal = pi.PurchaseItemSong.SongPrice;
                        }
                    }
                }
                if (discountSubtotal > 0)
                {
                    taxTotal = tax * discountSubtotal;
                    grandTotal = discountSubtotal + taxTotal;
                }
                else
                {
                    taxTotal = tax * subtotal;
                    grandTotal = subtotal + taxTotal;
                }
                ViewBag.subtotal = subtotal.ToString("c");
                ViewBag.discountSubtotal = discountSubtotal.ToString("c");
                ViewBag.taxTotal = taxTotal.ToString("c");
                ViewBag.GrandTotal = grandTotal.ToString("c");
                //End Calc Subtotals
                List<String> CreditCards = new List<String>();
                CreditCards.Add("Use new card (enter below)");
                int CC1index = ActiveCartPurchase.PurchaseUser.CreditCard1.Length - 4;
                String CC1 = ActiveCartPurchase.PurchaseUser.CreditCard1.Substring(CC1index, 4);
                String CC1Type = ActiveCartPurchase.PurchaseUser.CCType1.ToString();
                CC1 = "********" + CC1 + " " + CC1Type;
                CreditCards.Add(CC1);
                if(!String.IsNullOrEmpty(ActiveCartPurchase.PurchaseUser.CreditCard2))
                {
                    int CC2index = ActiveCartPurchase.PurchaseUser.CreditCard1.Length - 4;
                    String CC2 = ActiveCartPurchase.PurchaseUser.CreditCard2.Substring(CC2index, 4);
                    String CC2Type = ActiveCartPurchase.PurchaseUser.CCType2.ToString();
                    CC2 = "**********" + CC2 + " " + CC2Type;
                    CreditCards.Add(CC2);
                }
                SelectList SelectCreditCards = new SelectList(CreditCards, "CreditCardId");
                ViewBag.selectCreditCards = SelectCreditCards;
                return View("Details", purchase);
            }
        }

        // GET: Purchases/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Purchases/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PurchaseID,isPurchased,PurchaseDate,isGift")] Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                db.Purchases.Add(purchase);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(purchase);
        }

        // GET: Purchases/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            return View(purchase);
        }

        // POST: Purchases/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PurchaseID,isPurchased,PurchaseDate,isGift")] Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchase).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(purchase);
        }

        // GET: Purchases/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseItem purchaseItem = db.PurchaseItems.Find(id);
            if (purchaseItem == null)
            {
                return HttpNotFound();
            }
            return View(purchaseItem);
        }

        // POST: Purchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PurchaseItem purchaseitem = db.PurchaseItems.Find(id);
            db.PurchaseItems.Remove(purchaseitem);
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
    }
}
