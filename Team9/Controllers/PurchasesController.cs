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

        public PurchaseViewModel calcPVM (Purchase p)
        {
            decimal subtotal = 0;
            decimal discountSubtotal = -1;
            decimal taxTotal = 0;
            decimal tax = .0825m;
            decimal grandTotal = 0;
            foreach (PurchaseItem pi in p.PurchaseItems)
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
            PurchaseViewModel PVM = new PurchaseViewModel();
            PVM.PurchaseID = p.PurchaseID;
            PVM.subtotal = subtotal.ToString("c");
            PVM.discountTotal = discountSubtotal.ToString("c");
            PVM.taxTotal = taxTotal.ToString("c");
            PVM.grandTotal = grandTotal.ToString("c");
            return PVM;
        }

        public void getCards(AppUser u)
        {
            List<CreditCard> userCards = new List<CreditCard>();
            if (!u.CC1.Equals(null))
            {
                userCards.Add(u.CC1);
            }
            if (!u.CC2.Equals(null))
            {
                userCards.Add(u.CC2);
            }
            //create list
            SelectList list = new SelectList(userCards, "CreditCardID", "displayNumber");
            ViewBag.AllCards = list;
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

        public Decimal getAverageAlbumRating(int? id)
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
                bool hasDiscount = false;
                decimal taxTotal = 0;
                decimal tax = .0825m;
                decimal grandTotal = 0;
                foreach (PurchaseItem pi in ActiveCartPurchase.PurchaseItems)
                {
                    if (pi.Equals(null))
                    {
                        continue;
                    }
                    else if (pi.isAlbum)
                    {
                        subtotal += pi.PurchaseItemAlbum.AlbumPrice;
                        if (pi.PurchaseItemAlbum.isDiscounted)
                        {
                            discountSubtotal += pi.PurchaseItemAlbum.DiscountAlbumPrice;
                            hasDiscount = true;
                        }
                        else
                        {
                            discountSubtotal += pi.PurchaseItemAlbum.AlbumPrice;
                        }
                    }
                    else
                    {
                        subtotal += pi.PurchaseItemSong.SongPrice;
                        if (pi.PurchaseItemSong.isDiscoutned)
                        {
                            discountSubtotal += pi.PurchaseItemSong.DiscountPrice;
                            hasDiscount = true;
                        }
                        else
                        {
                            discountSubtotal += pi.PurchaseItemSong.SongPrice;
                        }
                    }
                }
                if (hasDiscount)
                {
                    discountSubtotal += 1;
                    taxTotal = tax * discountSubtotal;
                    grandTotal = discountSubtotal + taxTotal;
                }
                else
                {
                    discountSubtotal = -1;
                    taxTotal = tax * subtotal;
                    grandTotal = subtotal + taxTotal;
                }
                ViewBag.hasDiscount = hasDiscount;
                ViewBag.subtotal = subtotal.ToString("c");
                ViewBag.discountSubtotal = discountSubtotal.ToString("c");
                ViewBag.taxTotal = taxTotal.ToString("c");
                ViewBag.GrandTotal = grandTotal.ToString("c");
                //End Calc Subtotals
                List<PurchaseItemViewModel> PIDisplay = new List<PurchaseItemViewModel>();
                List<PurchaseItem> currentPurchaseItems = ActiveCartPurchase.PurchaseItems.ToList();
                foreach(PurchaseItem pi in currentPurchaseItems)
                {
                    if (pi.isAlbum)
                    {
                        PurchaseItemViewModel PIVM = new PurchaseItemViewModel();
                        PIVM.PurchaseItem = pi;
                        PIVM.PurchaseItemRating = getAverageAlbumRating(pi.PurchaseItemAlbum.AlbumID);
                        PIDisplay.Add(PIVM);
                    }
                    else
                    {
                        PurchaseItemViewModel PIVM = new PurchaseItemViewModel();
                        PIVM.PurchaseItem = pi;
                        PIVM.PurchaseItemRating = getAverageSongRating(pi.PurchaseItemSong.SongID);
                        PIDisplay.Add(PIVM);
                    }
                }


                return View(PIDisplay);
            }
            else
            {
                Purchase newPurchase = new Purchase();
                newPurchase.PurchaseUser = db.Users.Find(CurrentUserId);
                db.Purchases.Add(newPurchase);
                db.SaveChanges();
                var query2 = from p in db.Purchases
                             where p.isPurchased == false && p.PurchaseUser.Id == CurrentUserId
                             select p;
                List<Purchase> newPurchaseList = query2.ToList();
                Purchase dbNewPurchase = newPurchaseList[0];
                //ViewModel Code Start
                List<PurchaseItemViewModel> PIDisplay = new List<PurchaseItemViewModel>();
                ViewBag.subtotal = 0.ToString("c");
                ViewBag.discountSubtotal = 0.ToString("c");
                ViewBag.taxTotal = 0.ToString("c");
                ViewBag.GrandTotal = 0.ToString("c");
                ViewBag.hasDiscount = false;
                return View(PIDisplay);
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
                PurchaseViewModel PVM = calcPVM(ActiveCartPurchase);
                //End Calc Subtotals


                //create list and execute query
                AppUser CurrentUser = db.Users.Find(CurrentUserId);
                //get Cards
                getCards(CurrentUser);
                return View("Details", PVM);
            }
        }

        //POST for Purchase
        [HttpPost]
        public ActionResult Details(PurchaseViewModel Purchase, Int32 CreditCardID, bool newCard, string newCardNumber)
        {

            if (ModelState.IsValid)
            {
                Purchase currentPurchase = db.Purchases.Find(Purchase.PurchaseID);
                foreach(PurchaseItem pi in currentPurchase.PurchaseItems)
                {
                    if (pi.isAlbum)
                    {
                        if (pi.PurchaseItemAlbum.isDiscounted)
                        {
                            pi.PurchaseItemPrice = pi.PurchaseItemAlbum.DiscountAlbumPrice;
                        }
                        else
                        {
                            pi.PurchaseItemPrice = pi.PurchaseItemAlbum.AlbumPrice;
                        }
                    }
                    else
                    {
                        if (pi.PurchaseItemSong.isDiscoutned)
                        {
                            pi.PurchaseItemPrice = pi.PurchaseItemSong.DiscountPrice;
                        }
                        else
                        {
                            pi.PurchaseItemPrice = pi.PurchaseItemSong.SongPrice;
                        }
                    }
                }
                if (newCard)
                {
                    CreditCard newCardUse = new CreditCard();
                    String CurrentUserId = User.Identity.GetUserId();
                    newCardUse.CCNumber = newCardNumber;
                    newCardUse.CardOwner = db.Users.Find(CurrentUserId);
                    db.Creditcards.Add(newCardUse);
                    db.SaveChanges();
                    var query = from c in db.Creditcards
                                where c.CCNumber == newCardNumber && c.CardOwner.Id == CurrentUserId
                                select c;
                    List<CreditCard> newCardList = query.ToList();
                    CreditCard finalNewCard = newCardList[0];
                    currentPurchase.PurchaseCard = finalNewCard;
                }
                else
                {
                    currentPurchase.PurchaseCard = db.Creditcards.Find(CreditCardID);
                }
                currentPurchase.PurchaseDate = DateTime.Now;
                currentPurchase.isPurchased = true;
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(Purchase);
        }


        // GET: Purchases/Gift/5
        public ActionResult Gift(int? id)
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
                PurchaseViewModel PVM = calcPVM(ActiveCartPurchase);
                //End Calc Subtotals


                //create list and execute query
                AppUser CurrentUser = db.Users.Find(CurrentUserId);
                getCards(CurrentUser);
                return View("Gift", PVM);
            }
        }

        //POST for Gift
        [HttpPost]
        public ActionResult Gift(PurchaseViewModel Purchase, Int32 CreditCardID, bool newCard, string newCardNumber, string giftEmail)
        {
            Purchase currentPurchase = db.Purchases.Find(Purchase.PurchaseID);
            if (ModelState.IsValid)
            {
                foreach (PurchaseItem pi in currentPurchase.PurchaseItems)
                {
                    if (pi.isAlbum)
                    {
                        if (pi.PurchaseItemAlbum.isDiscounted)
                        {
                            pi.PurchaseItemPrice = pi.PurchaseItemAlbum.DiscountAlbumPrice;
                        }
                        else
                        {
                            pi.PurchaseItemPrice = pi.PurchaseItemAlbum.AlbumPrice;
                        }
                    }
                    else
                    {
                        if (pi.PurchaseItemSong.isDiscoutned)
                        {
                            pi.PurchaseItemPrice = pi.PurchaseItemSong.DiscountPrice;
                        }
                        else
                        {
                            pi.PurchaseItemPrice = pi.PurchaseItemSong.SongPrice;
                        }
                    }
                }
                if (newCard)
                {
                    CreditCard newCardUse = new CreditCard();
                    String CurrentUserId = User.Identity.GetUserId();
                    newCardUse.CCNumber = newCardNumber;
                    newCardUse.CardOwner = db.Users.Find(CurrentUserId);
                    db.Creditcards.Add(newCardUse);
                    db.SaveChanges();
                    var query = from c in db.Creditcards
                                where c.CCNumber == newCardNumber && c.CardOwner.Id == CurrentUserId
                                select c;
                    List<CreditCard> newCardList = query.ToList();
                    CreditCard finalNewCard = newCardList[0];
                    currentPurchase.PurchaseCard = finalNewCard;
                }
                else
                {
                    currentPurchase.PurchaseCard = db.Creditcards.Find(CreditCardID);
                }
                var query2 = from u in db.Users
                             select u;
                List<AppUser> userList = query2.ToList();
                List<String> userEmails = new List<String>();
                foreach(AppUser a in userList)
                {
                    userEmails.Add(a.Email);
                }
                if (userEmails.Contains(giftEmail))
                {
                    var query3 = from u in db.Users
                                 where u.Email == giftEmail
                                 select u;
                    List<AppUser> giftUserList = query3.ToList();
                    AppUser giftUser = giftUserList[0];
                    currentPurchase.GiftUser = giftUser;
                }
                else
                {
                    PurchaseViewModel PVM = calcPVM(currentPurchase);
                    getCards(currentPurchase.PurchaseUser);
                    ViewBag.giftWarning = "Enter a valid Username";
                    return View("Gift",PVM);
                }
                currentPurchase.PurchaseDate = DateTime.Now;
                currentPurchase.isPurchased = true;
                currentPurchase.isGift = true;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PurchaseViewModel PVM2 = calcPVM(currentPurchase);
            return View("Gift", PVM2);
        }



        // GET: myMusic
        public ActionResult myMusic()
        {
            String CurrentUserId = User.Identity.GetUserId();
            var query = from p in db.Purchases
                        where p.isPurchased == true && (p.PurchaseUser.Id == CurrentUserId || p.GiftUser.Id == CurrentUserId)
                        select p;



            // Create a list of selected albums
            List<Purchase> Purchases = query.ToList();
            List<Song> mySongs = new List<Song>();
            foreach(Purchase p in Purchases)
            {
                foreach(PurchaseItem pi in p.PurchaseItems)
                {
                    if (pi.isAlbum)
                    {
                        foreach(Song s in pi.PurchaseItemAlbum.Songs)
                        {
                            mySongs.Add(s);
                        }
                    }
                    else
                    {
                        mySongs.Add(pi.PurchaseItemSong);
                    }
                }
            }

            //Create a view bag to store the number of selected albums
            ViewBag.TotalSongCount = mySongs.Count();

            List<SongIndexViewModel> SongsDisplay = new List<SongIndexViewModel>();

            foreach (Song a in mySongs)
            {
                SongIndexViewModel AVM = new SongIndexViewModel();

                AVM.Song = a;

                AVM.SongRating = getAverageSongRating(a.SongID);

                SongsDisplay.Add(AVM);

            }

            return View(SongsDisplay);
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

        // GET: Purchases
        public ActionResult orderHistory()
        {
            String CurrentUserId = User.Identity.GetUserId();
            var query = from p in db.Purchases
                        where p.isPurchased == true && p.PurchaseUser.Id == CurrentUserId
                        orderby p.PurchaseDate descending
                        select p;

            List<Purchase> PurchasedCartList = query.ToList();
            List<PurchaseItem> PurcahseItems = new List<PurchaseItem>();
            foreach(Purchase p in PurchasedCartList)
            {
                foreach(PurchaseItem pi in p.PurchaseItems)
                {
                    PurcahseItems.Add(pi);
                }
            }

                //End Calc Subtotals
                List<PurchaseItemViewModel> PIDisplay = new List<PurchaseItemViewModel>();
                foreach (PurchaseItem pi in PurcahseItems)
                {
                    if (pi.isAlbum)
                    {
                        PurchaseItemViewModel PIVM = new PurchaseItemViewModel();
                        PIVM.PurchaseItem = pi;
                        PIVM.PurchaseItemRating = getAverageAlbumRating(pi.PurchaseItemAlbum.AlbumID);
                        PIDisplay.Add(PIVM);
                    }
                    else
                    {
                        PurchaseItemViewModel PIVM = new PurchaseItemViewModel();
                        PIVM.PurchaseItem = pi;
                        PIVM.PurchaseItemRating = getAverageSongRating(pi.PurchaseItemSong.SongID);
                        PIDisplay.Add(PIVM);
                    }
                }
                return View(PIDisplay);
            }

        //GET: ReportHome
        public ActionResult ReportHome()
        {
            return View();
        }

        //GET: SongReports
        public ActionResult songReport()
        {
            var query = from s in db.Songs
                        select s;

            List<Song> allSongs = query.ToList();
            List<SongReportViewModel> songReports = new List<SongReportViewModel>();
            foreach(Song s in allSongs)
            {
                SongReportViewModel sivm = new SongReportViewModel();
                var query2 = from pi in db.PurchaseItems
                             where pi.Purchase.isPurchased == true && pi.PurchaseItemSong.SongID == s.SongID
                             select pi;
                List<PurchaseItem> songPurchaseItem = query2.ToList();
                Int32 purchaseCount = 0;
                Decimal totalRevenue = 0;
                foreach(PurchaseItem pi in songPurchaseItem)
                {
                    purchaseCount += 1;
                    totalRevenue += pi.PurchaseItemPrice;
                }
                sivm.purchaseCount = purchaseCount;
                sivm.songRevenue = totalRevenue;
                sivm.Song = s;
                songReports.Add(sivm);
            }
            return View(songReports);
        }

        public ActionResult AlbumReport()
        {
            var query = from a in db.Albums
                        select a;

            List<Album> allAlbums = query.ToList();
            List<AlbumReportViewModel> AlbumReports = new List<AlbumReportViewModel>();
            foreach (Album a in allAlbums)
            {
                AlbumReportViewModel aivm = new AlbumReportViewModel();
                var query2 = from pi in db.PurchaseItems
                             where pi.Purchase.isPurchased == true && pi.PurchaseItemAlbum.AlbumID == a.AlbumID
                             select pi;
                List<PurchaseItem> AlbumPurchaseItem = query2.ToList();
                Int32 purchaseCount = 0;
                Decimal totalRevenue = 0;
                foreach (PurchaseItem pi in AlbumPurchaseItem)
                {
                    purchaseCount += 1;
                    totalRevenue += pi.PurchaseItemPrice;
                }
                aivm.purchaseCount = purchaseCount;
                aivm.AlbumRevenue = totalRevenue;
                aivm.Album = a;
                AlbumReports.Add(aivm);
            }
            return View(AlbumReports);
        }

        public ActionResult genreReport()
        {
            var query = from g in db.Genres
                        select g;

            List<Genre> allGenres = query.ToList();
            List<GenreReportViewModel> grvmList = new List<GenreReportViewModel>();
            foreach(Genre g in allGenres)
            {
                GenreReportViewModel grvm = new GenreReportViewModel();
                grvm.totalRev = 0.ToString("c");
                grvm.songRev = 0.ToString("c");
                grvm.songCount = 0;
                grvm.albumCount = 0;
                grvm.topArtist = "N/A";
                grvm.Genre = g;
                Decimal topRevenue = 0;
                foreach(Artist a in g.GenreArtists )
                {
                    Int32 artistSongPurchaseCount = 0;
                    Int32 artistAlbumPurchaseCount = 0;
                    Decimal artistSongRev = 0;
                    Decimal artistAlbumRev = 0;
                    Decimal totalArtistRev = 0;
                    var query2 = from pi in db.PurchaseItems
                                 select pi;
                    List<PurchaseItem> allPurchaseItems = query2.ToList();
                    List<PurchaseItem> songsPurchased = new List<PurchaseItem>();
                    List<PurchaseItem> albumsPurchased = new List<PurchaseItem>();
                    foreach (PurchaseItem pi in allPurchaseItems)
                    {
                        if (!pi.isAlbum)
                        {
                            if (pi.PurchaseItemSong.SongArtist.Contains(a))
                            {
                                songsPurchased.Add(pi);
                            }
                        }
                        else
                        {
                            if (pi.PurchaseItemAlbum.AlbumArtist.Contains(a))
                            {
                                albumsPurchased.Add(pi);
                            }
                        }
                    }
                    artistSongPurchaseCount = songsPurchased.Count();
                    foreach(PurchaseItem pi in songsPurchased)
                    {
                        artistSongRev += pi.PurchaseItemPrice;
                    }
                    artistAlbumPurchaseCount = albumsPurchased.Count();
                    foreach (PurchaseItem pi in albumsPurchased)
                    {
                        artistAlbumRev += pi.PurchaseItemPrice;
                    }
                    totalArtistRev = artistAlbumRev + artistSongRev;
                    if(totalArtistRev > topRevenue)
                    {
                        topRevenue = totalArtistRev;
                        grvm.topArtist = a.ArtistName;
                        grvm.albumRev = artistAlbumRev.ToString("c");
                        grvm.albumCount = artistAlbumPurchaseCount;
                        grvm.songRev = artistSongRev.ToString("c");
                        grvm.songCount = artistSongPurchaseCount;
                        grvm.totalRev = totalArtistRev.ToString("c");
                    }
                }
                grvmList.Add(grvm);
            }
            return View(grvmList);
        }
    }
}
