import socket

if __name__ == '__main__':
    client()


def client():
    host = '192.168.1.108'  
    port = 12345          

    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.connect((host, port))
        while True:
            message = input("introduce un mensaje para enviar o teclea 'exit' para salir: ")
            if message.lower() == 'exit':
                break
            s.sendall(message.encode())
            data = s.recv(1024)
            print('recibido del servidor:', data.decode())