import time
from mpu6050 import mpu6050

mpu = mpu6050(0x68)

# LEER DATOS MPU

while True:
    gyro_data = mpu.get_gyro_data()
    print("Gyro:  [X: {:.2f}, Y: {:.2f}, Z: {:.2f}] rad/s".format( gyro_data['x'], gyro_data['y'], gyro_data['z']))
    time.sleep(1)