/*
  							  	****************
*******************************  C SOURCE FILE  **********************************
** 								**************** 						        **
** 																		        **
** project  : BSPLGT8F328D												    	**
** filename : DrvSYS.c	 		 												**
** version  : 1.0 													   			**
** date     : April 01, 2013 										   			**
** 			  		 	 												   		**
**********************************************************************************
** 																		   		**
** Copyright (c) 2013, 	Logic Green Technologies								**
** All rights reserved.                                                    		**
**                                                                         		**
**********************************************************************************
VERSION HISTORY:
----------------
Version 	: 1.0
Date 		: April 01, 2013
Revised by 	: LGT Software Group
Description : Original version.
*/


/** complier directives */
#define _DRVSYS_SRC_C_

/**********************************************************************************
***					           	 	MODULES USED								***													  	
**********************************************************************************/
#include "allinone.h"

#if (MCK_MCLKSEL == 0) && (MCK_RCMEN == 0)
	#warning <W0:System Settings> Internal 32MHz/RC should be enabled for using as main clock source
#endif

#if (MCK_MCLKSEL == 1) && (MCK_OSCMEN == 0)
	#warning <W0:System Settings> External high speed crystal should be enabled for using as main clock source
#endif

#if (MCK_MCLKSEL == 2) && (MCK_RCKEN == 0)
	#warning <W0:System Settings> Internal 32KHz/RC should be enabled for using as main clock source
#endif

#if (MCK_MCLKSEL == 3) && (MCK_OSCKEN == 0)
	#warning <W0:System Settings> External low speed crystal should be enabled for using as main clock source
#endif

#if (MCK_OSCMEN == 1) && (MCK_OSCKEN == 1)
	#warning <E0:System Settings> OSCM and OSCK can not be enabled at a time.
	#error OSCM and OSCK can not be enabled at a time.
#endif

/**********************************************************************************
***					            MACRO AND DEFINITIONS							***													  	
**********************************************************************************/  
/** Argument for Clock Set */
#define MCK_CLKENA ((MCK_OSCKEN << 3) | (MCK_OSCMEN << 2) | (MCK_RCKEN << 1) | MCK_RCMEN)

/**********************************************************************************
***					            EXPORTED FUNCTIONS								***													  	
**********************************************************************************/  
/**
 * @fn void DrvSYS_Init(void)
 */
void DrvSYS_Init(void)
{
#if (MCK_CLKDIV != 3)
	DrvCLK_SetClockDivider(MCK_CLKDIV);
#endif	

#if (MCK_MCLKSEL != 0)	
	DrvCLK_EnableClockSource(MCK_CLKENA);
	// wait for clock stable, eg. us

#if (MCK_OSCMEN == 1) && (MCK_MCLKSEL == 1)
	DrvMISC_Delayus(100);
#endif

#if (MCK_OSCKEN == 1) && (MCK_MCLKSEL == 3)
	DrvMISC_Delayms(500);
#endif

	DrvCLK_SetMainClock(MCK_MCLKSEL);
	NOP(); NOP();
#endif

#if (SYS_SWDD == 1)
	DrvMISC_SetSWDD();
#endif

#if (SYS_PUD == 1)
	DrvMISC_SetPUD();
#endif

#if (SYS_RSTIOEN == 1) || (SYS_VREFIOEN == 1)
	IOCR |= 0x80;
	IOCR |= (0x80 | ((SYS_RSTIOEN << 1) | SYS_VREFIOEN));
#endif

#if (MCK_CKOSEL == 1)
	CLKPR |= 0x40;
#endif

#if (MCK_CKOSEL == 2)
	CLKPR |= 0x20;
#endif
}

/**
 * @fn void DrvCLK_SetClk(u8 u8ClkEna)
 */
void DrvCLK_EnableClockSource(u8 u8ClkEna)
{
	u8 u8Reg;
	
	u8Reg = PMCR & 0xf;
	u8Reg |= u8ClkEna;

	DrvCLK_SetClock(u8Reg);
}

/**
 * @fn void DrvCLK_DisableClk(u8 u8ClkEna)
 */
void DrvCLK_DisableClockSource(u8 u8ClkDis)
{
	u8 u8Reg;

	u8Reg = PMCR & 0xf;
	u8Reg &= (~u8ClkDis);

	DrvCLK_SetClock(u8Reg);
}

/**
 * @fn static void DrvCLK_SetClock(u8 u8ClkEna)
 */
void DrvCLK_SetClock(u8 u8ClkEna)
{
	volatile u8 u8Tmp;

	Compiler_SetClk();

	u8Tmp = u8ClkEna;
}

/**
 * @fn void DrvCLK_SetMCLK(u8 u8MclkSel)
 */
void DrvCLK_SetMainClock(u8 u8MclkSel)
{
	volatile u8 u8Tmp;

	Compiler_SetMclk();

	u8Tmp = u8MclkSel;
}

/**
 * @fn void DrvCLK_SetDiv(u8 u8ClkDiv)
 */
void DrvCLK_SetClockDivider(u8 u8ClkDiv)
{
	volatile u8 u8Tmp;
	Compiler_SetClkDiv();

	u8Tmp = u8ClkDiv;
}

/**
 * @fn void DrvCLK_CloseCLKO(void)
 */
void DrvCLK_CloseCLKO(void)
{
#if (MCK_CKOSEL == 1)
	CLKPR &= 0xBF;
#endif
#if (MCK_CKOSEL == 2)
	CLKPR &= 0xDF;
#endif
}

/**********************************************************************************
*** 									EOF 									*** 													
**********************************************************************************/  

