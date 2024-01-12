using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameURL
{
    // DB����
    public static class DBServer
    {
        //http://127.0.0.1:5001
        //https://cebone.shop:443
        public static readonly string Server_URL = "https://cebone.shop:443";
        public static readonly string Server_WebSocketURL = "wss://cebone.shop:443";

        public static readonly string getNationsPath = "/nations";//���� ����Ʈ ��ȸ
        public static readonly string getJobsPath = "/jobs";//���� ����Ʈ ��ȸ
        public static readonly string getCharacterInfoPath = "/characters";//ĳ���� ���� ��ȸ(����, ���� ����, ���� ��)
        public static readonly string getCharacterSkillInfoPath = "/characters/skills";// ĳ���� �� ��ų ��� ��ȸ
        public static readonly string getShopItemListPath = "/items/{item_type}/{character_level}/{character_job}";// - ������ Ÿ�� + ���� �� ���� �� ������ ����Ʈ ��ȸ(����)
        public static readonly string getShopBuyPath = "/items/buy";// ������ ����
        public static readonly string getInventoryPath = "/items/inven/{item_type}";// ĳ���� �κ��丮 ��ȸ
        public static readonly string getItemEquipPath = "/items/equip";// ĳ���� ��� ����
        public static readonly string getItemUnequipPath = "/items/unequip";// ĳ���� ��� ����
        public static readonly string getItemSellPath = "/items/sell";// ĳ���� ��� �Ǹ�
        public static readonly string getMatchRoomListPath = "/matches/room?";//page=0&size=10";// GET: ��ġ �� ����Ʈ ��ȸ
        public static readonly string getNewMatchRoomPath = "/matches/room";// POST: ��ġ �� ����
        public static readonly string getRoomEnterPath = "/matches/room/enter";// ��ġ �� ����

        public static readonly string getGreetingPath = "/play/greeting/{matchId}";// �÷��̾� �� ���� �� ȯ�� �޽���
        public static readonly string getReadyPath = "/play/ready/{matchId}";// �غ�
        public static readonly string getStartPath = "/play/start/{matchId}";// ���� ����
        public static readonly string getGameTurnPath = "/play/game/turn/{matchId}";// �� ����(��ų ���)
        public static readonly string getGameEndPath = "/play/game/end/{matchId}";// ���� �����
        public static readonly string getGameSurrenderPath = "/play/game/surrender/{matchId}";// �׺�
        public static readonly string getGameQuitPath = "/play/game/quit/{matchId}";// ���ư���
    }

    // ȸ������, �α��� ���� ����� ����
    public static class AuthServer
    {
        //http://127.0.0.1:5000
        //https://cebone.shop:443
        public static readonly string Server_URL = "https://cebone.shop:443";

        public static readonly string userLogInPath = "/users/login";// �α���
        public static readonly string userJoinPath = "/users/join";// ȸ������
        public static readonly string userRegisterPath = "/users/character";// ȸ������ �� ĳ���� ����
    }
}
