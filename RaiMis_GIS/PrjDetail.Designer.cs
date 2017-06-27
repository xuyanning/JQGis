namespace RailwayGIS
{
    partial class PrjDetail
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.propertyGridEx4 = new RailwayGIS.PropertyGridEx();
            this.propertyGridEx3 = new RailwayGIS.PropertyGridEx();
            this.propertyGridEx2 = new RailwayGIS.PropertyGridEx();
            this.propertyGridEx1 = new RailwayGIS.PropertyGridEx();
            this.listView1 = new System.Windows.Forms.ListView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 4000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.chart1);
            this.panel1.Controls.Add(this.propertyGridEx4);
            this.panel1.Controls.Add(this.propertyGridEx3);
            this.panel1.Controls.Add(this.propertyGridEx2);
            this.panel1.Controls.Add(this.propertyGridEx1);
            this.panel1.Location = new System.Drawing.Point(6, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1806, 184);
            this.panel1.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 184);
            this.label1.TabIndex = 22;
            this.label1.Text = "工程进度";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chart1
            // 
            chartArea1.CursorX.Interval = 2D;
            chartArea1.CursorX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Days;
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Location = new System.Drawing.Point(1383, 3);
            this.chart1.Name = "chart1";
            series1.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.LeftRight;
            series1.BorderWidth = 3;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.Red;
            series1.Name = "分项工程1";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Date;
            series1.YValuesPerPoint = 10;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series2.BorderWidth = 3;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Color = System.Drawing.Color.Green;
            series2.Name = "分项工程2";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Date;
            series3.BorderWidth = 3;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Color = System.Drawing.Color.Blue;
            series3.Name = "分项工程3";
            series3.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Date;
            this.chart1.Series.Add(series1);
            this.chart1.Series.Add(series2);
            this.chart1.Series.Add(series3);
            this.chart1.Size = new System.Drawing.Size(411, 167);
            this.chart1.TabIndex = 18;
            this.chart1.Text = "chartProgress";
            // 
            // propertyGridEx4
            // 
            this.propertyGridEx4.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.propertyGridEx4.FirstColWidth = 120;
            this.propertyGridEx4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.propertyGridEx4.HelpVisible = false;
            this.propertyGridEx4.Location = new System.Drawing.Point(1045, 3);
            this.propertyGridEx4.Name = "propertyGridEx4";
            this.propertyGridEx4.Size = new System.Drawing.Size(330, 167);
            this.propertyGridEx4.TabIndex = 17;
            this.propertyGridEx4.ToolbarVisible = false;
            this.propertyGridEx4.ViewForeColor = System.Drawing.Color.Blue;
            // 
            // propertyGridEx3
            // 
            this.propertyGridEx3.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.propertyGridEx3.FirstColWidth = 120;
            this.propertyGridEx3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.propertyGridEx3.HelpVisible = false;
            this.propertyGridEx3.Location = new System.Drawing.Point(707, 3);
            this.propertyGridEx3.Name = "propertyGridEx3";
            this.propertyGridEx3.Size = new System.Drawing.Size(330, 167);
            this.propertyGridEx3.TabIndex = 16;
            this.propertyGridEx3.ToolbarVisible = false;
            this.propertyGridEx3.ViewForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            // 
            // propertyGridEx2
            // 
            this.propertyGridEx2.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.propertyGridEx2.FirstColWidth = 120;
            this.propertyGridEx2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.propertyGridEx2.HelpVisible = false;
            this.propertyGridEx2.Location = new System.Drawing.Point(370, 3);
            this.propertyGridEx2.Name = "propertyGridEx2";
            this.propertyGridEx2.Size = new System.Drawing.Size(330, 167);
            this.propertyGridEx2.TabIndex = 15;
            this.propertyGridEx2.ToolbarVisible = false;
            this.propertyGridEx2.ViewForeColor = System.Drawing.Color.Red;
            // 
            // propertyGridEx1
            // 
            this.propertyGridEx1.FirstColWidth = 120;
            this.propertyGridEx1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.propertyGridEx1.HelpVisible = false;
            this.propertyGridEx1.Location = new System.Drawing.Point(32, 3);
            this.propertyGridEx1.Name = "propertyGridEx1";
            this.propertyGridEx1.Size = new System.Drawing.Size(330, 167);
            this.propertyGridEx1.TabIndex = 14;
            this.propertyGridEx1.ToolbarVisible = false;
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(1761, 70);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(48, 47);
            this.listView1.TabIndex = 21;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.Visible = false;
            // 
            // PrjDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.panel1);
            this.Name = "PrjDetail";
            this.Size = new System.Drawing.Size(1815, 195);
            this.VisibleChanged += new System.EventHandler(this.PrjDetail_VisibleChanged);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private PropertyGridEx propertyGridEx4;
        private PropertyGridEx propertyGridEx3;
        private PropertyGridEx propertyGridEx2;
        private PropertyGridEx propertyGridEx1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Label label1;
    }
}
