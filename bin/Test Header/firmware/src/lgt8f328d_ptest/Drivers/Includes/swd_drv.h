#ifndef _SWD_DRV_H_
#define _SWD_DRV_H_

#include "global.h"

typedef enum {
	E_NO_START,
	E_1B_START
} E_START_SEL;

typedef enum {
	E_STOP_VAL0,
	E_STOP_VAL1
} E_STOP_VAL;

typedef enum {
	E_SCMD_OCD_GPR,			//SWD CMD of general registers
	E_SCMD_OCD_IO,			//SWD CMD of I/O register
	E_SCMD_OCD_SRAM,		//SWD CMD of SRAM
} E_SCMD_OCD;


/*SWD Port selection*/
#define SWD_CLK				(1 << 4)		//PB4
#define SWD_DAT				(1 << 5)		//PB5
#define VPP_EN				(1 << 3)		//PB3
#define EXRESET				(1 << 2)		//PB2

#define SWD_PORT_PIN		PINB 
#define SWD_PORT_DIR		DDRB
#define SWD_PORT_SET		PORTB

#define SWD_CLK_CLR() 		(SWD_PORT_SET &= ~SWD_CLK)		
#define SWD_CLK_SET()   	(SWD_PORT_SET |= SWD_CLK)
#define SWD_DAT_CLR()		(SWD_PORT_SET &= ~SWD_DAT)
#define SWD_DAT_SET()		(SWD_PORT_SET |= SWD_DAT)

#define VPPEN_SET()			PORTB |= 0x08
#define	VPPEN_CLR()			PORTB &= 0xf7

#define	EXRESET_ON()		PORTB &= 0xfb
#define EXRESET_OFF()		PORTB |= 0x04

#define	SWD_Delay()			do { NOP(); NOP(); NOP(); NOP(); NOP(); NOP(); NOP(); NOP(); } while (0)

void SWD_Init( void );
void SWD_SendByte(E_START_SEL start, u8 data, E_STOP_VAL stop);
u8 SWD_ReadByte(E_START_SEL start, E_STOP_VAL stop);
void SWD_SendIdle(u8 idleCnt);

#endif
