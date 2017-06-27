namespace RailwayGIS
{
    partial class TestForm
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
            this.videoPanel1 = new RailwayGIS.VideoPanel();
            this.prjDetail1 = new RailwayGIS.PrjDetail();
            this.listView1 = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // videoPanel1
            // 
            this.videoPanel1.Location = new System.Drawing.Point(74, 319);
            this.videoPanel1.Name = "videoPanel1";
            this.videoPanel1.Size = new System.Drawing.Size(702, 215);
            this.videoPanel1.TabIndex = 0;
            // 
            // prjDetail1
            // 
            this.prjDetail1.Location = new System.Drawing.Point(170, 84);
            this.prjDetail1.Name = "prjDetail1";
            this.prjDetail1.Size = new System.Drawing.Size(1815, 333);
            this.prjDetail1.TabIndex = 1;
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(85, 127);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(121, 97);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(838, 563);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.prjDetail1);
            this.Controls.Add(this.videoPanel1);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.ResumeLayout(false);

        }

        #endregion

        private VideoPanel videoPanel1;
        private PrjDetail prjDetail1;
        private System.Windows.Forms.ListView listView1;
    }
}