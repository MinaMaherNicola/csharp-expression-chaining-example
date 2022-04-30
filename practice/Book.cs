using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practice
{
    public class Book
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public bool Discount { get; set; }
        public decimal? Percentage
        {
            get { return Discount == false ? null : _percentage; }
            set { _percentage = Discount == false ? null : value; }
        }

        private decimal? _percentage;
    }
}
