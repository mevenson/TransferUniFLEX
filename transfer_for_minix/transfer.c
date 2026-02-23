#include <stdio.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <sgtty.h>
#include <pwd.h>

#include "duart.h"

#define SIZEOFBUFFER    512
#define SIZEOFFILENAME  512
#define SIZEOFDIRECTORY 512
#define SIZEOFDIRNAME   512
#define MAXPATHLEN      128

#define version "transfer version 0.4:10\n"

FILE *popen();
char *getcwd();

int fdBadBlock;

int verbose = 0;

char device[32];
unsigned char buffer   [SIZEOFBUFFER];
unsigned char filename [SIZEOFFILENAME];
unsigned char dir      [SIZEOFDIRECTORY];

unsigned char commandLine [128];
unsigned char badBlockFile [SIZEOFFILENAME];

char ackBuffer[2];
char nakBuffer[2];

char command;

struct sgttyb settings;
struct stat statBuffer;
char padding[128];

int commPort;

unsigned int crc_table[] =
{
    0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50a5, 0x60c6, 0x70e7,
    0x8108, 0x9129, 0xa14a, 0xb16b, 0xc18c, 0xd1ad, 0xe1ce, 0xf1ef,
    0x1231, 0x0210, 0x3273, 0x2252, 0x52b5, 0x4294, 0x72f7, 0x62d6,
    0x9339, 0x8318, 0xb37b, 0xa35a, 0xd3bd, 0xc39c, 0xf3ff, 0xe3de,
    0x2462, 0x3443, 0x0420, 0x1401, 0x64e6, 0x74c7, 0x44a4, 0x5485,
    0xa56a, 0xb54b, 0x8528, 0x9509, 0xe5ee, 0xf5cf, 0xc5ac, 0xd58d,
    0x3653, 0x2672, 0x1611, 0x0630, 0x76d7, 0x66f6, 0x5695, 0x46b4,
    0xb75b, 0xa77a, 0x9719, 0x8738, 0xf7df, 0xe7fe, 0xd79d, 0xc7bc,
    0x48c4, 0x58e5, 0x6886, 0x78a7, 0x0840, 0x1861, 0x2802, 0x3823,
    0xc9cc, 0xd9ed, 0xe98e, 0xf9af, 0x8948, 0x9969, 0xa90a, 0xb92b,
    0x5af5, 0x4ad4, 0x7ab7, 0x6a96, 0x1a71, 0x0a50, 0x3a33, 0x2a12,
    0xdbfd, 0xcbdc, 0xfbbf, 0xeb9e, 0x9b79, 0x8b58, 0xbb3b, 0xab1a,
    0x6ca6, 0x7c87, 0x4ce4, 0x5cc5, 0x2c22, 0x3c03, 0x0c60, 0x1c41,
    0xedae, 0xfd8f, 0xcdec, 0xddcd, 0xad2a, 0xbd0b, 0x8d68, 0x9d49,
    0x7e97, 0x6eb6, 0x5ed5, 0x4ef4, 0x3e13, 0x2e32, 0x1e51, 0x0e70,
    0xff9f, 0xefbe, 0xdfdd, 0xcffc, 0xbf1b, 0xaf3a, 0x9f59, 0x8f78,
    0x9188, 0x81a9, 0xb1ca, 0xa1eb, 0xd10c, 0xc12d, 0xf14e, 0xe16f,
    0x1080, 0x00a1, 0x30c2, 0x20e3, 0x5004, 0x4025, 0x7046, 0x6067,
    0x83b9, 0x9398, 0xa3fb, 0xb3da, 0xc33d, 0xd31c, 0xe37f, 0xf35e,
    0x02b1, 0x1290, 0x22f3, 0x32d2, 0x4235, 0x5214, 0x6277, 0x7256,
    0xb5ea, 0xa5cb, 0x95a8, 0x8589, 0xf56e, 0xe54f, 0xd52c, 0xc50d,
    0x34e2, 0x24c3, 0x14a0, 0x0481, 0x7466, 0x6447, 0x5424, 0x4405,
    0xa7db, 0xb7fa, 0x8799, 0x97b8, 0xe75f, 0xf77e, 0xc71d, 0xd73c,
    0x26d3, 0x36f2, 0x0691, 0x16b0, 0x6657, 0x7676, 0x4615, 0x5634,
    0xd94c, 0xc96d, 0xf90e, 0xe92f, 0x99c8, 0x89e9, 0xb98a, 0xa9ab,
    0x5844, 0x4865, 0x7806, 0x6827, 0x18c0, 0x08e1, 0x3882, 0x28a3,
    0xcb7d, 0xdb5c, 0xeb3f, 0xfb1e, 0x8bf9, 0x9bd8, 0xabbb, 0xbb9a,
    0x4a75, 0x5a54, 0x6a37, 0x7a16, 0x0af1, 0x1ad0, 0x2ab3, 0x3a92,
    0xfd2e, 0xed0f, 0xdd6c, 0xcd4d, 0xbdaa, 0xad8b, 0x9de8, 0x8dc9,
    0x7c26, 0x6c07, 0x5c64, 0x4c45, 0x3ca2, 0x2c83, 0x1ce0, 0x0cc1,
    0xef1f, 0xff3e, 0xcf5d, 0xdf7c, 0xaf9b, 0xbfba, 0x8fd9, 0x9ff8,
    0x6e17, 0x7e36, 0x4e55, 0x5e74, 0x2e93, 0x3eb2, 0x0ed1, 0x1ef0
};

void write_fdTTY (bytePtr, count)
unsigned char *bytePtr;
int count;
{
  int i;
  BYTE status = 0;
  BYTE *portadm = (BYTE*)DUART1;

  /* if commPort is in DUART 2, set address to DUART2 */
  
  if (commPort == 3 || commPort == 4)
    portadm = (BYTE*)DUART2;

  for (i = 0; i < count; i++)
  {
    /* wait for transmitter to be ready to accept a character */
    status = 0;
    while ((status & S_TRDY) == 0)
    {
      /* if commPort is the B side of the DUART, do this */
      if (commPort == 1 || commPort == 3)
      	status = *((BYTE*)(portadm + DSTATA));
      else
      	status = *((BYTE*)(portadm + DSTATB));
    }
 
    if (commPort == 1 || commPort == 3)
      *(BYTE*)(portadm + DDATAA) = bytePtr[i];
    else
      *(BYTE*)(portadm + DDATAB) = bytePtr[i];
  }
}

void read_fdTTY (bytePtr, count)
unsigned char *bytePtr;
int count;
{
  int i;
  BYTE status = 0;
  BYTE *portadm = (BYTE*)DUART1;

  /* if commPort is in DUART 2, set address to DUART2*/
  if (commPort == 3 || commPort == 4)
    portadm = (BYTE*)DUART2;

  for (i = 0; i < count; i++)
  {
    /* wait for receiver to be ready to provide a character */
    status = 0;
    while ((status & S_RRDY) == 0)
    {
      /* if commPort is the B side of the DUART, do this */
      if (commPort == 1 || commPort == 3)
      	status = *((BYTE*)(portadm + DSTATA));
      else
      	status = *((BYTE*)(portadm + DSTATB));
    }
 
    if (commPort == 1 || commPort == 3)
      bytePtr[i] = *(BYTE*)(portadm + DDATAA);
    else
      bytePtr[i] = *(BYTE*)(portadm + DDATAB);
  }
}

/* Use getcwd from minix library */
char *getwd (buf, maxSize)
char* buf;
int maxSize;
{
  char *result;
  char *returnBuffer;
  
  buf[0] = 0x00;

  if ((returnBuffer = getcwd(buf, maxSize)) == (char*)NULL)
  {
    return ((char*)NULL);
  }
    
  return (buf);
}

unsigned int CalcCCITT (data, startIndex, length, seed, final)
unsigned char * data;
int startIndex;
int length;
unsigned int seed;
unsigned int final;
{

  int count;
  unsigned int crc;
  unsigned int temp;
  int dataindex;

  crc = seed;
  dataindex = startIndex;

  for (count = 0; count < length; ++count)
  {
    temp = (data[dataindex++] ^ (crc >> 8)) & 0xff;
    crc = crc_table[temp] ^ (crc << 8);
  }

  return (unsigned int)(crc ^ final);
}

/* returns  1 if a directory exists with this name              */
/* returns  0 if directory does not exists but a file does      */
/* retuens -1 if stat fails - no file or directory by this name */

int directoryExists(dirName)
unsigned char* dirName;
{
    int returnValue;

    /* dirname might be a directory name or a file name */
    /* stat will check for either */

    returnValue = stat(dirName, &statBuffer);
    if (returnValue == 0) /* 0 is good status from stat call */
    {
        /* we found a directory entry name  that macthes  */
        /* if this is a directory - return 1              */
        /*              otherwise - return 0              */

        if ((statBuffer.st_mode & 0x4000) == 0x4000)
            returnValue = 1;      /* found a directory by this name */
        else
            returnValue = 0;      /* found a file by this name */
    }
    else
        returnValue = -1; /* failed stat call - no file or directory */

    return (returnValue);
}

int fileExists(filename)
char *filename;
{
  return access(filename, 0) != -1;
}

int CreateFile (filename)
char *filename;
{
  int fdFile;

  /* if the file exists - delete it */
  if (fileExists (filename))
  {
    unlink(filename);
  }

  fdFile = creat(filename, 0x7F); /* create with all permissions set */
  if (fdFile == -1)
  {
    /* file was not created - report error*/

    printf ("could not create file: %s", filename);
  }

  return (fdFile);
}

/* This is used to get both the size and the CCITT */
/* in eithwer case we do not send the final ACK to */
/* receiving the low byte of the CCITT. The caller */
/* will send the ACK or NAK depending on the value */
/* received */

int GetWord ()
{
  unsigned int value = 0;
  unsigned char buff[2];

  read_fdTTY(&buff[0], 1);         /* get high byte of CCITT */
  write_fdTTY(ackBuffer, 1);       /* ack it */
  read_fdTTY(&buff[1], 1);         /* get low byte of CCITT */
  value = (buff[0] * 256) + buff[1];

  return (value);
}

void recvFile ()
{
  int i;

  int currentBlock;
  int fdFile;
  int size;
  int recv_ccitt;
  int calc_ccitt;
  int retryCount;
  int statReturn;

  /* remote will be sending a filename */

  for (i = 0; i < SIZEOFFILENAME; i++)
  {
    read_fdTTY(&filename[i], 1);

    /* remote will send 0x00 to end input of filename */

    if (filename[i] != 0x00)
      write_fdTTY(ackBuffer, 1);
    else
      break;
  }

  /* we need to see if the filename includes */
  /* any directory paths (/)                 */

  for (i = 0; i < strlen(filename); i++)
  {
    /* see if there is a directory that needs */
    /* to be created */

    if (filename[i] == '/' && i > 0)
    {
      if (verbose == 1)
        printf("found slash at offset %d in filename\n", i);

      filename[i] = 0x00;     /* terminate */

      statReturn = directoryExists(filename);
      switch (statReturn)
      {
      case 1:
          if (verbose == 1)
              printf("directory %s already exists\n", filename);
          break;
      case 0:
      case -1:
          if (verbose == 1)
              printf("creating directory %s\n", filename);
          sprintf(commandLine, "mkdir %s", filename);
          system(commandLine);
          break;
      }
      filename[i] = '/';
    }
  }

  printf("creating filename: %s\n", filename);

  /* before we send the ACK to the file name, */
  /* create the file and get ready to write   */

  fdFile = CreateFile(filename);
  write_fdTTY(ackBuffer, 1);  /* now we can ACK the filename */

  /* now we can get the data for the file     */
  /* the first thing that the remote will     */
  /* send is the number of bytes in the block */
  /*
          when done, the remote will send a
          block size of 0000
  */

  currentBlock = 0;
  while ((size = GetWord()) != 0) /* do not send ACK yet */
  {
    retryCount = 0;
    currentBlock++;

    /* now we have the size - get the data */
    /* if the calcualted CCITT and the received CCITT   */
    /* match, we will send an ACK and break out of this */
    /* while loop and go up to the outer while loop     */

    while (size > 0)
    {
      /* if this is a retry - get the size again */

      if (retryCount > 0)    /* retries need new count   */
        size = GetWord();   /* size = 0 when PC is done */

      retryCount++;

      if (verbose == 1)
        printf ("getting %d bytes for block %d\n", size, currentBlock);
        
      write_fdTTY(ackBuffer, 1); /* now ACK the size */

      /* get the bytes for the data block */

      for (i = 0; i < size; i++)
      {
        read_fdTTY(&buffer[i], 1);    /* get data */
        write_fdTTY(ackBuffer, 1);    /* send ACK */
      }

      /* now get the recv_ccitt */
      if (verbose == 1)
        printf ("getting the CCITT for block %d\n", currentBlock);

      recv_ccitt = GetWord();   /* do not final send ACK */
      calc_ccitt = CalcCCITT (buffer, 0, size, 0xffff, 0);

      if (recv_ccitt == calc_ccitt)
      {
        retryCount = 0;    /* set to not retry this block */
        if (verbose == 1)
          printf ("writing %d bytes to file for block %d\n", size, currentBlock);
          
        write(fdFile, buffer, size);

        /* we are done with time consuming stuff - send ACK */

        write_fdTTY(ackBuffer, 1); /* OK - send ACK */
        break;          /* break out of retry loop */
      }
      else
      {
        /* write the bad block out to a file */

        sprintf(badBlockFile, "%s.%d.%d.bb", filename, currentBlock, retryCount);
        fdBadBlock = CreateFile(badBlockFile);

        write (fdBadBlock, buffer, size);
        close (fdBadBlock);

        /* report error - retry count was already set above */

        printf("CCITT received - %04x : calculated %04x\n"
               , recv_ccitt
               , calc_ccitt
              );
        write_fdTTY(nakBuffer, 1); /* now send NAK */

        /* no break here - stay in retry loop */

      }
    }
    if (size == 0)
      break;
  }

  write_fdTTY(ackBuffer, 1);    /* send ACK to file size of zero */

  printf ("Closing file %s\n", filename);
  close (fdFile);
}

void sendFileInfo(filename)
char *filename;
{
  int i;
  char response[1];
  char *statPtr;
  char byteToSend[1];

  if (verbose == 1)
    printf("waiting for remote to request a filename\n");
    
  read_fdTTY(response, 1); /* wait for remote to ask for filename */

  if (response[0] == 0x06)
  {
    /* first send the statBuffer */
    if (verbose == 1)
      printf("sending statBuffer\n");
      
    statPtr = (char*)&statBuffer;
    write_fdTTY(statPtr, sizeof(statBuffer));

    /* now send the filename - there will always be a null byte */
    if (verbose == 1)
      printf("sending filename\n");
      
    for (i = 0; i < 16; i++)
    {
      byteToSend[0] = filename[i];
      write_fdTTY(byteToSend, 1);
      if (filename[i] == 0x00)
        break;
    }
  }
  else
    printf("non- ACK received: %02x\n", response[0]);
}

void sendDirectory ()
{
  int i;
  int fdParent;
  int deFDN;
  unsigned char dirEntry[16];
  unsigned char buf[32];
  char filename[15];
  char *cwd;
  char currentWorkingDir[512];
  char tempbuf[MAXPATHLEN];
  int fdDir;
  int count;
  int usingcwd;
  int statReturn;
  int proceed;

  char response[1];

  /*
      UniFLEX
        struct stat // structure returned by stat (size = 21)
        {
            int   st_dev ;    // 0x00 - device number
            int   st_ino ;    // 0x02 - fdn number
            unsigned st_mode; // 0x04 - file mode and permissions
            char st_nlink;    // 0x06 - file link count
            int st_uid;       // 0x07 - file owner's user id
            long st_size;     // 0x09 - file size in bytes
            long st_mtime;    // 0x0D - last modified time
            long st_spr;      // 0x11 - spare - future use only
                              // 0x15 -     next byte after
        };
        
      minix
        struct stat {
            short int st_dev;
            unsigned short st_ino;
            unsigned short st_mode;
            short int st_nlink;
            short int st_uid;

            short int st_gid;    // <- this is where they start to differ
            short int st_rdev;

            long st_size;        // they sync back up here
            long st_atime;       // UniFLEX does not have this one
            long st_mtime;
            long st_ctime;       // or this one
        };
  */
  /* remote will be sending a directory to browse */

  for (i = 0; i < SIZEOFDIRECTORY; i++)
  {
    read_fdTTY(&dir[i], 1);

    /* remote will send 0x00 to end input of directory name*/

    if (dir[i] != 0x00)
      write_fdTTY(ackBuffer, 1);   /* ack each directory byte */
    else
      break;
  }

  /* Get the list of files in this directory before sending the ACK */
  /* for now just display a message */

  if (strlen(dir) > 0)
  {
      /* first we MUST make sure that the string passed in is actually a directory */

      statReturn = directoryExists(dir);
      switch (statReturn)
      {
      case 1:     /* directory exists */
          proceed = 1;
          printf("directory exists\n");
          break;

      case 0:     /* directory does not exist, but a file with that name does */
          proceed = 0;
          printf("directory does not exists, but a normal file does\n");
          break;

      case -1:    /* neither a directory or normal file exists with specified name */
          printf("neither a directory nor a normal file exists\n");
          proceed = 0;
          break;
      }

      if (dir[0] == '/')    /* absolute path from root. */
      {
          printf("browsing absolute path: %s\n", dir);
          usingcwd = 0;
      }
      else
      {
          printf("browsing relative path: %s\n", dir);
          usingcwd = 0;
      }
  }
  else
  {
      usingcwd = 1;
      cwd = getwd(currentWorkingDir, 511);
      for (i = 0; i < strlen(cwd); i++)
      {
          dir[i] = cwd[i];
          dir[i + 1] = 0x00;
      }
      printf("browsing <current directory> %s\n", dir);
      proceed = 1;
  }

  if (proceed == 1)
  {
      /* do processing to get list and then ack the directory name */
      {
          if (verbose == 1) printf("openning %s\n", dir);
          fdDir = open(dir, O_RDONLY);
          if (fdDir != 0)
          {
              if (verbose == 1) printf("opened %s - skipping '.' directory\n", dir);
              count = read(fdDir, buf, 16);    /* skip the . directory */

              /* now send the ACK - tells remote to start requesting filenames */
              if (verbose == 1) printf("sending ACK to tell remote it is OK to start requesting filenames\n");
              write_fdTTY(ackBuffer, 1);  /* now we can ACK the directory name */

              while (count > 0)
              {
                  count = read(fdDir, dirEntry, 16);        /* get a directory entry */
                  if (count == 16)
                  {
                      deFDN = dirEntry[0] * 256 + dirEntry[1];  /* get FDN for this file */

                      /* if the FDN is not zero - then it is valid */
                      /* and we have a filename */
                      if (deFDN != 0x00)
                      {
                          if (verbose == 1) printf("FDN of file = %04x name = ", deFDN);
                          for (i = 0; i < 14; i++)
                              filename[i] = dirEntry[i + 2];
                          filename[14] = 0x00;

                          tempbuf[0] = 0x00;   /* make sure tempbuf is terminated */
                          if (usingcwd == 0)
                          {
                              strcpy(tempbuf, dir);
                              strcat(tempbuf, "/");
                          }
                          strcat(tempbuf, filename);
                          if (verbose == 1) printf("%s\n", filename);

                          stat(tempbuf, &statBuffer);  /* we need full path for stat */
                          sendFileInfo(filename); /* will wait for the request */
                      }
                  }
              }
              if (verbose == 1)
				printf("telling remote to stop requesting filenames\n");
				
              /* when we are done - send a statBuffer with all zeros */
              /* and a blank filename */
              
              statBuffer.st_dev = 0;
              statBuffer.st_ino = 0;
              statBuffer.st_mode = 0;
              statBuffer.st_nlink = 0;
              statBuffer.st_uid = 0;
              statBuffer.st_gid = 0;
              statBuffer.st_rdev = 0;
              statBuffer.st_size = 0;
              statBuffer.st_atime = 0;
              statBuffer.st_mtime = 0;
              statBuffer.st_ctime = 0;
              for (i = 0; i < 16; i++)
                  filename[i] = 0x00;
              sendFileInfo(filename); /* will wait for the request */

              close(fdDir);
          }
          else
          {
              printf("could not open '.' file\n");
              write_fdTTY(ackBuffer, 1);  /* ACK the directory name */
          }
      }
  }
  else
  {
      /* not a valid directory - now send the ACK - */
      /* tells remote to start requesting filenames */
      if (verbose == 1)
          printf("sending ACK to tell remote it is OK to start requesting filenames\n");
      write_fdTTY(ackBuffer, 1);  /* ACK the directory name */

      if (verbose == 1)
          printf("telling remote to stop requesting filenames\n");
      /* when we are done - send a statBuffer with all zeros and and a blank filename */
      statBuffer.st_dev = 0;
      statBuffer.st_ino = 0;
      statBuffer.st_mode = 0;
      statBuffer.st_nlink = 0;
      statBuffer.st_uid = 0;
      statBuffer.st_gid = 0;
      statBuffer.st_rdev = 0;
      statBuffer.st_size = 0;
      statBuffer.st_atime = 0;
      statBuffer.st_mtime = 0;
      statBuffer.st_ctime = 0;
      for (i = 0; i < 16; i++)
          filename[i] = 0x00;
      sendFileInfo(filename); /* will wait for the request */
  }
}

void sendWord (wordToSend)
unsigned int wordToSend;
{
  char singleByte[1];

  singleByte[0] = wordToSend / 256; write_fdTTY(singleByte, 1);
  singleByte[0] = wordToSend % 256; write_fdTTY(singleByte, 1);
}

void sendBlock (buf, length)
char* buf;
int length;
{
  int i;
  char singleByte[1];

  for (i = 0; i < length; i++)
  {
    singleByte[0] = buf[i];
    write_fdTTY(singleByte, 1);
  }
}

void sndFile ()
{
  int i;
  int fd;
  int blockSize;
  int blockNumber;
  int ccitt;
  char response [1];
  char buf [256];

  blockNumber = 1;

  /* The first step is to accept the filename from the remote */
  /* remote will be sending the name of the file to be sent   */

  for (i = 0; i < SIZEOFFILENAME; i++)
  {
    /* remote will send 0x00 to end input of filename */
    /* no need to delay writing ACK to null byte since*/
    /* we will be initiating the next move            */
    read_fdTTY(&filename[i], 1);
    write_fdTTY(ackBuffer, 1);

    if (filename[i] == 0x00)
      break;
  }
  printf ("remote requesting file: %s\n", filename);

  /* receipt of the 0x00 character that means we have the filename */
  /* the remote is now expecting start sending blocks of data
        two byte file size
        data[filesize]
        CCITT
      so - open the file and start sending blocks of data. send an
      empty block (byte count = 0 with no data or CCITT to end
  */
  fd = open(filename, O_RDONLY);
  if (fd != 0)
  {
    printf("opened file: %s\n", filename);
    while (fd != 0) /* this is the block sending loop */
    {
      blockSize = read(fd, buf, 256); /* get a block - up to 256 bytes at a time */
      while (blockSize > 0) /* this is the retry loop */
      {
        ccitt = CalcCCITT (buf, 0, blockSize, 0xffff, 0);
        if (verbose == 1)
          printf("sending %d bytes as block %d CCITT = %04x\n"
            , blockSize
            , blockNumber
            , ccitt);

        /* send the blockSize to the remote if not zero*/
        sendWord(blockSize);
        sendBlock(buf, blockSize);
        sendWord (ccitt);

        /* now wait for ACK or NAK to data just sent */
        read_fdTTY(response, 1);
        if (response[0] == 0x06)
        {
          blockNumber++;
          break;  /* out of retry loop - do next block */
        }
      }

      /* if we get blockSize = 0 then we are done */
      if (blockSize == 0)
        break;
    }

    /* now send the an empty block */
    sendWord(0);

    /* and close the file */
    close (fd);
    printf("closed the file and sent empty block\n");
  }
}

/* get the current working directory and send back to remote
   as a null terminated string */
   
void sndCurrDir()
{
    int i;
    int nameLength;
    char *cwd;
    unsigned char byteToSend[1];
    char currentWorkingDir[512];
    
    cwd = getwd(currentWorkingDir, 511);
    nameLength = strlen(cwd);
    
    /* now send the current working directory - ends with a null byte */
    if (verbose == 1)
        printf("sending cwd: %s\n", cwd);
        
    for (i = 0; i < nameLength; i++)
    {
        byteToSend[0] = (unsigned char)cwd[i];
        write_fdTTY(byteToSend, 1);
    }
    byteToSend[0] = 0x00;
    write_fdTTY(byteToSend, 1);
}

void print_ulong(n) 
unsigned long n;
{
    char buf[12]; /* enough for 32-bit decimal */
    int i = 0;
    unsigned long q, r;

    if (n == 0) { putchar('0'); return; }

    while (n != 0) 
    {
        q = n / 10;     /* make sure division is done in 32-bit */
        r = n - q * 10; /* remainder without % */
        buf[i++] = '0' + r;
        n = q;
    }

    while (--i >= 0) putchar(buf[i]);
}

/*
    Skips whitespace
    Stops on first non-digit
    Avoids undefined behavior
    No ANSI prototypes
    No C99 features
*/
unsigned long atoul(s)
char *s;
{
    unsigned long v;

    v = 0;

    /* Skip leading spaces */
    while (*s == ' ' || *s == '\t')
        s++;

    while (*s >= '0' && *s <= '9') {
        v = v * 10 + (*s - '0');
        s++;
    }

    return v;
}

/* handle requests (commands) from the client using ttyfd */

void main (argc, argv)
int argc;
char **argv;
{
  int i;
  int j;
  unsigned long baudRate = 19200;
  unsigned char brCode = 0xCC;
  unsigned char *baud;
  char c;
  
  commPort = 1;    /* set default in case it is not supplied */
  
  ackBuffer[0] = 0x06; ackBuffer[1] = 0x00;
  nakBuffer[0] = 0x15; nakBuffer[1] = 0x00;

  printf(version);

  /* make sure user specified a port number */

  if (argc < 2)
    printf("no tty number provided - using tty1\n");

  /* start parsing at argv[1] since argv[0] is the program name */

  for (i = 1; i < argc; i++)
  {
    if(
        (argv[i][0] == '-' || argv[i][0] == '+') &&
        (argv[i][1] == 'v' || argv[i][1] == 'V')
      )
    {
      verbose = 1;
    }
    else if(
        (argv[i][0] == '-' || argv[i][0] == '+') &&
        (argv[i][1] == 'b' || argv[i][1] == 'B') &&
        (argv[i][2] == '=')
      )
    {
      baud = (unsigned char *)(argv[i] + 3);/* point to the numeric value */
      baudRate = atoul(baud);
      brCode = setRateCode(baudRate);
    }
    else
    {
    	/* no dash means this is the comm port */
        commPort = argv[i][0] & 0x07;	/* only 1 through 4 is valid */
    }
  }
  
  printf("verbose is %s\n", verbose == 1 ? "on" : "off");

  /* clear out the buffer */

  for (i = 0; i < SIZEOFBUFFER; i++)
    buffer[i] = 0x00;

  /* open the comm port */
  if (commPort > 0 && commPort < 5)
  {
    /* printf ("Setting up COM %d for %ld baud\n", commPort, baudRate); */
    printf("Setting up COM %d for ", commPort);
    print_ulong(baudRate);
    printf(" baud\n");

    initPort (commPort, brCode);  /* set up comm port @ correct baud */
  }
  else
  {
    printf("Only ports 1 through 4 are valid\n");
    exit(1);
  }

  /* we are going to use direct I/O access instead of using the /dev/ttyx */
  /*
  sprintf(device, "/dev/tty%s", commPort);
  fdTTY = open(device, O_RDWR);
  if (fdTTY < 0)
  {
    printf("Invalid tty device number specified\n");
  }
  
  printf("file descriptor for the %s is %d\n", device, fdTTY);
  */
  
  /* turn off echo */
  /*
  gtty(fdTTY, &settings);
  settings.sg_flags = RAW;
  stty(fdTTY, &settings);
  */
  
  /* send a notice to commPort to make sure server knows we are minix */
  /* write_fdTTY("minix", 5); */

  printf ("size of stat buffer = %d\n", sizeof(statBuffer));
  
  while (1)
  {
    printf("waiting for remote\n");
    read_fdTTY(buffer, 1);               /* read a byte          */
    printf("remote sent a byte\n");

    printf("received: %02x\n", buffer[0]); /* Print the read data  */
    command = buffer[0];

    if (command == 0x02 ||
        command == 0x03 ||
        command == 0x04 ||
        command == 0x05)
    {
      printf("sending ACK to command\n");
      write_fdTTY(ackBuffer, 1);

      if (command == 0x02)        /* receive a file from PC */
      {
          recvFile();
      }
      else if (command == 0x03)   /* send a directory listing to PC */
      {
          sendDirectory();
      }
      else if (command == 0x04)   /* send a file to the remote */
      {
          sndFile();
      }
      else if (command == 0x05)   /* send a cwd to the remote */
      {
          sndCurrDir();
      }
    }
  }
}
