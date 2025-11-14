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

/* this is from the test.c program where I tested writing to DUART1 */
void init_rate(rate)
BYTE rate;
{
  *(BYTE*)(DUART1 + DCOMMA) = 0x20;    /* reset receiver                  */
  *(BYTE*)(DUART1 + DCOMMA) = 0x15;    /* CRA: mode = 1, enable RX and TX */
  *(BYTE*)(DUART1 + DCOMMA) = 0x05;    /* CRA: enable RX and TX           */
  *(BYTE*)(DUART1 + DMODEA) = 0x13;    /* MRA1: no RTS, no parity, 8 bits */
  *(BYTE*)(DUART1 + DMODEA) = 0x07;    /* MRA2: no RTS, 1 stop bit        */
  *(BYTE*)(DUART1 + DCLOCA) = rate;    /* set CSRA to the baud rate       */
  *(BYTE*)(DUART1 + DAUXCR) = 0xE4;    /* AUX control register = set 2    */
}

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
*/ 	

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
    *(portadm + DCOMMB) = (BYTE)0x20;  /* RESET RECEIVER                  */
    *(portadm + DCOMMB) = (BYTE)0x15;  /* CRB: MODE=1, ENABLE RX&TX       */
    *(portadm + DCOMMB) = (BYTE)0x05;  /* CRB: ENABLE RX & TX             */
    *(portadm + DMODEB) = (BYTE)0x13;  /* MRB1: NO RTS, NO PARITY, 8 BITS */
    *(portadm + DMODEB) = (BYTE)0x07;  /* MRB2: RTS,1 STOP BIT            */
    *(portadm + DCLOCB) = rate;        /* sets baud rate                  */
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

