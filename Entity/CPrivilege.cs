using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlCompare.Infrastructure.Entity
{
    [Alias("xmlcmp_auth")]
    public class CPrivilege
    {
        [Alias("user_id")]
        [Required]
        public string CUserId { get; set; }

        [Alias("func_id")]
        [Required]
        public int CFunctionId { get; set; }
    }
}
