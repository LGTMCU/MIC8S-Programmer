/*******************************include*******************************/
#include "allinone.h"

u8 SWD_Port_Init( void )
{
	SWD_CLK_CLR();
	SWD_DAT_CLR();

	return 1;
}

void SWD_SendByte(E_START_SEL start, u8 data, E_STOP_VAL stop)
{
	u8 bCnt;

	//start bit
	if(start == E_1B_START) {
		SWD_CLK_CLR();
		SWD_Delay();
		SWD_DAT_CLR();
		SWD_Delay();
		SWD_CLK_SET();
		SWD_Delay();
	}

	//data byte
	for(bCnt = 0; bCnt < 8; bCnt++) {
		SWD_CLK_CLR();
		if(data & 0x01)
			SWD_DAT_SET();
		else
			SWD_DAT_CLR();
		SWD_Delay();
		data >>= 1;
		SWD_CLK_SET();
		SWD_Delay();
	}

	//stop bit
	SWD_CLK_CLR();
	if(stop == E_STOP_VAL1)
		SWD_DAT_SET();
	else
		SWD_DAT_CLR();
	SWD_Delay();
	SWD_CLK_SET();
	SWD_Delay();
}

u8 SWD_ReadByte(E_START_SEL start, E_STOP_VAL stop)
{
	u8 bCnt;
	volatile u8 bRev = 0;

	//start bit
	if(start == E_1B_START) {
		SWD_CLK_CLR();
		SWD_Delay();
		SWD_DAT_CLR();
		SWD_Delay();
		SWD_CLK_SET();
		SWD_Delay();
	}

	//set data input
	//SWD_PORT_DIR &= (~SWD_DAT);
	DrvGPIO_DisableOutput(B, OUT5);

	//data bye
	for(bCnt = 0; bCnt < 8; bCnt++) {
		bRev >>= 1;
		SWD_CLK_CLR();
		SWD_Delay();
		SWD_CLK_SET();
			
		if(PORTB & SWD_DAT)
			bRev |= 0x80;
		SWD_Delay();
	}
	//set data out put
	//SWD_PORT_DIR |= SWD_DAT;
	DrvGPIO_EnableOutput(B, OUT5);
	
	//stop bit
	SWD_CLK_CLR();
	if(stop == E_STOP_VAL0)
		SWD_DAT_CLR();
	else
		SWD_DAT_SET();
	SWD_Delay();
	SWD_CLK_SET();

	return bRev;
}

void SWD_SendIdle(u8 idleCnt)
{
	u8 bCnt;
	SWD_DAT_SET();

	for(bCnt = 0; bCnt < idleCnt; bCnt++) {
		SWD_CLK_CLR();
		SWD_Delay();
		SWD_CLK_SET();
		SWD_Delay();
	}		
}

