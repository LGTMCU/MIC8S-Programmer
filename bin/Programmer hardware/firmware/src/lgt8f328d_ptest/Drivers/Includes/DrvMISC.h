/*
  							  	****************
*******************************  C HEADER FILE  **********************************
** 								**************** 						        **
** 																		        **
** project  : BSPLGT8F0XA												    	**
** filename : DrvMISC.h 	   	 													**
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
 * @file DrvMISC.h
 * @brief Header File of MISC
 *		
 */

#ifndef _MISC_H_
#define _MISC_H_

/**********************************************************************************
***					          TYPEDEFS AND STRUCTURES							***													  	
**********************************************************************************/

/**********************************************************************************
***					          	EXPORTED VARIABLES								***													  	
**********************************************************************************/
#ifndef _MISC_SRC_
#endif

#define DrvMISC_SetPUD() MCUCR |= 0x10
#define DrvMISC_SetSWDD() MCUSR |= 0x9F

#define DrvMISC_ClearResetFlags() MCUSR = 0
#define DrvMISC_GetResetFlags() (MCUSR & 0x3F)
#define DrvMISC_GetPORResetFlag() (MCUSR & 0x1)
#define DrvMISC_GetExtResetFlag() ((MCUSR >> 1) & 0x1)
#define DrvMISC_GetLVRResetFlag() ((MCUSR >> 2) & 0x1)
#define DrvMISC_GetWDTResetFlag() ((MCUSR >> 3) & 0x1)
#define DrvMISC_GetPDRFResetFlag() ((MCUSR >> 5) & 0x1)

#define DrvMISC_GetGUID3() (GUID3)
#define DrvMISC_GetGUID2() (GUID2)
#define DrvMISC_GetGUID1() (GUID1)
#define DrvMISC_GetGUID0() (GUID0)

#define DrvMISC_GetBGR() (GUID0)

#define DrvMISC_WriteGPR0(value) GPIOR0 = value
#define DrvMISC_WriteGPR1(value) GPIOR1 = value
#define DrvMISC_WriteGPR2(value) GPIOR2 = value
#define DrvMISC_ReadGPR0() (GPIOR0)
#define DrvMISC_ReadGPR1() (GPIOR1)
#define DrvMISC_ReadGPR2() (GPIOR2)

#define delay10us()	do { NOP(); NOP(); NOP(); NOP(); NOP(); NOP(); NOP(); NOP(); \
						 NOP(); NOP(); NOP(); NOP(); NOP(); NOP(); NOP(); NOP(); \
						 NOP(); NOP(); NOP(); NOP(); NOP(); NOP(); NOP(); NOP(); \
						 NOP(); NOP(); NOP(); NOP(); NOP(); NOP(); NOP(); NOP(); \
						  NOP(); NOP(); NOP(); NOP(); NOP(); NOP(); NOP(); NOP(); } while(0)

#define delay40us() do {  delay10us(); delay10us(); delay10us(); delay10us(); } while(0)

#define delay90us() do { delay40us(); delay40us(); delay10us(); } while(0);


#define delayus(us) DrvMISC_Delayus(us)
#define delayms(ms) DrvMISC_Delayms(ms)

/**********************************************************************************
*** 						  	EXPORTED FUNCTIONS								*** 													
**********************************************************************************/
void DrvMISC_Delayus(u16 us);
void DrvMISC_Delayms(u16 ms);
void DrvMISC_CLRI(void);
void DrvMISC_RESI(void);
void DrvMISC_SoftReset(void);

#endif
/**********************************************************************************
***					          			EOF										***													  	
**********************************************************************************/

