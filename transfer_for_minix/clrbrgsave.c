#include <stdio.h>

#define BYTE unsigned char
#define BRG_SAVE_BASE 0x00FF0DD6

BYTE brgTestMode1;
BYTE brgTestMode2;

void main ()
{
    brgTestMode1 = *((BYTE*)(BRG_SAVE_BASE));
    brgTestMode2 = *((BYTE*)(BRG_SAVE_BASE + 1));
    
    printf ("DU1 BRGTestModeSave is %s", brgTestMode1 != 0 ? "on" : "off");
    if (brgTestMode1 != 0)
        printf (" - setting it off\n");
    else
        printf ("\n");
    printf ("DU2 BRGTestModeSave is %s", brgTestMode2 != 0 ? "on" : "off");
    if (brgTestMode2 != 0)
        printf (" - setting it off\n");
    else
        printf ("\n");

    *((BYTE*)(BRG_SAVE_BASE    )) = 0x00;
    *((BYTE*)(BRG_SAVE_BASE + 1)) = 0x00;
}
