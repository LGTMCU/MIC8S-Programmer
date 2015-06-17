//===================================================
// Auto generated file : 2015-03-29 12:57:17
//===================================================

#include "allinone.h"

// Import external definitions
extern void init_modules(void);

u8 s_ch;
u16	s_address;
u8 s_data[2];
u16 s_buffer[256];

u8 errcode;

int main(void)
{
	u16 dwVar = 0;

	// Device initialization
	init_modules();
	SWD_Init();

	//DrvUSART_SendStr("Hello LGT8P653A\n");

	// Add your code from here
	for(;;)
	{
		errcode = 0;

		s_ch = DrvUSART_GetChar();

		if(s_ch == CMD_LOAD_ADDRESS)
		{
			s_address = DrvUSART_GetChar();
			s_address |= (DrvUSART_GetChar() << 8);
		
			check_eop();
		}
		else if(s_ch == CMD_PAGE_PROG)
		{
			dwVar = DrvUSART_GetChar();
			dwVar |= DrvUSART_GetChar() << 8;

			/*
			if(dwVar != OTP_SECTOR_SIZE)
			{
				errcode |= 0x80;
			}
			*/

			for(dwVar = 0; dwVar < OTP_SECTOR_SIZE; dwVar++)
			{
				s_buffer[dwVar] = DrvUSART_GetChar();
				s_buffer[dwVar] |= DrvUSART_GetChar() << 8;
			}

			check_eop();

			testif_otp_sector_write(s_address, s_buffer);			
			
		}
		else if(s_ch == CMD_PAGE_VERIFY)
		{
			// sector address
			dwVar = DrvUSART_GetChar();
			dwVar |= DrvUSART_GetChar() << 8;

			for(dwVar = 0; dwVar < OTP_SECTOR_SIZE; dwVar++)
			{
				s_buffer[dwVar] = DrvUSART_GetChar();
				s_buffer[dwVar] |= DrvUSART_GetChar() << 8;
			}

			check_eop();

			s_ch = testif_otp_sector_verify(s_address, s_buffer);	
			DrvUSART_SendChar(s_ch);			
		}
		else if(s_ch == CMD_PAGE_VERIFY2)
		{
			check_eop();

			s_ch = testif_otp_sector_verify(s_address, s_buffer);	
			DrvUSART_SendChar(s_ch);
		}
		else if(s_ch == CMD_FUSE_PROG)
		{
			s_ch = DrvUSART_GetChar(); // which fuse word ?

			dwVar = DrvUSART_GetChar();
			dwVar |= DrvUSART_GetChar() << 8; // fuse word contents

			check_eop();

			testif_superon();
			testif_fuse_write(s_ch, dwVar); 
			testif_superoff();
		}
		else if(s_ch == CMD_FUSE_VERIFY)
		{
			s_ch = DrvUSART_GetChar();

			dwVar = DrvUSART_GetChar();
			dwVar |= DrvUSART_GetChar() << 8;

			check_eop();

			testif_superon();
			s_ch = testif_fuse_verify(s_ch, dwVar);
			testif_superoff();

			DrvUSART_SendChar(s_ch);
		}
		else if(s_ch == CMD_CTLS)
		{
			s_ch = DrvUSART_GetChar();
			check_eop();

			if(s_ch == CMD_READ_ID)
			{
				testif_readid();
			}
			else if(s_ch == CMD_BASIC)
			{
				testif_basic();
			}
		}

		send_insync();

	}

	return 0;
}
