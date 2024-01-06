using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameURL
{
    // DB서버
    public static class DBServer
    {
        //http://127.0.0.1:5001
        //http://15.164.232.189:8080
        public static readonly string Server_URL = "http://15.164.232.189:8080";

        public static readonly string getNationsPath = "/nations";

        public static readonly string getJobsPath = "/jobs";
    }

    // 회원가입, 로그인 등을 담당할 서버
    public static class AuthServer
    {
        //http://127.0.0.1:5000
        //http://15.164.232.189:8080
        public static readonly string Server_URL = "http://15.164.232.189:8080";

        public static readonly string userLogInPath = "/users/login";

        public static readonly string userJoinPath = "/users/join";

        public static readonly string userRegisterPath = "/users/character";
    }
}
