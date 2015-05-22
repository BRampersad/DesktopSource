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
            this.captureMethodCombo.Location = new System.Drawing.Point(94, 17);
            this.captureMethodCombo.Name = "captureMethodCombo";
            this.captureMethodCombo.Size = new System.Drawing.Size(550, 28);
            this.captureMethodCombo.TabIndex = 0;
            // 
            // captureLabel
            // 
            this.captureLabel.AutoSize = true;
            this.captureLabel.Location = new System.Drawing.Point(12, 20);
            this.captureLabel.Name = "captureLabel";
            this.captureLabel.Size = new System.Drawing.Size(66, 20);
            this.captureLabel.TabIndex = 1;
            this.captureLabel.Text = "Capture";
            // 
            // DesktopSourcePropertyPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 455);
            this.Controls.Add(this.captureLabel);
            this.Controls.Add(this.captureMethodCombo);
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