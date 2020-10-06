using System;
using System.Collections.Generic;
using System.Text;

namespace UNO
{
    public class Card : IComparable
    {
        private string Name { get; set; }
        private string ImageSrc { get; set; }
        private uint CardNumber { get; set; }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
