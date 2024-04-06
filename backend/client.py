import socket
import time

def send_data_to_server(data):
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as sock:
        sock.connect(('localhost', 8080))
        for line in data:
            sock.sendall(line.encode('utf-8'))
            print("Data sent to server:", line)
            time.sleep(0.1)  # Sleep for a short time to avoid overwhelming the server

if __name__ == "__main__":
    with open('datalog.txt', 'r') as file:
        data_lines = file.readlines()
    send_data_to_server(data_lines)