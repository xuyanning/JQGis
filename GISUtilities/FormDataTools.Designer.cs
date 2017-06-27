namespace GISUtilities
{
    partial class FormDataTools
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
            this.button1 = new System.Windows.Forms.Button();
            this.btnHeight = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(17, 51);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 48);
            this.button1.TabIndex = 0;
            this.button1.Text = "ChainInfo+Server->ProjectInfo+3DMaxLine";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnHeight
            // 
            this.btnHeight.Location = new System.Drawing.Point(19, 12);
            this.btnHeight.Name = "btnHeight";
            this.btnHeight.Size = new System.Drawing.Size(113, 33);
            this.btnHeight.TabIndex = 1;
            this.btnHeight.Text = "ChainInfo+坡度-》三维坐标TXT";
            this.btnHeight.UseVisualStyleBackColor = true;
            this.btnHeight.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(149, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(596, 33);
            this.label1.TabIndex = 2;
            this.label1.Text = "读取C：\\GISData\\Common\\pd.xlsx获取坡度表，读取sqlite或者excel二维线路信息，计算生成包含高程的三维链路TXT，需要手工导入到GI" +
    "SDB中的ChainInfo等";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(149, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(596, 33);
            this.label2.TabIndex = 3;
            this.label2.Text = "根据GisDB中的ChainInfo，Server端工点表，生成本地工点表TXT以及用于建模的三维中线Txt文件。注意，由于Server端数据错误，里程非法的工点" +
    "表需要人工确认";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(19, 105);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(115, 58);
            this.button2.TabIndex = 4;
            this.button2.Text = "本地里程+本地工点+Server桥墩-》本地桥墩等单位工程表";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(149, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(596, 33);
            this.label3.TabIndex = 3;
            this.label3.Text = "生成PierInfo以及ContPierInfo分别导入到skyline加载数据用的Excel表，注意文件名、表名以及列属性（设置为常规）";
            // 
            // FormDataTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(769, 261);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnHeight);
            this.Controls.Add(this.button1);
            this.Name = "FormDataTools";
            this.Text = "GIS数据处理工具";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnHeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
    }
}