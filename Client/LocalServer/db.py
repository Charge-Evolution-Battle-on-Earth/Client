from flask import Flask, jsonify
from flask_sqlalchemy import SQLAlchemy

app = Flask(__name__)
app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///nations.db'
db = SQLAlchemy(app)

class Nation(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    continentName = db.Column(db.String(50), nullable=False)
    continentCode = db.Column(db.String(10), nullable=False)

@app.route('/nations', methods=['GET'])
def get_nations():
    nations = Nation.query.all()
    nation_list = [
        {"continentName": nation.continentName, "continentCode": nation.continentCode}
        for nation in nations
    ]
    return jsonify(nation_list)

if __name__ == '__main__':
    with app.app_context():
        db.create_all()  # DB 테이블 생성

        # 초기 데이터 추가
        if not Nation.query.all():
            sample_data = [
                {"continentName": "조선", "continentCode": "KOR"},
                {"continentName": "당", "continentCode": "CHN"},
                {"continentName": "일본", "continentCode": "JPN"},
                {"continentName": "로마", "continentCode": "ROM"},
                {"continentName": "오스만", "continentCode": "OSM"}
            ]

            for data in sample_data:
                nation = Nation(**data)
                db.session.add(nation)

            db.session.commit()

    app.run(port=5001, debug=True)
