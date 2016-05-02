using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace AvionicsInstrumentControlDemo
{
    public partial class Gmap : Form
    {
        //double LatitudeValue=0, LongitudeValue=0;
        bool counter = true;

        private void Gmap_Load(object sender, EventArgs e)
        {
            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            //GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly; // uses server only, does not save to cache
            //GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache; //uses server and saves to cache
            GMap.NET.GMaps.Instance.Mode=GMap.NET.AccessMode.CacheOnly; //uses cache only, if available
            gMapControl1.SetPositionByKeywords("Singapore, Singapore");
            gMapControl1.MinZoom = 3;
            gMapControl1.MaxZoom = 18;
            gMapControl1.Zoom = 7;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            gMapControl1.SetPositionByKeywords(countryTextBox.Text + ", " + cityTextBox.Text);
            resultTextBox1.Text = gMapControl1.Position.ToString();
            GMapOverlay overlay = new GMapOverlay("markers");
            GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(1.3553794,103.8677444),GMarkerGoogleType.green);
            overlay.Markers.Add(marker);
            gMapControl1.Overlays.Add(overlay);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            gMapControl1.Position = new PointLatLng(float.Parse(latTextBox.Text), float.Parse(lonTextBox.Text));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (counter)
            {
                gMapControl1.SetPositionByKeywords("Malaysia, Penang");
                counter = false;
            }
            else
            {
                gMapControl1.Position = new PointLatLng(1.3553794,103.8677444);
                counter = true;
            }
        }

        public Gmap()
        {
            InitializeComponent();
        }
    }
}
