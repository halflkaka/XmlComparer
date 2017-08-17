using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace XmlCompare.ToolSetting.Views
{
    /// <summary>
    /// Interaction logic for EmptyView
    /// </summary>
    /// 
    [Export]
    public partial class EmptyView : UserControl
    {
        public EmptyView()
        {
            InitializeComponent();
        }
    }
}
