#define REED_PIN_L_M 12
#define REED_PIN_L_S 14

#define REED_PIN_R_M 25
#define REED_PIN_R_S 33

// Left
bool leftMainStatus = false;
bool leftSecondStatus = false;
bool leftReverse = false;

unsigned long lastRevolutionLeftMain = 0;
unsigned long lastRevolutionLeftSecond = 0;
unsigned long previousRevolutionLeftMain = 0;
float revolutionsPerSecondLeft = 0;

void IRAM_ATTR handleRevolutionLeftMain() {
  unsigned long currentTime = millis();

  if (leftSecondStatus == false) {
    previousRevolutionLeftMain = lastRevolutionLeftMain;

    leftSecondStatus = true;
    leftMainStatus = false;
    lastRevolutionLeftMain = currentTime;

    if (leftReverse) {
      if (lastRevolutionLeftMain - lastRevolutionLeftSecond > 100) {
        leftReverse = false;
      } else {
        revolutionsPerSecondLeft = (-85.0 / (lastRevolutionLeftMain - lastRevolutionLeftSecond)) / 3;
      }
    }
  }
}

void IRAM_ATTR handleRevolutionLeftSecond() {
  unsigned long currentTime = millis();

  if (leftMainStatus == false) {
    leftMainStatus = true;
    leftSecondStatus = false;
    lastRevolutionLeftSecond = currentTime;
    if (lastRevolutionLeftSecond - lastRevolutionLeftMain > 500) {
      leftReverse = true;
    } else {
      leftReverse = false;
      revolutionsPerSecondLeft = 85.0 / (lastRevolutionLeftSecond - lastRevolutionLeftMain);
    }
  }
}

// Right
bool rightMainStatus = false;
bool rightSecondStatus = false;
bool rightReverse = false;

unsigned long lastRevolutionRightMain = 0;
unsigned long lastRevolutionRightSecond = 0;
unsigned long previousRevolutionRightMain = 0;
float revolutionsPerSecondRight = 0;

void IRAM_ATTR handleRevolutionRightMain() {
  unsigned long currentTime = millis();

  if (rightSecondStatus == false) {
    previousRevolutionRightMain = lastRevolutionRightMain;

    rightSecondStatus = true;
    rightMainStatus = false;
    lastRevolutionRightMain = currentTime;

    if (rightReverse) {
      if (lastRevolutionRightMain - lastRevolutionRightSecond > 100) {
        rightReverse = false;
      } else {
        revolutionsPerSecondRight = (-85.0 / (lastRevolutionRightMain - lastRevolutionRightSecond)) / 3;
      }
    }
  }
}

void IRAM_ATTR handleRevolutionRightSecond() {
  unsigned long currentTime = millis();

  if (rightMainStatus == false) {
    rightMainStatus = true;
    rightSecondStatus = false;
    lastRevolutionRightSecond = currentTime;
    if (lastRevolutionRightSecond - lastRevolutionRightMain > 500) {
      rightReverse = true;
    } else {
      rightReverse = false;
      revolutionsPerSecondRight = 85.0 / (lastRevolutionRightSecond - lastRevolutionRightMain);
    }
  }
}

void setup() {
  Serial.begin(115200);

  // Left
  pinMode(REED_PIN_L_M, INPUT_PULLUP);
  pinMode(REED_PIN_L_S, INPUT_PULLUP);

  // Right
  pinMode(REED_PIN_R_M, INPUT_PULLUP);
  pinMode(REED_PIN_R_S, INPUT_PULLUP);

  // Attach interrupts for left
  attachInterrupt(digitalPinToInterrupt(REED_PIN_L_M), handleRevolutionLeftMain, RISING);
  attachInterrupt(digitalPinToInterrupt(REED_PIN_L_S), handleRevolutionLeftSecond, RISING);

  // Attach interrupts for right
  attachInterrupt(digitalPinToInterrupt(REED_PIN_R_M), handleRevolutionRightMain, RISING);
  attachInterrupt(digitalPinToInterrupt(REED_PIN_R_S), handleRevolutionRightSecond, RISING);
}

void loop() {
  // Left calculations
  if (!leftReverse && millis() > lastRevolutionLeftMain + (lastRevolutionLeftMain - previousRevolutionLeftMain) && lastRevolutionLeftMain != 0) {
    revolutionsPerSecondLeft = (1000.0 / (millis() - lastRevolutionLeftMain)) / 2;
  }
  if (leftReverse && millis() > lastRevolutionLeftMain + (lastRevolutionLeftMain - previousRevolutionLeftMain) && lastRevolutionLeftMain != 0) {
    revolutionsPerSecondLeft = (-(1000.0 / (millis() - lastRevolutionLeftMain)) / 2) / 3;
  }

  // Right calculations
  if (!rightReverse && millis() > lastRevolutionRightMain + (lastRevolutionRightMain - previousRevolutionRightMain) && lastRevolutionRightMain != 0) {
    revolutionsPerSecondRight = (1000.0 / (millis() - lastRevolutionRightMain)) / 2;
  }
  if (rightReverse && millis() > lastRevolutionRightMain + (lastRevolutionRightMain - previousRevolutionRightMain) && lastRevolutionRightMain != 0) {
    revolutionsPerSecondRight = (-(1000.0 / (millis() - lastRevolutionRightMain)) / 2) / 3;
  }

  // Print left and right RPS
  delay(100);
  Serial.println(String(revolutionsPerSecondLeft) +","+ String(revolutionsPerSecondRight));
}