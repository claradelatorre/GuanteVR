import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from sklearn.manifold import TSNE
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder, StandardScaler
from tensorflow.keras import layers, models, optimizers
from tensorflow.keras.utils import to_categorical
from tensorflow.keras.callbacks import EarlyStopping

data = pd.read_csv('https://raw.githubusercontent.com/claradelatorre/GuanteVR/main/Prediction-Model/fingersdata.csv')

# PREPROCESAMIENTO DE DATOS
X = data.iloc[:, 1:].values  # LECTURAS DEL SENSOR
y = data.iloc[:, 0].values  # GESTOS

# CODIFICAR ETIQUETAS
label_encoder = LabelEncoder()
y_encoded = label_encoder.fit_transform(y)
y_categorical = to_categorical(y_encoded)

# DIVIDIR LOS DATOS EN ENTRENAMIENTO Y PRUEBA
X_train, X_test, y_train, y_test = train_test_split(X, y_categorical, test_size=0.2, random_state=42)

# RED NEURONAL
model = models.Sequential([
    layers.Dense(128, input_shape=(5,), activation='relu'),
    layers.Dropout(0.3),
    layers.Dense(64, activation='relu'),
    layers.Dropout(0.3),
    layers.Dense(3, activation='softmax')
])

# COMPILAR MODELO
model.compile(optimizer='adam', loss='categorical_crossentropy', metrics=['accuracy'])

early_stopping = EarlyStopping(monitor='val_loss', patience=5, restore_best_weights=True)
history = model.fit(X_train, y_train, epochs=100, batch_size=16, validation_split=0.2, callbacks=[early_stopping])

# EVALUAR MODELO
loss, accuracy = model.evaluate(X_test, y_test)
print(f"pérdida: {loss}, precisión: {accuracy}")

# GUARDA EL MODELO EN FORMATO .H5
model.save('model.h5')

print("modelo guardado como model.h5")

# VISUALIZAR DATOS
tsne = TSNE(n_components=2, random_state=42)
X_tsne = tsne.fit_transform(X)
colors = ['rojo', 'verde', 'azul']  
plt.figure(figsize=(10, 8))
for i, color in enumerate(colors):
    plt.scatter(X_tsne[y_encoded == i, 0], X_tsne[y_encoded == i, 1], c=color, label=label_encoder.classes_[i])
plt.title('visualización t-SNE de los gestos')
plt.legend()
plt.show()
