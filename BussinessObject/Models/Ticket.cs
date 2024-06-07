using System;
using System.Collections.Generic;

namespace BussinessObject.Models
{
    public partial class Ticket
    {
        public int ShowId { get; set; }
        public int? UserId { get; set; }
        public string Otp { get; set; } = null!;
        public int Row { get; set; }
        public DateTime? Date { get; set; }
        public int Col { get; set; }
        public bool IsUsed { get; set; }

        public virtual Show Show { get; set; } = null!;
        public virtual User? User { get; set; }
    }
}
