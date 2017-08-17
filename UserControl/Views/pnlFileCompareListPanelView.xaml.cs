using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace XmlCompare.UseControl.Views
{
    /// <summary>
    /// Interaction logic for pnlFileCompareListPanelView
    /// </summary>
    /// 
    [Export]
    public partial class pnlFileCompareListPanelView : UserControl
    {
        public pnlFileCompareListPanelView()
        {
            InitializeComponent();
        }
    }
}
