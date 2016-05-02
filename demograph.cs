#region Information
/* Project  : CanSat                                                         */
/* File     : demograph.cs                                                   */
/* Version  : 1.1 final                                                      */
/* Language : C#                                                             */
/* Summary  : Graphs of certain data                                         */
/* Creation : 26/02/2016                                                     */
/* Updated  : 25/03/2016                                                     */
/* Author   : Yeung Mun Lok                                                  */
/*            Update Logs

              1.1
                -StreamWriter
                    -All IO (Writer/Reader) removed
                -Cleanup
                    -Any debugging functions/comments and all unused code or stuff removed

              1.0.3
                -More on closing threads
                    -Apparently, the previous solutions doesnt work properly. Will focus more on this
                    aborting/closing of threads
                    -After some testing, cross form calling doesnt set the variable closeRequested in the first iteration
                    of the thread cycle. Only after setting flag and running one cycle of threadloop, then it detects flag changed
                    -ClosedThreadLoop method removed as its not needed anymore
                
                -Solved, kind of, until something crops up
                    -Waste so much time for one short sentence Application.Exit called in main window
                        -Informs all to terminate. Calls handler FormClosing here. Sets flag, exits thread loop.

                -Time stamp label added
                    -Changed axis labels to time instead of OADate
                
                -Added StreamWriter
                    -For testing only. Remove all instances of IO when finalized; notably in Load, timer, closing, loopfunction

              1.0.2
                -Closing of threads
                    -Previous eventhandler codes are replaced as its not stable
                    -Added flag to determine if main window closed
                    -If closed, exit thread loop and close form

              1.0.0
                -New Windows form demograph.cs
                    -To chart graph from data
                    -Temperature and Altitude
                    -All X axis spans 120 seconds
                -Temperature graph
                    -Data values shown are real temp and computed average temp
                -Altitude graph
                    -Data values shown are altitude a.s.l
                    -Considering to be changed to relative altitude
                -Added eventhandler to suspend threadloop
                    -Since suspend/resume deprecated, other solution used
                    -Creating a ManualResetEvent
                        -_event is set to pass signal to threadloop to continue when the program is running
                        -_event resets when main window closes, therefore stops threadloop

*/
#endregion
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace AvionicsInstrumentControlDemo
{
    public partial class graphWindow : Form
    {
        private Thread addDataRunner;
        private Random rand = new Random(); //not for testing, actually used. Do not remove
        public delegate void AddDataDelegate();
        public AddDataDelegate addDataDel;
        private volatile bool closeRequested = false;
        float realtemp=19,avgtemp=0,altitude=40,maxtemp=0,maxavgtemp=0,maxaltitude=0;//initialisation of values

        public graphWindow()
        {
            InitializeComponent();
        }

        

        private void graphWindow_Load(object sender, EventArgs e)
        {
            //define a thread to add values into chart
            ThreadStart addDataThreadStart = new ThreadStart(AddDataThreadLoop);
            addDataRunner = new Thread(addDataThreadStart);
            addDataDel += new AddDataDelegate(AddData);
            //add series into chart
            startData(null, new EventArgs());
            closeRequested = false;            
        }

        private void startData(object sender, EventArgs e)
        {
            //add series from min to max
            DateTime minValue = DateTime.Now;
            DateTime maxValue = minValue.AddSeconds(120);//change value in bracket for the axis span

            //Chart 1 = Temperature. All colors can be changed
            chartTemp.ChartAreas[0].AxisX.Minimum = minValue.ToOADate();
            chartTemp.ChartAreas[0].AxisX.Maximum = maxValue.ToOADate();
            chartTemp.ChartAreas[0].AxisY.Minimum = 15;
            chartTemp.Series.Clear();
       
            Series newTempSeries = new Series("Temperature");
            newTempSeries.ChartType = SeriesChartType.Line;
            newTempSeries.BorderWidth = 2;
            newTempSeries.Color = Color.OrangeRed;
            newTempSeries.XValueType = ChartValueType.DateTime;
            chartTemp.Series.Add(newTempSeries);
            Series newTempAve = new Series("Avg Temp");
            newTempAve.ChartType = SeriesChartType.Line;
            newTempAve.BorderWidth = 2;
            newTempAve.Color = Color.Blue;
            newTempAve.XValueType = ChartValueType.DateTime;
            chartTemp.Series.Add(newTempAve);
            chartTemp.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm:ss";
            chartTemp.ChartAreas[0].AxisX.Title = "Time";
            chartTemp.ChartAreas[0].AxisY.Title = "Temperature (°) ";

            //Chart 2 = Altitude
            chartAltitude.ChartAreas[0].AxisX.Minimum = minValue.ToOADate();
            chartAltitude.ChartAreas[0].AxisX.Maximum = maxValue.ToOADate();
            chartAltitude.ChartAreas[0].AxisY.Minimum = 40;
            chartAltitude.Series.Clear();
            Series newAltitudeSeries = new Series("Altitude");
            newAltitudeSeries.ChartType = SeriesChartType.Line;
            newAltitudeSeries.BorderWidth = 2;
            newAltitudeSeries.Color = Color.Green;
            newAltitudeSeries.XValueType = ChartValueType.DateTime;
            chartAltitude.Series.Add(newAltitudeSeries);
            chartAltitude.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm:ss";
            chartAltitude.ChartAreas[0].AxisX.Title = "Time";
            chartAltitude.ChartAreas[0].AxisY.Title = "Altitude above sea level (m) ";

            //start thread to add data into chart
            addDataRunner.Start();
        }

        private void AddDataThreadLoop()//Problems problems
        {
            //closeRequested not set in the below method. if CloseThreadLoop called in this form, it changes.
            //try and catch so when the form closes, invoke throws exception
            while (!closeRequested)
            {
                try {
                    chartTemp.Invoke(addDataDel);                    
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                    Thread.Sleep(1000);
            } //do loop while flag is not set
        }

        public void AddData()
        {
            DateTime timeStamp = DateTime.Now; //get current time
            label1.Text = timeStamp.ToLongTimeString(); //write into label
            AddNewPoint(timeStamp, chartTemp.Series[0]); //add current temperature point to graph
            AddAvgPoint(timeStamp, chartTemp.Series[1]); //add avg temperature point to graph
            AddAltiPoint(timeStamp, chartAltitude.Series[0]); //add altitude point to graph
        }

        public void AddAltiPoint(DateTime timeStamp, Series ptSeries)
        {
            double newVal = 0;
            if (ptSeries.Points.Count > 0)
            {
                newVal = ptSeries.Points[ptSeries.Points.Count - 1].YValues[0] + ((rand.NextDouble() * 2) - 1);
            }
            if (newVal < 0)
                newVal = 0;
            ptSeries.Points.AddXY(timeStamp.ToOADate(), altitude);

            double removeBefore = timeStamp.AddSeconds((double)(90) * (-1)).ToOADate();
            while (ptSeries.Points[0].XValue < removeBefore)
            {
                ptSeries.Points.RemoveAt(0);
            }
            chartAltitude.ChartAreas[0].AxisX.Minimum = ptSeries.Points[0].XValue;
            chartAltitude.ChartAreas[0].AxisX.Maximum = DateTime.FromOADate(ptSeries.Points[0].XValue).AddMinutes(2).ToOADate();
            chartAltitude.ChartAreas[0].AxisY.Maximum = Convert.ToInt32(maxaltitude) + 1;
            chartAltitude.Invalidate();
        }
        public void AddAvgPoint(DateTime timeStamp, Series ptSeries)
        {
            double newVal = 0;
            if (ptSeries.Points.Count > 0)
            {
                newVal = ptSeries.Points[ptSeries.Points.Count - 1].YValues[0] + ((rand.NextDouble() * 2) - 1);
            }
            if (newVal < 0)
                newVal = 0;
            ptSeries.Points.AddXY(timeStamp.ToOADate(), avgtemp);

            double removeBefore = timeStamp.AddSeconds((double)(90) * (-1)).ToOADate();
            while (ptSeries.Points[0].XValue < removeBefore)
            {
                ptSeries.Points.RemoveAt(0);
            }
            chartTemp.ChartAreas[0].AxisX.Minimum = ptSeries.Points[0].XValue;
            chartTemp.ChartAreas[0].AxisX.Maximum = DateTime.FromOADate(ptSeries.Points[0].XValue).AddMinutes(2).ToOADate();
            chartTemp.ChartAreas[0].AxisY.Maximum = Convert.ToInt32(maxavgtemp) + 1;
            chartTemp.Invalidate();
        }
        public void AddNewPoint(DateTime timeStamp, Series ptSeries)
        {
            double newVal = 0;
            if (ptSeries.Points.Count > 0)
            {
                newVal = ptSeries.Points[ptSeries.Points.Count - 1].YValues[0] + ((rand.NextDouble() * 2) - 1);
            }
            if (newVal < 0)
                newVal = 0;
            ptSeries.Points.AddXY(timeStamp.ToOADate(), realtemp);

            double removeBefore = timeStamp.AddSeconds((double)(90) * (-1)).ToOADate();
            while (ptSeries.Points[0].XValue < removeBefore)
            {
                ptSeries.Points.RemoveAt(0);
            }
            chartTemp.ChartAreas[0].AxisX.Minimum = ptSeries.Points[0].XValue;
            chartTemp.ChartAreas[0].AxisX.Maximum = DateTime.FromOADate(ptSeries.Points[0].XValue).AddMinutes(2).ToOADate();
            chartTemp.ChartAreas[0].AxisY.Maximum = Convert.ToInt32(maxtemp) + 1;
            chartTemp.Invalidate();
        }

        public void setTemp(float temp)
        {
            realtemp = temp;
            if (avgtemp == 0)
                avgtemp = temp;
            avgtemp = (avgtemp + realtemp) / 2;
            if (temp >= maxtemp)
                maxtemp = temp;
            if (avgtemp >= maxavgtemp)
                maxavgtemp = avgtemp;
        }

        public void setAltitude(float alti)
        {
            altitude = alti;
            if (alti >= maxaltitude)
                maxaltitude = alti;
        }

        private void hideButton_Click(object sender, EventArgs e)
        {
            Hide();
        }        

        private void graphWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            closeRequested = true;
        }
    }
}
