/*
Version: 1.0
Newly created class file for intepretation of NMEA sentences

Work in progress

*/

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Data;
using System.Collections.Generic;
using GMap.NET.WindowsForms;

namespace AvionicsInstrumentControlDemo.AvionicsInstrumentsControls
{
    class NMEAIntepreter
    {
        string[] deSplit;
        string NMEAformat,SatLat, SatLng;
        StringBuilder SatTime = new StringBuilder();
        int index = 0, format=0;
        int LatIndex, LngIndex, TimeIndex;

        public string[] GetWords(string val)
        {
            return val.Split(',');
        }

        public int NMEAParse(string message)
        {
            deSplit = GetWords(message);
            
            NMEAformat = deSplit[0];

            switch (NMEAformat)
            {
                case "$GPGGA":
                    return 1;
                case "$GPRMC":
                    return 2;
                //blablabla etc add more for other other formats

                default:
                    return 0;
            }
        }

        /*public string LatLngPosition(string lat, string lng)
        {
            SatLat = lat;
            SatLng = lng;
            return SatLat+SatLng;
        }*/

        public string UTCTime(string time)
        {
            float GPSTime = float.Parse(time);
            if (GPSTime == 0)
            {
                return "Time cannot be parsed";
            }

            int Hour = Convert.ToInt32(GPSTime / 10000);
            int Min = Convert.ToInt32((GPSTime % 10000) / 100);
            int Sec = Convert.ToInt32(GPSTime % 100);

            SatTime.Append(Hour.ToString());
            SatTime.Append(":");
            SatTime.Append(Min.ToString());
            SatTime.Append(":");
            SatTime.Append(Sec.ToString());

            return SatTime.ToString();
        }

        public void Intepreter(string SerialMessage)
        {
            format = NMEAParse(SerialMessage);

            switch (format)
            {
                case 0:
                    break;
                case 1:
                    //GPGGA
                    LatIndex = 2;
                    LngIndex = 4;
                    TimeIndex = 1;
                    break;
                case 2:
                    //GPRMC
                    TimeIndex = 1;
                    LatIndex = 3;
                    LngIndex = 5;
                    break;
                default:
                    TimeIndex = LatIndex = LngIndex = 0;
                    break;
            }

            GMap.NET.PointLatLng LatLng = new GMap.NET.PointLatLng(Convert.ToDouble(deSplit[LatIndex]),
                Convert.ToDouble(deSplit[LngIndex]));
        }
    }
}
