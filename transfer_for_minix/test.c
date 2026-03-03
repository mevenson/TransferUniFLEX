#include <stdio.h>
#include <sgtty.h>

#define BYTE unsigned char
#define WORD unsigned int
#define LONG unsigned long

/* #define DUART1   0xFE0000 */
#define S_RRDY	 0x01
#define S_TRDY	 0x04

#define RDATA  7
#define TDATA  7

#undef B19200
#undef B9600
#undef B4800

#define B19200 0xCC
#define B9600  0xBB
#define B4800  0x99

BYTE rates [3] = 
{
  B19200, 
  B9600,  
  B4800
};

BYTE *rateNames [3] =
{
  (BYTE*)"B19200", 
  (BYTE*)"B9600",  
  (BYTE*)"B4800"
};

#include "duart.h"

BYTE getresponse (prompt)
char *prompt;
{
  struct sgttyb new_tty_mode, old_tty_mode;
  int response;

  ioctl(0,TIOCGETP, &old_tty_mode);
  new_tty_mode = old_tty_mode;
  new_tty_mode.sg_flags |= CBREAK;
  new_tty_mode.sg_flags &= ~ECHO;
  ioctl(0,TIOCGETP, &new_tty_mode);
  
  printf("%s", prompt);
  /*
  while ((response = getchar()) != 0x0A && response != ' ')
  */
  response = getc(stdin);
  
  ioctl(0,TIOCGETP, &old_tty_mode);
  
  return((BYTE)response);
}

void sendCharacter (c)
BYTE c;
{
    BYTE status = 0;

    /* wait for transmit register empty */
    while ((status & S_TRDY) != S_TRDY)
    {
      status = *((BYTE*)(DUART1 + DCOMMA)) & (BYTE)S_TRDY;
      printf ("status: %02x\n", status);
    }
    
    *(BYTE*)(DUART1 + TDATA) = c;
}

void main (argc, argv)
int argc;
char *argv[];
{
    int i;
    BYTE c;
    BYTE *message = (BYTE *)"Hello World\r\n";
    BYTE *msgPtr = message;
    BYTE rate = 0xCC;	/* start with 19200 baud */
    
    while (1)
    {
      printf("Press return to test %02x", rate);
      c = getresponse("");

      switch (c)
      {
      	case 0x4E:
      	case 0x6E:
      		rate -= 0x11;
      		break;
      	case 0x0A:
      		break;
      }
      printf("\n");

      if (rate != 0x00)
      {
        if (c == 0x0A || c == 0x4E || c == 0x6E)
        {
          printf("Setting baud rate to %02x\n", (BYTE)rate);
/*          init_rate((BYTE)rate); */
          initPort(1, (BYTE)rate);

          msgPtr = message;
          printf ("Sending message: %s\n", msgPtr);
          while (*msgPtr)
          {
            sendCharacter(*msgPtr++);
          }
        }
      }
      else
        break;
    }  
}

