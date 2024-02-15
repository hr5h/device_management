#include <BLEDevice.h>
#include <BLEServer.h>
#include <BLEUtils.h>
#include <BLE2902.h>

BLECharacteristic *pCharacteristic;
bool deviceConnected = false;
uint8_t value = 0;
int LED_BUILTIN = 2;

// Сайт для генерирования UUID:
// https://www.uuidgenerator.net/

#define SERVICE_UUID        "4fafc201-1fb5-459e-8fcc-c5c9c331914b"
#define CHARACTERISTIC_UUID "beb5483e-36e1-4688-b7f5-ea07361b26a8"


class MyServerCallbacks: public BLEServerCallbacks {
    void onConnect(BLEServer* pServer) {
      Serial.println("Подключено");
      deviceConnected = true;
    };

    void onDisconnect(BLEServer* pServer) {
      Serial.println("Отключено");
      deviceConnected = false;
      pServer->startAdvertising();
    }
};

void printResistor()
{
  delay(3000);
  char send[8];
  float t = random(0, 10);
  String str = "r" + String(t) + "x10^7";
  strcpy(send, str.c_str());
  pCharacteristic->setValue(send);
  pCharacteristic->notify();
  digitalWrite(LED_BUILTIN, LOW);
}

class ElemCallbacs : public BLECharacteristicCallbacks {
  void onWrite(BLECharacteristic *pCharacteristic) {
      std::string value1 = pCharacteristic->getValue();
      if (value1 == "1")
      {
        digitalWrite(LED_BUILTIN, HIGH);
        /*
        char send[8];
        String str = "s";
        strcpy(send, str.c_str());
        pCharacteristic->setValue(send);
        pCharacteristic->notify();
        */
        printResistor();
      }
        
    if (value1 == "0")
      {
        digitalWrite(LED_BUILTIN, LOW);
      }
    }
  };

void setup() {
  Serial.begin(9600);
  pinMode (LED_BUILTIN, OUTPUT);
  deviceConnected = false;
  // создаем BLE-устройство:
  BLEDevice::init("IPS_1");

  // Создаем BLE-сервер:
  BLEServer *pServer = BLEDevice::createServer();
  pServer->setCallbacks(new MyServerCallbacks());

  // Создаем BLE-сервис:
  BLEService *pService = pServer->createService(SERVICE_UUID);

  // Создаем BLE-характеристику:
  pCharacteristic = pService->createCharacteristic(
                      CHARACTERISTIC_UUID,
                      BLECharacteristic::PROPERTY_READ   |
                      BLECharacteristic::PROPERTY_WRITE  |
                      BLECharacteristic::PROPERTY_NOTIFY |
                      BLECharacteristic::PROPERTY_INDICATE
                    );

  pCharacteristic->setCallbacks(new ElemCallbacs());

  // https://www.bluetooth.com/specifications/gatt/viewer?attributeXmlFile=org.bluetooth.descriptor.gatt.client_characteristic_configuration.xml
  // создаем BLE-дескриптор:
  pCharacteristic->addDescriptor(new BLE2902());
  // запускаем сервис:
  pService->start();

  // запускаем оповещения (advertising):
  pServer->getAdvertising()->start();
  Serial.println("Waiting a client connection to notify...");  //  "Ждем подключения клиента, чтобы отправить ему уведомление..."
}

void loop() {
  if (deviceConnected) {
    char send[8];
    int t = random(0, 20);
    int h = random(0, 20);
    String s = String(t);
    String str = "t" + s;
    strcpy(send, str.c_str());
    pCharacteristic->setValue(send);
    pCharacteristic->notify();
    s = String(h);
    str = "h" + s;
    strcpy(send, str.c_str());
    pCharacteristic->setValue(send);
    pCharacteristic->notify();
    int v = 10;
    str = "v" + String(v);
    strcpy(send, str.c_str());
    pCharacteristic->setValue(send);
    pCharacteristic->notify();
    int b = random(0, 100);
    str = "b" + String(b);
    strcpy(send, str.c_str());
    pCharacteristic->setValue(send);
    pCharacteristic->notify();
  }
  delay(1500);
}