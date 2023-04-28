
namespace Shrexxeso
{
    partial class PopupWhenEnded
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopupWhenEnded));
            this.Label_Result = new System.Windows.Forms.Label();
            this.button_continue = new System.Windows.Forms.Button();
            this.button_exit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Label_Result
            // 
            this.Label_Result.AutoSize = true;
            this.Label_Result.Font = new System.Drawing.Font("Segoe UI", 28.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Label_Result.Location = new System.Drawing.Point(55, 10);
            this.Label_Result.Name = "Label_Result";
            this.Label_Result.Size = new System.Drawing.Size(288, 62);
            this.Label_Result.TabIndex = 0;
            this.Label_Result.Text = "Shrek Wins!";
            // 
            // button_continue
            // 
            this.button_continue.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_continue.Location = new System.Drawing.Point(40, 90);
            this.button_continue.Name = "button_continue";
            this.button_continue.Size = new System.Drawing.Size(130, 50);
            this.button_continue.TabIndex = 1;
            this.button_continue.Text = "Try Again";
            this.button_continue.UseVisualStyleBackColor = true;
            // 
            // button_exit
            // 
            this.button_exit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_exit.Location = new System.Drawing.Point(220, 90);
            this.button_exit.Name = "button_exit";
            this.button_exit.Size = new System.Drawing.Size(130, 50);
            this.button_exit.TabIndex = 2;
            this.button_exit.Text = "Exit";
            this.button_exit.UseVisualStyleBackColor = true;
            // 
            // PopupWhenEnded
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 153);
            this.Controls.Add(this.button_exit);
            this.Controls.Add(this.button_continue);
            this.Controls.Add(this.Label_Result);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PopupWhenEnded";
            this.RightToLeftLayout = true;
            this.Text = "Shrexxeso";
            this.Load += new System.EventHandler(this.PopupWhenEnded_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Label_Result;
        private System.Windows.Forms.Button button_continue;
        private System.Windows.Forms.Button button_exit;
    }
}