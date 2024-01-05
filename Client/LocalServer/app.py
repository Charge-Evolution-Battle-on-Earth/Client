from flask import Flask, request, jsonify
from flask_sqlalchemy import SQLAlchemy
from flask_cors import CORS
from werkzeug.security import check_password_hash, generate_password_hash
import jwt
from datetime import datetime, timedelta, timezone

app = Flask(__name__)
CORS(app)
app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///site.db'
app.config['SECRET_KEY'] = 'your_secret_key'  # Replace with your actual secret key
db = SQLAlchemy(app)

class User(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    email = db.Column(db.String(120), unique=True, nullable=False)
    nickname = db.Column(db.String(20), unique=True, nullable=False)
    password = db.Column(db.String(128), nullable=False)

    def __repr__(self):
        return f"User('{self.email}', '{self.nickname}')"

    def set_password(self, password):
        self.password = generate_password_hash(password)

    def check_password(self, password):
        return check_password_hash(self.password, password)

with app.app_context():
    db.create_all()

@app.route('/users/join', methods=['POST'])
def join_user():
    try:
        data = request.get_json()

        # Extract necessary information from input
        email = data['email']
        nickname = data['nickname']
        password = data['password']

        # Create a new user
        new_user = User(email=email, nickname=nickname)
        new_user.set_password(password)

        # Add to the database and commit
        db.session.add(new_user)
        db.session.commit()

        return jsonify(message='User joined successfully'), 201
    except Exception as e:
        # Print exception information for debugging
        print("Exception:", e)
        return jsonify(error=str(e)), 500

@app.route('/users/login', methods=['POST'])
def login_user():
    try:
        data = request.get_json()

        # Extract necessary information from input
        email = data['email']
        password = data['password']

        # Find the user by email
        user = User.query.filter_by(email=email).first()

        # If the user exists and the password matches, login is successful
        if user and user.check_password(password):
            # JWT token creation
            expiration_date = datetime.now(timezone.utc) + timedelta(days=1)
            token_payload = {'email': email, 'exp': expiration_date}
            access_token = jwt.encode(token_payload, app.config['SECRET_KEY'], algorithm='HS256')

            return jsonify(accessToken=access_token), 200
        else:
            return jsonify(message='Login failed. Check your email and password.'), 401
    except Exception as e:
        # Print exception information for debugging
        print("Exception:", e)
        return jsonify(error=str(e)), 500

@app.route('/nations', methods=['GET'])
def get_nations():
    # 예시 데이터
    nations = [
        {"continentName": "조선", "continentCode": "KOR"},
        {"continentName": "당", "continentCode": "CHN"},
        {"continentName": "일본", "continentCode": "JPN"},
        {"continentName": "로마", "continentCode": "ROM"},
        {"continentName": "오스만", "continentCode": "OSM"}
        # 추가적인 나라 정보
    ]

    return jsonify(nations)

if __name__ == '__main__':
    app.run(debug=True)
