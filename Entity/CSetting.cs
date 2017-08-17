using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlCompare.Infrastructure.Entity
{
    [Alias("xmlcmp_setting")]
    public class CSetting
    {
        [Alias("name")]
        [Required]
        public string Name { get; set; }

        [Alias("value")]
        [Required]
        public string Value { get; set; }
    }
}
