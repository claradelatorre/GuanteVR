import spidev
import time

spi = spidev.SpiDev()
spi.open(0, 0)
spi.max_speed_hz = 1000000
 
try:
    while True:
        values = [read_sensor(i) for i in range(5)]
        print("valores de los sensores: {}".format(values))
        #time.sleep(0.5)  

except KeyboardInterrupt:
    spi.close()


#LEER DATOS CANAL DE MCP3008
def read_sensor(channel):
    adc = spi.xfer2([1, (8 + channel) << 4, 0])
    data = ((adc[1] & 3) << 8) + adc[2]
    return data
