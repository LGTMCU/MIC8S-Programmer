###############################################################################
# Makefile for the project lgt8f328d_ptest
###############################################################################

## General Flags
PROJECT = lgt8f328d_ptest
MCU = atmega328p
TARGET = lgt8f328d_ptest.elf
CC = avr-gcc

CPP = avr-g++

## Options common to compile, link and assembly rules
COMMON = -mmcu=$(MCU)

## Compile options common for all C compilation units.
CFLAGS = $(COMMON)
CFLAGS += -Wall -gdwarf-2 -O0 -std=gnu99 -funsigned-char -funsigned-bitfields -fpack-struct -fshort-enums
CFLAGS += -MD -MP -MT $(*F).o -MF dep/$(@F).d 

## Assembly specific flags
ASMFLAGS = $(COMMON)
ASMFLAGS += $(CFLAGS)
ASMFLAGS += -x assembler-with-cpp -Wa,-gdwarf2

## Linker flags
LDFLAGS = $(COMMON)
LDFLAGS +=  -Wl,-Map=lgt8f328d_ptest.map


## Intel Hex file production flags
HEX_FLASH_FLAGS = -R .eeprom -R .fuse -R .lock -R .signature

HEX_EEPROM_FLAGS = -j .eeprom
HEX_EEPROM_FLAGS += --set-section-flags=.eeprom="alloc,load"
HEX_EEPROM_FLAGS += --change-section-lma .eeprom=0 --no-change-warnings


## Include Directories
INCLUDES = -I"E:\Projects\Projects\LGT8P653A\Test\lgt8f328d_ptest\Drivers\Includes" -I"E:\Projects\Projects\LGT8P653A\Test\lgt8f328d_ptest\." 

## Objects that must be built in order to link
OBJECTS = DrvGPIO.o DrvUSART.o DrvMISC.o DrvSYS.o init_auto.o main.o swd_drv.o testif_drv.o 

## Objects explicitly added by the user
LINKONLYOBJECTS = 

## Build
all: $(TARGET) lgt8f328d_ptest.hex lgt8f328d_ptest.eep lgt8f328d_ptest.lss size

## Compile
DrvGPIO.o: .././Drivers/Sources/DrvGPIO.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

DrvUSART.o: .././Drivers/Sources/DrvUSART.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

DrvMISC.o: .././Drivers/Sources/DrvMISC.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

DrvSYS.o: .././Drivers/Sources/DrvSYS.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

init_auto.o: .././init_auto.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

main.o: .././main.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

swd_drv.o: .././swd_drv.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

testif_drv.o: ../testif_drv.c
	$(CC) $(INCLUDES) $(CFLAGS) -c  $<

##Link
$(TARGET): $(OBJECTS)
	 $(CC) $(LDFLAGS) $(OBJECTS) $(LINKONLYOBJECTS) $(LIBDIRS) $(LIBS) -o $(TARGET)

%.hex: $(TARGET)
	avr-objcopy -O ihex $(HEX_FLASH_FLAGS)  $< $@

%.eep: $(TARGET)
	-avr-objcopy $(HEX_EEPROM_FLAGS) -O ihex $< $@ || exit 0

%.lss: $(TARGET)
	avr-objdump -h -S $< > $@

size: ${TARGET}
	@echo
	@avr-size -C --mcu=${MCU} ${TARGET}

## Clean target
.PHONY: clean
clean:
	-rm -rf $(OBJECTS) lgt8f328d_ptest.elf dep/* lgt8f328d_ptest.hex lgt8f328d_ptest.eep lgt8f328d_ptest.lss lgt8f328d_ptest.map


## Other dependencies
-include $(shell mkdir dep 2>NUL) $(wildcard dep/*)

