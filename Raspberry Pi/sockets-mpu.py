import socket
import time
from mpu6050 import mpu6050
from collections import deque

mpu = mpu6050(0x68)

host = '192.168.1.108'
port = 12345


# CREACIÓN SOCKET
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
server_socket.bind((host, port))
server_socket.listen(1)
print("esperando conexión...")
client_socket, addr = server_socket.accept()
print("conexión establecida con: ", addr)

gyro_data_queues = {'x': deque(), 'y': deque(), 'z': deque()}

try:
    while True:
        gyro_data = mpu.get_gyro_data()
        smoothed_gyro_data = {
                'x': smooth_data(gyro_data_queues['x'], gyro_data['x']),
                'y': smooth_data(gyro_data_queues['y'], gyro_data['y']),
                'z': smooth_data(gyro_data_queues['z'], gyro_data['z'])
        }
        gyro_str = f"{smoothed_gyro_data['x']:.2f},{smoothed_gyro_data['y']:.2f},{smoothed_gyro_data['z']:.2f}\n"
        client_socket.sendall(gyro_str.encode('utf-8'))
        time.sleep(0.1)

except KeyboardInterrupt: 
    print("programa terminado")

finally:
    client_socket.close()
    server_socket.close()
    print("conexiones cerradas")

# CALIBRACIÓN / SUAVIZAR VALORES DEL SENSOR
def smooth_data(data_queue, new_data, window_size=10):
    if len(data_queue) >= window_size:
        data_queue.popleft()
    data_queue.append(new_data)
    return sum(data_queue) / len(data_queue)