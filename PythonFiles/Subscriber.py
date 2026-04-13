import paho.mqtt.client as mqtt
import json

def on_connect(client, userdata, flags, rc):
    print("Connected!", rc)
    client.subscribe("rua/mqtt/test123")

def on_message(client, userdata, msg):
    try:
        data = json.loads(msg.payload.decode())
        print("디바이스:", data["device"])
        print("상태:", data["status"])

if data["device"] == "led":
    if data["status"] == "on":
        print("💡 LED 켜기 실행")
    elif data["status"] == "off":
        print("💡 LED 끄기 실행")
        
    except:
        print("⚠️ JSON 아님:", msg.payload.decode())

client = mqtt.Client()
client.on_connect = on_connect
client.on_message = on_message

client.connect("broker.hivemq.com", 1883, 60)
client.loop_forever()
