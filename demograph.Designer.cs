namespace AvionicsInstrumentControlDemo
{
    partial class graphWindow
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.chartTemp = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.hideButton = new System.Windows.Forms.Button();
            this.chartAltitude = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chartTemp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartAltitude)).BeginInit();
            this.SuspendLayout();
            // 
            // chartTemp
            // 
            chartArea3.Name = "ChartArea1";
            this.chartTemp.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.chartTemp.Legends.Add(legend3);
            this.chartTemp.Location = new System.Drawing.Point(12, 12);
            this.chartTemp.Name = "chartTemp";
            this.chartTemp.Size = new System.Drawing.Size(982, 400);
            this.chartTemp.TabIndex = 0;
            this.chartTemp.Text = "chart1";
            // 
            // hideButton
            // 
            this.hideButton.Location = new System.Drawing.Point(877, 804);
            this.hideButton.Name = "hideButton";
            this.hideButton.Size = new System.Drawing.Size(117, 42);
            this.hideButton.TabIndex = 1;
            this.hideButton.Text = "Hide";
            this.hideButton.UseVisualStyleBackColor = true;
            this.hideButton.Click += new System.EventHandler(this.hideButton_Click);
            // 
            // chartAltitude
            // 
            chartArea4.Name = "ChartArea1";
            this.chartAltitude.ChartAreas.Add(chartArea4);
            legend4.Name = "Legend1";
            this.chartAltitude.Legends.Add(legend4);
            this.chartAltitude.Location = new System.Drawing.Point(12, 418);
            this.chartAltitude.Name = "chartAltitude";
            this.chartAltitude.Size = new System.Drawing.Size(982, 380);
            this.chartAltitude.TabIndex = 1;
            this.chartAltitude.Text = "chart2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(561, 817);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            // 
            // graphWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 853);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chartAltitude);
            this.Controls.Add(this.hideButton);
            this.Controls.Add(this.chartTemp);
            this.Name = "graphWindow";
            this.Text = "Data Graph";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.graphWindow_FormClosing);
            this.Load += new System.EventHandler(this.graphWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartTemp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartAltitude)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartTemp;
        private System.Windows.Forms.Button hideButton;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartAltitude;
        private System.Windows.Forms.Label label1;
    }
}