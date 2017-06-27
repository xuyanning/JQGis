namespace RailwayGIS
{
    partial class MessageForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.panel1 = new System.Windows.Forms.Panel();
            this.listView1 = new System.Windows.Forms.ListView();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.propertyGridEx4 = new RailwayGIS.PropertyGridEx();
            this.propertyGridEx3 = new RailwayGIS.PropertyGridEx();
            this.propertyGridEx2 = new RailwayGIS.PropertyGridEx();
            this.propertyGridEx1 = new RailwayGIS.PropertyGridEx();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.Location = new System.Drawing.Point(567, -78);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(414, 293);
            this.webBrowser1.TabIndex = 4;
            this.webBrowser1.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.listView1);
            this.panel1.Controls.Add(this.chart1);
            this.panel1.Controls.Add(this.propertyGridEx4);
            this.panel1.Controls.Add(this.propertyGridEx3);
            this.panel1.Controls.Add(this.propertyGridEx2);
            this.panel1.Controls.Add(this.propertyGridEx1);
            this.panel1.Location = new System.Drawing.Point(-1, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1015, 536);
            this.panel1.TabIndex = 23;
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(3, 349);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1002, 181);
            this.listView1.TabIndex = 23;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // chart1
            // 
            chartArea1.CursorX.Interval = 2D;
            chartArea1.CursorX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Days;
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Location = new System.Drawing.Point(339, 3);
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
            this.chart1.Size = new System.Drawing.Size(666, 167);
            this.chart1.TabIndex = 18;
            this.chart1.Text = "chartProgress";
            // 
            // timer1
            // 
            this.timer1.Interval = 10000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // propertyGridEx4
            // 
            this.propertyGridEx4.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.propertyGridEx4.FirstColWidth = 120;
            this.propertyGridEx4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.propertyGridEx4.HelpVisible = false;
            this.propertyGridEx4.Location = new System.Drawing.Point(675, 176);
            this.propertyGridEx4.Name = "propertyGridEx4";
            this.propertyGridEx4.Size = new System.Drawing.Size(330, 167);
            this.propertyGridEx4.TabIndex = 17;
            this.propertyGridEx4.ToolbarVisible = false;
            this.propertyGridEx4.ViewForeColor = System.Drawing.Color.Red;
            // 
            // propertyGridEx3
            // 
            this.propertyGridEx3.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.propertyGridEx3.FirstColWidth = 120;
            this.propertyGridEx3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.propertyGridEx3.HelpVisible = false;
            this.propertyGridEx3.Location = new System.Drawing.Point(339, 176);
            this.propertyGridEx3.Name = "propertyGridEx3";
            this.propertyGridEx3.Size = new System.Drawing.Size(330, 167);
            this.propertyGridEx3.TabIndex = 16;
            this.propertyGridEx3.ToolbarVisible = false;
            this.propertyGridEx3.ViewForeColor = System.Drawing.Color.ForestGreen;
            // 
            // propertyGridEx2
            // 
            this.propertyGridEx2.CommandsActiveLinkColor = System.Drawing.Color.Blue;
            this.propertyGridEx2.DisabledItemForeColor = System.Drawing.SystemColors.HotTrack;
            this.propertyGridEx2.FirstColWidth = 120;
            this.propertyGridEx2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.propertyGridEx2.HelpVisible = false;
            this.propertyGridEx2.Location = new System.Drawing.Point(3, 176);
            this.propertyGridEx2.Name = "propertyGridEx2";
            this.propertyGridEx2.Size = new System.Drawing.Size(330, 167);
            this.propertyGridEx2.TabIndex = 15;
            this.propertyGridEx2.ToolbarVisible = false;
            this.propertyGridEx2.ViewForeColor = System.Drawing.Color.Blue;
            // 
            // propertyGridEx1
            // 
            this.propertyGridEx1.FirstColWidth = 120;
            this.propertyGridEx1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.propertyGridEx1.HelpVisible = false;
            this.propertyGridEx1.Location = new System.Drawing.Point(3, 3);
            this.propertyGridEx1.Name = "propertyGridEx1";
            this.propertyGridEx1.Size = new System.Drawing.Size(330, 167);
            this.propertyGridEx1.TabIndex = 14;
            this.propertyGridEx1.ToolbarVisible = false;
            // 
            // MessageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(1009, 535);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.webBrowser1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "MessageForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MessageForm";
            this.TopMost = true;
            this.VisibleChanged += new System.EventHandler(this.MessageForm_VisibleChanged);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private PropertyGridEx propertyGridEx4;
        private PropertyGridEx propertyGridEx3;
        private PropertyGridEx propertyGridEx2;
        private PropertyGridEx propertyGridEx1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Timer timer1;
    }
}