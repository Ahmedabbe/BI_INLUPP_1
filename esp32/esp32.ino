#include <WiFi.h>
#include <Esp32MQTTClient.h>
#include <DHT.h>
#include <ArduinoJson.h>
#include "time.h"

#define DIFF 0.5
float prevTemp = 0; // for saving last uploaded temp

#define DHT_PIN 4
#define DHT_TYPE DHT11

DHT dht(DHT_PIN,DHT_TYPE);
IOTHUB_CLIENT_LL_HANDLE deviceClient;

char* ssid = "Ahmed";
char* pass = "KompIS123";
char *conn = "HostName=AhmedAlhasaniIOT20.azure-devices.net;DeviceId=esp32;SharedAccessKey=rEQbiEEJYH0BQW+Y0Y+9mbxcs27f1n16SQpEB49nP7o=";

bool messagePending = false;
int interval = 1000*5;
unsigned long prevMillis = 0;
time_t epochTime;


bool checkTempDiff(float currentTemp) {
  if (currentTemp == 0) return false;
  if (currentTemp < (prevTemp - DIFF) || currentTemp > (prevTemp + DIFF)) { //check if temp difference exceedes 0.5
    prevTemp = currentTemp;
    return true;
  }
  return false;
}


void setup() {
  initSerial();
  initWifi();
  initEpochTime();
  initDHT();
  initDevice();
}

void loop() {
  unsigned long currentMillis = millis();

  if(!messagePending) {
    if((currentMillis - prevMillis) >= interval) {
      prevMillis = currentMillis;

      epochTime = time(NULL);
      Serial.printf("Current Time: %lu. ", epochTime);

      float temperature = dht.readTemperature();
      float humidity = dht.readHumidity();
      
      if(checkTempDiff(temperature)){
        if(!(std::isnan(temperature)) && !(std::isnan(humidity))){
          char payload[256];
          
          DynamicJsonDocument doc(sizeof(payload));
          doc["type"] = "dht";
          doc["deviceId"] = "esp32";
          doc["epochTime"] = epochTime;
          doc["temperature"] = temperature;
          doc["humidity"] = humidity;
  
          
          serializeJson(doc, payload);
          
          sendMessage(payload);       
        }
      }else{
      Serial.println("Awaiting significant change in temp reading..");
      delay(1000*2); 
      }  
    }
  }

  IoTHubClient_LL_DoWork(deviceClient);
  delay(100);
}
