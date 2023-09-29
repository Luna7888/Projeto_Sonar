#include <Servo.h>

int posicaoServoMotor = 0; 
const int trigPin = 9;
const int echoPin = 10;
int ledVerde = 7;
int ledAmarelo = 3;
int ledVermelho = 5;
long duration;
int distancia;

Servo servo; 

void setup() 
{
  pinMode(ledVerde, OUTPUT);
  pinMode(ledAmarelo, OUTPUT);
  pinMode(ledVermelho, OUTPUT);

  pinMode(trigPin, OUTPUT); // Sets the trigPin as an Output
  pinMode(echoPin, INPUT); // Sets the echoPin as an Input
  Serial.begin(9600); // Starts the serial communication
  servo.attach(2);
  servo.write(0);
  posicaoServoMotor = 0;


}

void ligarLeds(long cm) {

  if (cm <= 30) {
    digitalWrite(ledVermelho, HIGH);
    digitalWrite(ledAmarelo, LOW);
    digitalWrite(ledVerde, LOW);
  }
  else if (cm > 30 and cm < 90) {
    digitalWrite(ledAmarelo, HIGH);
    digitalWrite(ledVermelho, LOW);
    digitalWrite(ledVerde, LOW);
  }
  else {
    digitalWrite(ledVerde, HIGH);
    digitalWrite(ledAmarelo, LOW);
    digitalWrite(ledVermelho, LOW);
  }

}


void loop() 
{
  //para pos igual a 0, enquanto pos for menor que 180, incrementa 1 em pos
   for (posicaoServoMotor = 0; posicaoServoMotor < 180; posicaoServoMotor += 10)
   { 
    servo.write(posicaoServoMotor);// Comanda o servo para ir para o posição da variável pos
    digitalWrite(trigPin, LOW);
    delayMicroseconds(2);
    digitalWrite(trigPin, HIGH);
    delayMicroseconds(10);
    digitalWrite(trigPin, LOW);
    duration = pulseIn(echoPin, HIGH);
    distancia = duration * 0.034 / 2;
    Serial.println((String)distancia + ";"+ (String)posicaoServoMotor);
    ligarLeds(distancia);
    delay(300);

  }
  //para pos igual a 0, enquanto pos for menor que 180, incrementa 1 em pos
  for (posicaoServoMotor = 180; posicaoServoMotor > 0; posicaoServoMotor = posicaoServoMotor - 10) 
  {
    servo.write(posicaoServoMotor);// Comanda o servo para ir para o posição da variável pos
    digitalWrite(trigPin, LOW);
    delayMicroseconds(2);
    digitalWrite(trigPin, HIGH);
    delayMicroseconds(10);
    digitalWrite(trigPin, LOW);
    duration = pulseIn(echoPin, HIGH);
    distancia = duration * 0.034 / 2;
    Serial.println((String)distancia + ";"+ (String)posicaoServoMotor);
    ligarLeds(distancia);
    delay(300);
  }



}
