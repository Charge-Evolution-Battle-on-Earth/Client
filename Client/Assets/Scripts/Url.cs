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
        public static readonly string getMatchRoomListPath = "/matches/room";// GET: ��ġ �� ����Ʈ ��ȸ, POST: ��ġ �� ����
        public static readonly string getRoomEnterPath = "/matches/room/enter";// ��ġ �� ����

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
