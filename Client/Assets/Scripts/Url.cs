using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameURL
{
    // DB����
    public static class DBServer
    {
        public static readonly string Server_URL = "http://127.0.0.1:5001";

        public static readonly string getNationsPath = "/nations";

        public static readonly string getJobsPath = "/jobs";
    }

    // ȸ������, �α��� ���� ����� ����
    public static class AuthServer
    {
        public static readonly string Server_URL = "http://127.0.0.1:5000";

        public static readonly string userLogInPath = "/users/login";

        public static readonly string userJoinPath = "/users/join";
    }
}
