#define REED_PIN_L_M 12
#define REED_PIN_L_S 14

#define REED_PIN_R_M 33
#define REED_PIN_R_S 25


// left
bool leftMainStatus = false;
bool leftSecondStatus = false;
bool leftReverse = false;

unsigned long lastRevolutionLeftMain = 0;
unsigned long lastRevolutionLeftSecond = 0;

// maybe remove later
unsigned long previousRevolutionLeftMain = 0;

float revolutionsPerSecondLeft = 0;

// deze moet eerst
void IRAM_ATTR handleRevolutionLeftMain() {

  unsigned long currentTime = millis();

  if(leftSecondStatus == false){

    // maybe remove later
    previousRevolutionLeftMain = lastRevolutionLeftMain;


    leftSecondStatus = true;
    leftMainStatus = false;
    lastRevolutionLeftMain = currentTime;

    if(leftReverse){
      if(lastRevolutionLeftMain-lastRevolutionLeftSecond>100){
        leftReverse = false;
      }
      else{
        revolutionsPerSecondLeft = (-85.0/(lastRevolutionLeftMain-lastRevolutionLeftSecond))/3;


        // Serial.println("snelheid = -" + String(lastRevolutionLeftMain-lastRevolutionLeftSecond));
      }
    }
  }

}

// dan kort erna deze
void IRAM_ATTR handleRevolutionLeftSecond() {

  unsigned long currentTime = millis();

  if(leftMainStatus == false){
    leftMainStatus = true;
    leftSecondStatus = false;
    lastRevolutionLeftSecond = currentTime;
    if(lastRevolutionLeftSecond-lastRevolutionLeftMain>500){
      leftReverse = true;
    }else{
      leftReverse = false;
      revolutionsPerSecondLeft = 85.0/(lastRevolutionLeftSecond-lastRevolutionLeftMain);



      // Serial.println("snelheid = " + String(85.0/(lastRevolutionLeftSecond-lastRevolutionLeftMain)));
      // Serial.println("toeren per seconde = " + String(1000.0 / (currentTime - previousRevolutionLeftMain)));
    }
  }

}

void setup() {

  Serial.begin(115200);

  pinMode(REED_PIN_L_M, INPUT_PULLUP);
  pinMode(REED_PIN_L_S, INPUT_PULLUP);
  pinMode(REED_PIN_R_M, INPUT_PULLUP);
  pinMode(REED_PIN_R_S, INPUT_PULLUP);


  attachInterrupt(digitalPinToInterrupt(REED_PIN_L_M), handleRevolutionLeftMain, RISING);
  attachInterrupt(digitalPinToInterrupt(REED_PIN_L_S), handleRevolutionLeftSecond, RISING);

}

void loop() {
  if(!leftReverse && millis() > lastRevolutionLeftMain + (lastRevolutionLeftMain - previousRevolutionLeftMain) && lastRevolutionLeftMain != 0){
    revolutionsPerSecondLeft = (1000.0 / (millis() - lastRevolutionLeftMain))/2;
  }
  if(leftReverse && millis() > lastRevolutionLeftMain + (lastRevolutionLeftMain - previousRevolutionLeftMain) && lastRevolutionLeftMain != 0){
    revolutionsPerSecondLeft = (-(1000.0 / (millis() - lastRevolutionLeftMain))/2)/3;
  }

  delay(100);
  Serial.println(revolutionsPerSecondLeft);


}
