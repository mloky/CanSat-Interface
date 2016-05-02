#region Information
/*****************************************************************************/
/* Project  : CanSat                                                         */
/* File     : DemoWindow.cs                                                  */
/* Version  : 1.4.0 Final                                                    */
/* Language : C#                                                             */
/* Summary  : Visual representation of the telemetry data                    */
/* Creation : 03/02/2016                                                     */
/* Updated  : 01/04/2016    Post-submission: 12/04/2016                      */
/* Author   : Yeung Mun Lok                                                  */
/*            Update Logs

              Updates post-1.4.0 and Report Submission (15/04/16)
                -Optimization
                    -Used Replace to strip stringFinalSplit array of \r then split \n
                        -Some code commented out and will be deleted because of it
                    -Attitude Indicator will not work so well as it depends on the timing of incoming
                    packets of data. Currently, based on 250ms every data reading
                    -Closing program before closing serial port will result in unresponsive task
                    for the current implementation
**************Final 1.4.0**************************************************************************
                -Graph
                    -May change temperature graph to something else as the change is insignificant
                -Writing to file
                    -Implementation done. To-do list:
                        -Removing extra characters in gps lat/long and time if null is sent
                        -Done
                    -File is locked when program is running ie. not able to open file
                -Intepreter class removal
                    -No use for it anymore
                -Cleanup
                    -Unused code/functions/comments removed
                -Closing of window before serial is closed
                    -Implementation being explored to stop the closing before serial is closed
                    to ensure the program doesn't hang.
              1.3.5
                -Added Graph
                    -Display 2 line graphs of values of temperature,average temperature
                    and altitude.
                    -Showing/Hiding graph
                        -WIP to reshow previous created graph
                        -Improved, currently working.
                -Closing Events
                    -Optimised forms closing events
                    -Exceptions thrown when graph window is closed and trying to reopen
                    -Exceptions thrown in graph thread 
                    when main window is closed while graph window is open
                    -Apparently, the previous solutions above doesnt work properly. Will focus more on this
                    aborting/closing of threads
                    -Solved. Look at demograph.cs for details
                -Regarding anything about graphs, look at demograph.cs for more details

                -Upcoming StreamReader/StreamWriter
                    -Writing to .csv file to be used to store data or plot graph
                    -No of max files set to 10. Can be changed if needed

              1.3.0
                -Implemented GPS into main window
                    -frmPP.cs, frmMap.cs, Gmap.cs obselete, removed
                    -Working with static values as well as buttons that add/clear markers
                    -Path drawn between markers
                -Note: Regarding Delay/Time per packet
                    -Time per data packet might change if arduino code delay changes
                    -Vertical Speed, Attitude affected
                -Added Intepreter class (optional)
                    -In case the data coming in is not parsed or a complete NMEA sentence

              1.2.8
                -GPS Window integration added (frmPP.cs, frmMap.cs, Gmap.cs)
                    -frmPP.cs, frmMap.cs
                        -frmPP takes input of latitude/longitude
                        -Loads frmMap.cs
                        -frmMap loads static Google Maps at the entered location
                        -Testing code to see if it works
                        -Gmap.cs updated. This set is obsolete
                    -Gmap.cs
                        -Using GoogleMapProvider to load map
                        -Overlay for markers added
                        -WIP on real time updates from serial

              1.2.6
                -Thermometer Bar added to display temperature. Working as intended
                    -Range set to between 20 to 43 degrees. 0.5 degrees per step or change
                    
                -Vertical Indicator = Redo, Done
                    -Instead of using accelerometer/gyro, use altimeter to compare distance over time per data packet
                    to obtain falling speed
                
                -Attitude Indicator = Work in progress
                    -Accelerometer/Gyro to determine orientation
                    -Change current position by adding gyro data to initPosition

              1.2.5
                -Thermometer background added
                    -Working on the algorithms to display the red filling

              1.2
                -Attitude Indicator = Work in progress
                    -Gyro not stabilised. Fluctuations in the sensor
                    -When stationary, there is values
                    -Sudden movement or reaction is inaccurate
                    -Yet to solve factor problem for the values
                -Changed vertical speed parameters by a negative sign to 
                    ensure + is upwards - is down
                -Adding component class to the project rendered the entire project unable to build
                    -Redid the whole program from 1.0 because I did not backup before
                    -Lesson learned to backup before any major changes
                    -Solution: Copied and pasted one of the created instruments and modified. Not sure how it works

              1.1
                -Working on instruments
                -Heading Indicator = Done
                    -Using the magnetometer as compass. May cause fluctuations when near electronic devices
                -Vertical Indicator = Done
                    -Accelerometer to determine the falling rate subtract gravity (x^2 + y^2 + z^2 = r^2)
                -Altitude Indicator = Done
                    -BMP180 Baro Sensor. Altimeter on board wrt sea level
                -new_vs.ino data delay changed to 200ms to reduce load on program
                    -Better visual, rather than fast jumping values

              1.0
                -Settled serial port communications and extraction of values from xbee
                -Only works with new_vs.ino uploaded to Arduino board
                -Removed some unwanted components/code/comments 
                -Instruments not used yet so as to confirm incoming data
                                                                             */
/*                                                                           */
/*****************************************************************************/

//Credits to Guillaume CHOUTEAU for the original Instruments and codes.
//
//-----------------------------------------------------------------------------------
/*General changelog by Mun Lok to Version 1.0:
 * Removed Airspeed Indicator as it is not needed
 * Removed original sliders to control the instruments
 * Added a serial port for communication
 * Added some functions to the instruments codes (AttitudeIndicator/TurnCoordinator) to be called in this form. Refer to the stated instruments codes
 * Edited some formulas and variables to change the values that the board is sending in to suit the instruments
 * Added extra buttons/text boxes for the program

 * Actual representation may differ in the program and in reality
*/

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;

namespace AvionicsInstrumentControlDemo
{
    public partial class DemoWindow : Form
    {
        graphWindow g = new graphWindow();

        #region Instrument Variables
        float accelX, accelY, accelZ, gyroX, gyroY, gyroZ, pres, alti, temp, magX, magY, magZ;
        float initAlti, initPress, initVertical, initTemp;
        float relAlti, relPress, relVertical, relTemp, relPositionX, relPositionY, relPositionZ;
        float avgRelAlti, avgRelVertical, avgRelTemp, avgRelPress;
        float avgAccelX, avgAccelY, avgAccelZ, avgMagX, avgMagY, avgMagZ, avgGyroX, avgGyroY, avgGyroZ;

        float attitudeX = 0, attitudeY = 0, attitudeZ = 0;

        float prevAlti, newAlti;
        string receivingValues;
        StringBuilder rawValues = new StringBuilder();

        StringBuilder stringMessage = new StringBuilder();
        #endregion

        #region GPS variables
        //Below are variables for GPS control
        StringBuilder GPSTime = new StringBuilder();
        int counter = 0, MarkerIndex = 1;
        bool ValidGPS = false;
        GMapOverlay overlay = new GMapOverlay("markers");
        GMapOverlay polyOverlay = new GMapOverlay("polylines");
        //Testing values for the gps below
        PointLatLng[] gpsTestValues = new PointLatLng[8];
        List<PointLatLng> points = new List<PointLatLng>();
        PointLatLng prevPos = new PointLatLng();
        MarkerTooltipMode MouseOverMarker = MarkerTooltipMode.OnMouseOver;
        #endregion

        #region Read/Write variables
        //declarations to write to file using IO stream
        StreamWriter WriteToCSV;
        //To simplify file creation, all file names will start with a number followed by "data.csv" eg. "1data.csv"
        StringBuilder FileName = new StringBuilder("data.csv");
        StringBuilder DataToWrite = new StringBuilder();
        int FileCounter = 1, DataRows = 1;
        #endregion
      
        public DemoWindow()
        {
            InitializeComponent();
        }

        private void DemoWinow_Load(object sender, EventArgs e)
        {
            #region InstrumentControls
            accelX = 0;
            accelY = 0;
            accelZ = 0;
            magX = 0;
            magY = 0;
            magZ = 0;
            gyroX = 0;
            gyroY = 0;
            gyroZ = 0;
            pres = 0;
            alti = 0;
            temp = 0;
            prevAlti = 0;
            newAlti = 0;
            relPositionX = 0;
            relPositionY = 0;
            relPositionZ = 0;
            avgAccelX = avgAccelY = avgAccelZ = avgGyroX = avgGyroY = avgGyroZ = avgMagX = avgMagY = avgMagZ = avgRelAlti = avgRelPress = avgRelTemp = 0;
            #endregion

            #region GPS initialisation
            // Below are codes for the GPS control
            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance; //uses Google Maps server
            //GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly; // uses server only, does not save to cache
            GMaps.Instance.Mode = AccessMode.ServerAndCache; //uses both
            //GMap.NET.GMaps.Instance.Mode=GMap.NET.AccessMode.CacheOnly; //uses cache only, if available
            gMapControl1.SetPositionByKeywords("Singapore, Singapore");
            gMapControl1.MinZoom = 8;
            gMapControl1.MaxZoom = 20;
            gMapControl1.Zoom = 12;
            gMapControl1.DragButton = MouseButtons.Left;//set dragging with left click

            zoomLabel.Text = "Zoom Level: " + gMapControl1.Zoom;
            timeLabel.Text = "Time (UTC): " + DateTime.Now.ToLongTimeString();
            #endregion

            #region Read/Write File Name Initialisation
            //Initialisation of file stream
            //Sets limit to number of file creation. Current limit = 10            
            FileCounter = 1;
            while (File.Exists(FileCounter.ToString() + FileName.ToString()))
            {
                FileCounter++;
                if (FileCounter > 9)
                {
                    MessageBox.Show("All allocated files used. Please clear unused files\nProgram replacing last used file", "Warning");
                    break;
                }
            }
            fileLabel.Text = FileCounter.ToString() + FileName.ToString();
            WriteToCSV = new StreamWriter(new FileStream(fileLabel.Text, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite));//initialize, create, opens stream
            WriteToCSV.WriteLine("Accel X,Accel Y,Accel Z,Mag X,Mag Y,Mag Z,Gyro X,Gyro Y,Gyro Z,Pressure,Temperature,Altitude,GPS Lat,GPS Long,Time");
            #endregion
        }

        public void showGraphButton_Click(object sender, EventArgs e)
        {
            //opens up the graph window. If object not found/disposed, create new and show
            try {
                g.Show();
            }
            catch (ObjectDisposedException)
            {
                g = new graphWindow();
                g.Show();
            }
        }

        //Functions for serial communication
        //---------------------------------------------------------------------------------------------

        private void valueChanged(object sender, EventArgs e)
        {
            
            #region InstrumentChanges
            String[] stringFinalSplit = receivingValues.Replace("\r",string.Empty).Split('\n'); //splits the string into different parameters
            receivingValues = "";

            if (stringFinalSplit.Length == 17 || stringFinalSplit.Length == 14) //sends values into labels //17 if GPS included 14 if not
            {
                accelLabelX.Text = stringFinalSplit[1];
                accelX = float.Parse(stringFinalSplit[1]);
                accelLabelY.Text = stringFinalSplit[2];
                accelY = float.Parse(stringFinalSplit[2]);
                accelLabelZ.Text = stringFinalSplit[3];
                accelZ = float.Parse(stringFinalSplit[3]);
                magLabelX.Text = stringFinalSplit[4];
                magX = float.Parse(stringFinalSplit[4]);
                magLabelY.Text = stringFinalSplit[5];
                magY = float.Parse(stringFinalSplit[5]);
                magLabelZ.Text = stringFinalSplit[6];
                magZ = float.Parse(stringFinalSplit[6]);
                gyroLabelX.Text = stringFinalSplit[7];
                gyroX = float.Parse(stringFinalSplit[7]);
                gyroLabelY.Text = stringFinalSplit[8];
                gyroY = float.Parse(stringFinalSplit[8]);
                gyroLabelZ.Text = stringFinalSplit[9];
                gyroZ = float.Parse(stringFinalSplit[9]);
                pressureLabel.Text = stringFinalSplit[10] + " hPa";
                pres = float.Parse(stringFinalSplit[10]);
                tempLabel.Text = stringFinalSplit[11] + " C";
                temp = float.Parse(stringFinalSplit[11]);
                altiLabel.Text = stringFinalSplit[12] + " m";
                alti = float.Parse(stringFinalSplit[12]);
            }

            g.setTemp(temp);
            g.setAltitude(alti);

            if (avgAccelX == 0 && avgAccelY == 0 && avgAccelZ == 0)
            {
                avgAccelX = accelX;
                avgAccelY = accelY;
                avgAccelZ = accelZ;
            }
            if (avgGyroX == 0 && avgGyroY == 0 && avgGyroZ == 0)
            {
                avgGyroX = gyroX;
                avgGyroY = gyroY;
                avgGyroZ = gyroZ;
            }
            if (avgMagX == 0 && avgMagY == 0 && avgMagZ == 0)
            {
                avgMagX = magX;
                avgMagY = magY;
                avgMagZ = magZ;
            }

            relPress = pres;
            avgRelPress = (avgRelPress + relPress) / (float)2.0;
            serialPressurelabel.Text = pres.ToString();
            avgPressureLabel.Text = avgRelPress.ToString();

            //Thermometer control

            if (temp != relTemp)
            {
                relTemp = temp;
                if (avgRelTemp == 0)
                    avgRelTemp = temp;
                avgRelTemp = (avgRelTemp + relTemp) / (float)2.0;
                serialTemperaturelabel.Text = temp.ToString();
                avgTemperatureLabel.Text = avgRelTemp.ToString();
                tempIndicator1.SetTempIndicator(relTemp);
            }

            //Altimeter control
            if (alti != relAlti)
            {
                prevAlti = newAlti;
                newAlti = alti;
                relAlti = alti - initAlti;
                avgRelAlti = (avgRelAlti + relAlti) / (float)2.0;
                altimeterInstrumentControl1.SetAlimeterParameters(relAlti);
                serialAltitudeLabel.Text = alti.ToString();
                relAltitudelabel.Text = relAlti.ToString();
                avgAltitudeLabel.Text = avgRelAlti.ToString();

            }

            //Vertical control

            //Wrong code below. Forgot about the altimeter
            //float combinedAccel = (float)9.8 - (float)Math.Sqrt(accelX * accelX + accelY * accelY + accelZ * accelZ);

            float combinedVelocity = (prevAlti - newAlti) / 0.2f;
            serialVerticalLabel.Text = combinedVelocity.ToString();
            relVertical = combinedVelocity - initVertical;
            avgRelVertical = (avgRelVertical + relVertical) / (float)2.0;
            verticalSpeedInstrumentControl1.SetVerticalSpeedIndicatorParameters(relVertical);
            avgVerticalSpeedLabel.Text = avgRelVertical.ToString();

            //Heading control
            //Note: Magnetometer very sensitive. Will not work indoors especially around electronic equipments

            float heading = (float)(Math.Atan2(-magY, magZ) * 180 / Math.PI);

            if (heading < 0)
                heading = 360 + heading;
            serialHeadinglabel.Text = heading.ToString();
            headingIndicatorInstrumentControl1.SetHeadingIndicatorParameters(heading);

            //Attitude control
            //Work in progress, values in rad/s

            if (!(gyroX <= 0.02 && gyroX >= -0.02))
            {
                attitudeX = gyroX / 4.0f;
                relPositionX = relPositionX + attitudeX;
            }
            if (!(gyroY <= 0.05 && gyroY >= -0.05))
            {
                attitudeY = gyroY / 4.0f;
                relPositionY = relPositionY + attitudeY;
            }

            if (!(gyroZ <= 0.03 && gyroZ >= -0.03))
            {
                attitudeZ = gyroZ / 4.0f;
                relPositionZ = relPositionZ + attitudeZ;
            }

            horizonInstrumentControl1.SetPitchParameter(relPositionY); //use for radians
            horizonInstrumentControl1.SetRollAngleParameter(relPositionZ); //use for radians
            //horizonInstrumentControl1.SetAttitudeIndicatorParameters(relPositionX, relPositionZ); //use if arguments in degree
            #endregion

            #region GPS
            //All gps changes to be done in its function
            try {
                ValidGPS = GPSValueChanged(stringFinalSplit[13], stringFinalSplit[14], stringFinalSplit[15]);
            }
            catch (FormatException)
            {
                gpsStatusLabel.Text = "GPS position not valid!";
            }
            catch (IndexOutOfRangeException)
            {
                gpsStatusLabel.Text = "GPS not connected!";
            }
            catch
            {
                gpsStatusLabel.Text = "Unknown error";
            }
            #endregion

            #region Read/Write instructions

            DataToWrite.Clear(); //resets stringbuilder
            /*for (int i = 1; i <= stringFinalSplit.Length - 1; i++)
            {
                DataToWrite.Append(stringFinalSplit[i]).Append(',');
            }*/
            DataToWrite.Append(accelX).Append(',');
            DataToWrite.Append(accelY).Append(',');
            DataToWrite.Append(accelZ).Append(',');
            DataToWrite.Append(magX).Append(',');
            DataToWrite.Append(magY).Append(',');
            DataToWrite.Append(magZ).Append(',');
            DataToWrite.Append(gyroX).Append(',');
            DataToWrite.Append(gyroY).Append(',');
            DataToWrite.Append(gyroZ).Append(',');
            DataToWrite.Append(pres).Append(',');
            DataToWrite.Append(temp).Append(',');
            DataToWrite.Append(alti);
            if (stringFinalSplit.Length > 14)
            {
                try {
                    DataToWrite.Append(',').Append(stringFinalSplit[13]).Append(',').Append(stringFinalSplit[14]).Append(',').Append(stringFinalSplit[15]);
                }
                catch (IndexOutOfRangeException)
                {

                }
            }
            //below codes not needed anymore as the stripping loop has been added
            //if (stringFinalSplit.Length > 14 && stringFinalSplit[13] != "null\r" && stringFinalSplit[14] != "null\r")
            //    DataToWrite.Append(',').Append(stringFinalSplit[13]).Append(',').Append(stringFinalSplit[14]).Append(',').Append(stringFinalSplit[15]);
            //else if (stringFinalSplit[13] == "null\r" || stringFinalSplit[14] == "null\r")
            //    DataToWrite.Append(',').Append("null,null,").Append(stringFinalSplit[15]);
            //DataToWrite.Append(stringFinalSplit[stringFinalSplit.Length - 1]);
            WriteToCSV.Write(DataToWrite.ToString());//Writes to file with line terminator
            #endregion
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            rawValues = rawValues.Append(serialPort1.ReadExisting()); //stores data into a string
            //Looks for open and close curly bracket match eg. {234\r\n59oliu} 
            //then only store into string that is ready for processing
            receivingValues = Regex.Match(rawValues.ToString(), @"\{([^{}]*)\}").Groups[1].Value;

            if (receivingValues != "")
            {
                rawValues.Clear();
                this.Invoke(new EventHandler(valueChanged));
            }
        }

        //---------------------------------------------------------------------------------------------

        //Display raw data from incoming data
        private void telemetryCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!telemetryCheckBox1.Checked)
            {

                telemetryGroupBox.Visible = false;
            }
            else
            {

                telemetryGroupBox.Visible = true;
            }
        }

        private void addMarkerButton_Click(object sender, EventArgs e)
        {
            zoomLabel.Text = "Zoom Level: " + gMapControl1.Zoom;
            if (GPSTime.Length>=1)
                timeLabel.Text = "Time (UTC): " + TimeConversionToUTC8(DateTime.Now.ToLongTimeString());//GPSTime.ToString());
            if (!prevPos.IsEmpty)
                points.Add(prevPos);
            points.Add(gMapControl1.Position);
            GMarkerGoogle marker = new GMarkerGoogle(gMapControl1.Position, GMarkerGoogleType.green);
            marker.ToolTip = new GMapBaloonToolTip(marker);
            marker.ToolTip.Stroke = new Pen(Color.Black);
            marker.ToolTipMode = MouseOverMarker;
            Brush color = new SolidBrush(Color.Transparent);
            marker.ToolTip.Fill = color;
            marker.ToolTipText = gMapControl1.Position.ToString();

            GMapPolygon polyline = new GMapPolygon(points, "myLine");
            polyline.Stroke = new Pen(Color.Blue, 1);

            overlay.Markers.Add(marker);
            polyOverlay.Polygons.Add(polyline);
            gMapControl1.Overlays.Add(overlay);
            gMapControl1.Overlays.Add(polyOverlay);
            points.Clear();
            prevPos = gMapControl1.Position;
            gMapControl1.Zoom -= 1;
            gMapControl1.Zoom += 1;
            MarkerIndex++;
        }

        private void clearMarkersButton_Click(object sender, EventArgs e)
        {        
            overlay.Markers.Clear();
            polyOverlay.Polygons.Clear();
            points.Clear();
            prevPos = new PointLatLng();
            MarkerIndex = 1;
        }

        private void resetRelButton_Click(object sender, EventArgs e)
        {
            relAltitudelabel.Text = "0";
            initAlti = float.Parse(serialAltitudeLabel.Text);
            initPress = float.Parse(serialPressurelabel.Text);
            initVertical = float.Parse(serialVerticalLabel.Text);
            initTemp = float.Parse(serialTemperaturelabel.Text);
            relPositionX = 0;
            relPositionY = 0;
            relPositionZ = 0;

            if (relAlti == 0 && relTemp == 0 && relPress == 0)
            {
                avgRelAlti = initAlti;
                avgRelPress = initPress;
                avgRelTemp = initTemp;
                avgRelVertical = initVertical;

            }
        }        

        private void startButton_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox1.Text;
            }
            catch
            {
                MessageBox.Show("Please choose a port", "Error", MessageBoxButtons.OK);
            }
            //serialPort1.BaudRate = 9600;
            if (!serialPort1.IsOpen)
                try
                {
                    serialPort1.Open();
                }
                catch
                {
                    MessageBox.Show("Port is not found", "Error", MessageBoxButtons.OK);
                }
            if (serialPort1.IsOpen)
            {
                startButton.Enabled = false;
                stopButton.Enabled = true;
                serialPort1.DiscardInBuffer();
                showGraphButton.Enabled = true;
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardInBuffer();
                serialPort1.Close();
                startButton.Enabled = true;
                stopButton.Enabled = false;
                showGraphButton.Enabled = false;
            }
        }

        private void leftCameraButton_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write("2");
                }
                catch (TimeoutException)
                {
                    cameraLabel.Text = "Serial Timeout! Try again.";
                }
            }
            else
            {
                cameraLabel.Text = "Serial not open. Try again.";
            }
        }

        private void centerCameraButton_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write("1");
                }
                catch (TimeoutException)
                {
                    cameraLabel.Text = "Serial Timeout! Try again.";
                }
            }
            else
                cameraLabel.Text = "Serial not open. Try again.";
        }

        private void rightCameraButton_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write("3");
                }
                catch (TimeoutException)
                {
                    cameraLabel.Text = "Serial Timeout! Try again.";
                }
            }
            else
                cameraLabel.Text = "Serial not open. Try again.";
        }

        private string TimeConversionToUTC8(string time)
        {
            string[] TimePlus8 = time.Split(':');
            int hour = Convert.ToInt16(TimePlus8[0]);
            hour = hour + 8;
            if (hour > 24)
                hour = hour - 24;
            StringBuilder RealTime = new StringBuilder();
            RealTime.Append(hour.ToString());
            RealTime.Append(':');
            RealTime.Append(TimePlus8[1]);
            RealTime.Append(':');
            RealTime.Append(TimePlus8[2]);
            return RealTime.ToString();
        }

        private bool GPSValueChanged(string lat, string lng, string time)
        {
            GPSTime.Clear();
            if (lat == "null\r" || lng == "null\r")
            {
                return false;
            }
            try
            {
                gMapControl1.Position = new PointLatLng(Convert.ToDouble(lat), Convert.ToDouble(lng));
                GPSTime.Append(time);
                statusLabel.Text = "Valid data";
            }
            catch (FormatException)
            {
                statusLabel.Text = "Format Exception";
                return false;
            }

            zoomLabel.Text = "Zoom Level: " + gMapControl1.Zoom;
            timeLabel.Text = "Time (UTC): " + TimeConversionToUTC8(GPSTime.ToString());
            counter++;
                        
            //Draw line from prevpos to present position
            if (!prevPos.IsEmpty)
                points.Add(prevPos);
            points.Add(gMapControl1.Position);
            GMapPolygon polyline = new GMapPolygon(points, "myLine");
            polyline.Stroke = new Pen(Color.Blue, 1);
            polyOverlay.Polygons.Add(polyline);
            gMapControl1.Overlays.Add(polyOverlay);
            points.Clear();
            prevPos = gMapControl1.Position;
            return true;
        }

        //remember to remove this function. For testing purpose only.
        //Un-comment this function for debugging purposes

        private void randomGenButton_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            DataToWrite.Clear();
            for (int i = 1; i <= 3; i++)
            {
                DataToWrite.Append((rand.Next(0, 100) / 10f).ToString()).Append(',');
            }
            for (int i = 4; i <= 6; i++)
            {
                DataToWrite.Append((rand.Next(0, 200) / 10f).ToString()).Append(',');
            }
            for (int i = 7; i <= 9; i++)
            {
                DataToWrite.Append(rand.NextDouble().ToString("0.00")).Append(',');
            }
            DataToWrite.Append("1003.41,");
            DataToWrite.Append(rand.Next(20, 30)).Append(',');
            DataToWrite.Append(rand.Next(50, 200)).Append(',');
            //writes current position and time
            DataToWrite.Append(gMapControl1.Position.Lat.ToString()).Append(',').Append(gMapControl1.Position.Lng.ToString())
                .Append(',').Append(DateTime.Now.ToLongTimeString());
            WriteToCSV.WriteLine(DataToWrite.ToString());
            //display datarows in label
            //
            //
            //
            //
            DataRows++;
        }

        private void DemoWinow_FormClosing(object sender, FormClosingEventArgs e) //close everything
        {
            if (serialPort1.IsOpen)
            {
                e.Cancel = true; //cancels the closing event if serial is still open
                
                try {
                    serialPort1.DiscardInBuffer();
                    serialPort1.Close();
                }
                catch {
                    statusLabel.Text = "Closing port";
                    MessageBox.Show("Please close serial port first!", "Warning");
                }
            }

            else if (!serialPort1.IsOpen)
            {
                e.Cancel = false;
                WriteToCSV.Close();
                //Waste lots of time for this one short sentence. Informs all to terminate. Calls all FormClosing
                Application.Exit();
            }         
        }

    }
}