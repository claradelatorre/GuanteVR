import csv
import time
from gpiozero import MCP3008

if __name__ == "__main__":
        while True:
            gesture = input("introduce el gesto (Rock, Paper, Scissors, o exit): ")
            if gesture.lower() == 'exit':
                break
            collect_data(gesture)

# LEER DATOS FLEXORES
def read_sensors():
    adc = [MCP3008(channel=i) for i in range(5)]
    return [sensor.value for sensor in adc]

# GUARDADO DE DATOS EN UN FICHERO CSV
def collect_data(gesture, samples=200):
    with open('fingersdata.csv', 'a', newline='') as file:
        writer = csv.writer(file)
        if file.tell() == 0:
            writer.writerow(['Gesture', 'Thumb', 'Index', 'Middle', 'Ring', 'Pinky'])
        print(f"recogiendo {samples} muestras para el gesto: {gesture}")
        for _ in range(samples):
            data = read_sensors()
            writer.writerow([gesture] + data)
            time.sleep(1)
        print(f"completada recogida de datos para el gesto: {gesture}")

