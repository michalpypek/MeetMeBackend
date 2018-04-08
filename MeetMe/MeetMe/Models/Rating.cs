using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MeetMe.Models
{
    public class Rating
    {
        

        private int NumberOfRates { get; set; }
        private int Sum { get; set; }

        public  Rating()
        {
            Sum = 0;
            NumberOfRates = 0;
        }
        public bool AddRating(int r)
        {
            if (r > 0 && r <= 5)
            {
                NumberOfRates++;
                Sum += r;
                return true;
            }
            return false;
        }

        public double GetRating()
        {
            return Sum / NumberOfRates;  
        }

        public bool NumberOfRatesGT3()
        {
            if (NumberOfRates > 3) return true;
            return false;
        }

    }
}