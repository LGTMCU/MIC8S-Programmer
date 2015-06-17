#include "allinone.h"

u8 check_eop(void)
{
	u8 ch, i = 0;

	do {
		ch = DrvUSART_GetChar();

	} while((ch != CMD_EOP) && (++i != 0));

	return ch == CMD_EOP;	
}

void send_insync(void)
{
	DrvUSART_SendChar(CMD_INSYNC);
}

void testif_basic(void)
{
	SWD_SendIdle(100);

	SWD_SendByte(1, 0x00, 1);

	SWD_SendByte(1, 0xd0, 0);
	SWD_SendByte(0, 0xaa, 0);
	SWD_SendByte(0, 0x55, 0);
	SWD_SendByte(0, 0xaa, 0);
	SWD_SendByte(0, 0x55, 1);
	SWD_SendIdle(4);
}

void testif_readid(void)
{
	u8 tmp[4];

	SWD_SendByte(1, 0xae, 1);
	SWD_SendIdle(4);
	
	tmp[0] = SWD_ReadByte(1, 0);
	tmp[1] = SWD_ReadByte(0, 0);
	tmp[2] = SWD_ReadByte(0, 0);
	tmp[3] = SWD_ReadByte(0, 1);
	SWD_SendIdle(4);

	DrvUSART_SendChar(tmp[0]);
	DrvUSART_SendChar(tmp[1]);
	DrvUSART_SendChar(tmp[2]);
	DrvUSART_SendChar(tmp[3]);

}

void testif_otp_cseq(u8 cw, u16 addr)
{
	u8 tmp0 = addr & 0xff;
	u8 tmp1 = (cw << 2) | ((addr >> 8) & 0x3);

	SWD_SendByte(1, 0xb2, 0);
	SWD_SendByte(0, tmp0, 0);
	SWD_SendByte(0, tmp1, 1);
	SWD_SendIdle(8);	 
}

void testif_otp_dseq(u8 type, u16 data)
{
	u8 tmp0 = data & 0xff;
	u8 tmp1 = (type << 7) | ((data >> 8) & 0x3f);

	SWD_SendByte(1, 0xb3, 0);
	SWD_SendByte(0, tmp0, 0);
	SWD_SendByte(0, tmp1, 1);
	SWD_SendIdle(8);	 
}

void testif_otp_vppon(void)
{
	// set program mode 
	testif_otp_cseq(OTP_PTM, 0);
	// enable HV power supply
	VPPEN_SET();
	// delay for vpp stable
	delayms(10);
}

void testif_otp_vppoff(void)
{
	// disable HV power supply
	VPPEN_CLR();
	// delay for vpp stable
	delayms(20);

	// set PTM = 000 
	testif_otp_cseq(OTP_IDLE, 0);
}

void testif_otp_write(u8 pif, u16 addr, u16 data)
{	
	// prepare data for program
	testif_otp_dseq(0, data);
	// set program mode 
	testif_otp_cseq(0x10, 0);
	// set prog
	testif_otp_cseq(0x12, 0);
	// set address and pif
	testif_otp_cseq((0x12 | pif), addr);
	// set pwe to do program
	testif_otp_cseq((0x16 | pif), addr);
	// Tpw timing (about 90us)
	delay90us();
	//delayus(80);

	// reset pwe
	testif_otp_cseq((0x12 | pif), addr);
	// reset prog
	testif_otp_cseq((0x10 | pif), addr);
	// Tvs timing (about 10us)
	delay10us();
	//delayus(10);
}

u8 testif_otp_read(void)
{
	u8 tmp0, tmp1;

	SWD_SendByte(1, 0xab, 1);
	SWD_SendIdle(4);

	tmp0 = SWD_ReadByte(1, 0);
	tmp1 = SWD_ReadByte(0, 1);
	SWD_SendIdle(4);

	return (tmp1 & 0xfe) | (tmp0 & 0x1);
}

// prepare for data verification
void testif_otp_vstart(u8 pif, u16 addr)
{
	// dtype = 1 for verify
	testif_otp_dseq(1, 0);
	// set start address
	testif_otp_cseq(pif, addr);
}

// do sector based data verification
u8 testif_otp_sector_verify(u16 addr, u16 *pdata)
{
	u16 i = 0;

	testif_otp_vstart(0, addr);

	for(i = 0; i < OTP_SECTOR_SIZE; i++)
	{
		testif_otp_dseq(1, pdata[i]);
	}

	return testif_otp_read();
}

u8 testif_otp_sector_write(u16 addr, u16 *pdata)
{
	u16 i = 0;
	testif_otp_vppon();

	for(i = 0; i < OTP_SECTOR_SIZE; i++)
	{
		testif_otp_write(OTP_DAT, (addr+i), pdata[i]);
	}

	testif_otp_vppoff();
}

void testif_fuse_write(u8 addr, u16 data)
{
	testif_otp_vppon();

	testif_otp_write(OTP_CFG, addr, data);

	testif_otp_vppoff();
}

u8 testif_fuse_verify(u8 addr, u16 data)
{
	testif_otp_vstart(OTP_CFG, addr);
	testif_otp_dseq(1, data);

	return testif_otp_read();
}
