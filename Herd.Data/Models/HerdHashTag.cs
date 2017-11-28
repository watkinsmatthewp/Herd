using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Data.Models
{
    public class HashTag : IComparable<HashTag>
    {
        public string Name { get; set; }
        public decimal Score { get; set; }

        public int CompareTo(HashTag other)
        {
            return Score.CompareTo(other.Score);
        }
    }
}
