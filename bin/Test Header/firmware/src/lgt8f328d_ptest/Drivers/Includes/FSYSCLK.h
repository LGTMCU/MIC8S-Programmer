#ifndef __FCLK_H__
#define __FCLK_H__

#include "macros_auto.h"

#ifndef MCK_FOSC
	#define MCK_FOSC 16000000
#endif

#ifndef MCK_MCLKSEL
	#define MCK_MCLKSEL 0x0
#endif

#ifndef MCK_CLKDIV
	#define MCK_CLKDIV 0x3
#endif

#if (MCK_MCLKSEL == 0x01) || (MCK_MCLKSEL == 0x03)
	#define FCLK (MCK_FOSC >> MCK_CLKDIV)
#elif (MCK_MCLKSEL == 0x00)
	#define FCLK (32000000 >> MCK_CLKDIV)
#elif (MCK_MCLKSEL == 0x02)
	#define FCLK (32000 >> MCK_CLKDIV)
#endif

#endif
