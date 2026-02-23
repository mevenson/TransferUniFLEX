/* INITIALIZE DUART1 */

#ifndef BYTE
#define BYTE unsigned char
#endif

/***********************************************************
* EQUATES. THESE ROUTINES USE A 68681 DUART1 AT $FE0000
* FOR THE PT68K-2/4 COMPUTER
***********************************************************/

#define DUART1    0x00FE0000           /* DUART 1 ADDRESS          */
#define DUART2    0x00FE0040           /* DUART 2 ADDRESS          */
#define DMODEA    1                    /* PORT A MODE REG          */
#define DCLOCA    3                    /* PORT A CLOCK REG         */
#define DSTATA    3                    /* PORT A STATUS REG        */
#define DCOMMA    5                    /* PORT A COMMAND REGISTER  */
#define DDATAA    7                    /* PORT A DATA REGISTERS    */
#define DAUXCR    9                    /* AUXILIARY CONTROL REG    */
#define DINCHG    9                    /* INPUT PORT CHANGE REG    */
#define DINTMK    11                   /* INTERRUPT MASK REG       */
#define DINTST    11                   /* INTERRUPT STATUS REG     */
#define DCTUPR    13                   /* CTR/TIMER UPPER REGISTER */
#define DCTLOW    15                   /* CTR/TIMER LOWER REGISTER */
#define DMODEB    17                   /* PORT B MODE REG          */
#define DCLOCB    19                   /* PORT B CLOCK REG         */
#define DSTATB    19                   /* PORT B STATUS REG        */
#define DCOMMB    21                   /* PORT B COMMAND REGISTER  */
#define DDATAB    23                   /* PORT B DATA REGISTERS    */
#define DIVECT    25                   /* INTERRUPT VECTOR REG     */
#define DIPORT    27                   /* INPUT PORT               */
#define DOPORT    27                   /* OUTPUT PORT              */
#define PIT       0x00FE0080           /* PARALLEL INTERFACE/TIMER */
#define PSRR      3                    /* PIT SERVICE CONTROL REG  */
#define PPCDDR    9                    /* PIT PORT C DIRECTION REG */
#define S_TRDY    0x04
#define S_RRDY    0x01

void waitHalfSecond ()
{
  unsigned long i;
  
  for (i = 1L; i != 0L; i++);
}

unsigned char setRateCode (rate)
long rate;
{
  unsigned char brCode = 0xCC;
  
  switch ((unsigned int)(rate / 10))
  {
  	/* divide by 10 so that the value can be an int */
        
 	case 11520: brCode = 0xFF; break;
  	case 5760:  brCode = 0xEE; break;
  	case 3840:  brCode = 0xDD; break;
  	case 1920:  brCode = 0xCC; break;
  	case 960:  	brCode = 0xBB; break;
  	case 480:  	brCode = 0xAA; break;
  	case 240:  	brCode = 0x99; break;
  	case 120:  	brCode = 0x88; break;
  	case 60:  	brCode = 0x77; break;
  	case 30:  	brCode = 0x66; break;
  	case 15:  	brCode = 0x55; break;
  	case 10:  	brCode = 0x44; break;
 	default:  /* to 19200 */        
  	  brCode = 0xCC;
  	  break;
   }
   
   return (brCode);
}

/* this is from the test.c program where I tested writing to DUART1 */
void init_rate(rate)
BYTE rate;
{
  *(BYTE*)(DUART1 + DCOMMA) = 0x20;    /* reset receiver                  */
  *(BYTE*)(DUART1 + DCOMMA) = 0x15;    /* CRA: mode = 1, enable RX and TX */
  *(BYTE*)(DUART1 + DCOMMA) = 0x05;    /* CRA: enable RX and TX           */
  *(BYTE*)(DUART1 + DMODEA) = 0x13;    /* MRA1: no RTS, no parity, 8 bits */
  *(BYTE*)(DUART1 + DMODEA) = 0x07;    /* MRA2: no RTS, 1 stop bit        */
  *(BYTE*)(DUART1 + DAUXCR) = 0xE4;    /* AUX control register = set 2    */
  *(BYTE*)(DUART1 + DCLOCA) = rate;    /* set CSRA to the baud rate       */
}

/* setting the Aux Control Register to 0xE4 has the following effect:

  Bit 7 = 1 -> Select Extended / Alternate Baud Rate Set
    This is the most important bit.
      On the 68681:
        0 = Normal baud rate table
        1 = Alternate baud rate table
    So this immediately switches CSRs to the alternate set of speeds.
        
  Bits 6-4 = 110  
    These bits control the Counter / Timer operating mode.
      x110 xxxx selects:
        BRG Test Mode
          This is the special high-speed generator mode that 
          repurposes CSR encodings.
            Effect:
              CSR values now map to 19.2k / 38.4k / 57.6k / 115.2k
              Counter/timer not used as timer/divider
              Internal baud logic changes behavior
            This is why high speeds suddenly become possible.        
            
    Table 4-4. Selection of Clock Sources for the Counter and Timer Modes 
+-------------------------------------------------------------------------------------------------------+
|                                                 ACR[6]                                                +
+-------------------------------------------------- +---------------------------------------------------+
|                                0                  |                  1                                +
+-------------------------------------------------- |---------------------------------------------------+
| Counter Mode Clock Sources               ACR[5:4] | Timer Mode Clock Sources                 ACR[5:4] |
+-------------------------------------------------- +---------------------------------------------------+
| External Input via Input Port Pin 2 (lP2)   00    | External Input via Input Port Pin 2 (lP2)   00    |
+-------------------------------------------------- +---------------------------------------------------+
| Channel A lX Transmitter Clock TxCA         01    | External Input Divide by 16 via Input       01    |
|                                                   |     Port Pin 2 (IP2)                              |
+-------------------------------------------------- +---------------------------------------------------+
| Channel B lX Transmitter Clock TxCB         10    | Crystal Oscillator via Xl/ClK and X2        10    |
|                                                   |     Inputs                                        |
+-------------------------------------------------- +---------------------------------------------------+
| Crystal Oscillator Divide by 16 via         11    | Crystal Oscillator Divide by 16 via         11    |
| Xl/ClK and X2 Inputs Xl/ClK and X2 Inputs         | Xl/ClK Input Pin                                  |
+-------------------------------------------------- +---------------------------------------------------+
| External Input Divide by 16 via             11    | External Input via Xl/ClK Input Pin         10    |
| Xl/ClK Input Pin                                  |---------------------------------------------------+
|                                                   | External Input Divide by 16 via             11    |
|                                                   | Xl/ClK Input Pin                                  |
+-------------------------------------------------- +---------------------------------------------------+
|   NOTE: Only those functions which show the register programming are available for use.               |
+-------------------------------------------------- +---------------------------------------------------+

  Bit 3-0 -> No special IP3 behavior
    Unused for most PT68K setups.
    
  Bit 2 = 1 Enable Baud Rate Generator on IP2 / OP2 path
        This bit ties BRG signals into timer / output logic.
        On most PT68K boards this bit is harmless but typical.
        
  Bits 1–0 = 00
    No special output port or timer clock source selection.

        This field selects which bits of the input port change register can cause the input change bit 
        in the interrupt status register (lSR[7]) to be set. If a bit of ACR[3:O] is set to the "enabled" 
        state (set to one) and interrupt mask register bit seven is set (IMR[7]=1) to enable input port 
        change interrupts, then the setting of the corresponding bit in the input port change register by 
        an external transition on that input pin will result in ISR[7] being set and an interrupt output 
        generated. If a bit of ACRC[3:O] is in the "disabled" state (cleared to zero), the setting of that 
        bit in the input port change register has no effect on ISR[7].
    
    Net Effect of ACR = 0xE4
        Writing 0xE4 does three major things:
        1.  Enables Alternate Baud Rate Table
                CSR values no longer use the default speeds.
        2. Enables BRG Test Mode (critical)
                This unlocks the high-speed baud rates:
                CSR Nibble	Resulting Baud
                0xB	        9600
                0xC	        19200
                0xD	        38400
                0xE	        57600
                0xF	        115200
            (assuming 3.6864 MHz crystal)
*/

/*
	enter:	commNumber = 1 thru 4
			rate =  0xFF for IPX/counter mode (1x)
					0xEE for IPX/counter mode
					0xDD for Timer/special
					0xCC for 19200
					0xBB for  9600
					0xAA for  1800	<- weird
					0x99 for  4800
					0x88 for  2400
					0x77 for  2000	<- another weird one
					0x66 for  1200
					0x55 for   600
					0x44 for   200
					0x33 for   150
					0x22 for   134.5
					0x11 for   110
					0x00 for    75

    from the Motorola 68681 spec sheet:
    ----------  -------    -------
    ACR Bit 7   0          1 
    ----------  -------    -------
    0000        50         75 
    0001        110        110 
    0010        134.5      134.5 
    0011        200        150 
    0100        300        300 
    0101        600        600 
    0110        1200       1200 
    0111        1050       2000 
    1000        2400       2400 
    1001        4800       4800 
    1010        7200       1800 
    1011        9600       9600 
    1100        38.4k      19.2k 
    1101        Timer      Timer 
    1110        IP3-16X    IP3-16X 
    1111        IP3-1X     IP3-1X 
    ----------  -------    -------
*/

/*
------------+------------+------------+------------+-----------=+    
              AUX Control register = 0xE4 means we are using    |
                                                   |  this one  |
------------+------------+------------+------------+------------+
|           |Normal BRG  |Normal BRG  |BRG Test    |BRG Test    |
| CSR[7:4]  |ACR[7] = 0  |ACR[7] = 1  |ACR[7] = 0  |ACR[7] = 1  |
|           |ACR[6] = 0  |ACR[6] = 0  |ACR[6] = 1  |ACR[6] = 1  |
+-----------+------------+------------+------------+------------+
| 0000      | 50         | 75         | 4,800      | 7,200      |
| 0001      | 110        | 110        | 880        | 880        |
| 0010      | 134.5      | 134.5      | 1,076      | 1,076      |
| 0011      | 200        | 150        | 19.2 k     | 14.4 k     |
| 0100      | 300        | 300        | 28.8 k     | 28.8 k     |
| 0101      | 600        | 600        | 57.6 k     | 57.6 k     |
| 0110      | 1,200      | 1,200      | 115.2 k    | 115.2 k    |
| 0111      | 1,050      | 2,000      | 1,050      | 2,000      |
| 1000      | 2,400      | 2,400      | 57.6 k     | 57.6 k     |
| 1001      | 4,800      | 4,800      | 4,800      | 4,800      |
| 1010      | 7,200      | 1,800      | 57.6 k     | 14.4 k     |
| 1011      | 9,600      | 9,600      | 9,600      | 9,600      |
| 1100      | 38.4 k     | 19.2 k     | 38.4 k     | 19.2 k     |
| 1101      | Timer      | Timer      | Timer      | Timer      |
| 1110      | I/O2 – 16x | I/O2 – 16x | I/O2 – 16x | I/O2 – 16x |
| 1111      | I/O2 – 1x  | I/O2 – 1x  | I/O2 – 1x  | I/O2 – 1x  |
+-----------+------------+------------+------------+------------|
|                 A 0x24       B 0xA4       C 0x64       D 0xE4 |
+-----------+---- ^ -----+---- ^ -----+     ^      +---- ^ -----|
|                 |            |            |            |      |
| rate < 0xCC ----|            |            |            |      |
| rate = 0xCC -----------------|            |            |      |
| rate = 0xDD ------------------------------|            |      |
| rate > 0xCC ------------------------------| or --------|      |
|                                                               |
|   if rate = 00-BB use A                   ACR = 0x24          |
|   if rate = CC    use B                   ACR = 0xA4          |
|   if rate = DD    use C and set rate = C  ACR = 0x64 or 0xE4  |
|   if rate = EE    use C and set rate = A  ACR = 0x64 or 0xE4  |
|   if rate = FF    use C and set rate = 6  ACR = 0x64 or 0xE4  |
+--------------------------------------------------------------*/

void initPort (commNumber, rate)
int commNumber;
BYTE rate;
{
  BYTE D0;
  
  BYTE *portadm = (BYTE*)DUART1;

  /* if commNumber is in DUART 2, add 0x40 to address */
  if (commNumber == 3 || commNumber == 4)
    portadm += 0x40;

  /* if commNumber is the B side of the DUART, do this */
  if (commNumber == 2 || commNumber == 4)
  {
    *(portadm + DCOMMB) = (BYTE)0x20; /* RESET RECEIVER                  */
    *(portadm + DCOMMB) = (BYTE)0x15; /* CRB: MODE=1, ENABLE RX&TX       */
    *(portadm + DCOMMB) = (BYTE)0x05; /* CRB: ENABLE RX & TX             */
    *(portadm + DMODEB) = (BYTE)0x13; /* MRB1: NO RTS, NO PARITY, 8 BITS */
    *(portadm + DMODEB) = (BYTE)0x07; /* MRB2: RTS,1 STOP BIT            */
    *(portadm + DCLOCB) = rate;       /* sets baud rate                  */
  }
  else
  {
    *(portadm + DCOMMA) = (BYTE)0x20; /* RESET RECEIVER                  */
    *(portadm + DCOMMA) = (BYTE)0x15; /* CRA: MODE=1, ENABLE RX&TX       */
    *(portadm + DCOMMA) = (BYTE)0x05; /* CRA: ENABLE RX & TX             */
    *(portadm + DMODEA) = (BYTE)0x13; /* MRA1: NO RTS, NO PARITY, 8 BITS */
    *(portadm + DMODEA) = (BYTE)0x07; /* MRA2: NO RTS,1 STOP BIT         */
    *(portadm + DCLOCA) = rate;       /* SET CSRA TO BAUD RATE           */
  }
  
  if (rate <= 0x88) /* use table A above */
  {
      *(portadm + DAUXCR) = (BYTE)0x24; /* ACR = SET 2, BGR = test      */
  }
  else if (rate == 0x99) /* if 2400 */
  {
      *(portadm + DAUXCR) = (BYTE)0xA4; /* ACR = SET 2, BGR = test      */
      *(portadm + DCLOCA) = 0x88;       /* SET CSRA TO BAUD RATE        */
  }
  else if (rate == 0xAA) /* if 4800                      */
  {
      *(portadm + DAUXCR) = (BYTE)0xE4; /* ACR = SET 2, BGR = test      */
      *(portadm + DCLOCA) = 0x99;       /* SET CSRA TO BAUD RATE        */
  }
  else if ((rate == 0xBB) || (rate == 0xCC)) /* if 9600 or 19200        */
  {                                          /* leave these alone       */
      *(portadm + DAUXCR) = (BYTE)0xE4; /* ACR = SET 2, BGR = test      */
  }
  else if (rate == 0xDD) /* if 38400 - select set 1 and set to 0xCC     */
  {
    *(portadm + DAUXCR) = (BYTE)0x64;   /* ACR = SET 1, BGR = test      */
    *(portadm + DCLOCA) = 0xCC;         /* SET CSRA TO BAUD RATE        */
  }
  else if (rate == 0xEE) /* if 57600 - select set 2 and set to 0x88 */
  {
    *(portadm + DAUXCR) = (BYTE)0x64;   /* ACR = SET 1, BGR = test      */
    *(portadm + DCLOCA) = 0xAA;         /* SET CSRA TO BAUD RATE        */
  }
  else if (rate == 0xFF) /* if 115200 - select set 2 and set to 0x66 */
  {
    *(portadm + DAUXCR) = (BYTE)0x64;   /* ACR = SET 1, BGR = test      */
    *(portadm + DCLOCA) = 0x66;         /* SET CSRA TO BAUD RATE        */
  }
  else
    *(portadm + DAUXCR) = (BYTE)0xE4; /* AUX CONTROL REG = SET 2         */
  
  /* make sure the receive buffer gets emptied */

/*  
  if (commNumber == 2 || commNumber == 4)
  {
    while (*(BYTE*)(portadm + DSTATB) & (BYTE)0x01 == (BYTE)0x01)
      D0 = *(BYTE*)(portadm + DDATAB);
  }
  else
  {
    while (*(BYTE*)(portadm + DSTATA) & (BYTE)0x01 == (BYTE)0x01)
      D0 = *(BYTE*)(portadm + DDATAA);
  }

  waitHalfSecond();
*/

}

