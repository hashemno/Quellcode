#define F_CPU 1000000UL
#include <avr/interrupt.h>
#include <avr/io.h>
#include <util/delay.h>
#include <stdbool.h>

volatile uint16_t ms = 0;
volatile uint8_t sec = 0;
volatile uint8_t min = 0;
volatile uint8_t hour = 0;
volatile uint8_t displayMode = 0; // 0 for hour/minute display, 1 for second display

volatile uint8_t heiligkeit = 0;
volatile bool ini = false;




void decToBin(uint8_t dec, uint8_t bin[6]) {
	for(int i = 0; i < 6; i++){
		bin[i] = (dec >> i) & 1;
	}
}

void showMin(uint8_t bin[6]) {
	if(bin[0] == 1)
	PORTC |= (1 << 5);
	else
	PORTC &= ~(1 << 5);

	if(bin[1] == 1)
	PORTC |= (1 << 4);
	else
	PORTC &= ~(1 << 4);

	if(bin[2] == 1)
	PORTC |= (1 << 3);
	else
	PORTC &= ~(1 << 3);

	if(bin[3] == 1)
	PORTC |= (1 << 2);
	else
	PORTC &= ~(1 << 2);

	if(bin[4] == 1)
	PORTC |= (1 << 1);
	else
	PORTC &= ~(1 << 1);

	if(bin[5] == 1)
	PORTC |= (1 << 0);
	else
	PORTC &= ~(1 << 0);
}

void showHour(uint8_t bin[5]) {
	if(bin[0] == 1)
	PORTB |= (1 << PB2);
	else
	PORTB &= ~(1 << PB2);

	if(bin[1] == 1)
	PORTB |= (1 << PB1);
	else
	PORTB &= ~(1 << PB1);

	if(bin[2] == 1)
	PORTB |= (1 << PB0);
	else
	PORTB &= ~(1 << PB0);

	if(bin[3] == 1)
	PORTD |= (1 << PD5);
	else
	PORTD &= ~(1 << PD5);

	if(bin[4] == 1)
	PORTD |= (1 << PD7);
	else
	PORTD &= ~(1 << PD7);
}

uint8_t prell(uint8_t button) {
	if((PIND & (1 << button)) == 0){
		_delay_ms(40); // Eine kurze Verzögerung zur Entprellung
		return (PIND & (1 << button));
	}
	return 1;
}

void offLED(){
	PORTC &= ~0b00111111;
	PORTD &= ~0b00100010;
	PORTB &= ~0b00000111;
}

void controlBr(){
	if(heiligkeit == 0){
		_delay_ms(0);
		offLED();
		_delay_ms(4);
	}
	else if(heiligkeit == 1){
		_delay_ms(2);
		offLED();
		_delay_ms(2);
	}
	else if(heiligkeit == 2){
		_delay_ms(1);
		offLED();
		_delay_ms(3);
	}

}

int main(void) {
	ASSR |= (1 << AS2);
	TCCR2A |= (1 << WGM21); // CTC
	TCCR2B |= (1 << CS20) | (1 << CS22) | (1 << CS21); // Prescaler 1024
	OCR2A = 32 - 1;
	TIMSK2 |= (1 << OCIE2A);

	// Konfiguriere die Ports für die LED-Anzeigen
	DDRC |= 0b00111111; // PC0 bis PC5 für Sekunden
	DDRD |= 0b00100010; // PD1 und PD5 für Minuten und Stunden
	DDRB |= 0b00000111; // PB0 bis PB2 für Minuten und Stunden
	
	DDRD &= ~(1 << PD2); // PD2 als Eingang für den Taster
	DDRD &= ~(1 << PD0); // PD0 als Eingang für den Taster
	DDRD &= ~(1 << PD7); // PD7 als Eingang für den Taster
	PORTD |= (1 << PD0); // Pull-Up-Widerstand für PD0 aktivieren
	PORTD |= (1 << PD2); // Pull-Up-Widerstand für PD2 aktivieren
	PORTD |= (1 << PD7); // Pull-Up-Widerstand für PD7 aktivieren
	
	// Konfiguration des Interrupts für den externen Taster
	EICRA |= (1 << ISC01); // Trigger bei fallender Flanke
	EIMSK |= (1 << INT0); // INT0-Interrupt aktivieren
	
	ini = true; //
	// Globale Interrupts aktivieren
	sei();
	while (1) {
		uint8_t minBin[6] = {0}; // Initialisierung für Minutenanzeige
			
		if (displayMode == 0){
			decToBin(sec, minBin);
			// Anzeige der Sekunden aktualisieren
			showMin(minBin);
			controlBr();
			
		} else if (displayMode == 1){
			uint8_t hourBin[5] = {0}; // Initialisierung für Stundenanzeige
			decToBin(hour, hourBin);
			decToBin(min, minBin);
			
			// Anzeige der Stunden aktualisieren
			showHour(hourBin);
			// Anzeige der Minuten aktualisieren
			showMin(minBin);
			controlBr();
			
		} else if (displayMode == 2){
			if(prell(0) == 0){
				if(heiligkeit == 0)
					heiligkeit = 1;
				else if(heiligkeit == 1)
					heiligkeit = 2;
				else if(heiligkeit == 2)
					heiligkeit = 0;
			}
			decToBin(12, minBin);	
			showMin(minBin);
			controlBr();
			
		}
	}
}

// Timer Overflow Interrupt-Handler für Timer 2
ISR(TIMER2_COMPA_vect) {
	sec++;
	if(sec >= 60) {
		min++;
		sec = 0;
	}
	if (min >= 60){
		hour++;
		min = 0;
	}
	if (hour > 23){
		hour = 0;
	}
}

ISR(INT0_vect) {
	if (prell(2) == 0 && ini) {
		// Anzeigemodus ändern, unabhängig von seinem vorherigen Zustand
		if (displayMode == 0)//sec mode
			displayMode = 1;
		else if(displayMode == 1)// min, hour mode
			displayMode = 2;
		else if(displayMode == 2)// brightness mode
			displayMode = 0;
	}
}