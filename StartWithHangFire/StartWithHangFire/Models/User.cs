using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StartWithHangFire.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
