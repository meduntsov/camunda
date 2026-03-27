from flask import Flask, request, jsonify

app = Flask(__name__)

@app.get('/health')
def health():
    return {'status': 'ok'}

@app.post('/assistant/summary')
def summary():
    payload = request.get_json(force=True)
    title = payload.get('title', 'Unknown request')
    status = payload.get('status', 'Draft')
    description = payload.get('description', '')
    return jsonify({
        'result': f"[OpenClaw] {title} is currently {status}. Key points: {description[:180]}"
    })

@app.post('/assistant/draft-comment')
def draft_comment():
    payload = request.get_json(force=True)
    title = payload.get('title', 'request')
    intent = payload.get('intent', 'review')
    return jsonify({
        'result': f"[OpenClaw draft] For {title}: I recommend we {intent.lower()} after validating budget and risk." 
    })

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=8081)
