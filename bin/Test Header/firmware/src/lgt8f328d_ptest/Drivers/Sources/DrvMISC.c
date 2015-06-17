/*
  							  	****************
*******************************  C SOURCE FILE  **********************************
** 								**************** 						        **
** 																		        **
** project  : BSPLGT8F328D												    	**
** filename : DrvAC.c	  		   	 											**
** version  : 1.0 													   			**
** date     : April 01, 2014 										   			**
** 			  		 	 												   		**
**********************************************************************************
** 																		   		**
** Copyright (c) 2014, 	LogicGreen Technologies Co., LTD						**
** All rights reserved.                                                    		**
**                                                                         		**
**********************************************************************************
VERSION HISTORY:
----------------
Version 	: 1.0
Date 		: April 01, 2014
Revised by 	: LogicGreen Software Group
Description : Original version.
*/

/**
 * @file DrvAC.c
 * @brief Source File of AC
 */

/** complier directives */
#define _DRVMISC_SRC_

/**********************************************************************************
***					            MODULES USED									***													  	
**********************************************************************************/ 
#include "allinone.h"
	
/**********************************************************************************
***					     	 MACROS AND DEFINITIONS								***													  	
**********************************************************************************/ 
#ifndef FCLK
#error FSYSCLK.h should be included!!!
#endif

#define ARGS_CYS_PER_US ((FCLK + 500000)/1000000)

static u8 _sreg = 0;

/**********************************************************************************
*** 						  	EXPORTED FUNCTIONS								*** 													
**********************************************************************************/

void DrvMISC_Delayus(u16 us)
{
	volatile u16 i;
	u16 cycles = (us >> 4) * (ARGS_CYS_PER_US);

	for(i = 0; i < cycles; i++)
	{
	}
}

void DrvMISC_Delayms(u16 ms)
{
	u16 cycles = ms;

	do {
		DrvMISC_Delayus(1000);
	} while (--cycles > 0);
}

void DrvMISC_CLRI(void)
{
	_sreg = SREG;
	CLI();
}

void DrvMISC_RESI(void)
{
	SREG = _sreg;
}

void DrvMISC_SoftReset(void)
{
	 VDTCR |= 0xe0;
	 VDTCR &= 0xbf;
}

/**********************************************************************************
*** 									EOF 									*** 													
**********************************************************************************/

