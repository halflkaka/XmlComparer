using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlCompare.Infrastructure.Entity
{
    [Alias("xmlcmp_func")]
    public class CFunction
    {
        [Alias("id")]
        [Required]
        public int Id { get; set; }

        [Alias("func_name")]
        [Required]
        public string Name { get; set; }
    }
}
