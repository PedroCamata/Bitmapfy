namespace Bitmapfy
{
    partial class Form1
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
            this.FileSelectButton = new System.Windows.Forms.Button();
            this.Status = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // FileSelectButton
            // 
            this.FileSelectButton.Location = new System.Drawing.Point(12, 22);
            this.FileSelectButton.Name = "FileSelectButton";
            this.FileSelectButton.Size = new System.Drawing.Size(200, 40);
            this.FileSelectButton.TabIndex = 2;
            this.FileSelectButton.Text = "Select File";
            this.FileSelectButton.UseVisualStyleBackColor = true;
            this.FileSelectButton.Click += new System.EventHandler(this.FileSelectButton_Click);
            // 
            // Status
            // 
            this.Status.AutoSize = true;
            this.Status.Location = new System.Drawing.Point(228, 34);
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(105, 17);
            this.Status.TabIndex = 3;
            this.Status.Text = "No file selected";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 76);
            this.Controls.Add(this.Status);
            this.Controls.Add(this.FileSelectButton);
            this.Name = "Form1";
            this.Text = "Bitmapfy";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button FileSelectButton;
        private System.Windows.Forms.Label Status;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}

