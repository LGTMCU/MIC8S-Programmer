#ifndef __GPIO_H
#define __GPIO_H

#include "allinone.h"

#define OUT7 0x80
#define OUT6 0x40
#define OUT5 0x20
#define OUT4 0x10
#define OUT3 0x08
#define OUT2 0x04
#define OUT1 0x02
#define OUT0 0x01

#define IN7 0x7
#define IN6 0x6
#define IN5 0x5
#define IN4 0x4
#define IN3 0x3
#define IN2 0x2
#define IN1 0x1
#define IN0 0x0

#define DrvGPIO_DisableOutput(PN, ION) \
	DDR##PN &= ~ION

#define DrvGPIO_EnableOutput(PN, ION) \
	DDR##PN |= ION

#define DrvGPIO_SetPort(PN, ION) \
	PORT##PN |= ION

#define DrvGPIO_ClearPort(PN, ION) \
	PORT##PN &= ~ION

#define DrvGPIO_TogglePort(PN, ION) \
	PORT##PN = PIN##PN ^ ION

#define DrvGPIO_SetPullup(PN, ION) \
	DDR##PN &= ~ION; \
	PORT##PN &= ~ION

#define DrvGPIO_GetPort(PN) (PIN##PN)
#define DrvGPIO_GetSinglePin(PN, ION) ((PIN##PN & (1 << ION)) >> ION)

void DrvGPIO_Init(void);

#endif
