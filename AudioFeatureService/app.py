from flask import Flask, request, jsonify
# -*- coding: utf-8 -*-
import librosa
import numpy as np
import os

app = Flask(__name__)

UPLOAD_FOLDER = './uploads'
os.makedirs(UPLOAD_FOLDER, exist_ok=True)

def analyze_audio(file_path):
    y, sr = librosa.load(file_path)

    tempo, _ = librosa.beat.beat_track(y=y, sr=sr)
    energy = float(np.mean(librosa.feature.rms(y=y)))
    danceability = float(librosa.feature.spectral_flatness(y=y).mean())

    return {
        'tempo': float(tempo),
        'energy': energy,
        'danceability': danceability
    }

@app.route('/analyze', methods=['POST'])
def analyze():
    file = request.files['file']
    file_path = os.path.join(UPLOAD_FOLDER, file.filename)
    file.save(file_path)

    try:
        result = analyze_audio(file_path)
    finally:
        if os.path.exists(file_path):
            os.remove(file_path)

    return jsonify(result)

if __name__ == '__main__':
    app.run(debug=True)

