using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace XmlCompare.UseControl.Views
{
    /// <summary>
    /// Interaction logic for frmCmpFileListView
    /// </summary>
    /// 
    [Export]
    public partial class frmCmpFileListView : UserControl
    {
        public frmCmpFileListView()
        {
            InitializeComponent();
        }
    }
}
