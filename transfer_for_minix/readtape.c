#include <fcntl.h>
#include <unistd.h>
#include <stdio.h>

#define BUFSIZE 512
unsigned long fileContentSize;
unsigned long remainingBytesToRead;

/* error handling function */
void error (msg)
char *msg;
{
  perror(msg);
  exit(1);
}

int main(argc, argv)
int argc;
char *argv[];
{
  int dev_fd, out_fd;
  unsigned char buffer[BUFSIZE];
  unsigned int bytes_read, bytes_written;
  
  int i;
  unsigned long bytesToRead;
  unsigned char size[4];
  
  /* check for correct number of arguments */
  if (argc != 3)
  {
    fprintf(stderr, "Usage: %s <device> <output file>\n", argv[0]);
    exit(1);
  } 
  
  /* open the device file for reading */
  dev_fd = open(argv[1], O_RDONLY);
  if (dev_fd < 0)
  {
    perror("Error opening device file");
    exit(1);
  }
  
  /* open the output file for writing */
  out_fd = creat(argv[2], O_WRONLY, 0644);
  if (out_fd < 0)
  {
    perror("Error opening output file");
    close(dev_fd);
    exit(1);
  }
  
  /* first get the first 4 bytes that specify the file content size */
  read(dev_fd, size, 4);
  fileContentSize = 0;
  
  for (i = 0; i < 4; i++)
  {
    fileContentSize = fileContentSize << 8;  /* keep shifting previous */
    fileContentSize += size[i];
  }
  
 printf("filesize = 0x%08X\n", fileContentSize);
  
  /* read from the device and write to the output file */
  remainingBytesToRead = fileContentSize;
  while (remainingBytesToRead > 0)
  {
    if (remainingBytesToRead > (unsigned long)BUFSIZE)
    {
      bytesToRead = (unsigned long)BUFSIZE;
      remainingBytesToRead = remainingBytesToRead - (unsigned long)BUFSIZE;
    }
    else
    {
      bytesToRead = remainingBytesToRead;
      remainingBytesToRead = 0L;
    }
     
    bytes_read = read(dev_fd, buffer, (int)bytesToRead);
    bytes_written = write(out_fd, buffer, bytes_read);
    if (bytes_written != bytes_read)
    {
      perror("Error writing to output file");
      close(dev_fd);
      close(out_fd);
      exit(1);
    }
  }
  
  if(bytes_read < 0)
  {
    perror("Error reading from device");
  }
  
  /* close the files */
  close(dev_fd);
  close(out_fd);
  
  printf("Device content dumped successfully\n");
  return (0);
}

