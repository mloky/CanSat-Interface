#region Information
/*****************************************************************************/
/* Project  : CanSatDemo                                                     */
/* File     : InstrumentControl.cs                                           */
/* Version  : 1.1                                                              */
/* Language : C#                                                             */
/* Summary  : Control for the thermometer class indicator                    */
/* Creation : 12/02/2016                                                     */
/* Author   : Yeung Mun Lok                                                  */
/* History  :                                                                */
/*          Updated 1.1
            -Thermometer bar added to resources
            -Algorithms added to translate the variable(s) from the parameter
            method
                -Mapping added to determine width of bar for the stated temperature
            -Changed how the thermometer bar was drawn
                -Using a blank bitmap template, draw on that, use that template to draw on window

            1.0
            -Indicator created from bitmap image
            -Added to resources
            -Setup methods and required initializing componenets
/*****************************************************************************/
#endregion

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Data;

namespace AvionicsInstrumentControlDemo
{
    class TempIndicator : InstrumentControl
    {
        #region Fields

        // Parameters
        float temp,low1=20,low2=1,high1=43,high2=423;
        int convertedTemp=1;
        
        

        // Images
        Bitmap thermometerBar = new Bitmap(AvionicsInstrumentControlDemo.AvionicsInstrumentsControls.AvionicsInstrumentsControlsRessources.thermometer_bar,424,31+2);
        Bitmap thermometer = new Bitmap(AvionicsInstrumentControlDemo.AvionicsInstrumentsControls.AvionicsInstrumentsControlsRessources.thermometer);
        #endregion

        #region Constructor
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public TempIndicator()
        {
            // Double bufferisation
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint, true);
        }

        #endregion

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            convertedTemp = Convert.ToInt32(low2 + (temp - low1) * (high2 - low2) / (high1 - low1));
        }
        #endregion

        #region Paint

        protected override void OnPaint(PaintEventArgs pe)
        {
            // Calling the base class OnPaint
            base.OnPaint(pe);

            //temp = 28.7f;
            //convertedTemp = Convert.ToInt32(low2 + (temp - low1) * (high2 - low2) / (high1 - low1));

            thermometer.MakeTransparent(Color.Yellow);
            thermometerBar.MakeTransparent(Color.Yellow);
            //scaling of window and components
            float scale = (float)this.Width / thermometer.Width;

            //actual points of thermometerBar
            int thermometerBarWidth = Convert.ToInt32(66 * scale);
            int thermometerBarHeight = Convert.ToInt32(21 * scale);
            //actual size of thermometerBar
            int barWidth = Convert.ToInt32(423 * scale);
            int barHeight = Convert.ToInt32((31+2) * scale);

            //creating new blank Bitmap image for thermometerBar
            Bitmap bar = new Bitmap(Convert.ToInt32(convertedTemp*scale), barHeight);
            Graphics gBar = Graphics.FromImage(bar);

            //draw thermometerBar into Bitmap bar
            gBar.DrawImage(thermometerBar, new Rectangle(0,0,Convert.ToInt32(thermometerBar.Width*scale)
                ,Convert.ToInt32(thermometerBar.Height*scale))
                , new Rectangle(0,0,convertedTemp,thermometerBarHeight),GraphicsUnit.Pixel);

            //draw thermometer
            pe.Graphics.DrawImage(thermometer, 0, 0, (float)(thermometer.Width * scale), (float)(thermometer.Height * scale));
            //draw thermometerBar
            pe.Graphics.DrawImage(bar, thermometerBarWidth, thermometerBarHeight);

        }

        #endregion

        #region Methods

        public void SetTempIndicator(float floatTemp)
        {
            temp = floatTemp;
            convertedTemp = Convert.ToInt32(low2 + (temp - low1) * (high2 - low2) / (high1 - low1));
            if (convertedTemp < 0)
                convertedTemp = 0;
            else if (convertedTemp > 423)
                convertedTemp = 423;
            this.Refresh();
        }

        #endregion
    }
}
