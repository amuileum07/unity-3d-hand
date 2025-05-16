import cv2 as cv
from hand_detector import HandDetector
import socket
import json

MIN_DETECTION_CONFIDENCE = 0.7
MIN_TRACKING_CONFIDENCE = 0.8
SERVER_IP = '127.0.0.1'
SERVER_PORT = 5052

for idx in [2,1,0]:
    WEBCAM_ID = idx
    cap = cv.VideoCapture(WEBCAM_ID)
    if cap.isOpened():
        break
if not cap.isOpened():
    print(f"Error: Could not open webcam with ID {WEBCAM_ID}.")
    exit()

detector = HandDetector(max_hands=2, detection_con=MIN_DETECTION_CONFIDENCE, min_track_con=MIN_TRACKING_CONFIDENCE)

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
server_address = (SERVER_IP, SERVER_PORT)

while True:
    frame = cap.read()[1]
    h = frame.shape[0]

    frame, hands = detector.find_hands(frame)

    data = {'Left': [], 'Right': []}

    if hands:
        for hand in hands:
            hand_type = hand['type']  # 'Left' 또는 'Right'
            lm_list = hand['lmList']
            for lm in lm_list:
                data[hand_type].extend([lm[0], h - lm[1], lm[2]])  # y좌표 반전

        sock.sendto(json.dumps(data).encode('utf-8'), server_address)

    cv.imshow(f'webcam {WEBCAM_ID}', frame)
    if cv.waitKey(1) & 0xFF == 27:
        break

cap.release()