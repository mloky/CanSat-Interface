using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AvionicsInstrumentControlDemo
{
    public partial class frmPP : Form
    {
        public frmPP()
        {
            InitializeComponent();
        }
        #region Member Variables

        // Local variables used to hold the present
        // position as latitude and longitude
        public string Latitude;
        public string Longitude;

        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                string data = serialPort1.ReadExisting();
                string[] strArr = data.Split('$');
                for (int i = 0; i < strArr.Length; i++)
                {
                    string strTemp = strArr[i];
                    string[] lineArr = strTemp.Split(',');
                    if (lineArr[0] == "GPGGA")
                    {
                        try
                        {
                            //Latitude
                            Double dLat = Convert.ToDouble(lineArr[2]);
                            dLat = dLat / 100;
                            string[] lat = dLat.ToString().Split('.');
                            Latitude = lineArr[3].ToString() +
                            lat[0].ToString() + "." +
                            ((Convert.ToDouble(lat[1]) /
                            60)).ToString("#####");

                            //Longitude
                            Double dLon = Convert.ToDouble(lineArr[4]);
                            dLon = dLon / 100;
                            string[] lon = dLon.ToString().Split('.');
                            Longitude = lineArr[5].ToString() +
                            lon[0].ToString() + "." +
                            ((Convert.ToDouble(lon[1]) /
                            60)).ToString("#####");

                            //Display
                            txtLat.Text = "1.343371";//Latitude;
                            txtLong.Text = "103.681534";//Longitude;

                            btnMapIt.Enabled = true;
                        }
                        catch
                        {
                            //Cannot Read GPS values
                            txtLat.Text = "GPS Unavailable";
                            txtLong.Text = "GPS Unavailable";
                            btnMapIt.Enabled = false;
                        }
                    }
                }
            }
            else
            {
                txtLat.Text = "COM Port Closed";
                txtLong.Text = "COM Port Closed";
                btnMapIt.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
            {
                timer1.Enabled = false;
            }
            else
            {
                timer1.Enabled = true;
            }

            if (button1.Text == "Update")
                button1.Text = "Stop Updates";
            else
                button1.Text = "Update";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnMapIt_Click(object sender, EventArgs e)
        {
            //frmMap f = new frmMap(Latitude, Longitude);
            frmMap f = new frmMap(Convert.ToString(1.343371), Convert.ToString(103.681534));
            f.Show();
        }

        private void serialOpenButton_Click(object sender, EventArgs e)
        {
            serialPort1.PortName = comboBox1.Text;
            try
            {
                serialPort1.Open();
                serialOpenButton.Enabled = false;
                serialCloseButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                timer1.Enabled = false;
                //button1.Text = "Update";
                return;
            }
        }

        private void serialCloseButton_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.DiscardInBuffer();
                serialPort1.Close();
                serialCloseButton.Enabled = false;
                serialOpenButton.Enabled = true;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Gmap g = new Gmap();
            g.Show();
        }
    }
}
