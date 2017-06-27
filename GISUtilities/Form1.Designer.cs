namespace GISUtilities
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnChain = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnModel = new System.Windows.Forms.Button();
            this.btnVW = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnChain
            // 
            this.btnChain.Location = new System.Drawing.Point(669, 42);
            this.btnChain.Margin = new System.Windows.Forms.Padding(2);
            this.btnChain.Name = "btnChain";
            this.btnChain.Size = new System.Drawing.Size(116, 27);
            this.btnChain.TabIndex = 0;
            this.btnChain.Text = "断链合并";
            this.btnChain.UseVisualStyleBackColor = true;
            //this.btnChain.Click += new System.EventHandler(this.btnChain_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnModel
            // 
            this.btnModel.Location = new System.Drawing.Point(669, 83);
            this.btnModel.Name = "btnModel";
            this.btnModel.Size = new System.Drawing.Size(116, 28);
            this.btnModel.TabIndex = 1;
            this.btnModel.Text = "模型加载表处理";
            this.btnModel.UseVisualStyleBackColor = true;
            //this.btnModel.Click += new System.EventHandler(this.btnModel_Click);
            // 
            // btnVW
            // 
            this.btnVW.Location = new System.Drawing.Point(669, 131);
            this.btnVW.Name = "btnVW";
            this.btnVW.Size = new System.Drawing.Size(116, 28);
            this.btnVW.TabIndex = 1;
            this.btnVW.Text = "施工工点表处理";
            this.btnVW.UseVisualStyleBackColor = true;
            //this.btnVW.Click += new System.EventHandler(this.btnVW_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(345, 48);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 35);
            this.button1.TabIndex = 3;
            this.button1.Text = "七参数";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(345, 112);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "四参数";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(137, 83);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(826, 241);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnVW);
            this.Controls.Add(this.btnModel);
            this.Controls.Add(this.btnChain);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnChain;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnModel;
        private System.Windows.Forms.Button btnVW;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}

