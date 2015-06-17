//---------------------------------------------------
// Communication protocol definition
// Data Frame definition
//	[CMD] [DATAIN] [EOP] {DATAOUT} [SYNC]
//
// [CMD]	: 1 byte, from host to device
// [DATAIN] : n bytes, from host to device
// [EOP]	: always 0x20, from host to device
// {DATAOUT}: data from device to host, optional
// [SYNC]	: always 0x14, from device to host
//
// Part 1: OTP programming & test
// CMD_LOAD_ADDRESS : 0x55
//		- DATAIN : 2bytes address
// CMD_OTP_PROG : 0x64
//		- DATAIN : 2bytes length + [userdata]
// CMD_OTP_VERIFY : 0x74
//		- DATAIN : 2bytes address + [userdata]
//		- DATAOUT : 1 bytes result
// 
// Part 2: Configuration words
//	CMD_CFG_PROG : 0x62
//		- DATAIN : 1byte addresss + 2bytes configuration words
//
// 	CMD_CFG_VERIFY : 0x72
//		- DATAIN : 1byte address + 2bytes configuration words
//		- DATAOUT : 1bytes result
//
// Part 5: System test
// 	CMD_CTLS : 0xa0
//		- DATAIN: 	0x44 : Read SWDID
//				0xff : BAISC COMMAND	
//
