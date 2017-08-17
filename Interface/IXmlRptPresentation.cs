using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlCompare.Infrastructure.Interface
{
    public interface IXmlRptPresentation
    {
        DataTable PresentReport(string reportpath);
    }
}
