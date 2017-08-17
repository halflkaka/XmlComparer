using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace XmlCompare.DAL.Views
{
    /// <summary>
    /// Interaction logic for DALView
    /// </summary>
    /// 
    [Export]
    public partial class DALView : UserControl
    {
        public DALView()
        {
            InitializeComponent();
        }
    }
}
