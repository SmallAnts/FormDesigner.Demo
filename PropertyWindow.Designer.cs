namespace Smart.FormDesigner.Demo
{
    partial class PropertyWindow
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
            this.Propertybox = new Smart.FormDesigner.PropertyboxControl();
            this.SuspendLayout();
            // 
            // Propertybox
            // 
            this.Propertybox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Propertybox.Location = new System.Drawing.Point(0, 0);
            this.Propertybox.Name = "Propertybox";
            this.Propertybox.Size = new System.Drawing.Size(204, 461);
            this.Propertybox.TabIndex = 0;
            // 
            // PropertyWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(204, 461);
            this.Controls.Add(this.Propertybox);
            this.Name = "PropertyWindow";
            this.Text = "属性";
            this.ResumeLayout(false);

        }

        #endregion

        internal PropertyboxControl Propertybox;
    }
}