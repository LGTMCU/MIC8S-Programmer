using System;
using System.Windows;
using System.Windows.Media;
using MahApps.Metro.Controls;
using System.IO.Ports;
using System.Windows.Controls;

namespace MICS8S_Programmer
{
    /// <summary>
    /// Interaction logic for SerialSettings.xaml
    /// </summary>
    public partial class SerialSettings : MetroWindow
    {
        public SerialSettings()
        {
            InitializeComponent();

            Owner = Application.Current.MainWindow;

            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            foreach (string item in SerialPort.GetPortNames())
            {
                cbPortSel.Items.Add(item);
            }

            if (cbPortSel.HasItems)
            {
                cbPortSel.SelectedIndex = 0;
            }
        }

        private void portStoop_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mwin = Owner as MainWindow;

            if (mwin.comPort != null && mwin.comPort.IsOpen)
            {
                mwin.comPort.Close();
                mwin.comPort = null;
            }

            mwin.btnPortSettings.Content = "settings";
            mwin.btnPortSettings.Background = Brushes.Transparent;

            Close();
        }

        private void portStart_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mwin = Owner as MainWindow;

            if (mwin.comPort != null && mwin.comPort.IsOpen)
            {
                mwin.comPort.Close();
            }

            if (cbPortSel.SelectedValue != null)
            {
                mwin.portName = cbPortSel.SelectedValue.ToString();
            }

            if (cbParityType.SelectedValue != null)
            {
                mwin.parity = cbParityType.SelectedValue.ToString();
            }

            if (cbDataBits.SelectedValue != null)
            {
                mwin.dataBits = cbDataBits.SelectedValue.ToString();
            }

            if (cbStopBits.SelectedValue != null)
            {
                mwin.stopBits = cbStopBits.SelectedValue.ToString();
            }

            if (cbBaudRate.SelectedValue != null)
            {
                mwin.baudRate = cbBaudRate.SelectedValue.ToString();
            }

            if (!string.IsNullOrEmpty(mwin.portName))
            {
                mwin.comPort = new SerialPort();
                mwin.comPort.PortName = mwin.portName;
                mwin.comPort.BaudRate = Convert.ToInt32(mwin.baudRate);
                mwin.comPort.DataBits = Convert.ToInt32(mwin.dataBits);
                mwin.comPort.Handshake = Handshake.None;

                if (mwin.parity.StartsWith("Odd"))
                {
                    mwin.comPort.Parity = Parity.Odd;
                }
                else if (mwin.parity.StartsWith("Even"))
                {
                    mwin.comPort.Parity = Parity.Even;
                }
                else
                {
                    mwin.comPort.Parity = Parity.None;
                }

                if (mwin.stopBits.StartsWith("2"))
                {
                    mwin.comPort.StopBits = StopBits.Two;
                }
                else
                {
                    mwin.comPort.StopBits = StopBits.One;
                }

                try
                {
                    mwin.comPort.Open();
                    mwin.btnPortSettings.Content = mwin.portName;
                    mwin.btnPortSettings.Background = Brushes.SkyBlue;

                    mwin.send_basic();

                    /*
                    (new System.Threading.Thread(() =>
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            mwin.send_basic();
                        }));
                    })).Start();
                    */
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No valid serial port found!!", "Warning",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            else
            {
                MessageBox.Show("No valid serial port found!!", "Warning", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }

            Close();
        }
    }
}
