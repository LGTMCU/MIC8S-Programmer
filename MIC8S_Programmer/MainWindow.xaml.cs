using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Dragablz;
using MahApps.Metro.Controls;

namespace MICS8S_Programmer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        // serial port settings
        public string portName { get; set; }
        public string baudRate { get; set; }
        public string dataBits { get; set; }
        public string stopBits { get; set; }
        public string parity { get; set; }

        private SerialSettings dlgPortSettings;
        public SerialPort comPort;

        // otp program
        private string fmPath { get; set; }
        private OTP pmOTP { get; set; }
        public int psSector { get; set; }

        // configuration words
        public int cw1_fosc { get; set; }
        public int cw1_sut { get; set; }
        public int cw1_rcm { get; set; }
        public bool cw1_osco { get; set; }

        public int cw2_mmode { get; set; }
        public int cw2_tcyc { get; set; }
        public bool cw2_mcre { get; set; }
        public int cw2_lvdt { get; set; }
        public bool cw2_wdte { get; set; }
        public int cw2_pmod { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();

            dlgPortSettings = null;
            comPort = null;

            tblMain.SelectedIndex = 0;
            psSector = 0;

            pmOTP = new OTP();

            cw1_fosc = 1;
            cw1_sut = 3;
            cw1_rcm = 1;
            cw1_osco = true;

            cw2_mmode = 1;
            cw2_tcyc = 2;
            cw2_lvdt = 6;
            cw2_pmod = 1;
            cw2_mcre = true;
            cw2_wdte = true;

            DataContext = this;
             
        }

        private void otpFileButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog o_dlg = new System.Windows.Forms.OpenFileDialog();

            //o_dlg.InitialDirectory = Environment.CurrentDirectory;
            o_dlg.Filter = "hex files (*.hex)|*.hex|Binary files (*.bin)|*.bin|All files (*.*)|*.*";
            o_dlg.FilterIndex = 1;
            o_dlg.RestoreDirectory = true; // if false, may cause bug in winxp

            if (o_dlg.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            fmPath = o_dlg.FileName;

            if (string.Equals(".hex", Path.GetExtension(fmPath)))
            {
                read_hexfile(fmPath);
            }
            else
            {
                read_binfile(fmPath);
            }

        }

        private void read_binfile(string fmpath)
        {
            if (!string.IsNullOrEmpty(fmPath))
            {
                otpFileName.Text = fmPath;

                try
                {
                    using (BinaryReader br = new BinaryReader(File.Open(fmPath, FileMode.Open)))
                    {
                        pmOTP.reset();

                        int pos = 0;

                        int length = (int)br.BaseStream.Length;

                        while(pos < length)
                        {
                            byte inb = br.ReadByte();  
                            pmOTP.insert(pos, inb);

                            pos += sizeof(byte);
                        }
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message, "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void read_hexfile(string fmpath)
        {
            if (!string.IsNullOrEmpty(fmPath))
            {
                otpFileName.Text = fmPath;

                try
                {
                    using (StreamReader sr = new StreamReader(fmPath))
                    {
                        string aline;
                        int recLBA = 0;
                        pmOTP.reset();

                        while ((aline = sr.ReadLine()) != null)
                        {
                            hexline_process(ref recLBA, aline);
                        }
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message, "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void otp_load_adress(int address)
        {
            // NOTE: address is word based
            byte[] data = new byte[4];

            data[0] = global.CMD_LOAD_ADDRESS;
            data[1] = (byte)(address & 0xff);
            data[2] = (byte)((address >> 8) & 0xff);
            data[3] = global.CMD_EOP;

            portBusy();
            serial_send(data, 4);
            portBusy(false);
        }

        private void otp_page_program(ByteArray pdata)
        {
            byte[] data = new byte[4];

            data[0] = global.CMD_PAGE_PROG;
            data[1] = 0x00;
            data[2] = 0x01;

            portBusy();
            serial_send(data, 3, false);

            // send sector data
            serial_send(pdata.data, pdata.data.Length, false);

            data[0] = global.CMD_EOP;
            serial_send(data, 1);

            portBusy(false);

        }

        private bool otp_page_verify(int address, ByteArray pdata)
        {
            bool ret = false;

            byte[] data = new byte[4];

            data[0] = global.CMD_PAGE_VERIFY;
            data[1] = (byte)(address & 0xff);
            data[2] = (byte)((address >> 8) & 0xff);

            portBusy();
            serial_send(data, 3, false);

            // send sector data for verification
            serial_send(pdata.data, pdata.data.Length, false);

            data[0] = global.CMD_EOP;
            serial_send(data, 1, false);

            // get result
            if (serial_get(ref data, 1))
            {
                if (data[0] == 0x01)
                {
                    ret = true;
                }
            }

            INSYNC();

            portBusy(false);

            return ret;
        }

        private bool otp_page_verify2()
        {
            bool ret = false;

            byte[] data = new byte[2];

            data[0] = global.CMD_PAGE_VERIFY2;
            data[1] = global.CMD_EOP;

            portBusy();
            serial_send(data, 2, false);

            if (serial_get(ref data, 1))
            {
                if (data[0] == 0x01)
                {
                    ret = true;
                }
            }

            INSYNC();
            portBusy(false);

            return ret;
        }

        private void otpWriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!checkPort())
            {
                return;
            }

            otpProgressBar.Maximum = (4-psSector)*25;
            otpProgressBar.Value = 0;

            Thread nThread = new Thread(() =>
                {
                    bool bv = true;
                    double pb_value = 0;

                    try
                    {
                        for (int i = psSector; i < OTP.SECOTR_NUM; i++)
                        {
                            if (pmOTP.byteList[i].dirty)
                            {
                                // do sector program here
                                otp_load_adress((pmOTP.byteList[i].start >> 1));
                                otp_page_program(pmOTP.byteList[i]);

                                //if (otp_page_verify2() == false)
                                
                                if (otp_page_verify(pmOTP.byteList[i].start >> 1, pmOTP.byteList[i]) == false)
                                {
                                    string err = string.Format("OTP Verify Error at sector {0}", i);
                                    MessageBox.Show(err, "Error", MessageBoxButton.OK,
                                        MessageBoxImage.Error);

                                    bv = false;
                                }
                                
                            }

                            this.Dispatcher.Invoke((Action)(() =>
                                {
                                    if (bv == false)
                                        pb_value = otpProgressBar.Maximum;
                                    else
                                        pb_value += 25;

                                    update_progress(pb_value);

                                }));
                        }
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message, "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }

                });

            nThread.Start();
        }

        private void otpCheck_Click(object sender, RoutedEventArgs e)
        {
            // presettings
            otpProgressBar.Maximum = 4 * 256;

            otp1Sector.Background = Brushes.BlanchedAlmond;
            otp2Sector.Background = Brushes.BlanchedAlmond;
            otp3Sector.Background = Brushes.BlanchedAlmond;
            otp4Sector.Background = Brushes.BlanchedAlmond;
            otp1Sector.Text = "sector 0";
            otp2Sector.Text = "sector 1";
            otp3Sector.Text = "sector 2";
            otp4Sector.Text = "sector 3";

            Thread nThread = new Thread(() =>
                {                
                    int address;
                    byte[] data = new byte[3];
                    object[] obj = new object[4] { otp1Sector, otp2Sector, otp3Sector, otp4Sector };

                    send_basic();

                    portBusy();

                    try
                    {

                        for (int n = 0; n < 4; n++)
                        {
                            address = n * 256;

                            // load address
                            otp_load_adress(address);

                            // send data for verification
                            data[0] = global.CMD_PAGE_VERIFY;
                            data[1] = 0x00;
                            data[2] = (byte)n; // sector length

                            serial_send(data, 3, false);
                            data[0] = 0xff;
                            data[1] = 0x3f;
                            for (int i = 0; i < 256; i++)
                            {
                                serial_send(data, 2, false);

                                this.Dispatcher.Invoke((Action)(() =>
                                update_progress(n * 256 + i)));

                            }

                            data[0] = global.CMD_EOP;
                            serial_send(data, 1, false);

                            if (serial_get(ref data, 1))
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                    {
                                        if (data[0] != 0x01)
                                        {
                                            (obj[n] as TextBlock).Text = "XX";
                                            (obj[n] as TextBlock).Background = Brushes.Red;
                                        }
                                        else
                                        {
                                            (obj[n] as TextBlock).Text = "FF";
                                            (obj[n] as TextBlock).Background = Brushes.SkyBlue;
                                        }
                                    }));
                            }

                            INSYNC();
                        }
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message, "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }

                    portBusy(false);

                });

            nThread.Start();

        }

        private void btnFUSE1_Click(object sender, RoutedEventArgs e)
        {

            if (!checkPort())
                return;

            byte[] data = new byte[5];

            data[0] = global.CMD_FUSE_PROG;
            data[1] = 0x00;
            data[4] = global.CMD_EOP;

            data[3] = 0xfe;
            data[2] = 0x00;

            if (cw1_osco == true)
                data[3] |= 0x01;

            if (cw1_fosc == 1 || cw1_fosc == 3 || cw1_fosc == 4 || cw1_fosc == 5)
                data[2] |= 0x01;
            if (cw1_fosc == 1 || cw1_fosc == 2 || cw1_fosc == 4)
                data[2] |= 0x02;
            if (cw1_fosc == 1 || cw1_fosc == 2 || cw1_fosc == 3)
                data[2] |= 0x04;

            if (cw1_sut == 1 || cw1_sut == 3)
                data[2] |= 0x08;
            if (cw1_sut == 2 || cw1_sut == 3)
                data[2] |= 0x10;

            if (cw1_rcm == 1 || cw1_rcm == 3)
                data[2] |= 0x20;
            if (cw1_rcm == 0 || cw1_rcm == 1 || cw1_rcm == 4)
                data[2] |= 0x40;
            if (cw1_rcm == 0 || cw1_rcm == 1 || cw1_rcm == 2 || cw1_rcm == 3)
                data[2] |= 0x80;

            portBusy();
            serial_send(data, 5);

            // verify
            data[0] = global.CMD_FUSE_VERIFY;
            serial_send(data, 5, false);

            serial_get(ref data, 1);

            INSYNC();
            portBusy(false);

            if (data[0] != 0x1)
            {
                MessageBox.Show("Data verify error!", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

        }

        private void btnFUSE2_Click(object sender, RoutedEventArgs e)
        {
            if (!checkPort())
                return;

            byte[] data = new byte[5];

            data[0] = global.CMD_FUSE_PROG;
            data[1] = 0x01;
            data[4] = global.CMD_EOP;

            data[3] = 0xfe;
            data[2] = 0x00;

            if (cw2_mmode == 1)
                data[3] |= 1;

            if (cw2_pmod == 1)
                data[2] |= 0x01;
            if (cw2_wdte == true)
                data[2] |= 0x10;
            if (cw2_mcre == true)
                data[2] |= 0x20;
            
            // lvdt
            int i_lvdt = cw2_lvdt;

            if (cw2_lvdt == 6)
                i_lvdt = 0x7;

            data[2] |= (byte)(i_lvdt << 1);

            // tcyc
            if (cw2_tcyc == 1 || cw2_tcyc == 2)
                data[2] |= 0x40;
            if (cw2_tcyc == 2)
                data[2] |= 0x80;

            portBusy();
            serial_send(data, 5);

            // verify
            data[0] = global.CMD_FUSE_VERIFY;
            serial_send(data, 5, false);

            serial_get(ref data, 1);

            INSYNC();
            portBusy(false);

            if (data[0] != 0x1)
            {
                MessageBox.Show("Data verify error!", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void btnPortSettings_Click(object sender, RoutedEventArgs e)
        {
            if (dlgPortSettings == null)
            {
                dlgPortSettings = new SerialSettings();

                dlgPortSettings.Closed += (o, evt) => dlgPortSettings = null;

                dlgPortSettings.Show();
            }
            else 
            {
                dlgPortSettings.Activate();
                dlgPortSettings.Show();
            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            if (comPort != null && comPort.IsOpen)
            {
                comPort.Close();
            }
        }

        private bool INSYNC()
        {
            int i = 0;
            byte sync = 0;
            string stmp = string.Empty;

            if (comPort.IsOpen)
            {
                comPort.ReadTimeout = 1000;

                try
                {
                    do {
                        sync = Convert.ToByte(comPort.ReadByte());
                        //Thread.Sleep(10);
                    } while(++i < 0xff && sync != global.CMD_INSYNC);
                }
                catch (Exception e)
                {
                    comPort.DiscardOutBuffer();
                    comPort.DiscardInBuffer();

                    MessageBox.Show(e.Message, "Warining",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return sync == global.CMD_INSYNC;
        }

        private byte convert2Byte(string input)
        {
            byte result = 0;

            try
            {
                if (input.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
                {
                    result = Convert.ToByte(input, 16);
                }
                else
                {
                    result = Convert.ToByte(input);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return result;
        }

        private bool checkPort()
        {
            if (comPort == null || !comPort.IsOpen)
            {
                MessageBox.Show("Serial port is not ready!", "Warining", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void portBusy(bool isBusy = true)
        {
            //Brush old = btnPortSettings.Background;

            this.Dispatcher.BeginInvoke((Action)(() =>
                {
                    if (isBusy)
                    {
                        btnPortSettings.Background = Brushes.Red;
                    }
                    else if (comPort != null && comPort.IsOpen)
                    {
                        btnPortSettings.Background = Brushes.SkyBlue;
                    }
                    else
                    {
                        btnPortSettings.Background = Brushes.Transparent;
                    }
                }));
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!checkPort())
                return;

            //comPort.DiscardOutBuffer();
            //comPort.DiscardInBuffer();
            send_basic();

            byte[] data = new byte[4];

            data[0] = global.CMD_CTLS;
            data[1] = global.CMD_READ_ID;
            data[2] = global.CMD_EOP;

            portBusy();
            serial_send(data, 3, false);

            if (serial_get(ref data, 4))
            {
                txbDeviceID.Text = string.Format("0x{0:x2}{1:x2}{2:x2}{3:x2}", data[3], data[2], data[1], data[0]);
            }

            INSYNC();
            portBusy(false);

        }

        private void serial_send(byte [] data, int length, bool bINSYNC = true)
        {
            comPort.DiscardInBuffer();
            comPort.DiscardOutBuffer();

            comPort.Write(data, 0, length);

            if (bINSYNC)
            {
                // frame sync
                INSYNC();
            }
        }

        private bool serial_get(ref byte[] data, int length)
        {
            int got = 0;
            int rem = length;

            if (comPort.IsOpen)
            {
                comPort.ReadTimeout = 1000;

                try
                {
                    do
                    {
                        got = comPort.Read(data, length-rem, rem);
                        rem -= got;
                    } while (rem > 0);

                    return true;
                }
                catch (Exception e)
                {
                    comPort.DiscardInBuffer();
                    comPort.DiscardOutBuffer();

                    MessageBox.Show(e.Message, "Warining",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return false;
        }

        public void send_basic()
        {
            if(!checkPort())
            {
                return;
            }

            byte[] data = new byte[3];

            data[0] = global.CMD_CTLS;
            data[1] = global.CMD_BASIC;
            data[2] = global.CMD_EOP;

            portBusy();

            serial_send(data, 3);

            portBusy(false);

        }

        private void update_progress(double value)
        {
            if (value > 0 && otpProgressBar.Visibility == Visibility.Collapsed)
            {
                otpFileName.Visibility = Visibility.Collapsed;
                otpProgressBar.Visibility = Visibility.Visible;
            }

            otpProgressBar.Value = value;

            if (value >= (otpProgressBar.Maximum - 1))
            {
                otpProgressBar.Value = 0;
                otpProgressBar.Visibility = Visibility.Collapsed;
                otpFileName.Visibility = Visibility.Visible;
            }

        }

        private void hexline_process(ref int recLBA, string line)
        {
            int recUSBA = 0;

            if (!line[0].Equals(':'))
            {
                Exception e = new Exception("Wrong HEX File format!");
                throw e;
            }

            string recLen = line.Substring(1, 2);
            string ldOffset = line.Substring(3, 4);
            string recType = line.Substring(7, 2);

            if(recType.Equals("02"))
            {
                recUSBA = Convert.ToInt32(line.Substring(9, 4));
                recLBA += recUSBA << 4;
            }
            else if (recType.Equals("00"))
            {
                byte bdata;
                int recStart = 9;
                int wOffset = Convert.ToInt32(ldOffset, 16);
                int wRecLen = Convert.ToInt32(recLen, 16);
                int address = recLBA + wOffset;

                for (int i = 0; i < wRecLen; i+=1)
                {
                    string bstr = line.Substring(recStart, 2);
                    bdata = Convert.ToByte(bstr, 16);
                    pmOTP.insert(address, bdata);

                    recStart += 2;
                    address += 1;

                }
            }
        }

        private void otpVerifyButton_Click(object sender, RoutedEventArgs e)
        {
            send_basic();

            if (string.IsNullOrEmpty(otpFileName.Text))
            {
                MessageBox.Show("Please open firmware file for verification!", "Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                return;
            }

            otp1Sector.Background = Brushes.BlanchedAlmond;
            otp2Sector.Background = Brushes.BlanchedAlmond;
            otp3Sector.Background = Brushes.BlanchedAlmond;
            otp4Sector.Background = Brushes.BlanchedAlmond;
            otp1Sector.Text = "sector 0";
            otp2Sector.Text = "sector 1";
            otp3Sector.Text = "sector 2";
            otp4Sector.Text = "sector 3";

            Thread nThread = new Thread(() =>
                {
                    bool bv = false;
                    object[] o_sectors = new object[] { otp1Sector, otp2Sector, otp3Sector, otp4Sector };

                    for (int i = psSector; i < 4; i++)
                    {
                        TextBlock tb = o_sectors[i] as TextBlock;
                        otp_load_adress((pmOTP.byteList[i].start >> 1));
                        bv = otp_page_verify(pmOTP.byteList[i].start >> 1, pmOTP.byteList[i]);

                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            if (bv)
                            {
                                tb.Text = "PASS";
                                tb.Background = Brushes.SkyBlue;
                            }
                            else
                            {
                                tb.Text = "FAIL";
                                tb.Background = Brushes.Red;
                            }
                        }));
                    }
                });

            nThread.Start();
        }

        private void btnFUSE1Verify_Click(object sender, RoutedEventArgs e)
        {
            if (!checkPort())
                return;

            byte[] data = new byte[5];

            data[0] = global.CMD_FUSE_VERIFY;
            data[1] = 0x00;
            data[4] = global.CMD_EOP;

            data[3] = 0xfe;
            data[2] = 0x00;

            if (cw1_osco == true)
                data[3] |= 0x01;

            if (cw1_fosc == 1 || cw1_fosc == 3 || cw1_fosc == 4 || cw1_fosc == 5)
                data[2] |= 0x01;
            if (cw1_fosc == 1 || cw1_fosc == 2 || cw1_fosc == 4)
                data[2] |= 0x02;
            if (cw1_fosc == 1 || cw1_fosc == 2 || cw1_fosc == 3)
                data[2] |= 0x04;

            if (cw1_sut == 1 || cw1_sut == 3)
                data[2] |= 0x08;
            if (cw1_sut == 2 || cw1_sut == 3)
                data[2] |= 0x10;

            if (cw1_rcm == 1 || cw1_rcm == 3)
                data[2] |= 0x20;
            if (cw1_rcm == 0 || cw1_rcm == 1 || cw1_rcm == 4)
                data[2] |= 0x40;
            if (cw1_rcm == 0 || cw1_rcm == 1 || cw1_rcm == 2 || cw1_rcm == 3)
                data[2] |= 0x80;

            portBusy();
            serial_send(data, 5, false);

            serial_get(ref data, 1);

            INSYNC();
            portBusy(false);

            if (data[0] != 0x1)
            {
                MessageBox.Show("Data verify error!", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Data verfiy pass!", "Pass", MessageBoxButton.OK,
                    MessageBoxImage.Asterisk);
            }
        }

        private void btnFUSE2Verify_Click(object sender, RoutedEventArgs e)
        {
            if (!checkPort())
                return;

            byte[] data = new byte[5];

            data[0] = global.CMD_FUSE_VERIFY;
            data[1] = 0x01;
            data[4] = global.CMD_EOP;

            data[3] = 0xfe;
            data[2] = 0x00;

            if (cw2_mmode == 1)
                data[3] |= 1;

            if (cw2_pmod == 1)
                data[2] |= 0x01;
            if (cw2_wdte == true)
                data[2] |= 0x10;
            if (cw2_mcre == true)
                data[2] |= 0x20;

            // lvdt
            int i_lvdt = cw2_lvdt;

            if (cw2_lvdt == 6)
                i_lvdt = 0x7;

            data[2] |= (byte)(i_lvdt << 1);

            // tcyc
            if (cw2_tcyc == 1 || cw2_tcyc == 2)
                data[2] |= 0x40;
            if (cw2_tcyc == 2)
                data[2] |= 0x80;

            portBusy();
            serial_send(data, 5, false);

            serial_get(ref data, 1);

            INSYNC();
            portBusy(false);

            if (data[0] != 0x1)
            {
                MessageBox.Show("Data verify error!", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Data verfiy pass!", "Pass", MessageBoxButton.OK,
                    MessageBoxImage.Asterisk);
            }
        }

    }
}
