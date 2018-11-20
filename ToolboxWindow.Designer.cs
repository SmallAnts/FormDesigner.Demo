namespace Smart.FormDesigner.Demo
{
    partial class ToolboxWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Toolbox = new Smart.FormDesigner.ToolboxControl();
            this.SuspendLayout();
            // 
            // Toolbox
            // 
            this.Toolbox.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.Toolbox.Designer = null;
            this.Toolbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Toolbox.Location = new System.Drawing.Point(0, 0);
            this.Toolbox.Name = "Toolbox";
            this.Toolbox.Size = new System.Drawing.Size(184, 461);
            this.Toolbox.TabIndex = 0;
            // 
            // ToolboxWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(184, 461);
            this.Controls.Add(this.Toolbox);
            this.Name = "ToolboxWindow";
            this.Text = "工具箱";
            this.ResumeLayout(false);

        }

        #endregion

        internal ToolboxControl Toolbox;
    }
}