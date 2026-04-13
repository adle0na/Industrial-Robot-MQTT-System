import paho.mqtt.client as mqtt
import json

client = mqtt.Client()
client.connect("broker.hivemq.com", 1883, 60)
client.loop_start()

is_running = False

print("R 입력 시 컨베이어 ON/OFF")

while True:
    key = input("입력: ")

    if key.lower() == "r":
        is_running = not is_running

        data = {
            "conveyor": "start" if is_running else "stop"
        }

        client.publish("rua/mqtt/test123", json.dumps(data))
        print("보냄:", data)
