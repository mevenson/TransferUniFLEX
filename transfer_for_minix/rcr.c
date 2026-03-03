#include <stdio.h>
#include <fcntl.h>

#define BUFSIZE 16384

int i;
int inp_fd;
int done;
int bytesRead; 
int crCount;

unsigned char buffer[BUFSIZE + 1];

int main (argc, argv)
int argc;
char *argv[];
{
  crCount = 0;
  	  
  fprintf(stderr, "rcr - Replace Carriage Returns with Line Feeds\n");
  if (argc == 2)
  {
    /* open the input file for reading */
    inp_fd = open(argv[1], O_RDONLY);

    /* loop on getting BUFSIZE blocks of data to search for <cr> */
    done = 0;
    while (done == 0)
    {
      /* read in a BUFSIZE block of data */
      bytesRead = read(inp_fd, buffer, BUFSIZE);

      /* make sure the buffer is terminated for the printf */
      /* we can do this safely because we made the buffer  */
      /* size BUFSIZE + 1 to accomdate the null terminator */
      
      buffer[bytesRead] = (unsigned char)0x00;
            
      /* if there is still data to process - do it */
      if (bytesRead > 0)
      {
      	/* search the block replacing <cr> with <lf> */
      	for (i = 0; i < bytesRead; i++)
      	{
          if (buffer[i] == (unsigned char)0x0D) /* is this character a <cr>? */
          {
            /* yes - replace it with a <lf> */
            buffer[i] = (unsigned char)0x0A;
            crCount++;          /* bump the line count */
          }
        }
        
        /* send the line to the console [stdout] */
        printf("%s", buffer);

        /* if we did not read a full BUFSIZE chunk - we are done */
        if (bytesRead != BUFSIZE)
          done = 1;
      }
      else	      /* otherwise - signal we are done */
        done = 1;
    }
    fprintf(stderr, "\n");
    
    close(inp_fd);
    
    fprintf(stderr, "%d lines converted to <lf> from <cr> endings\n", crCount);
  }
  else
    fprintf(stderr, "Usage: rcr <input file>\n    output is to stdout\n");
}
