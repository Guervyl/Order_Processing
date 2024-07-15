using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tranzaksyon.Database.Models
{
    public class OrderLog
    {
        public int Id { get; set; }
        public Order Order { get; set; }
        public int OrderId { get; set; }

        public string Error { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
