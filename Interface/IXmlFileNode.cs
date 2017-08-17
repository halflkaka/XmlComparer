using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlCompare.Infrastructure.Interface
{
    public interface IXmlFileNode
    {
        //properties
        Dictionary<string, string> m_attributes { get; set; }
        Dictionary<string, IXmlFileNode> m_childNodes { get; set; }
        string text { get; set; }
        string name { get; set; }
        string key { get; set; }
        string xPath { get; set; }
    }
}
