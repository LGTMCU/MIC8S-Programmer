######################################################
# Auto generated Makefile @2015-03-29 13:17:25
######################################################

PROJECT = lgt8f328d_ptest
CHIP = atmega328p
ID = m328p
TARGET = $(PROJECT).elf
TCPATH = C:/WinAVR-20100110/bin/
CC = $(TCPATH)avr-gcc
CPP = $(TCPATH)avr-gcc
OL = O0
COMMON = -mmcu=$(CHIP)
CFLAGS = $(COMMON) -Wall -gdwarf-2 -std=gnu99 -$(OL) -funsigned-char -funsigned-bitfields -fpack-struct -fshort-enums 
GENDFLAGS = 
LDFLAGS = $(COMMON) -Wl,-Map=$(PROJECT).map
HEX_FLASH_FLAGS = -R .eeprom -R .fuse -R .lock -R .singature
HEX_EEPROM_FLAGS = -j .eeprom --set-section-flags=.eeprom="alloc,load" --change-section-lma .eeprom=0 --no-change-warnings
INCLUDES =  -I./Drivers/Includes -I.
OBJECTS =  DrvGPIO.o DrvUSART.o DrvMISC.o DrvSYS.o init_auto.o main.o swd_drv.o
EXTOBJS = 

all: $(TARGET) $(PROJECT).hex $(PROJECT).lss
	@echo
	@echo build done!

DrvGPIO.o : ./Drivers/Sources/DrvGPIO.c
	$(CC) $(INCLUDES) $(GENDFLAGS) $(CFLAGS) -c ./Drivers/Sources/DrvGPIO.c
DrvUSART.o : ./Drivers/Sources/DrvUSART.c
	$(CC) $(INCLUDES) $(GENDFLAGS) $(CFLAGS) -c ./Drivers/Sources/DrvUSART.c
DrvMISC.o : ./Drivers/Sources/DrvMISC.c
	$(CC) $(INCLUDES) $(GENDFLAGS) $(CFLAGS) -c ./Drivers/Sources/DrvMISC.c
DrvSYS.o : ./Drivers/Sources/DrvSYS.c
	$(CC) $(INCLUDES) $(GENDFLAGS) $(CFLAGS) -c ./Drivers/Sources/DrvSYS.c
init_auto.o : ./init_auto.c
	$(CC) $(INCLUDES) $(GENDFLAGS) $(CFLAGS) -c ./init_auto.c
main.o : ./main.c
	$(CC) $(INCLUDES) $(GENDFLAGS) $(CFLAGS) -c ./main.c
swd_drv.o : ./swd_drv.c
	$(CC) $(INCLUDES) $(GENDFLAGS) $(CFLAGS) -c ./swd_drv.c


$(TARGET): $(OBJECTS)
	$(CC) $(LDFLAGS) $(OBJECTS) $(EXTOBJS) $(LIBDIRS) $(LIBS) -o $(TARGET)

$(PROJECT).hex: $(TARGET)
	$(TCPATH)avr-objcopy -O ihex $(HEX_FLASH_FLAGS) $(PROJECT).elf $(PROJECT).hex

$(PROJECT).lss: $(TARGET)
	$(TCPATH)avr-objdump -h -S $(PROJECT).elf > $(PROJECT).lss
	@echo
	@$(TCPATH)avr-size --mcu=$(CHIP) $(TARGET)

AVRDUDE: $(PROJECT).hex
	@avrdude/avrdude.exe -F -cjtag2isp -p$(ID) -Pusb -Uflash:w:$(PROJECT).hex:i

JTAGMKII: $(PROJECT).hex
	@mkII/jtagiceii.exe -d $(CHIP) -vf -pf -if $(PROJECT).hex

clean: 
	@for %f in (*.hex *.lss *.elf *.o) do @if exist %f @del %f
	@echo cleanup done!

