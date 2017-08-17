using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlCompare.Infrastructure.Entity
{
    public class CLogRec
    {
        public string time_stamp;
        public string category;
        public string source_tag_name;
        public string source_identifier;
        public string path;
        public string detail_attribute_name;
        public string detail_attribute_old_value;
        public string detail_attribute_new_value;
    }
}
