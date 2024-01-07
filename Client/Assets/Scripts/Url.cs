using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameURL
{
    // DB����
    public static class DBServer
    {
        //http://127.0.0.1:5001
        //http://15.164.232.189:8080
        public static readonly string Server_URL = "http://15.164.232.189:8080";

        public static readonly string getNationsPath = "/nations";//���� ����Ʈ ��ȸ
        public static readonly string getJobsPath = "/jobs";//���� ����Ʈ ��ȸ
        public static readonly string getCharacterInfoPath = "/characters";//ĳ���� ���� ��ȸ(����, ���� ����, ���� ��)
        public static readonly string getCharacterSkillInfoPath = "/characters/skills";// ĳ���� �� ��ų ��� ��ȸ
        public static readonly string getShopItemListPath = "/items/{item_type}/{character_level}/{character_job}";// - ������ Ÿ�� + ���� �� ���� �� ������ ����Ʈ ��ȸ(����)
    }

    // ȸ������, �α��� ���� ����� ����
    public static class AuthServer
    {
        //http://127.0.0.1:5000
        //http://15.164.232.189:8080
        public static readonly string Server_URL = "http://15.164.232.189:8080";

        public static readonly string userLogInPath = "/users/login";// �α���
        public static readonly string userJoinPath = "/users/join";// ȸ������
        public static readonly string userRegisterPath = "/users/character";// ȸ������ �� ĳ���� ����
    }
}
