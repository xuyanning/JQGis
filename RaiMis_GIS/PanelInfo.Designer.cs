namespace RailwayGIS
{
    partial class PanelInfo
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.labelConsLog = new System.Windows.Forms.Label();
            this.labelDistance = new System.Windows.Forms.Label();
            this.labelNextProj = new System.Windows.Forms.Label();
            this.labelNavTotal = new System.Windows.Forms.Label();
            this.labelMileage = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelConsLog
            // 
            this.labelConsLog.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelConsLog.Location = new System.Drawing.Point(3, 108);
            this.labelConsLog.Name = "labelConsLog";
            this.labelConsLog.Size = new System.Drawing.Size(251, 81);
            this.labelConsLog.TabIndex = 1;
            this.labelConsLog.Text = "附近5公里实名情况:\\t\\t\\n.......................................";
            // 
            // labelDistance
            // 
            this.labelDistance.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelDistance.Location = new System.Drawing.Point(3, 88);
            this.labelDistance.Name = "labelDistance";
            this.labelDistance.Size = new System.Drawing.Size(239, 15);
            this.labelDistance.TabIndex = 2;
            this.labelDistance.Text = "距离:";
            this.labelDistance.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelNextProj
            // 
            this.labelNextProj.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelNextProj.Location = new System.Drawing.Point(3, 69);
            this.labelNextProj.Name = "labelNextProj";
            this.labelNextProj.Size = new System.Drawing.Size(239, 15);
            this.labelNextProj.TabIndex = 3;
            this.labelNextProj.Text = "下一工点";
            this.labelNextProj.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelNavTotal
            // 
            this.labelNavTotal.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelNavTotal.Location = new System.Drawing.Point(3, 28);
            this.labelNavTotal.Name = "labelNavTotal";
            this.labelNavTotal.Size = new System.Drawing.Size(239, 15);
            this.labelNavTotal.TabIndex = 4;
            this.labelNavTotal.Text = "巡视进度";
            this.labelNavTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelMileage
            // 
            this.labelMileage.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelMileage.Location = new System.Drawing.Point(3, 48);
            this.labelMileage.Name = "labelMileage";
            this.labelMileage.Size = new System.Drawing.Size(239, 15);
            this.labelMileage.TabIndex = 5;
            this.labelMileage.Text = "当前里程";
            this.labelMileage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(254, 18);
            this.label1.TabIndex = 6;
            this.label1.Text = "尝试扫一扫";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // PanelInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelConsLog);
            this.Controls.Add(this.labelDistance);
            this.Controls.Add(this.labelNextProj);
            this.Controls.Add(this.labelNavTotal);
            this.Controls.Add(this.labelMileage);
            this.Name = "PanelInfo";
            this.Size = new System.Drawing.Size(257, 194);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelConsLog;
        private System.Windows.Forms.Label labelDistance;
        private System.Windows.Forms.Label labelNextProj;
        private System.Windows.Forms.Label labelNavTotal;
        private System.Windows.Forms.Label labelMileage;
        private System.Windows.Forms.Label label1;
    }
}
