    #include <xc.h>
// CONFIG1L
#pragma config PLLDIV = 1       // PLL Prescaler Selection bits (No prescale (4 MHz oscillator input drives PLL directly))
#pragma config CPUDIV = OSC1_PLL2// System Clock Postscaler Selection bits ([Primary Oscillator Src: /1][96 MHz PLL Src: /2])
#pragma config USBDIV = 1       // USB Clock Selection bit (used in Full-Speed USB mode only; UCFG:FSEN = 1) (USB clock source comes directly from the primary oscillator block with no postscale)

// CONFIG1H
#pragma config FOSC = INTOSC_HS // Oscillator Selection bits (Internal oscillator, HS oscillator used by USB (INTHS))
#pragma config FCMEN = OFF      // Fail-Safe Clock Monitor Enable bit (Fail-Safe Clock Monitor disabled)
#pragma config IESO = OFF       // Internal/External Oscillator Switchover bit (Oscillator Switchover mode disabled)

// CONFIG2L
#pragma config PWRT = OFF       // Power-up Timer Enable bit (PWRT disabled)
#pragma config BOR = OFF         // Brown-out Reset Enable bits (Brown-out Reset enabled in hardware only (SBOREN is disabled))
#pragma config BORV = 3         // Brown-out Reset Voltage bits (Minimum setting)
#pragma config VREGEN = OFF     // USB Voltage Regulator Enable bit (USB voltage regulator disabled)

// CONFIG2H
#pragma config WDT = OFF        // Watchdog Timer Enable bit (WDT disabled (control is placed on the SWDTEN bit))
#pragma config WDTPS = 32768    // Watchdog Timer Postscale Select bits (1:32768)

// CONFIG3H
#pragma config CCP2MX = OFF     // CCP2 MUX bit (CCP2 input/output is multiplexed with RB3)
#pragma config PBADEN = OFF      // PORTB A/D Enable bit (PORTB<4:0> pins are configured as analog input channels on Reset)
#pragma config LPT1OSC = OFF    // Low-Power Timer 1 Oscillator Enable bit (Timer1 configured for higher power operation)
#pragma config MCLRE = ON       // MCLR Pin Enable bit (MCLR pin enabled; RE3 input pin disabled)

// CONFIG4L
#pragma config STVREN = OFF      // Stack Full/Underflow Reset Enable bit (Stack full/underflow will cause Reset)
#pragma config LVP = OFF         // Single-Supply ICSP Enable bit (Single-Supply ICSP enabled)
#pragma config XINST = OFF      // Extended Instruction Set Enable bit (Instruction set extension and Indexed Addressing mode disabled (Legacy mode))

// CONFIG5L
#pragma config CP0 = OFF        // Code Protection bit (Block 0 (000800-001FFFh) is not code-protected)
#pragma config CP1 = OFF        // Code Protection bit (Block 1 (002000-003FFFh) is not code-protected)
#pragma config CP2 = OFF        // Code Protection bit (Block 2 (004000-005FFFh) is not code-protected)
#pragma config CP3 = OFF        // Code Protection bit (Block 3 (006000-007FFFh) is not code-protected)

// CONFIG5H
#pragma config CPB = OFF        // Boot Block Code Protection bit (Boot block (000000-0007FFh) is not code-protected)
#pragma config CPD = OFF        // Data EEPROM Code Protection bit (Data EEPROM is not code-protected)

// CONFIG6L
#pragma config WRT0 = OFF       // Write Protection bit (Block 0 (000800-001FFFh) is not write-protected)
#pragma config WRT1 = OFF       // Write Protection bit (Block 1 (002000-003FFFh) is not write-protected)
#pragma config WRT2 = OFF       // Write Protection bit (Block 2 (004000-005FFFh) is not write-protected)
#pragma config WRT3 = OFF       // Write Protection bit (Block 3 (006000-007FFFh) is not write-protected)

// CONFIG6H
#pragma config WRTC = OFF       // Configuration Register Write Protection bit (Configuration registers (300000-3000FFh) are not write-protected)
#pragma config WRTB = OFF       // Boot Block Write Protection bit (Boot block (000000-0007FFh) is not write-protected)
#pragma config WRTD = OFF       // Data EEPROM Write Protection bit (Data EEPROM is not write-protected)

// CONFIG7L
#pragma config EBTR0 = OFF      // Table Read Protection bit (Block 0 (000800-001FFFh) is not protected from table reads executed in other blocks)
#pragma config EBTR1 = OFF      // Table Read Protection bit (Block 1 (002000-003FFFh) is not protected from table reads executed in other blocks)
#pragma config EBTR2 = OFF      // Table Read Protection bit (Block 2 (004000-005FFFh) is not protected from table reads executed in other blocks)
#pragma config EBTR3 = OFF      // Table Read Protection bit (Block 3 (006000-007FFFh) is not protected from table reads executed in other blocks)

// CONFIG7H
#pragma config EBTRB = OFF       // Boot Block Table Read Protection bit (Boot block (000000-0007FFh) is not protected from table reads executed in other blocks)

    void init();
    static volatile unsigned short costam=0,temp2=0,temp=0,timer3=0,sub_wert=0,i=0,feuer=0,runter=0,ccptimerH=0,ccptimerL=0,pdown=0,timeros=0;
    static volatile unsigned int timer_high[6] =
    {
        0x0000,0x0000,0x0000,0x0000,
        0x0000,0x0000
    };

    static volatile unsigned int timer_low[8] =
    {
        0x0000,0x0000,0x0000,0x0000,
        0x0000,0x0000,0x0000,0x0000
    };

    static volatile unsigned int lookup[256] = {
0x0000,
0x0100,
0x0180,
0x0200,
0x0280,
0x02FF,
0x0380,
0x03FF,
0x0480,
0x0500,
0x0580,
0x0600,
0x0680,
0x0777,
0x08AA,
0x09C6,
0x0AC4,
0x0BCC,
0x0CE0,
0x0DFF,
0x0F2A,
0x1021,
0x1160,
0x1266,
0x1371,
0x1482,
0x1599,
0x16B5,
0x17D7,
0x18FF,
0x19D5,
0x1B05,
0x1C3B,
0x1D16,
0x1E54,
0x1F32,
0x2079,
0x215A,
0x223B,
0x238D,
0x2471,
0x2554,
0x2638,
0x2799,
0x287F,
0x2966,
0x2A4C,
0x2BBB,
0x2CA4,
0x2D8D,
0x2E77,
0x2F60,
0x3049,
0x3132,
0x32B8,
0x33A4,
0x3490,
0x357C,
0x3668,
0x3754,
0x3840,
0x392D,
0x3ACC,
0x3BBB,
0x3CAA,
0x3D99,
0x3E88,
0x3F77,
0x4065,
0x4154,
0x4243,
0x4332,
0x44F1,
0x45E3,
0x46D4,
0x47C6,
0x48B8,
0x49AA,
0x4A9C,
0x4B8D,
0x4C7F,
0x4D71,
0x4E63,
0x4F54,
0x5046,
0x5138,
0x5321,
0x5416,
0x550A,
0x55FF,
0x56F4,
0x57E8,
0x58DD,
0x59D2,
0x5AC6,
0x5BBB,
0x5CAF,
0x5DA4,
0x5E99,
0x5F8D,
0x6082,
0x6177,
0x626B,
0x6360,
0x6454,
0x6549,
0x676E,
0x6865,
0x695D,
0x6A54,
0x6B4C,
0x6C43,
0x6D3B,
0x6E32,
0x6F2A,
0x7021,
0x7119,
0x7210,
0x7308,
0x73FF,
0x74F6,
0x75EE,
0x76E5,
0x77DD,
0x78D4,
0x79CC,
0x7AC3,
0x7BBB,
0x7CB2,
0x7DAA,
0x7EA1,
0x7F99,
0x8090,
0x8188,
0x827F,
0x8376,
0x85F4,
0x86EE,
0x87E8,
0x88E3,
0x89DD,
0x8AD7,
0x8BD2,
0x8CCC,
0x8DC6,
0x8EC0,
0x8FBB,
0x90B5,
0x91AF,
0x92AA,
0x93A4,
0x949E,
0x9599,
0x9693,
0x978D,
0x9888,
0x9982,
0x9A7C,
0x9B76,
0x9C71,
0x9D6B,
0x9E65,
0x9F60,
0xA05A,
0xA154,
0xA24F,
0xA349,
0xA443,
0xA53E,
0xA638,
0xA732,
0xA82D,
0xA927,
0xAA21,
0xAB1B,
0xAC16,
0xAD10,
0xAE0A,
0xAF05,
0xAFFF,
0xB0F9,
0xB1F4,
0xB2EE,
0xB3E8,
0xB4E3,
0xB5DD,
0xB6D7,
0xB7D2,
0xB8CC,
0xB9C6,
0xBAC0,
0xBDDD,
0xBEDA,
0xBFD7,
0xC0D4,
0xC1D2,
0xC2CF,
0xC3CC,
0xC4C9,
0xC5C6,
0xC6C3,
0xC7C0,
0xC8BE,
0xC9BB,
0xCAB8,
0xCBB5,
0xCCB2,
0xCDAF,
0xCEAD,
0xCFAA,
0xD0A7,
0xD1A4,
0xD2A1,
0xD39E,
0xD49B,
0xD599,
0xD696,
0xD793,
0xD890,
0xD98D,
0xDA8A,
0xDB88,
0xDC85,
0xDD82,
0xDE7F,
0xDF7C,
0xE079,
0xE176,
0xE274,
0xE371,
0xE46E,
0xE56B,
0xE668,
0xE765,
0xE863,
0xE960,
0xEA5D,
0xEB5A,
0xEC57,
0xED54,
0xEE52,
0xEF4F,
0xF04C,
0xF149,
0xF246,
0xF343,
0xF440,
0xF53E,
0xF63B,
0xF738,
0xF835,
0xF932,
0xFA2F,
0xFB2D,
0xFC2A,
0xFD27
    };


    static volatile unsigned int wertig[16] =
    {
        0x0000,0x0000,0x0000,0x0000,
        0x0000,0x0000,0x0000,0x0000,
        0x0000,0x0000,0x0000,0x0000,
        0x0000,0x0000,0x0000,0x0000
    };

    void interrupt tc_clrHigh(void) {
        if(PIR1bits.TMR1IF == 1)
            {
                feuer = 0;
                PIR1bits.TMR1IF = 0;
                pdown=0;
                runter = 0;
            }

            if(PIR1bits.CCP1IF == 1 )
            {
                runter = 0;
                PIR1bits.CCP1IF = 0;
                T3CONbits.TMR3ON = 1;
                TMR3H = 0x00;
                TMR3L = 0x00;
                costam = 0;
                if(pdown == 1)
                {
                    TMR1H=0;
                    TMR1L=0;
                    feuer = 1;
//                    timer_low[7] = timer_low[6];
//                    timer_low[6] = timer_low[5];
//                    timer_low[5] = timer_low[4];
//                    timer_low[4] = timer_low[3];
                    timer_low[3] = timer_low[2];
                    timer_low[2] = timer_low[1];
                    timer_low[1] = timer_low[0];

                    temp=timer_high[1];
//                    timer_high[5] = timer_high[4];
//                    timer_high[4] = timer_high[3];
//                    timer_high[3] = timer_high[2];
//                    timer_high[2] = timer_high[1];
                    timer_high[1] = timer_high[0];
                    timer_low[0]=temp;
                    timer_high[0]=CCPR1H;
                    for(i=0; i<2; ++i)
                    {
                        temp2=timer_high[i];
                        timer3+=temp2<<2;
                    }
                    for(i=0; i<4; ++i)
                    {
                        temp2=timer_low[i];
                        timer3+=temp2;
                    }
                    timer3 = timer3 >> 3;
                    sub_wert = lookup[timer3];
                    CCPR2L=sub_wert;
                    CCPR2H=sub_wert>>8;
                    timer3=0;

                }
                else
                {
                    pdown = 1;
                }
            }

        if((PIR2bits.CCP2IF == 1) && (feuer == 1) && (pdown == 1))
        {
            PIR2bits.CCP2IF = 0;
            runter = 1;
//            costam = 1;
        }
        if((PIR2bits.CCP2IF == 1))
        {
            PIR2bits.CCP2IF = 0;
        }
    }
     void main(void)
     {
       init();
       CCPR2L=0xf0;
       CCPR2H=0xf0;
       ei();
    while(1)
        {

        if(TMR3H == 0xFF)
        {
            PORTBbits.RB0 = 1;
            T3CONbits.TMR3ON = 0;
            TMR3H = 0x00;
            TMR3L = 0x00;
            costam = 1;
        }
        else if(costam == 0)
        {
            if(runter == 0)
            {
                PORTBbits.RB0 = PORTCbits.RC2;
            }
            else
            {
                PORTBbits.RB0 = 1;
            }
        }

        }//end while
     }//end main



    void  init()
    {
       TRISB = 0xFE;
       PORTB = 0x00;
       TRISA = 0xBF;
       TRISC = 0xFF;
       LATA =  0x00;
       LATC =  0x00;
       OSCCONbits.IRCF2 = 1;
       OSCCONbits.IRCF1 = 1;
       OSCCONbits.IRCF0 = 1;
       TMR0L = 0x00;
       TMR0H = 0x00;
       TMR1L = 0x00;
       TMR1H = 0x00;
       TMR3L = 0x00;
       TMR3H = 0x00;
       TRISCbits.TRISC2 = 1;
       CCP1CON = 0b00000101;
       CCP2CON = 0b00001010;
       T1CON =  0b00001101;
       T3CON =  0b00110101;
       T0CON = 0x00;
       RCONbits.IPEN = 1;
       INTCON = 0x00;
       INTCONbits.PEIE = 1;
       PIE1 = 0b00000101;
       IPR1 = 0b00000101;
       IPR2 = 0x01;
       PIE2 = 0x01;
       INTCON2 = 0xf0;
       INTCON3 = 0x00;
    }