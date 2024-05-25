using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRSTWEb.Domain.Entities
{
    public class Discount
    {
        [Key]
        [ForeignKey("Book")]
        public int Id { get; set; }
        public decimal Percentage { get; set; }
        public DateTime SetTime { get; set; }
        public DateTime ExpirationTime { get; set; }

        public virtual Book Book { get; set; }
    }
}
