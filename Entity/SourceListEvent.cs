using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlCompare.Infrastructure.Entity
{
    public class SourceListEvent:PubSubEvent<ObservableCollection<string>>
    {
    }
}
