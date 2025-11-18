/* INITIALIZE DUART1 */

#ifndef BYTE
#define BYTE unsigned char

/***********************************************************
* EQUATES. THESE ROUTINES USE A 68681 DUART1 AT $FE0000
* FOR THE PT-68K-2 COMPUTER
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

void waitHalfSecond ()
{
  unsigned long i;
  
  for (i = 1L; i != 0L; i++);
}

void initPort ()
{
  BYTE D0;
  
  BYTE *portadm = (char*)DUART1;
  BYTE *portadp = (char*)(DUART1 + 16);

  *(portadm + DCOMMB) = (BYTE)0x15;  /* CRB: MODE=1, ENABLE RX&TX       */
  *(portadm + DCOMMB) = (BYTE)0x05;  /* CRB: ENABLE RX & TX             */
  *(portadm + DMODEB) = (BYTE)0x13;  /* MRB1: NO RTS, NO PARITY, 8 BITS */
  *(portadm + DMODEB) = (BYTE)0x27;  /* MRB2: RTS,1 STOP BIT            */
  *(portadm + DCLOCB) = (BYTE)0xBB;  /* sets 9600 baud                  */
  
  /* make sure the receive buffer gets emptied */
  while (*(BYTE*)(portadm + DSTATB) & (BYTE)0x01 == (BYTE)0x01)
    D0 = *(BYTE*)(portadm + DDATAB);
    
  waitHalfSecond();    
  
  *(portadm + DCOMMA) = (BYTE)0x20; /* RESET RECEIVER                  */
  *(portadm + DCOMMA) = (BYTE)0x15; /* CRA: MODE=1, ENABLE RX&TX       */
  *(portadm + DCOMMA) = (BYTE)0x05; /* CRA: ENABLE RX & TX             */
  *(portadm + DMODEA) = (BYTE)0x13; /* MRA1: NO RTS, NO PARITY, 8 BITS */
  *(portadm + DMODEA) = (BYTE)0x07; /* MRA2: NO RTS,1 STOP BIT         */
  *(portadm + DCLOCA) = (BYTE)0x88; /* SET CSRA TO 2400 BAUD           */
  *(portadm + DAUXCR) = (BYTE)0xE4; /* AUX CONTROL REG = SET 2         */
}

