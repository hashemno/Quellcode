#include <b15f/b15f.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <stdint.h>

//Globale Variablen
B15F& drv   = B15F::getInstance(); //Instanz erzeugen
int state   = -1; //1 -> sending || 0 -> listening
int channel = 0x0f;
uint8_t bufferCache[8*sizeof(uint8_t)] = {0};
uint8_t bufferCounter = 0;
uint8_t bytePot[32*sizeof(uint8_t)] = {0};
uint8_t potCounter = 0;
//Datei Variablen

//system("rm output");


//Verbosity Function
void showBytes(uint8_t leftByte, uint8_t rightByte) {
        printf("\n%x%x\n", leftByte, rightByte);
        //Showing bits which are about to be transfered
        for(int j = 0; j < 4; j++) {
                ((leftByte >> j) & 1) ? printf("%d", 1) : printf("%d", 0);
                ((rightByte >> j) & 1) ? printf("%d\n", 1) : printf("%d\n", 0);
        }
}

//
void showBuffer() {
        printf("[] show the already transmitted Symbols []\n");
        for(int i = 0; i < bufferCounter; i++) {
                printf("%x ", bufferCache[i]);
        }
        printf("\n");
}


/*Escapezeichen:
00           -> Start einer Übertragung (Mal schauen -> noch nicht implementiert)
01           -> Ende einer Übertragungssequenz -> Impliziert Toggle Sende/Empfang Zustand
<...>
06           -> ACK des vorherigen pakets -> letztes paket war okay
05           -> CRP des vorherigen Pakets -> letztes paket ist corrupted
<...>
09          -> Seperator für doppelte Zeichen
*/

//Escape Zeichen senden
void sendingEscape(uint8_t code) {
        drv.delay_ms(1000);
        drv.setRegister(&PORTA, 0);
        printf("Sending 0...\n");
        drv.delay_ms(1000);
        printf("Sending %x...\n", code);
        drv.setRegister(&PORTA, code);
        drv.delay_ms(1000);
}

//Sending the Checksum
void sendingChecksum(uint8_t checkSum) {
        printf("[*] Checksum %x\n", checkSum);
        sendingEscape(3); //Trenn Zeichen
        drv.delay_ms(1000);
        drv.setRegister(&PORTA, ((checkSum & 0xF0) >> 4));
        drv.delay_ms(1000);
        drv.setRegister(&PORTA, (checkSum & 0x0F));
}

//Funktion to send bytes
void sendBytes(uint8_t leftByte, uint8_t rightByte, uint8_t encoding) {
        drv.setRegister(&PORTA, leftByte);
        printf("Sending: %x\n", leftByte);
        drv.delay_ms(1000);
        //check for symbol doubling and send encoding <09> if two symbols are the same
        if(leftByte == rightByte) {
                sendingEscape(9); //seperator
        }
        drv.setRegister(&PORTA, rightByte);
        printf("Sending: %x\n", rightByte);
        drv.delay_ms(1000);
}


//Compute the Checksum of the transmitted Symbols
int getChecksum() {
        int checksum = 0;
        printf("\n[*] Calculating Checksum...\n");
        for(int i = 1; i < potCounter; i++) {
                printf("[*] bytes: %x\n", bytePot[i]);
                if(bytePot[i] == 0 && bytePot[i+1] == 9) {
                        i+=1;
                } else if(bytePot[i] == 0 && bytePot[i+1] == 3) {
                        break;
                }
                else {
                        checksum += bytePot[i];
                }
        }
        printf("\n");
        return checksum;
}

void resendPackage();
//This can be used for writing the transmitted symbols to stdout
void writeBytePotToSTDOUT() {
        for(int i = 0; i < potCounter; i++) {
                printf("%x", bytePot);
        }
}


//Auf Response des Empfängers warten,
//Hierbei gibt es zwei Escape Zeichen:
//      05 -> (ACK) Alles gut
//      06 -> (CRP) Das letzte Paket ist kaputt(basiert auf verschiedene Checksummen) -> es wird erneut gesendet
void listenForResponse() {
        uint8_t lastSymbol, nextSymbol = 0;
        //Listen for Response
        drv.setRegister(&DDRA, 0x00); //Toggel sending/receiving mode
        showBuffer();
        printf("\nWaiting for Status Code within the Response....\n");
        while(1) {
                lastSymbol = drv.getRegister(&PINA);
                printf("last Symbol: %x\n", lastSymbol);
                drv.delay_ms(250);

                //Main Response Block
                nextSymbol = drv.getRegister(&PINA);
                printf("nextSymbol: %x\n", nextSymbol);
                //drv.delay_ms(250);
                if(lastSymbol == 0 && nextSymbol == 5) {
                        fprintf(stderr, "[*] ACK Symbol was received.. -> last paket was fine\n");
                        //clear buffer Cache
                        writeBytePotToSTDOUT();
                        drv.setRegister(&DDRA, 0x0f);
                        drv.delay_ms(1000);
                        break;
                } else if(lastSymbol == 0 && nextSymbol == 6) {
                        fprintf(stderr, "[*] CRP received -> last paket was corrupted...\n");
                        resendPackage();
                }
        }
}

//Resend Damaged Packages
void resendPackage() {
        uint8_t checksum, leftByte, rightByte, encoding = 0;
        drv.setRegister(&DDRA, 0x0f);
        for(int i = 0; i < sizeof(bufferCache); i++) {
                leftByte = (bufferCache[i]&0xf0)>>4;
                checksum += leftByte;
                printf("[*] -> %x\n", leftByte);
                rightByte = (bufferCache[i]&0x0f);
                checksum += rightByte;
                printf("[*] -> %x\n", rightByte);
                encoding = 1 ? (leftByte == rightByte) : encoding = 0;
                sendBytes(leftByte, rightByte, encoding);
        }
        //sending checksum
        sendingChecksum(checksum);
        sendingEscape(1); //Terminating the Sequence
        listenForResponse();
}



void sendingState() {
                //uint8_t bufferCache[16*sizeof(uint8_t)] = {0};
                uint8_t buffer[8*sizeof(uint8_t)] = {0}; //der buffer muss noch an die groesse, der einzulesenden datei/ des einzulesenden buffers angepasst werden
                uint8_t leftByte, rightByte, parity, lastSymbol, encoding, checkSum = 0;


                /B15 Variablen/
                drv.setRegister(&DDRA, channel); //0 Empfänger -> setze die letzten 4 Bit auf F um auf diesen zu senden
                fprintf(stderr, "\n[*] Transmition Sequence is initialized...Sending on Channel %x\n", channel);

                //Read data from stdin and print the Binary on stdout
                while(fread(buffer, 1, sizeof(buffer), stdin) > 0) {
                        //printf("\nPrinting One Paket with size 8*8Bit -> 64bit\n");
                        for(int i = 1; i < sizeof(buffer); i+=2) {
                                //First Byte of 2 Byte
                                bufferCache[bufferCounter] = buffer[i];
                                bufferCounter++;
                                //splicing the byte
                                leftByte = (buffer[i]&0xf0) >> 4;
                                checkSum += leftByte;
                                if(leftByte == rightByte) {
                                        sendingEscape(9);
                                }
                                rightByte = buffer[i]&0x0f;
                                checkSum += rightByte;
                                showBytes(leftByte, rightByte);
                                sendBytes(leftByte, rightByte, encoding);

                                //Second Byte of 2 Byte
                                bufferCache[bufferCounter] = buffer[i-1];
                                bufferCounter++;
                                leftByte = (buffer[i-1]&0xf0) >> 4;
                                checkSum += leftByte;
                                if(leftByte == rightByte) {
                                        sendingEscape(9);
                                }
                                rightByte = buffer[i-1]&0x0f;
                                checkSum += rightByte;
                                showBytes(leftByte, rightByte);
                                sendBytes(leftByte, rightByte, encoding);
                        }
                        sendingChecksum(checkSum);
                        //End Transmission
                        sendingEscape(1); //terminate the Transmission Sequence
                        //Listen for Response
                        listenForResponse();
                        checkSum = 0;
                        bufferCounter = 0;
                        memset(bufferCache, 0, sizeof(bufferCache)); //Reset the Array
                }
}

void listeningState() {
                volatile uint8_t lastSymbol = -1;
                volatile uint8_t nextSymbol = -1;
                uint8_t parityBit, paketNr, fullByte = 0;
                uint8_t checkSum = 0;

                drv.setRegister(&DDRA, 0x00); //Bit 7-1 Eingabe & 0 Ausgabe
                fprintf(stderr, "[*] Starting the Listening Sequence... Data will be received...\n");
                while(1) {
                        //Beziehe das erste Symbol
                        lastSymbol = drv.getRegister(&PINA);
                        fprintf(stderr, "lastSymbol: %x\n", lastSymbol);

                        //Überprüfe, ob das Abbruchsignal empfangen wird
                        if(lastSymbol == 1 && nextSymbol == 0) {break;}

                        //Überprüfe ob die Symbole unterschiedlich sein -> neues Zeichen: Takt
                        if(nextSymbol != lastSymbol && nextSymbol != -1) {
                                fprintf(stderr, "Important Symbol: %x\n", lastSymbol);
                                bytePot[potCounter] = lastSymbol;
                                potCounter++;
                        }
                        //Beziehe das zweite Symbol
                        nextSymbol = drv.getRegister(&PINA);
                        fprintf(stderr, "nextSymbol: %x\n", nextSymbol);
                        drv.delay_ms(250);
                }
                fprintf(stderr, "[*] Transmittion Sequence Interupted...Checking the transmitted symbols...\n");
                fprintf(stderr, "Transmitted Symbols\n");
                for(int i = 1; i < potCounter-1; i++) {
                        //Seperator Doubling Symbols <09>
                        if(bytePot[i] == 0 && bytePot[i+1] == 9) {
                                fprintf(stderr,"%x", bytePot[i+2]);
                                i += 2;
                        }
                        //Seperator Paketsegment <03>
                        else if(bytePot[i] == 0 && bytePot[i+1] == 3) {
                                i += 2;
                                printf("%x%x", bytePot[i], bytePot[i+1]);
                                checkSum = (bytePot[i]<<4) | bytePot[i+1];
                                i += 2;
                        } else {
                                printf("%x", bytePot[i]);
                        }
                }
                fprintf(stderr, "\n[*] Transmitted Checksum: %d\n    ->Computed Checksum: %d\n", checkSum, getChecksum());
                //Examine the Paket received for errors
                //Change to Sending State & Send Status Code for Error Recognition
                drv.setRegister(&DDRA, channel);
                fprintf(stderr, "[*] Acknoledgement Transmition Sequence Initialized on Channel %x...\n", channel);
                if(checkSum != getChecksum()) {
                        sendingEscape(6);
                        //restart the listening state
                        listeningState(); //needs to be checked
                }
                else {
                        sendingEscape(5);
                        potCounter = 0;
                        memset(bytePot, 0, sizeof(bytePot));
                        checkSum = 0;
                }

}

//Hauptprogramm
int main(int argc, char* argv[]) {
        //check the input argument -> argc
        //Check the Initial State
        if(!strcmp("sender", argv[1])) {
                state = 1;
        } else if(!strcmp("listener", argv[1])) {
                state = 0;
        }
        //Main Loop
        while(1) {
                if(state == 1) {
                        sendingState();
                }
                else if(state == 0) {
                        listeningState();
                }
        }
}