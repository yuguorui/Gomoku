namespace Gomoku
{
    partial class SettingUI
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
            this.label1 = new System.Windows.Forms.Label();
            this.UseNeuro = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Min_Max_Dipth = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "使用神经网络评估函数";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // UseNeuro
            // 
            this.UseNeuro.AutoSize = true;
            this.UseNeuro.Location = new System.Drawing.Point(234, 25);
            this.UseNeuro.Name = "UseNeuro";
            this.UseNeuro.Size = new System.Drawing.Size(15, 14);
            this.UseNeuro.TabIndex = 1;
            this.UseNeuro.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(15, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "极大-极小树搜索深度";
            // 
            // Min_Max_Dipth
            // 
            this.Min_Max_Dipth.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Min_Max_Dipth.Location = new System.Drawing.Point(207, 56);
            this.Min_Max_Dipth.Name = "Min_Max_Dipth";
            this.Min_Max_Dipth.Size = new System.Drawing.Size(42, 23);
            this.Min_Max_Dipth.TabIndex = 3;
            this.Min_Max_Dipth.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // Setting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.Min_Max_Dipth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.UseNeuro);
            this.Controls.Add(this.label1);
            this.Name = "Setting";
            this.Text = "Setting";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox UseNeuro;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Min_Max_Dipth;
    }
}