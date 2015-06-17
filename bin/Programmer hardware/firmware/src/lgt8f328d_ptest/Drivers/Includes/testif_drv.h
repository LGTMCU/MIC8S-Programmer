#ifndef __TESTIF_DRV_H__
#define	__TESTIF_DRV_H__

#include "global.h"

#define	OTP_SECTOR_SIZE		256

/* Test protocol definition */
#define	CMD_CTLS			0xA0
#define	CMD_VPPON			0x00
#define CMD_VPPOFF			0x11
#define	CMD_MCU_RON			0x22
#define	CMD_MCU_ROFF		0x33
#define CMD_READ_ID			0x44
#define CMD_MCU_RUN			0x55
#define CMD_BASIC			0xFF

#define CMD_LOAD_ADDRESS	0x55
#define	CMD_PAGE_PROG		0x64
#define	CMD_PAGE_VERIFY		0x74
#define	CMD_PAGE_VERIFY2	0x75
#define	CMD_FUSE_PROG		0x62
#define	CMD_FUSE_VERIFY		0x72
#define	CMD_MISC_TEST		0x82
#define	CMD_OCC_WRITE		0x84
#define CMD_OCC_READ		0x94

#define	CMD_EOP				0x20
#define	CMD_INSYNC			0x14

#define	OTP_DAT				0
#define	OTP_CFG				1

#define	OTP_PTM				0x10
#define	OTP_PWE				0x04
#define	OTP_PPROG			0x02
#define	OTP_IDLE			0x00

typedef struct {
	u16 PTM:3;
	u16 PWE:1;
	u16 PPROG:1;
	u16 PIF:1;
	u16 PA:10;
} otp_aframe_t;

typedef struct {
	u16 DTYPE:1;
	u16 dummy:1;
	u16 DATA:14;
} otp_dframe_t;

typedef struct {
	u16 MEN:1;
	u16 PKGS:1;
	u16 TUS:2;
	u16 dummy:2;
	u16 RCM:2;
	u16 CTLD:8;
} misc_cframe_t;

typedef struct {
	u16 FS:2;
	u16 WEN:1;
	u16 REN:1;
	u16 dummy:4;
	u16 AD:8;
} occ_cframe_t;	

u8 check_eop(void);
void send_insync(void);

void testif_misc(u8 *pdata);
void testif_occ(u8 *pdata);

#endif
