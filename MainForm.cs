using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Smart.FormDesigner.Demo
{
    public class MainForm : Form
    {
        private ToolboxWindow toolboxWindow;
        private PropertyWindow propertyWindow;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton tbPreview;
        private ToolStripButton tbDelete;
        private Designer activeDesigner;

        public MainForm()
        {
            InitializeComponent();

            this.toolboxWindow = new ToolboxWindow();
            this.toolboxWindow.Show(this.dockPanel, DockState.DockLeft);

            this.propertyWindow = new PropertyWindow();
            this.propertyWindow.Show(this.dockPanel, DockState.DockRight);

            EnableUndoRedo();
        }

        private void dockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            if (this.dockPanel.ActiveDocument is DesignerDocument doc)
            {
                this.activeDesigner = doc.Designer;
                this.SelectionChanged(this.activeDesigner.SelectionService, EventArgs.Empty);

                this.toolboxWindow.Toolbox.Designer = this.activeDesigner;

                this.propertyWindow.Propertybox.SetComponents(this.activeDesigner.DesignerHost.Container.Components);

                this.EnableUndoRedo();

                this.tbPreview.Enabled = true;
                this.tbSaveForm.Enabled = true;
            }
            else
            {
                this.tbPreview.Enabled = false;
                this.tbSaveForm.Enabled = false;
            }
        }

        private bool DesignEvents_AddingVerb(IComponent primarySelection, DesignerVerb verb)
        {
            return true;
        }
        private void SelectionChanged(object sender, EventArgs e)
        {
            var selectionService = (ISelectionService)sender;
            int selectionCount = selectionService.SelectionCount;

            EnableAlignResize(selectionCount > 1);
            if (selectionCount >= 1)
            {
                this.miDeleteSelection.Enabled = true;
                this.miCopy.Enabled = true;
                this.tbDelete.Enabled = true;
            }
            else
            {
                this.miDeleteSelection.Enabled = false;
                this.miCopy.Enabled = false;
                this.tbDelete.Enabled = false;
            }

            this.propertyWindow.Propertybox.Designer = this.activeDesigner;
            if (selectionCount == 0)
            {
                this.propertyWindow.Propertybox.SetSelectedObjects(this.activeDesigner.DesignedForm);
            }
            else
            {
                var selected = new object[selectionCount];
                selectionService.GetSelectedComponents().CopyTo(selected, 0);
                this.propertyWindow.Propertybox.SetSelectedObjects(selected);
            }
        }
        private void ComponentAdded(object sender, ComponentEventArgs e)
        {
            this.propertyWindow.Propertybox.SetComponents(this.activeDesigner.DesignerHost.Container.Components);

            EnableUndoRedo();
        }
        private void ComponentRemoved(object sender, ComponentEventArgs e)
        {
            this.propertyWindow.Propertybox.SetComponents(this.activeDesigner.DesignerHost.Container.Components);

            EnableUndoRedo();
        }
        private void ComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            EnableUndoRedo();
        }

        private void NewDesignedForm()
        {
            string name = "from " + (this.dockPanel.DocumentsCount + 1);
            var rootType = typeof(CustomForm);

            var doc = new DesignerDocument(name, rootType);
            this.activeDesigner = doc.Designer;
            doc.FormClosing += (s, e) =>
            {
                EndDesign(doc.Designer);
            };
            doc.Designer.DesignEvents.AddingVerb += DesignEvents_AddingVerb;
            doc.Designer.SelectionService.SelectionChanged += SelectionChanged;
            doc.Designer.ComponentChangeService.ComponentAdded += ComponentAdded;
            doc.Designer.ComponentChangeService.ComponentRemoved += ComponentRemoved;
            doc.Designer.ComponentChangeService.ComponentChanged += ComponentChanged;
            doc.Show(dockPanel);
            tbSaveForm.Enabled = true;
        }
        private void OpenDesignedForm()
        {
            var openFileName = new OpenFileDialog();

            openFileName.Filter = "XML text format (*.xml)|*.xml|Proprietary text format (*.*)|*.*";
            openFileName.FilterIndex = 1;
            openFileName.RestoreDirectory = true;

            if (openFileName.ShowDialog() == DialogResult.OK)
            {
                this.NewDesignedForm();

                if (openFileName.FilterIndex == 1)
                {
                    var txtReader = new StreamReader(openFileName.FileName);
                    string layoutString = txtReader.ReadToEnd();
                    txtReader.Close();

                    this.activeDesigner.LayoutXML = layoutString;
                }
                else
                {
                    this.activeDesigner.LoadFromFile(openFileName.FileName);
                }
                tbSaveForm.Enabled = true;
            }
        }
        private void SaveDesignedForm()
        {
            var saveFileName = new SaveFileDialog();
            saveFileName.Filter = "XML Form (*.xml)|*.xml";
            saveFileName.FilterIndex = 1;
            saveFileName.RestoreDirectory = true;

            if (saveFileName.ShowDialog() == DialogResult.OK)
            {
                string test = this.activeDesigner.LayoutXML;

                TextWriter txtWriter = new StreamWriter(saveFileName.FileName);
                txtWriter.Write(test);
                txtWriter.Close();
            }
        }
        private void CheckDesignedForm()
        {
            if (this.activeDesigner.IsDirty == true)
            {
                if (MessageBox.Show("是否保存对表单的修改?", "确认提示",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SaveDesignedForm();
                }
            }
        }
        private void EndDesign(Designer designer)
        {
            if (designer == null) return;

            designer.SelectionService.SelectionChanged -= SelectionChanged;
            designer.ComponentChangeService.ComponentAdded -= ComponentAdded;
            designer.ComponentChangeService.ComponentRemoved -= ComponentRemoved;
            designer.ComponentChangeService.ComponentChanged -= ComponentChanged;
            CheckDesignedForm();
            designer.Active = false;
            designer.DesignContainer = null;
        }

        private void EnableAlignResize(bool enable)
        {
            this.miAlignBottom.Enabled = enable;
            this.miAlignMiddle.Enabled = enable;
            this.miAlignTop.Enabled = enable;
            this.miAlignCenter.Enabled = enable;
            this.miAlignRight.Enabled = enable;
            this.miAlignLeft.Enabled = enable;

            this.tbAlignBottom.Enabled = enable;
            this.tbAlignMiddle.Enabled = enable;
            this.tbAlignTop.Enabled = enable;
            this.tbAlignCenter.Enabled = enable;
            this.tbAlignLeft.Enabled = enable;
            this.tbAlignRight.Enabled = enable;

            this.miSameBoth.Enabled = enable;
            this.miSameWidth.Enabled = enable;
            this.miSameHeight.Enabled = enable;

            this.tbSameBoth.Enabled = enable;
            this.tbSameWidth.Enabled = enable;
            this.tbSameHeight.Enabled = enable;
        }
        private void EnableUndoRedo()
        {
            miUndo.Enabled = (this.activeDesigner?.UndoCount > 0);
            miRedo.Enabled = (this.activeDesigner?.RedoCount > 0);

            tbUndo.Enabled = (this.activeDesigner?.UndoCount > 0);
            tbRedo.Enabled = (this.activeDesigner?.RedoCount > 0);
        }

        #region 菜单事件

        private void miNewForm_Click(object sender, EventArgs e)
        {
            NewDesignedForm();
        }
        private void miOpenForm_Click(object sender, System.EventArgs e)
        {
            OpenDesignedForm();
        }
        private void miSaveForm_Click(object sender, System.EventArgs e)
        {
            SaveDesignedForm();
        }
        private void miExitDesigner_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void tbPreview_Click(object sender, EventArgs e)
        {
            if (this.dockPanel.ActiveDocument is DesignerDocument doc)
            {
                doc.Preview();
            }
        }
        private void tbDelete_Click(object sender, EventArgs e)
        {
            if (this.dockPanel.ActiveDocument is DesignerDocument doc)
            {
                doc.Designer.DeleteSelected();
            }
        }
        private void miAlignTop_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.Align(AlignType.Top);
        }
        private void miAlignMiddle_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.Align(AlignType.Middle);
        }
        private void miAlignBottom_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.Align(AlignType.Bottom);
        }
        private void miAlignLeft_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.Align(AlignType.Left);
        }
        private void miAlignCenter_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.Align(AlignType.Center);
        }
        private void miAlignRight_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.Align(AlignType.Right);
        }

        private void miSameHeight_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.MakeSameSize(ResizeType.SameHeight);
        }
        private void miSameWidth_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.MakeSameSize(ResizeType.SameWidth);
        }
        private void miSameBoth_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.MakeSameSize(ResizeType.SameHeight | ResizeType.SameWidth);
        }
        private void miUndo_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.Undo();
            miUndo.Enabled = (this.activeDesigner.UndoCount != 0);
            miRedo.Enabled = (this.activeDesigner.RedoCount != 0);
        }
        private void miRedo_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.Redo();
            miUndo.Enabled = (this.activeDesigner.UndoCount != 0);
            miRedo.Enabled = (this.activeDesigner.RedoCount != 0);
        }
        private void miDeleteSelection_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.DeleteSelected();
        }

        private void miCopy_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.CopyControls();
        }
        private void miPaste_Click(object sender, System.EventArgs e)
        {
            this.activeDesigner.PasteControls();
        }

        private void miAbout_Click(object sender, System.EventArgs e)
        {
            MessageBox.Show("Smart Form Designer" + Environment.NewLine +
                "Copyright © 2018 SmallAnts",
                "关于",
                MessageBoxButtons.OK);
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            this.toolboxWindow.Dispose();
            this.propertyWindow.Dispose();

            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.miNewForm = new System.Windows.Forms.MenuItem();
            this.miOpenForm = new System.Windows.Forms.MenuItem();
            this.miSaveForm = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.miExitDesigner = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.miUndo = new System.Windows.Forms.MenuItem();
            this.miRedo = new System.Windows.Forms.MenuItem();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.miAlignTop = new System.Windows.Forms.MenuItem();
            this.miAlignMiddle = new System.Windows.Forms.MenuItem();
            this.miAlignBottom = new System.Windows.Forms.MenuItem();
            this.menuItem12 = new System.Windows.Forms.MenuItem();
            this.miAlignLeft = new System.Windows.Forms.MenuItem();
            this.miAlignCenter = new System.Windows.Forms.MenuItem();
            this.miAlignRight = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.miSameHeight = new System.Windows.Forms.MenuItem();
            this.miSameWidth = new System.Windows.Forms.MenuItem();
            this.menuItem13 = new System.Windows.Forms.MenuItem();
            this.miSameBoth = new System.Windows.Forms.MenuItem();
            this.menuItem14 = new System.Windows.Forms.MenuItem();
            this.miCopy = new System.Windows.Forms.MenuItem();
            this.miPaste = new System.Windows.Forms.MenuItem();
            this.miDeleteSelection = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.miAbout = new System.Windows.Forms.MenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.tbNewForm = new System.Windows.Forms.ToolStripButton();
            this.tbOpenForm = new System.Windows.Forms.ToolStripButton();
            this.tbSaveForm = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tbPreview = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tbUndo = new System.Windows.Forms.ToolStripButton();
            this.tbRedo = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbDelete = new System.Windows.Forms.ToolStripButton();
            this.tbAlignLeft = new System.Windows.Forms.ToolStripButton();
            this.tbAlignCenter = new System.Windows.Forms.ToolStripButton();
            this.tbAlignRight = new System.Windows.Forms.ToolStripButton();
            this.tbAlignTop = new System.Windows.Forms.ToolStripButton();
            this.tbAlignMiddle = new System.Windows.Forms.ToolStripButton();
            this.tbAlignBottom = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tbSameWidth = new System.Windows.Forms.ToolStripButton();
            this.tbSameHeight = new System.Windows.Forms.ToolStripButton();
            this.tbSameBoth = new System.Windows.Forms.ToolStripButton();
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.vS2015LightTheme1 = new WeifenLuo.WinFormsUI.Docking.VS2015LightTheme();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem2,
            this.menuItem4});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miNewForm,
            this.miOpenForm,
            this.miSaveForm,
            this.menuItem6,
            this.miExitDesigner});
            this.menuItem1.Text = "文件(&F)";
            // 
            // miNewForm
            // 
            this.miNewForm.Index = 0;
            this.miNewForm.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
            this.miNewForm.Text = "新建(&N)";
            this.miNewForm.Click += new System.EventHandler(this.miNewForm_Click);
            // 
            // miOpenForm
            // 
            this.miOpenForm.Index = 1;
            this.miOpenForm.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.miOpenForm.Text = "打开(&O)";
            this.miOpenForm.Click += new System.EventHandler(this.miOpenForm_Click);
            // 
            // miSaveForm
            // 
            this.miSaveForm.Index = 2;
            this.miSaveForm.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
            this.miSaveForm.Text = "保存(&S)";
            this.miSaveForm.Click += new System.EventHandler(this.miSaveForm_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 3;
            this.menuItem6.Text = "-";
            // 
            // miExitDesigner
            // 
            this.miExitDesigner.Index = 4;
            this.miExitDesigner.Text = "退出(&X)";
            this.miExitDesigner.Click += new System.EventHandler(this.miExitDesigner_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miUndo,
            this.miRedo,
            this.menuItem11,
            this.menuItem8,
            this.menuItem9,
            this.menuItem14,
            this.miCopy,
            this.miPaste,
            this.miDeleteSelection});
            this.menuItem2.Text = "编辑(&E)";
            // 
            // miUndo
            // 
            this.miUndo.Index = 0;
            this.miUndo.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
            this.miUndo.Text = "撤销(&U)";
            this.miUndo.Click += new System.EventHandler(this.miUndo_Click);
            // 
            // miRedo
            // 
            this.miRedo.Index = 1;
            this.miRedo.Shortcut = System.Windows.Forms.Shortcut.CtrlY;
            this.miRedo.Text = "重做(&R)";
            this.miRedo.Click += new System.EventHandler(this.miRedo_Click);
            // 
            // menuItem11
            // 
            this.menuItem11.Index = 2;
            this.menuItem11.Text = "-";
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 3;
            this.menuItem8.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miAlignTop,
            this.miAlignMiddle,
            this.miAlignBottom,
            this.menuItem12,
            this.miAlignLeft,
            this.miAlignCenter,
            this.miAlignRight});
            this.menuItem8.Text = "对齐(&A)";
            // 
            // miAlignTop
            // 
            this.miAlignTop.Index = 0;
            this.miAlignTop.Text = "&Top";
            this.miAlignTop.Click += new System.EventHandler(this.miAlignTop_Click);
            // 
            // miAlignMiddle
            // 
            this.miAlignMiddle.Index = 1;
            this.miAlignMiddle.Text = "&Middle";
            this.miAlignMiddle.Click += new System.EventHandler(this.miAlignMiddle_Click);
            // 
            // miAlignBottom
            // 
            this.miAlignBottom.Index = 2;
            this.miAlignBottom.Text = "&Bottom";
            this.miAlignBottom.Click += new System.EventHandler(this.miAlignBottom_Click);
            // 
            // menuItem12
            // 
            this.menuItem12.Index = 3;
            this.menuItem12.Text = "-";
            // 
            // miAlignLeft
            // 
            this.miAlignLeft.Index = 4;
            this.miAlignLeft.Text = "&Left";
            this.miAlignLeft.Click += new System.EventHandler(this.miAlignLeft_Click);
            // 
            // miAlignCenter
            // 
            this.miAlignCenter.Index = 5;
            this.miAlignCenter.Text = "&Center";
            this.miAlignCenter.Click += new System.EventHandler(this.miAlignCenter_Click);
            // 
            // miAlignRight
            // 
            this.miAlignRight.Index = 6;
            this.miAlignRight.Text = "&Right";
            this.miAlignRight.Click += new System.EventHandler(this.miAlignRight_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 4;
            this.menuItem9.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miSameHeight,
            this.miSameWidth,
            this.menuItem13,
            this.miSameBoth});
            this.menuItem9.Text = "使用相同(&M)";
            // 
            // miSameHeight
            // 
            this.miSameHeight.Index = 0;
            this.miSameHeight.Text = "Same &Height";
            this.miSameHeight.Click += new System.EventHandler(this.miSameHeight_Click);
            // 
            // miSameWidth
            // 
            this.miSameWidth.Index = 1;
            this.miSameWidth.Text = "Same &Width";
            this.miSameWidth.Click += new System.EventHandler(this.miSameWidth_Click);
            // 
            // menuItem13
            // 
            this.menuItem13.Index = 2;
            this.menuItem13.Text = "-";
            // 
            // miSameBoth
            // 
            this.miSameBoth.Index = 3;
            this.miSameBoth.Text = "Same &Both";
            this.miSameBoth.Click += new System.EventHandler(this.miSameBoth_Click);
            // 
            // menuItem14
            // 
            this.menuItem14.Index = 5;
            this.menuItem14.Text = "-";
            // 
            // miCopy
            // 
            this.miCopy.Index = 6;
            this.miCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
            this.miCopy.Text = "复制(&C)";
            this.miCopy.Click += new System.EventHandler(this.miCopy_Click);
            // 
            // miPaste
            // 
            this.miPaste.Index = 7;
            this.miPaste.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
            this.miPaste.Text = "粘贴(&P)";
            this.miPaste.Click += new System.EventHandler(this.miPaste_Click);
            // 
            // miDeleteSelection
            // 
            this.miDeleteSelection.Index = 8;
            this.miDeleteSelection.Shortcut = System.Windows.Forms.Shortcut.Del;
            this.miDeleteSelection.Text = "删除(&D)";
            this.miDeleteSelection.Click += new System.EventHandler(this.miDeleteSelection_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 2;
            this.menuItem4.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.miAbout});
            this.menuItem4.Text = "帮助(&H)";
            // 
            // miAbout
            // 
            this.miAbout.Index = 0;
            this.miAbout.Text = "关于(&A)";
            this.miAbout.Click += new System.EventHandler(this.miAbout_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbNewForm,
            this.tbOpenForm,
            this.tbSaveForm,
            this.toolStripSeparator3,
            this.tbPreview,
            this.toolStripSeparator2,
            this.tbUndo,
            this.tbRedo,
            this.toolStripSeparator1,
            this.tbDelete,
            this.tbAlignLeft,
            this.tbAlignCenter,
            this.tbAlignRight,
            this.tbAlignTop,
            this.tbAlignMiddle,
            this.tbAlignBottom,
            this.toolStripSeparator4,
            this.tbSameWidth,
            this.tbSameHeight,
            this.tbSameBoth});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1008, 25);
            this.toolStrip.TabIndex = 7;
            this.toolStrip.Text = "toolStrip1";
            // 
            // tbNewForm
            // 
            this.tbNewForm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbNewForm.Image = global::Smart.FormDesigner.Demo.Properties.Resources.new_from_16x;
            this.tbNewForm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbNewForm.Name = "tbNewForm";
            this.tbNewForm.Size = new System.Drawing.Size(23, 22);
            this.tbNewForm.Text = "新建表单";
            this.tbNewForm.ToolTipText = "新建表单 (Ctrl + Ｎ)";
            this.tbNewForm.Click += new System.EventHandler(this.miNewForm_Click);
            // 
            // tbOpenForm
            // 
            this.tbOpenForm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbOpenForm.Image = global::Smart.FormDesigner.Demo.Properties.Resources.open_file_16x;
            this.tbOpenForm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbOpenForm.Name = "tbOpenForm";
            this.tbOpenForm.Size = new System.Drawing.Size(23, 22);
            this.tbOpenForm.Text = "打开文件";
            this.tbOpenForm.ToolTipText = "打开文件 (Ctrl + O)";
            this.tbOpenForm.Click += new System.EventHandler(this.miOpenForm_Click);
            // 
            // tbSaveForm
            // 
            this.tbSaveForm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbSaveForm.Enabled = false;
            this.tbSaveForm.Image = global::Smart.FormDesigner.Demo.Properties.Resources.save_16x;
            this.tbSaveForm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbSaveForm.Name = "tbSaveForm";
            this.tbSaveForm.Size = new System.Drawing.Size(23, 22);
            this.tbSaveForm.Text = "保存表单";
            this.tbSaveForm.ToolTipText = "保存表单 (Ctrl + S)";
            this.tbSaveForm.Click += new System.EventHandler(this.miSaveForm_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // tbPreview
            // 
            this.tbPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbPreview.Enabled = false;
            this.tbPreview.Image = global::Smart.FormDesigner.Demo.Properties.Resources.preview_16x;
            this.tbPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbPreview.Name = "tbPreview";
            this.tbPreview.Size = new System.Drawing.Size(23, 22);
            this.tbPreview.Text = "预览";
            this.tbPreview.ToolTipText = "预览 (F5)";
            this.tbPreview.Click += new System.EventHandler(this.tbPreview_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tbUndo
            // 
            this.tbUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbUndo.Enabled = false;
            this.tbUndo.Image = global::Smart.FormDesigner.Demo.Properties.Resources.undo_16x;
            this.tbUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbUndo.Name = "tbUndo";
            this.tbUndo.Size = new System.Drawing.Size(23, 22);
            this.tbUndo.Text = "撤销（Ctrl+Z）";
            this.tbUndo.Click += new System.EventHandler(this.miUndo_Click);
            // 
            // tbRedo
            // 
            this.tbRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbRedo.Enabled = false;
            this.tbRedo.Image = global::Smart.FormDesigner.Demo.Properties.Resources.redo_16x;
            this.tbRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbRedo.Name = "tbRedo";
            this.tbRedo.Size = new System.Drawing.Size(23, 22);
            this.tbRedo.Text = "重做（Ctrl+Y）";
            this.tbRedo.Click += new System.EventHandler(this.miRedo_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tbDelete
            // 
            this.tbDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbDelete.Enabled = false;
            this.tbDelete.Image = global::Smart.FormDesigner.Demo.Properties.Resources.delete_16x;
            this.tbDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbDelete.Name = "tbDelete";
            this.tbDelete.Size = new System.Drawing.Size(23, 22);
            this.tbDelete.Text = "删除选中项";
            this.tbDelete.Click += new System.EventHandler(this.tbDelete_Click);
            // 
            // tbAlignLeft
            // 
            this.tbAlignLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbAlignLeft.Enabled = false;
            this.tbAlignLeft.Image = global::Smart.FormDesigner.Demo.Properties.Resources.align_left_16x;
            this.tbAlignLeft.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbAlignLeft.Name = "tbAlignLeft";
            this.tbAlignLeft.Size = new System.Drawing.Size(23, 22);
            this.tbAlignLeft.Text = "左对齐";
            this.tbAlignLeft.Click += new System.EventHandler(this.miAlignLeft_Click);
            // 
            // tbAlignCenter
            // 
            this.tbAlignCenter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbAlignCenter.Enabled = false;
            this.tbAlignCenter.Image = global::Smart.FormDesigner.Demo.Properties.Resources.align_center_16x;
            this.tbAlignCenter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbAlignCenter.Name = "tbAlignCenter";
            this.tbAlignCenter.Size = new System.Drawing.Size(23, 22);
            this.tbAlignCenter.Text = "居中对齐";
            this.tbAlignCenter.Click += new System.EventHandler(this.miAlignCenter_Click);
            // 
            // tbAlignRight
            // 
            this.tbAlignRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbAlignRight.Enabled = false;
            this.tbAlignRight.Image = global::Smart.FormDesigner.Demo.Properties.Resources.align_right_16x;
            this.tbAlignRight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbAlignRight.Name = "tbAlignRight";
            this.tbAlignRight.Size = new System.Drawing.Size(23, 22);
            this.tbAlignRight.Text = "右对齐";
            this.tbAlignRight.Click += new System.EventHandler(this.miAlignRight_Click);
            // 
            // tbAlignTop
            // 
            this.tbAlignTop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbAlignTop.Enabled = false;
            this.tbAlignTop.Image = global::Smart.FormDesigner.Demo.Properties.Resources.align_top_16x;
            this.tbAlignTop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbAlignTop.Name = "tbAlignTop";
            this.tbAlignTop.Size = new System.Drawing.Size(23, 22);
            this.tbAlignTop.Text = "顶端对齐";
            this.tbAlignTop.Click += new System.EventHandler(this.miAlignTop_Click);
            // 
            // tbAlignMiddle
            // 
            this.tbAlignMiddle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbAlignMiddle.Enabled = false;
            this.tbAlignMiddle.Image = global::Smart.FormDesigner.Demo.Properties.Resources.align_middlle_16x;
            this.tbAlignMiddle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbAlignMiddle.Name = "tbAlignMiddle";
            this.tbAlignMiddle.Size = new System.Drawing.Size(23, 22);
            this.tbAlignMiddle.Text = "中间对齐";
            this.tbAlignMiddle.Click += new System.EventHandler(this.miAlignMiddle_Click);
            // 
            // tbAlignBottom
            // 
            this.tbAlignBottom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbAlignBottom.Enabled = false;
            this.tbAlignBottom.Image = global::Smart.FormDesigner.Demo.Properties.Resources.align_bottom_16x;
            this.tbAlignBottom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbAlignBottom.Name = "tbAlignBottom";
            this.tbAlignBottom.Size = new System.Drawing.Size(23, 22);
            this.tbAlignBottom.Text = "底端对齐";
            this.tbAlignBottom.Click += new System.EventHandler(this.miAlignBottom_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // tbSameWidth
            // 
            this.tbSameWidth.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbSameWidth.Enabled = false;
            this.tbSameWidth.Image = global::Smart.FormDesigner.Demo.Properties.Resources.same_width_16x;
            this.tbSameWidth.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbSameWidth.Name = "tbSameWidth";
            this.tbSameWidth.Size = new System.Drawing.Size(23, 22);
            this.tbSameWidth.Text = "使宽度相同";
            this.tbSameWidth.Click += new System.EventHandler(this.miSameWidth_Click);
            // 
            // tbSameHeight
            // 
            this.tbSameHeight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbSameHeight.Enabled = false;
            this.tbSameHeight.Image = global::Smart.FormDesigner.Demo.Properties.Resources.same_height_16x;
            this.tbSameHeight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbSameHeight.Name = "tbSameHeight";
            this.tbSameHeight.Size = new System.Drawing.Size(23, 22);
            this.tbSameHeight.Text = "使高度相同";
            this.tbSameHeight.Click += new System.EventHandler(this.miSameHeight_Click);
            // 
            // tbSameBoth
            // 
            this.tbSameBoth.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbSameBoth.Enabled = false;
            this.tbSameBoth.Image = global::Smart.FormDesigner.Demo.Properties.Resources.same_size_16x;
            this.tbSameBoth.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tbSameBoth.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbSameBoth.Name = "tbSameBoth";
            this.tbSameBoth.Size = new System.Drawing.Size(23, 22);
            this.tbSameBoth.Text = "使大小相同";
            this.tbSameBoth.Click += new System.EventHandler(this.miSameBoth_Click);
            // 
            // dockPanel
            // 
            this.dockPanel.AllowEndUserDocking = false;
            this.dockPanel.AllowEndUserNestedDocking = false;
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.DockBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(238)))), ((int)(((byte)(242)))));
            this.dockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow;
            this.dockPanel.Location = new System.Drawing.Point(0, 25);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Padding = new System.Windows.Forms.Padding(6);
            this.dockPanel.ShowAutoHideContentOnHover = false;
            this.dockPanel.ShowDocumentIcon = true;
            this.dockPanel.Size = new System.Drawing.Size(1008, 552);
            this.dockPanel.TabIndex = 8;
            this.dockPanel.Theme = this.vS2015LightTheme1;
            this.dockPanel.ActiveDocumentChanged += new System.EventHandler(this.dockPanel_ActiveDocumentChanged);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(1008, 577);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.toolStrip);
            this.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Menu = this.mainMenu;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "表单设计器";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private IContainer components;

        private MainMenu mainMenu;
        private MenuItem miNewForm;
        private MenuItem miOpenForm;
        private MenuItem miSaveForm;
        private MenuItem miExitDesigner;
        private MenuItem miCopy;
        private MenuItem miPaste;
        private MenuItem miUndo;
        private MenuItem miRedo;
        private MenuItem miAlignTop;
        private MenuItem miAlignMiddle;
        private MenuItem miAlignBottom;
        private MenuItem miAlignLeft;
        private MenuItem miAlignCenter;
        private MenuItem miAlignRight;
        private MenuItem miSameHeight;
        private MenuItem miSameWidth;
        private MenuItem miSameBoth;
        private MenuItem miDeleteSelection;
        private MenuItem miAbout;
        private MenuItem menuItem1;
        private MenuItem menuItem2;
        private MenuItem menuItem4;
        private MenuItem menuItem6;
        private MenuItem menuItem8;
        private MenuItem menuItem9;
        private MenuItem menuItem11;
        private MenuItem menuItem12;
        private MenuItem menuItem13;
        private MenuItem menuItem14;

        private ToolStrip toolStrip;
        private ToolStripButton tbNewForm;
        private ToolStripButton tbOpenForm;
        private ToolStripButton tbSaveForm;
        private ToolStripButton tbUndo;
        private ToolStripButton tbRedo;
        private ToolStripButton tbAlignLeft;
        private ToolStripButton tbAlignCenter;
        private ToolStripButton tbAlignRight;
        private ToolStripButton tbAlignTop;
        private ToolStripButton tbAlignMiddle;
        private ToolStripButton tbAlignBottom;
        private ToolStripButton tbSameWidth;
        private ToolStripButton tbSameHeight;
        private ToolStripButton tbSameBoth;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator4;

        private DockPanel dockPanel;
        private VS2015LightTheme vS2015LightTheme1;

        #endregion


    }
}
