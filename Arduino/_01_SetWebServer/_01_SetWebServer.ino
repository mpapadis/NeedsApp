// http://<IP>/arduino/webserver/ ss
#include <Wire.h>
#include <UnoWiFiDevEd.h>



const int VAL_PROBE = 0;
const int MOISTURE_LEVEL = 250;
String gv = "";

void setup() {

  Serial.begin(4800);
  Ciao.begin();
  Wifi.begin();

  pinMode(2, INPUT);

  delay(10000);
  Wifi.println("Web Server is up");
  Serial.println("nEEDSaPP");

}

void loop() {

  while (Wifi.available()) {
    Serial.println("in");
    process(Wifi);
  }
  delay(1000);

  // Serial.println("Humidity");
  // Serial.println((analogRead(0) - 1024) * -1 > 250);

  gv = getValues();
  Serial.println(gv);
  /*
    if ((analogRead(0) - 1024) * -1> 250) {
      digitalWrite(13, LOW);
       Serial.println("LOW");
    } else   {
      digitalWrite(13, HIGH);
      Serial.println("HIGH");
    }
    delay(1000);*/

}


String getValues() {
  String rtn = "";
  String tempID = "{3:";
  String ending = "},";
  String humidityID = "{5:";
  float humidityVal = 0;
  float tempVal = 0 ;
  int sensorReading;
  String temp = "";
  String humidity = "";
  for (int analogChannel = 0; analogChannel < 2; analogChannel++) {
    switch (analogChannel) {
      case 0:
        sensorReading = analogRead(analogChannel);
        humidityVal  = ((sensorReading - 1024) * -1);
        humidity = humidityID + humidityVal + ending ;
        break;
      case 1:
        sensorReading = analogRead(analogChannel);
        tempVal  = (((sensorReading  / 1024.0) * 5000) / 10);
        temp = tempID + tempVal + ending ;
        break;
    }
  }
  rtn = humidity + temp;
  return rtn;
  //Serial.println(rtn);
}

void process(WifiData client) {
  // read the command
  String command = client.readStringUntil('/');

  //Serial.println(command == "sensorsdata");
  Serial.println(command);
  if (command == "sensorsdata") {
    GetSensorsData(client);
  } else if (command == "wateron") {
    digitalWrite(13, HIGH);
	  GetEmptyRequest(client, "<html><body>ON</body></html>");
  } else if (command == "wateroff") {
    digitalWrite(13, LOW);
	  GetEmptyRequest(client, "<html><body>OFF</body></html>");
  }
  //else{
  //  GetEmptyRequest(client);
  //}
}

void SendSensorsData( ) {

  String sensorsDataArray = getValues();
  const char* connector = "rest";
  const char* server = "10.0.0.116";
  const char* command = "/api/ArduinoStations/[{1:4},{2:6}]";
  const char* method = "GET";

  CiaoData data = Ciao.write(connector, server, command, method);
  if (!data.isEmpty()) {
    Ciao.println( "State: " + String (data.get(1)) );
    Ciao.println( "Response: " + String (data.get(2)) );
    Serial.println( "State: " + String (data.get(1)) );
    Serial.println( "Response: " + String (data.get(2)) );
  }
  else {
    Ciao.println ("Write Error");
    Serial.println ("Write Error");
  }
}

void GetEmptyRequest(WifiData responseWifiData, String html) {
  responseWifiData.println("HTTP/1.1 200 OK");
  responseWifiData.println("Content-Type: text/html");
  responseWifiData.println("Connection: close");
  //client.println("Refresh: 20");  // refresh the page automatically every  sec
  responseWifiData.println();
  responseWifiData.println(html);
  responseWifiData.print(DELIMITER);
}

void GetSensorsData(WifiData responseWifiData) {
  responseWifiData.println("HTTP/1.1 200 OK");
  responseWifiData.println("Content-Type: text/html");
  responseWifiData.println("Connection: close");
  //responseWifiData.println("Refresh: 20");  // refresh the page automatically every  sec
  responseWifiData.println();
  responseWifiData.println("<html>");
  responseWifiData.println("<head> <title>Needsapp Results</title> </head>");
  responseWifiData.print("<body>");
  float sensorReading;
  for (int analogChannel = 0; analogChannel < 2; analogChannel++) {
    switch (analogChannel) {
      case 0:
        sensorReading = analogRead(analogChannel);
        responseWifiData.print("Humidity:  ");

        responseWifiData.print((sensorReading - 1024) * -1);
        responseWifiData.print("<br/>");
        break;
      case 1:
        sensorReading = analogRead(analogChannel);
        responseWifiData.print("Temp:  ");

        responseWifiData.print(((sensorReading  / 1024.0) * 5000) / 10);
        responseWifiData.print("c<br/>");
        break;
      case 2:
        sensorReading = analogRead(analogChannel);

        responseWifiData.print("SoilTemp:  ");
        responseWifiData.print(((sensorReading  / 1024.0) * 5000) / 10);
        responseWifiData.print("c<br/>");
        break;
    }
  }

  responseWifiData.print("</body>");
  responseWifiData.println("</html>");
  responseWifiData.print(DELIMITER);

}
