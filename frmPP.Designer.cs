namespace AvionicsInstrumentControlDemo
{
    partial class frmPP
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
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.txtLat = new System.Windows.Forms.TextBox();
            this.txtLong = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.btnMapIt = new System.Windows.Forms.Button();
            this.serialOpenButton = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.serialCloseButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtLat
            // 
            this.txtLat.Location = new System.Drawing.Point(127, 26);
            this.txtLat.Name = "txtLat";
            this.txtLat.Size = new System.Drawing.Size(102, 22);
            this.txtLat.TabIndex = 0;
            // 
            // txtLong
            // 
            this.txtLong.Location = new System.Drawing.Point(127, 63);
            this.txtLong.Name = "txtLong";
            this.txtLong.Size = new System.Drawing.Size(102, 22);
            this.txtLong.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Latitude";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Longitude";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(25, 234);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(125, 41);
            this.button1.TabIndex = 4;
            this.button1.Text = "Stop Updates";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnMapIt
            // 
            this.btnMapIt.Location = new System.Drawing.Point(203, 234);
            this.btnMapIt.Name = "btnMapIt";
            this.btnMapIt.Size = new System.Drawing.Size(127, 41);
            this.btnMapIt.TabIndex = 5;
            this.btnMapIt.Text = "Map Location";
            this.btnMapIt.UseVisualStyleBackColor = true;
            this.btnMapIt.Click += new System.EventHandler(this.btnMapIt_Click);
            // 
            // serialOpenButton
            // 
            this.serialOpenButton.Location = new System.Drawing.Point(127, 172);
            this.serialOpenButton.Name = "serialOpenButton";
            this.serialOpenButton.Size = new System.Drawing.Size(106, 41);
            this.serialOpenButton.TabIndex = 6;
            this.serialOpenButton.Text = "Open";
            this.serialOpenButton.UseVisualStyleBackColor = true;
            this.serialOpenButton.Click += new System.EventHandler(this.serialOpenButton_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(125, 117);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(104, 24);
            this.comboBox1.TabIndex = 7;
            this.comboBox1.Text = "COM1";
            // 
            // serialCloseButton
            // 
            this.serialCloseButton.Location = new System.Drawing.Point(277, 171);
            this.serialCloseButton.Name = "serialCloseButton";
            this.serialCloseButton.Size = new System.Drawing.Size(77, 41);
            this.serialCloseButton.TabIndex = 8;
            this.serialCloseButton.Text = "Close";
            this.serialCloseButton.UseVisualStyleBackColor = true;
            this.serialCloseButton.Click += new System.EventHandler(this.serialCloseButton_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(23, 176);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(89, 35);
            this.button2.TabIndex = 9;
            this.button2.Text = "New Map";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // frmPP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(374, 322);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.serialCloseButton);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.serialOpenButton);
            this.Controls.Add(this.btnMapIt);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtLong);
            this.Controls.Add(this.txtLat);
            this.Name = "frmPP";
            this.Text = "GPS";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox txtLat;
        private System.Windows.Forms.TextBox txtLong;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnMapIt;
        private System.Windows.Forms.Button serialOpenButton;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button serialCloseButton;
        private System.Windows.Forms.Button button2;
    }
}