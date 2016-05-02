/*****************************************************************************/
/* Project  : AvionicsInstrumentControlDemo                                  */
/* File     : DemoWondow.cs                                                  */
/* Version  : 1                                                              */
/* Language : C#                                                             */
/* Summary  : Start window of the project, use to test the instruments       */
/* Creation : 30/06/2008                                                     */
/* Autor    : Guillaume CHOUTEAU                                             */
/* History  :                                                                */
/*****************************************************************************/

//Credits to Guillaume CHOUTEAU for the original Instruments and codes.
//Also to Pongsakorn Poosankam for the webcam codes.
//-----------------------------------------------------------------------------------
/*Changelog by Mun Lok:
 * Removed Airspeed Indicator as it is not needed
 * Removed original sliders to control the instruments
 * Added a serial port for communication with modified board
 * Added some functions to the instruments codes (AttitudeIndicator/TurnCoordinator) to be called in this form. Refer to the stated instruments codes
 * Edited some formulas and variables to change the values that the board is sending in to suit the instruments
 * Added extra buttons/text boxes to facilitate testing of program
 * Actual representation may differ from the flight control structure and in reality
 * Added webcam videobox. Works on the built-in webcam installed on laptop and USB video webcam but only in Vista or older OS
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;

namespace AvionicsInstrumentControlDemo
{
    public partial class DemoWinow : Form
    {
        bool portOpen = false;
        int value, buttonPressed, inputValue;
        int roll, yaw, heave, pitch;
        public DemoWinow()
        {
            InitializeComponent();
            buttonPressed = 5;
        }
        WebCam webcam;
        private void DemoWinow_Load(object sender, EventArgs e)
        {
            //webcam = new WebCam();
            //webcam.InitializeWebCam(ref camVideo);
            camVideo.Visible = false;
            settingButton.Visible = false;
            webCamStartButton.Visible = false;
            webCamStopButton.Visible = false;
            roll = 0;
            yaw = 0;
            heave = 0;
            pitch = 0;
        }
        
        //Start of modified serial communication functions
        //---------------------------------------------------------------------------------------------
        private void valueChanged(object sender, EventArgs e)
        {
            if (value <= 64)
            {
                buttonPressed = 3;
                inputValue = value * 4;
                //yaw
            }
            else if (value <= 128)
            {
                buttonPressed = 1;
                inputValue = (value - 64) * 4;
                //roll
            }
            else if (value <= 192)
            {
                buttonPressed = 2;
                inputValue = (value - 127) * 4;
                //pitch
            }
            else
            {
                buttonPressed = 4;
                inputValue = (value - 191) * 4;
                //heave
            }

            switch(buttonPressed){
                case 1: if ((inputValue - roll > 12) || (roll - inputValue > 12))
                    {
                        horizonInstrumentControl1.SetRollAngleParameter(inputValue / 2 - 63.5f);
                        //Range from -6(left) to +6(right) Ratio of 1:1
                        turnCoordinatorInstrumentControl1.SetaircraftTurnRate(((255-inputValue) - 127) / 20.0f);
                        roll = inputValue;
                    }
                        break;
                    // button 1 for roll
                case 2: //horizonInstrumentControl1.SetPitchParameter(value);
                        //every number on the gauge represents 1000 ft/min. Ratio of 1:1
                        if ((inputValue - pitch > 12) || (pitch - inputValue > 12))
                        {
                            pitch = inputValue;
                            inputValue = 255 - inputValue;
                            verticalSpeedInstrumentControl1.SetVerticalSpeedIndicatorParameters((inputValue - 127) * 20);
                            horizonInstrumentControl1.SetPitchParameter(inputValue / 2 - 63.5f);
                            serialVerticalLabel.Text = ((inputValue - 127) * 20).ToString();
                        }
                        break;
                    //button 2 for pitch
                case 3: //Every number represents 1 degree Ratio 1:1
                        if ((inputValue - yaw > 12) || (yaw - inputValue > 12))
                        {
                            headingIndicatorInstrumentControl1.SetHeadingIndicatorParameters(inputValue);
                            serialHeadinglabel.Text = inputValue.ToString();
                            yaw = inputValue;
                        }
                        break;
                    //button 3 for yaw
                case 4: //Ratio 1:1
                        if ((inputValue - heave > 12) || (heave - inputValue > 12))
                        {
                            altimeterInstrumentControl1.SetAlimeterParameters(inputValue * 50);
                            serialAltitudeLabel.Text = (inputValue * 50).ToString();
                            heave = inputValue;
                        }
                        break;
                    //button 4 for heave
                /*default: //Turns all instruments
                        horizonInstrumentControl1.SetRollAngleParameter(inputValue / 2 - 63.5f);
                        turnCoordinatorInstrumentControl1.SetaircraftTurnRate((inputValue - 127) / 20.0f);
                        verticalSpeedInstrumentControl1.SetVerticalSpeedIndicatorParameters((inputValue - 127) * 20);
                        horizonInstrumentControl1.SetPitchParameter(inputValue / 2 - 63.5f);
                        headingIndicatorInstrumentControl1.SetHeadingIndicatorParameters(inputValue);
                        altimeterInstrumentControl1.SetAlimeterParameters(inputValue * 50);
                        serialAltitudeLabel.Text = (inputValue * 50).ToString();
                        serialHeadinglabel.Text = value.ToString();
                        serialVerticalLabel.Text = ((inputValue - 127) * 20).ToString();
                        break;*/
            }
            //altimeterInstrumentControl1.SetAlimeterParameters(value * 20);
            label6.Text = inputValue.ToString();
        }
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
                //motor = serialPort1.ReadByte();
                value = serialPort1.ReadByte();
                //feedbackData = value.ToString();
                this.Invoke(new EventHandler(valueChanged));
                //label6.Text = value.ToString();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                startButton.Enabled = true;
                stopButton.Enabled = false;
                portOpen = false;
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox1.Text;
            }
            catch {
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
                portOpen = true;
                startButton.Enabled = false;
                stopButton.Enabled = true;
            }
        }

        private void DemoWinow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
                serialPort1.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            buttonPressed = 4; //altimeter button
        }

        private void button3_Click(object sender, EventArgs e)
        {
            buttonPressed = 2;// turn button
        }

        private void button2_Click(object sender, EventArgs e)
        {
            buttonPressed = 1;// heading button
        }

        private void button1_Click(object sender, EventArgs e)
        {
            buttonPressed = 3;// vertical button
        }

        private void button5_Click(object sender, EventArgs e)
        {
            buttonPressed = 5;
        }

        private void webCamStartButton_Click(object sender, EventArgs e)
        {
            webcam.Start();// Starts communication with webcam
            webCamStopButton.Enabled = true;
            webCamStartButton.Enabled = false;
        }

        private void webCamStopButton_Click(object sender, EventArgs e)
        {
            webcam.Stop();// Stops communication with webcam
            webCamStartButton.Enabled = true;
            webCamStopButton.Enabled = false;
        }

        private void settingButton_Click(object sender, EventArgs e)
        {
            webcam.AdvanceSetting();// changes settings
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                pictureBox1.Visible = true;
            }
            else
                pictureBox1.Visible = false;
        }
    }
}