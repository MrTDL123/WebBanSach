
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.Models
{
    public class ApplicationUser : IdentityUser //Vì IdentityUser thiếu các cột cần thiết nên ta sẽ implement nó qua ApplicationUser
    {
        [Required]
        public string Name { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
    }
}
