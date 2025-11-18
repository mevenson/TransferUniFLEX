# TransferUniFLEX

  The TransferUniFLEX project consists of two parts. The part that runs on the PC (running Windows) and the 
  part that runs on the UniLFEX system on the Kees UniFLEX machine or minix 1.4b on the PT68K-2/4.

    1.  TransferUniFLEX:
          The GUI application for the PC is used to provide a user interface to transferfiles 
          between the UniFLEX or minix machine and a PC running Windows
          
    2.  tuff
          The command line UniFLEX program that runs to provide file transfer capabilities between it and 
          PC using TCPIP.

    3.  transfer
          The command line UniFLEX or minix program that runs to provide file transfer capabilities between
          it and PC using an  RS232 port.
          
  TransferUniFLEX - transfer was designed to accomplish the following objectives:

    1.  Send files from the PC to UniFLEX or minix 
    2.  Receive files on the PC from UniFLEX or minix
    3.  Browse the UniFLEX or minix file system to select files to transfer to the PC
    4.  Browse the PC to select files to send to UniFLEX or minix.

  Foe more information consult the Help file "TransferUniFLEX_-_transfer.chm"

<img width="2544" height="1392" alt="image" src="https://github.com/user-attachments/assets/059e3766-a4f0-4ecb-891e-03065bf5bbee" />

This is a screen shot of my Windows 10 machine running three instances of TransferUniFLEX. The one on the lower left is connected to the PT68K-2 COM1 port through the PC COM3 port at 19200 baud. The top middle one is connected to the Kees UniFLEX machine via TCPIP on address 10.0.0.128 port 1410. And lastly, the bottom right instance is connected to the PT68K-4 COM1 port through the PC COM9 port at 19200 baud.
