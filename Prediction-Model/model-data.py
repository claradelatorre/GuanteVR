import numpy as np
import socket
from tensorflow.keras.models import load_model
from flask import Flask, jsonify

# CARGA EL MODELO
model = load_model('https://raw.githubusercontent.com/claradelatorre/GuanteVR/main/Prediction-Model/model.h5')

# GUARDA LA ÚLTIMA PREDICCIÓN PARA UNITY
last_prediction = None

app = Flask(__name__)

# LANZA URL CON LA PREDICCIÓN
@app.route('/get_prediction', methods=['GET'])
def get_prediction():
    if last_prediction is not None:
        return jsonify({'predicción': last_prediction})
    else:
        return jsonify({'error': 'NO_PREDICCIÓN'}), 404

if __name__ == '__main__':
    import threading
    threading.Thread(target=sensor_server).start()
    app.run(host='0.0.0.0', port=5000)

# PREDICCIÓN DEL GESTO    
def predict(data):
    global last_prediction
    prediction = model.predict(np.array(data).reshape(1, -1))
    last_prediction = int(np.argmax(prediction, axis=1)[0])  
    return last_prediction

def sensor_server():
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.bind(('0.0.0.0', 12345))
        s.listen()
        print("servidor conectando...")
        conn, addr = s.accept()
        with conn:
            while True:
                data = conn.recv(1024)
                if not data:
                    break
                data = np.fromstring(data.decode(), sep=',', dtype=float)
                prediction = predict(data)
                print("predicción del gesto:", prediction)
