namespace Smart.FormDesigner.Demo
{
    partial class CustomForm
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
            this.defaultDesignerLoader1 = new Smart.FormDesigner.Serialization.DefaultDesignerLoader();
            this.designer1 = new Smart.FormDesigner.Designer();
            this.SuspendLayout();
            // 
            // designer1
            // 
            this.designer1.DesignedForm = this;
            this.designer1.DesignerLoader = this.defaultDesignerLoader1;
            this.designer1.GridSize = new System.Drawing.Size(8, 8);
            // 
            // CustomForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 267);
            this.KeyPreview = true;
            this.Name = "CustomForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "自定义表单";
            this.ResumeLayout(false);

        }

        #endregion
        private Serialization.DefaultDesignerLoader defaultDesignerLoader1;
        private Designer designer1;
    }
}