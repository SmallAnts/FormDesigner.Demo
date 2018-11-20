using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Smart.FormDesigner.Demo
{
    public partial class CustomForm : Form
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string LayoutXml
        {
            get { return this.designer1.LayoutXML; }
            set { this.designer1.LayoutXML = value; }
        }

        public CustomForm()
        {
            InitializeComponent();
        }

        // public 设计时事件可选
        public void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Click1");
        }
        public void button1_Click2(object sender, EventArgs e)
        {
            MessageBox.Show("Click2");
        }
    }
}
