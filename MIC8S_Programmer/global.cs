using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MICS8S_Programmer
{
    static class global
    {
        public const byte CMD_INSYNC = 0x14;
        public const byte CMD_EOP = 0x20;

        public const byte CMD_LOAD_ADDRESS = 0x55;
        public const byte CMD_PAGE_PROG = 0x64;
        public const byte CMD_PAGE_VERIFY = 0x74;
        public const byte CMD_PAGE_VERIFY2 = 0x75;
        public const byte CMD_FUSE_PROG = 0x62;
        public const byte CMD_FUSE_VERIFY = 0x72;
        public const byte CMD_MISC_TEST = 0x82;
        public const byte CMD_OCC_WRITE = 0x84;
        public const byte CMD_OCC_READ = 0x94;

        public const byte CMD_CTLS = 0xa0;
        public const byte CMD_VPPON = 0x00;
        public const byte CMD_VPPOFF = 0x11;
        public const byte CMD_MCU_RON = 0x22;
        public const byte CMD_MCU_ROFF = 0x33;
        public const byte CMD_READ_ID = 0x44;
        public const byte CMD_MCU_RUN = 0x55;
        public const byte CMD_BASIC = 0xff;
    }

    class ByteArray
    {
        public bool dirty { get; set; }
        public int start { get; set; }
        public byte[] data { get; private set; }

        public ByteArray(int address)
        {
            dirty = false;
            start = address & 0xfe00;
            data = new byte[512];

            for (int i = 0; i < data.Length; i+=2)
            {
                data[i] = 0xff;
                data[i + 1] = 0x3f;
            }
        }

        public void insert(byte item, int address)
        {
            if ((address & 0xfe00) == start)
            {
                data[address & 0x1ff] = item;

                if (dirty == false)
                {
                    if ((address & 1) == 1)
                    {
                        dirty = (item & 0x3f) != 0x3f;
                    }
                    else
                    {
                        dirty = item != 0xff;
                    }
                }
            }
            else
            {
                Exception exp = new Exception("Add to wrong sector!");
                throw exp;
            }
        }

        public void clear()
        {
            for (int i = 0; i < data.Length; i+=2)
            {
                data[i] = 0xff;
                data[i + 1] = 0x3f;
            }

            dirty = false;
        }

    }

    class OTP
    {
        public static readonly int SECOTR_NUM = 4;
        public static readonly int SECTOR_SIZE = 512;

        public List<ByteArray> byteList { get; set; }

        public OTP()
        {
            byteList = new List<ByteArray>();

            byteList.Add(new ByteArray(0));
            byteList.Add(new ByteArray(0x200));
            byteList.Add(new ByteArray(0x400));
            byteList.Add(new ByteArray(0x600));
        }

        public void insert(int address, byte item)
        {
            int index = address >> 9;

            if ((index + 1) > SECOTR_NUM)
            {
                Exception e = new Exception("HEX file more than 1KWord!");
                throw e;
            }

            byteList[index].insert(item, address);
        }

        public void reset()
        {
            byteList.Clear();
            byteList.Add(new ByteArray(0));
            byteList.Add(new ByteArray(0x200));
            byteList.Add(new ByteArray(0x400));
            byteList.Add(new ByteArray(0x600));
        }

    }

    class occAction
    {
        public bool isWrite { get; set; }
        public string address { get; set; }
        public string data { get; set; }
        public int loops { get; set; }

        public int _address()
        {
            if (string.IsNullOrEmpty(address))
            {
                return 0;
            }
            else if (address.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
            {
                return Convert.ToInt32(address, 16);
            }
            else
            {
                return Convert.ToInt32(address);
            }

        }

        public int _data()
        {
            if (string.IsNullOrEmpty(data))
            {
                return 0;
            }
            else if (data.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
            {
                return Convert.ToInt32(data, 16);
            }
            else
            {
                return Convert.ToInt32(data);
            }
        }

        public occAction()
        {
            isWrite = false;
            address = "0";
            data = "0";
        }

        public override string ToString()
        {
            if (isWrite)
            {
                return string.Format("Write: 0x{0:x2} to address[0x{1:x2}]",  _data(), _address());
            }
            else
            {
                return string.Format("Read: from address[0x{0:x2}]", _address());
            }
        }
    }

}
