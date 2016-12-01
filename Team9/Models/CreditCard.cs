using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Team9.Models
{
    public class CreditCard
    {
        public Int32 CreditCardID { get; set; }

        private String _CCNumber;
        public String CCNumber {
            get { return _CCNumber; }
            set
            {
                _CCNumber = value;
                if (String.IsNullOrEmpty(_CCNumber))
                {
                    _cardType = CCType.None;
                }
                else if (_CCNumber.Length == 15)
                {
                    _cardType = CCType.American_Express;
                }
                else if (_CCNumber.Substring(0, 2) == "54")
                {
                    _cardType = CCType.Mastercard;
                }
                else if (_CCNumber.Substring(0,1) == "4")
                {
                    _cardType = CCType.Visa;
                }
                else if (_CCNumber.Substring(0,1) == "6")
                {
                    _cardType = CCType.Discover;
                }
                else
                {
                    _cardType = CCType.None;
                }
                    if (_cardType == CCType.American_Express)
                    {
                        _displayNumber = "***********" + CCNumber.Substring(10, 4) + " - " + CardType.ToString();
                    }
                    else if (_cardType == CCType.None)
                    {
                    _displayNumber = "No second card on file";
                    }
                    else
                    {
                        _displayNumber = "************" + CCNumber.Substring(11, 4) + " - " + CardType.ToString();
                    }
                
            }
        }

        public enum CCType
        {
            Visa, American_Express, Discover, Mastercard, None
        }

        private CCType _cardType;
        public CCType CardType
        {
            get
            {
                return _cardType;
            }
           
        }

        private String _displayNumber;
        public String displayNumber
        {
            get
            {
                return _displayNumber;
            }
        }

        public virtual AppUser CardOwner { get; set; }
      
    }
}