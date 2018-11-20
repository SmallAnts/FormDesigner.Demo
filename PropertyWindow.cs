using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Smart.FormDesigner.Demo
{
    public partial class PropertyWindow : DockContent
    {
        public PropertyWindow()
        {
            InitializeComponent();
            this.Propertybox.ShowEventTab = true;
        }

    }
}
