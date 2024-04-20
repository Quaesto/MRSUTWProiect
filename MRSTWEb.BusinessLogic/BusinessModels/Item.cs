using MRSTWEb.BuisnessLogic.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRSTWEb.BuisnessLogic.BuisnessModels
{
    public class Item
    {
        public BookDTO Book { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Book.Price * Quantity;
    }
}
