using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tranzaksyon.Database.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public DateTime? ProcessedDate { get; set; }

        public IdentityUser User { get; set; }

        public string UserId { get; set; }
    }
}
