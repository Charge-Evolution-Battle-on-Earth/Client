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
        //https://cebone.shop:443
        public static readonly string Server_URL = "http://cebone.shop";
        public static readonly string PlayURL = "ws://cebone.shop:80/play";

        public static readonly string getNationsPath = "/nations";//���� ����Ʈ ��ȸ
        public static readonly string getJobsPath = "/jobs";//���� ����Ʈ ��ȸ
        public static readonly string getCharacterInfoPath = "/characters";//ĳ���� ���� ��ȸ(����, ���� ����, ���� ��)
        public static readonly string getCharacterSkillInfoPath = "/characters/skills";// ĳ���� �� ��ų ��� ��ȸ
        public static readonly string getShopItemListPath = "/items/{item_type}/{character_level}/{character_job}";// - ������ Ÿ�� + ���� �� ���� �� ������ ����Ʈ ��ȸ(����)
        public static readonly string getShopBuyPath = "/items/buy";// ������ ����
        public static readonly string getInventoryPath = "/items/inven/{item_type}";// ĳ���� �κ��丮 ��ȸ
        public static readonly string getinvenEquippedItemPath = "/items/inven/equipped-item/{item_type}"; // �������� ������ ��ȸ
        public static readonly string getItemEquipPath = "/items/equip";// ĳ���� ��� ����
        public static readonly string getItemUnequipPath = "/items/unequip";// ĳ���� ��� ����
        public static readonly string getItemSellPath = "/items/sell";// ĳ���� ��� �Ǹ�
        public static readonly string getMatchRoomListPath = "/matches/room?";//page=0&size=10";// GET: ��ġ �� ����Ʈ ��ȸ
        public static readonly string getNewMatchRoomPath = "/matches/room";// POST: ��ġ �� ����
        public static readonly string getRoomEnterPath = "/matches/room/enter";// ��ġ �� ����

        public static readonly string getGreetingPath = "/greeting/";// �÷��̾� �� ���� �� ȯ�� �޽���
        public static readonly string getReadyPath = "/ready/";// �غ�
        public static readonly string getStartPath = "/start/";// ���� ����
        public static readonly string getGameTurnPath = "/game/turn/";// �� ����(��ų ���)
        public static readonly string getGameEndPath = "/game/end/";// ���� �����
        public static readonly string getGameSurrenderPath = "/game/surrender/";// �׺�
        public static readonly string getGameQuitPath = "/game/quit/";// ���ư���
    }

    // ȸ������, �α��� ���� ����� ����
    public static class AuthServer
    {
        //http://127.0.0.1:5000
        //https://cebone.shop:443
        public static readonly string Server_URL = "http://cebone.shop";

        public static readonly string userLogInPath = "/users/login";// �α���
        public static readonly string userJoinPath = "/users/join";// ȸ������
        public static readonly string userRegisterPath = "/users/character";// ȸ������ �� ĳ���� ����
    }
}
