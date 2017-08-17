using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace XmlCompare.Core.Views
{
    /// <summary>
    /// Interaction logic for CoreView
    /// </summary>
    /// 
    [Export]
    public partial class CoreView : UserControl
    {
        public CoreView()
        {
            InitializeComponent();
        }
    }
}
