import socket
import time
import spidev

spi = spidev.SpiDev()
spi.open(0, 0)
spi.max_speed_hz = 1000000

# CREACIÓN DE SOCKET
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
host = '192.168.1.108' 
port = 12345
server_socket.bind((host, port))
server_socket.listen(1)
print("esperando conexión...")
client_socket, address = server_socket.accept()
print("conexión establecida con: " + str(address))

try:
    while True:
        values = [read_sensor(i) for i in range(5)] 
        message = ','.join(map(str, values)) + '\n' 
        client_socket.send(message.encode())  
        time.sleep(0.1)  
except KeyboardInterrupt:
    spi.close()
    client_socket.close()
    server_socket.close()

# FUNCIONES
def read_sensor(channel):
    adc = spi.xfer2([1, (8 + channel) << 4, 0])
    data = ((adc[1] & 3) << 8) + adc[2]
    return data
