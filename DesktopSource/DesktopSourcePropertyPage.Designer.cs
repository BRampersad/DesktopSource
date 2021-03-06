﻿namespace DesktopSource
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
            this.captureSettingsLbl = new System.Windows.Forms.Label();
            this.adapterLbl = new System.Windows.Forms.Label();
            this.outputLbl = new System.Windows.Forms.Label();
            this.regionLbl = new System.Windows.Forms.Label();
            this.topLbl = new System.Windows.Forms.Label();
            this.leftLbl = new System.Windows.Forms.Label();
            this.bottomLbl = new System.Windows.Forms.Label();
            this.rightLbl = new System.Windows.Forms.Label();
            this.topTextBox = new System.Windows.Forms.TextBox();
            this.leftTextBox = new System.Windows.Forms.TextBox();
            this.bottomTextBox = new System.Windows.Forms.TextBox();
            this.rightTextBox = new System.Windows.Forms.TextBox();
            this.refreshBtn = new System.Windows.Forms.Button();
            this.adapterTxtBox = new System.Windows.Forms.TextBox();
            this.outputTxtBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // captureMethodCombo
            // 
            this.captureMethodCombo.FormattingEnabled = true;
            this.captureMethodCombo.Location = new System.Drawing.Point(99, 6);
            this.captureMethodCombo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.captureMethodCombo.Name = "captureMethodCombo";
            this.captureMethodCombo.Size = new System.Drawing.Size(164, 21);
            this.captureMethodCombo.TabIndex = 0;
            this.captureMethodCombo.SelectedIndexChanged += new System.EventHandler(this.captureMethodCombo_SelectedIndexChanged);
            // 
            // captureLabel
            // 
            this.captureLabel.AutoSize = true;
            this.captureLabel.Location = new System.Drawing.Point(8, 8);
            this.captureLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.captureLabel.Name = "captureLabel";
            this.captureLabel.Size = new System.Drawing.Size(44, 13);
            this.captureLabel.TabIndex = 1;
            this.captureLabel.Text = "Capture";
            // 
            // captureSettingsLbl
            // 
            this.captureSettingsLbl.AutoSize = true;
            this.captureSettingsLbl.Location = new System.Drawing.Point(8, 33);
            this.captureSettingsLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.captureSettingsLbl.Name = "captureSettingsLbl";
            this.captureSettingsLbl.Size = new System.Drawing.Size(85, 13);
            this.captureSettingsLbl.TabIndex = 2;
            this.captureSettingsLbl.Text = "Capture Settings";
            // 
            // adapterLbl
            // 
            this.adapterLbl.AutoSize = true;
            this.adapterLbl.Location = new System.Drawing.Point(99, 33);
            this.adapterLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.adapterLbl.Name = "adapterLbl";
            this.adapterLbl.Size = new System.Drawing.Size(44, 13);
            this.adapterLbl.TabIndex = 3;
            this.adapterLbl.Text = "Adapter";
            // 
            // outputLbl
            // 
            this.outputLbl.AutoSize = true;
            this.outputLbl.Location = new System.Drawing.Point(100, 54);
            this.outputLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.outputLbl.Name = "outputLbl";
            this.outputLbl.Size = new System.Drawing.Size(39, 13);
            this.outputLbl.TabIndex = 4;
            this.outputLbl.Text = "Output";
            // 
            // regionLbl
            // 
            this.regionLbl.AutoSize = true;
            this.regionLbl.Location = new System.Drawing.Point(99, 80);
            this.regionLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.regionLbl.Name = "regionLbl";
            this.regionLbl.Size = new System.Drawing.Size(41, 13);
            this.regionLbl.TabIndex = 5;
            this.regionLbl.Text = "Region";
            // 
            // topLbl
            // 
            this.topLbl.AutoSize = true;
            this.topLbl.Location = new System.Drawing.Point(157, 80);
            this.topLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.topLbl.Name = "topLbl";
            this.topLbl.Size = new System.Drawing.Size(26, 13);
            this.topLbl.TabIndex = 8;
            this.topLbl.Text = "Top";
            // 
            // leftLbl
            // 
            this.leftLbl.AutoSize = true;
            this.leftLbl.Location = new System.Drawing.Point(157, 101);
            this.leftLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.leftLbl.Name = "leftLbl";
            this.leftLbl.Size = new System.Drawing.Size(25, 13);
            this.leftLbl.TabIndex = 9;
            this.leftLbl.Text = "Left";
            // 
            // bottomLbl
            // 
            this.bottomLbl.AutoSize = true;
            this.bottomLbl.Location = new System.Drawing.Point(145, 122);
            this.bottomLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.bottomLbl.Name = "bottomLbl";
            this.bottomLbl.Size = new System.Drawing.Size(40, 13);
            this.bottomLbl.TabIndex = 10;
            this.bottomLbl.Text = "Bottom";
            // 
            // rightLbl
            // 
            this.rightLbl.AutoSize = true;
            this.rightLbl.Location = new System.Drawing.Point(154, 144);
            this.rightLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.rightLbl.Name = "rightLbl";
            this.rightLbl.Size = new System.Drawing.Size(32, 13);
            this.rightLbl.TabIndex = 11;
            this.rightLbl.Text = "Right";
            // 
            // topTextBox
            // 
            this.topTextBox.Location = new System.Drawing.Point(195, 80);
            this.topTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.topTextBox.Name = "topTextBox";
            this.topTextBox.Size = new System.Drawing.Size(68, 20);
            this.topTextBox.TabIndex = 12;
            // 
            // leftTextBox
            // 
            this.leftTextBox.Location = new System.Drawing.Point(195, 101);
            this.leftTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.leftTextBox.Name = "leftTextBox";
            this.leftTextBox.Size = new System.Drawing.Size(68, 20);
            this.leftTextBox.TabIndex = 13;
            // 
            // bottomTextBox
            // 
            this.bottomTextBox.Location = new System.Drawing.Point(195, 122);
            this.bottomTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.bottomTextBox.Name = "bottomTextBox";
            this.bottomTextBox.Size = new System.Drawing.Size(68, 20);
            this.bottomTextBox.TabIndex = 14;
            // 
            // rightTextBox
            // 
            this.rightTextBox.Location = new System.Drawing.Point(195, 144);
            this.rightTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rightTextBox.Name = "rightTextBox";
            this.rightTextBox.Size = new System.Drawing.Size(68, 20);
            this.rightTextBox.TabIndex = 15;
            // 
            // refreshBtn
            // 
            this.refreshBtn.Location = new System.Drawing.Point(11, 144);
            this.refreshBtn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.refreshBtn.Name = "refreshBtn";
            this.refreshBtn.Size = new System.Drawing.Size(50, 17);
            this.refreshBtn.TabIndex = 18;
            this.refreshBtn.Text = "Refresh";
            this.refreshBtn.UseVisualStyleBackColor = true;
            this.refreshBtn.Click += new System.EventHandler(this.refreshBtn_Click);
            // 
            // adapterTxtBox
            // 
            this.adapterTxtBox.Location = new System.Drawing.Point(147, 33);
            this.adapterTxtBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.adapterTxtBox.Name = "adapterTxtBox";
            this.adapterTxtBox.Size = new System.Drawing.Size(116, 20);
            this.adapterTxtBox.TabIndex = 19;
            // 
            // outputTxtBox
            // 
            this.outputTxtBox.Location = new System.Drawing.Point(147, 54);
            this.outputTxtBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.outputTxtBox.Name = "outputTxtBox";
            this.outputTxtBox.Size = new System.Drawing.Size(116, 20);
            this.outputTxtBox.TabIndex = 20;
            // 
            // DesktopSourcePropertyPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 170);
            this.Controls.Add(this.outputTxtBox);
            this.Controls.Add(this.adapterTxtBox);
            this.Controls.Add(this.refreshBtn);
            this.Controls.Add(this.rightTextBox);
            this.Controls.Add(this.bottomTextBox);
            this.Controls.Add(this.leftTextBox);
            this.Controls.Add(this.topTextBox);
            this.Controls.Add(this.rightLbl);
            this.Controls.Add(this.bottomLbl);
            this.Controls.Add(this.leftLbl);
            this.Controls.Add(this.topLbl);
            this.Controls.Add(this.regionLbl);
            this.Controls.Add(this.outputLbl);
            this.Controls.Add(this.adapterLbl);
            this.Controls.Add(this.captureSettingsLbl);
            this.Controls.Add(this.captureLabel);
            this.Controls.Add(this.captureMethodCombo);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "DesktopSourcePropertyPage";
            this.Text = "DesktopSourcePropertyPage";
            this.Title = "DesktopSourcePropertyPage";
            this.Load += new System.EventHandler(this.DesktopSourcePropertyPage_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox captureMethodCombo;
        private System.Windows.Forms.Label captureLabel;
        private System.Windows.Forms.Label captureSettingsLbl;
        private System.Windows.Forms.Label adapterLbl;
        private System.Windows.Forms.Label outputLbl;
        private System.Windows.Forms.Label regionLbl;
        private System.Windows.Forms.Label topLbl;
        private System.Windows.Forms.Label leftLbl;
        private System.Windows.Forms.Label bottomLbl;
        private System.Windows.Forms.Label rightLbl;
        private System.Windows.Forms.TextBox topTextBox;
        private System.Windows.Forms.TextBox leftTextBox;
        private System.Windows.Forms.TextBox bottomTextBox;
        private System.Windows.Forms.TextBox rightTextBox;
        private System.Windows.Forms.Button refreshBtn;
        private System.Windows.Forms.TextBox adapterTxtBox;
        private System.Windows.Forms.TextBox outputTxtBox;

    }
}