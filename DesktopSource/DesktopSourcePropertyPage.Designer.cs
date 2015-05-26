namespace DesktopSource
{
    partial class DesktopSourcePropertyPage
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
            this.captureMethodCombo = new System.Windows.Forms.ComboBox();
            this.captureLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // captureMethodCombo
            // 
            this.captureMethodCombo.FormattingEnabled = true;
            this.captureMethodCombo.Location = new System.Drawing.Point(63, 11);
            this.captureMethodCombo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.captureMethodCombo.Name = "captureMethodCombo";
            this.captureMethodCombo.Size = new System.Drawing.Size(368, 21);
            this.captureMethodCombo.TabIndex = 0;
            this.captureMethodCombo.SelectedIndexChanged += new System.EventHandler(this.captureMethodCombo_SelectedIndexChanged);
            // 
            // captureLabel
            // 
            this.captureLabel.AutoSize = true;
            this.captureLabel.Location = new System.Drawing.Point(8, 13);
            this.captureLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.captureLabel.Name = "captureLabel";
            this.captureLabel.Size = new System.Drawing.Size(44, 13);
            this.captureLabel.TabIndex = 1;
            this.captureLabel.Text = "Capture";
            // 
            // DesktopSourcePropertyPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 296);
            this.Controls.Add(this.captureLabel);
            this.Controls.Add(this.captureMethodCombo);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "DesktopSourcePropertyPage";
            this.Text = "DesktopSourcePropertyPage";
            this.Title = "DesktopSourcePropertyPage";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox captureMethodCombo;
        private System.Windows.Forms.Label captureLabel;

    }
}