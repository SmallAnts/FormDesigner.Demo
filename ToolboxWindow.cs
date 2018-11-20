using System.ComponentModel;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Smart.FormDesigner.Demo
{
    public partial class ToolboxWindow : DockContent
    {
        public ToolboxWindow()
        {
            InitializeComponent();
            InitToolbox();
        }

        private void InitToolbox()
        {
            string groupName = "公共控件";
            this.Toolbox.AddToolboxItem(typeof(Button), groupName);
            this.Toolbox.AddToolboxItem(typeof(CheckBox), groupName);
            this.Toolbox.AddToolboxItem(typeof(CheckedListBox), groupName);
            this.Toolbox.AddToolboxItem(typeof(ComboBox), groupName);
            this.Toolbox.AddToolboxItem(typeof(DateTimePicker), groupName);
            this.Toolbox.AddToolboxItem(typeof(Label), groupName);
            this.Toolbox.AddToolboxItem(typeof(LinkLabel), groupName);
            this.Toolbox.AddToolboxItem(typeof(ListBox), groupName);
            this.Toolbox.AddToolboxItem(typeof(ListView), groupName);
            this.Toolbox.AddToolboxItem(typeof(MaskedTextBox), groupName);
            this.Toolbox.AddToolboxItem(typeof(MonthCalendar), groupName);
            this.Toolbox.AddToolboxItem(typeof(NotifyIcon), groupName);
            this.Toolbox.AddToolboxItem(typeof(NumericUpDown), groupName);
            this.Toolbox.AddToolboxItem(typeof(PictureBox), groupName);
            this.Toolbox.AddToolboxItem(typeof(ProgressBar), groupName);
            this.Toolbox.AddToolboxItem(typeof(RadioButton), groupName);
            this.Toolbox.AddToolboxItem(typeof(RichTextBox), groupName);
            this.Toolbox.AddToolboxItem(typeof(TextBox), groupName);
            this.Toolbox.AddToolboxItem(typeof(ToolTip), groupName);
            this.Toolbox.AddToolboxItem(typeof(TreeView), groupName);
            this.Toolbox.AddToolboxItem(typeof(WebBrowser), groupName);

             groupName = "容器";
            this.Toolbox.AddToolboxItem(typeof(FlowLayoutPanel), groupName);
            this.Toolbox.AddToolboxItem(typeof(GroupBox), groupName);
            this.Toolbox.AddToolboxItem(typeof(Panel), groupName);
            this.Toolbox.AddToolboxItem(typeof(SplitContainer), groupName);
            this.Toolbox.AddToolboxItem(typeof(TabControl), groupName);
            this.Toolbox.AddToolboxItem(typeof(TableLayoutPanel), groupName);


            groupName = "菜单和工具栏";
            this.Toolbox.AddToolboxItem(typeof(ContextMenuStrip), groupName);
            this.Toolbox.AddToolboxItem(typeof(MenuStrip), groupName);
            this.Toolbox.AddToolboxItem(typeof(StatusStrip), groupName);
            this.Toolbox.AddToolboxItem(typeof(ToolStrip), groupName);
            this.Toolbox.AddToolboxItem(typeof(ToolStripContainer), groupName);

            groupName = "数据";
            this.Toolbox.AddToolboxItem(typeof(BindingNavigator), groupName);
            this.Toolbox.AddToolboxItem(typeof(BindingSource), groupName);
            this.Toolbox.AddToolboxItem(typeof(DataGridView), groupName);

            groupName = "组件";
            this.Toolbox.AddToolboxItem(typeof(BackgroundWorker), groupName);
            this.Toolbox.AddToolboxItem(typeof(ErrorProvider), groupName);
            this.Toolbox.AddToolboxItem(typeof(Timer), groupName);

        }
    }
}
