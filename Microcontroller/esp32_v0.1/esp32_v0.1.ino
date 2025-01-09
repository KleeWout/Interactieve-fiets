#define REED_PIN_L_M 12
#define REED_PIN_L_S 14

#define REED_PIN_R_M 33
#define REED_PIN_R_S 25


const unsigned long interval = 100;
const unsigned long debounceDelay = 200;  

// left
unsigned long lastDebounceTimeLeft = 0;
unsigned long lastRevolutionLeft = 0;
unsigned long previousRevolutionLeft = 0;
float revolutionsPerSecondLeft = 0;

// right
unsigned long lastDebounceTimeRight = 0;
unsigned long lastRevolutionRight = 0;
unsigned long previousRevolutionRight = 0;
float revolutionsPerSecondRight = 0;


void IRAM_ATTR handleRevolutionLeft() {

  unsigned long currentTime = millis();

  if (( currentTime - lastDebounceTimeLeft) > debounceDelay) {
    previousRevolutionLeft = lastRevolutionLeft;
    lastDebounceTimeLeft = millis();
    revolutionsPerSecondLeft = 1000.0 / (currentTime - lastRevolutionLeft);
    lastRevolutionLeft = millis();
  }
}
void IRAM_ATTR handleRevolutionRight() {

  unsigned long currentTime = millis();

  if (( currentTime - lastDebounceTimeRight) > debounceDelay) {
    previousRevolutionRight = lastRevolutionRight;
    lastDebounceTimeRight = millis();
    revolutionsPerSecondRight = 1000.0 / (currentTime - lastRevolutionRight);
    lastRevolutionRight = millis();
  }
}

void setup() {

  Serial.begin(115200);

  pinMode(REED_PIN_L_M, INPUT_PULLUP);
  pinMode(REED_PIN_L_S, INPUT_PULLUP);
  pinMode(REED_PIN_R_M, INPUT_PULLUP);
  pinMode(REED_PIN_R_S, INPUT_PULLUP);

  attachInterrupt(digitalPinToInterrupt(REED_PIN_L_M), handleRevolutionLeft, RISING);
  attachInterrupt(digitalPinToInterrupt(REED_PIN_R_M), handleRevolutionRight, RISING);

}

void loop() {

  if(millis() > lastRevolutionLeft + (lastRevolutionLeft - previousRevolutionLeft)){
    revolutionsPerSecondLeft = 1000.0 / (millis() - lastRevolutionLeft);
  }
  if(millis() > lastRevolutionRight + (lastRevolutionRight - previousRevolutionRight)){
    revolutionsPerSecondRight = 1000.0 / (millis() - lastRevolutionRight);
  }

  delay(interval);

  Serial.println(String(revolutionsPerSecondLeft) +","+ String(revolutionsPerSecondRight));

}
