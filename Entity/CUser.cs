using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlCompare.Infrastructure.Entity
{
    [Alias("xmlcmp_user")]
    public class CUser
    {
        [Alias("id")]
        [Required]
        public string Id { get; set; }

        [Alias("user_name")]
        [Required]
        public string Name { get; set; }
    }
}
