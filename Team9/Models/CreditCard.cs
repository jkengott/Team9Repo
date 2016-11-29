using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team9.Models
{
    public class CreditCard
    {
        public Int32 CreditCardID { get; set; }

        public String CCNumber { get; set; }

        public enum CCType
        {
            Visa, American_Express, Discover, Mastercard, None
        }


        public CCType CardType
        {
            get
            {
                if (CCNumber.Count() == 15)
                {
                    return CCType.American_Express;
                }
                else if (CCNumber.Substring(0, 2) == "54")
                {
                    return CCType.Mastercard;
                }
                else if (CCNumber.Substring(0) == "4")
                {
                    return CCType.Visa;
                }
                else if (CCNumber.Substring(0) == "6")
                {
                    return CCType.Discover;
                }
                else
                {
                    return CCType.None;
                }
            }
            set
            {
                if (CCNumber.Count() == 15)
                {
                    CardType = CCType.American_Express;
                }
                else if (CCNumber.Substring(0, 2) == "54")
                {
                    CardType = CCType.Mastercard;
                }
                else if (CCNumber.Substring(0) == "4")
                {
                    CardType = CCType.Visa;
                }
                else if (CCNumber.Substring(0) == "6")
                {
                    CardType = CCType.Discover;
                }
                else
                {
                    CardType = CCType.None;
                }
            }
        }

        public String displayNumber
        {
            set
            {
                    string hiddennumber;
                    if (CardType.ToString() == "American_Express")
                    {
                        hiddennumber = "***********" + CCNumber.Substring(10, 4);
                         displayNumber = hiddennumber;
                    }
                    else
                    {
                        hiddennumber = "************" + CCNumber.Substring(11, 4);
                        displayNumber = hiddennumber;
                    }
            }
            get
            {
                string hiddennumber;
                if (CardType.ToString() == "American_Express")
                {
                    hiddennumber = "***********" + CCNumber.Substring(10, 4);
                    return hiddennumber;
                }
                else
                {
                    hiddennumber = "************" + CCNumber.Substring(11, 4);
                    return hiddennumber;
                }
            }
        }

        public virtual AppUser CardOwner { get; set; }
    }
}