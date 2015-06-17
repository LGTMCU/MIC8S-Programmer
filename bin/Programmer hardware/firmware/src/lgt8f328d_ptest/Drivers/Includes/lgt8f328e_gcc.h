#ifndef __LGT8F328E_GCCV_H
#define __LGT8F328E_GCCV_H

#include "global.h"

/** compiler relative macros */
#define __ASM asm

#define SEI() __ASM("sei")
#define CLI() __ASM("cli")
#define SLEEP() __ASM("sleep")
#define WDR() __ASM("wdr")
#define NOP() __ASM("nop")

/** interrupt */
#define L_VECTOR(N)	__vector_##N
#define LGT_VECTOR(NAME) \
void NAME (void) __attribute__ ((signal, used, externally_visible)); \
void NAME (void)

/**********************************************************************************
*** 						  	EXPORTED FUNCTIONS								*** 													
**********************************************************************************/
#define Compiler_SetClk() do { \
	__ASM("lds 	r20, 0xf2"); \
	__ASM("andi	r20, 0x70"); \
	__ASM("ori	r20, 0x80"); \
	__ASM("or	r24, r20"); \
	__ASM("sts	0xf2, r24"); \
	__ASM("sts	0xf2, r24"); \
	} while(0)

#define Compiler_SetMclk() do { \
	__ASM("lds 	r20, 0xf2"); \
	__ASM("andi	r20, 0x1f"); \
	__ASM("ori	r20, 0x80"); \
	__ASM("lsl	r24"); \
	__ASM("lsl	r24"); \
	__ASM("lsl	r24"); \
	__ASM("lsl	r24"); \
	__ASM("lsl	r24"); \
	__ASM("or	r24, r20"); \
	__ASM("sts	0xf2, r24"); \
	__ASM("sts	0xf2, r24"); \
	} while(0)

#define Compiler_SetClkDiv() do { \
	__ASM("lds 	r20, 0x61"); \
	__ASM("andi	r20, 0x60"); \
	__ASM("ori	r20, 0x80"); \
	__ASM("or	r24, r20"); \
	__ASM("sts	0x61, r24"); \
	__ASM("sts	0x61, r24"); \
	} while(0)

#define Compiler_SetWclk() do { \
	__ASM("lds 	r20, 0xf2"); \
	__ASM("andi	r20, 0x6f"); \
	__ASM("ori	r20, 0x80"); \
	__ASM("lsl	r24"); \
	__ASM("lsl	r24"); \
	__ASM("lsl	r24"); \
	__ASM("lsl	r24"); \
	__ASM("or	r24, r20"); \
	__ASM("sts	0xf2, r24"); \
	__ASM("sts	0xf2, r24"); \
	} while(0)

#define Compiler_SetWDT() do { \
	__ASM("mov	r25, r24"); \
	__ASM("ori	r25, 0x08"); \
	__ASM("sts	0x60, r25"); \
	__ASM("sts	0x60, r24"); \
	} while (0)


#define Compiler_LPMReadWord() do { \
	__ASM("mov 	r30, r24"); \
	__ASM("mov 	r31, r25"); \
	__ASM("lpm	r24, z+"); \
	__ASM("lpm	r25, z"); \
	__ASM("mov	r30, r22"); \
	__ASM("mov	r31, r23"); \
	__ASM("st	z+, r24"); \
	__ASM("st	z, r25"); \
	} while (0)
    
#endif
