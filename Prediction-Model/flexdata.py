import socket
import time
import spidev 

#CONFIGURACION SPI
spi = spidev.SpiDev()
spi.open(0, 0) 
spi.max_speed_hz = 1000000

def read_sensor(channel):
    adc = spi.xfer2([1, (8 + channel) << 4, 0])
    data = ((adc[1] & 3) << 8) + adc[2]
    normalize_value = (data / 1023.0)
    return normalize_value

def sendData(host, port):
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.connect((host, port))
        try:
            while True:
                valores = [read_sensor(i) for i in range(5)]
                message = ','.join(map(str, valores)) + '\n'
                s.sendall(message.encode())
                time.sleep(2)
        except KeyboardInterrupt:
            print("interrupción")

host = '192.168.1.108'
port = 12345

if __name__ == '__main__':
    sendData(host, port)
