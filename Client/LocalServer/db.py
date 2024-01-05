from flask import Flask, jsonify
from flask_sqlalchemy import SQLAlchemy

app = Flask(__name__)
app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///nations.db'
db = SQLAlchemy(app)

class Nation(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    continent_name = db.Column(db.String(50), nullable=False)
    continent_code = db.Column(db.String(10), nullable=False)

class Job(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    job_id = db.Column(db.Integer, nullable=False)
    job_nm = db.Column(db.String(50), nullable=False)
    level_stat_factor = db.Column(db.Float, nullable=False)
    stat_id = db.Column(db.Integer, db.ForeignKey('stat.id'), nullable=False)
    stat = db.relationship('Stat', backref=db.backref('jobs', lazy=True))

class Stat(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    hp = db.Column(db.Integer, nullable=False)
    atk = db.Column(db.Integer, nullable=False)
    mp = db.Column(db.Integer, nullable=False)
    speed = db.Column(db.Integer, nullable=False)

# 데이터베이스 초기화 함수
def initialize_database():
    with app.app_context():
        db.create_all()

    with app.app_context():
        # 기존 데이터 삭제
        db.session.query(Nation).delete()
        db.session.query(Job).delete()
        db.session.query(Stat).delete()

        # 새로운 데이터 추가
        sample_nation_data = [
            {"continent_name": "조선", "continent_code": "KOR"},
            {"continent_name": "당", "continent_code": "CHN"},
            {"continent_name": "일본", "continent_code": "JPN"},
            {"continent_name": "로마", "continent_code": "ROM"},
            {"continent_name": "오스만", "continent_code": "OSM"}
        ]

        for data in sample_nation_data:
            nation = Nation(continent_name=data["continent_name"], continent_code=data["continent_code"])
            db.session.add(nation)

        sample_job_data = [
            {"job_id": 1, "job_nm": "Warrior", "level_stat_factor": 1.5, "stat": {"hp": 100, "atk": 20, "mp": 10, "speed": 5}},
            {"job_id": 2, "job_nm": "Archer", "level_stat_factor": 1.8, "stat": {"hp": 80, "atk": 25, "mp": 15, "speed": 8}},
            {"job_id": 3, "job_nm": "Mage", "level_stat_factor": 2.0, "stat": {"hp": 60, "atk": 15, "mp": 30, "speed": 7}},
         # 추가적인 직업 데이터들...
        ]

        for data in sample_job_data:
            stat_data = data.pop("stat")
            stat = Stat(**stat_data)
            db.session.add(stat)

            job = Job(stat=stat, **data)
            db.session.add(job)

        db.session.commit()

# /nations 엔드포인트
@app.route('/nations', methods=['GET'])
def get_nations():
    nations = Nation.query.all()
    nation_list = [
        {"continentName": nation.continent_name, "continentCode": nation.continent_code}
        for nation in nations
    ]
    return jsonify(nation_list)

# /jobs 엔드포인트
@app.route('/jobs', methods=['GET'])
def get_jobs():
    jobs = Job.query.all()
    job_list = [
        {
            "jobId": job.job_id,
            "jobNm": job.job_nm,
            "levelStatFactor": job.level_stat_factor,
            "stat": {
                "hp": job.stat.hp,
                "atk": job.stat.atk,
                "mp": job.stat.mp,
                "speed": job.stat.speed
            }
        }
        for job in jobs
    ]
    return jsonify(job_list)

if __name__ == '__main__':
    initialize_database()
    app.run(port=5001, debug=True)
