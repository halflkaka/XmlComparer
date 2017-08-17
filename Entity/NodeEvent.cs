using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlCompare.Infrastructure.Interface;

namespace XmlCompare.Infrastructure.Entity
{
    public class NodeEvent: PubSubEvent<IXmlSettingNode>
    {
    }
}
