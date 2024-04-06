# merge_to_video.py
import cv2
import os
import sys
from datetime import datetime

def create_video(image_folder, output_path):
    images = [img for img in sorted(os.listdir(image_folder)) if img.endswith(".png")]
    frame = cv2.imread(os.path.join(image_folder, images[0]))
    height, width, layers = frame.shape

    video_name = f'recording_{datetime.now().strftime("%Y%m%d_%H%M%S")}.mp4'
    video = cv2.VideoWriter(os.path.join(output_path, video_name), cv2.VideoWriter_fourcc(*'mp4v'), 30, (width, height))

    for image in images:
        video.write(cv2.imread(os.path.join(image_folder, image)))

    cv2.destroyAllWindows()
    video.release()

if __name__ == "__main__":
    image_dir = sys.argv[1]
    print("directory" + image_dir)
    create_video(image_dir, image_dir)
