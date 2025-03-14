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
        //https://cebone.shop:443
        public static readonly string Server_URL = "http://cebone.shop";
        public static readonly string PlayURL = "ws://cebone.shop:80/play";

        public static readonly string getNationsPath = "/nations";//나라 리스트 조회
        public static readonly string getJobsPath = "/jobs";//직업 리스트 조회
        public static readonly string getCharacterInfoPath = "/characters";//캐릭터 정보 조회(스탯, 레벨 나라, 직업 등)
        public static readonly string getCharacterSkillInfoPath = "/characters/skills";// 캐릭터 별 스킬 목록 조회
        public static readonly string getShopItemListPath = "/items/{item_type}/{character_level}/{character_job}";// - 아이템 타입 + 레벨 및 직업 별 아이템 리스트 조회(상점)
        public static readonly string getShopBuyPath = "/items/buy";// 아이템 구매
        public static readonly string getInventoryPath = "/items/inven/{item_type}";// 캐릭터 인벤토리 조회
        public static readonly string getinvenEquippedItemPath = "/items/inven/equipped-item/{item_type}"; // 장착중인 아이템 조회
        public static readonly string getItemEquipPath = "/items/equip";// 캐릭터 장비 장착
        public static readonly string getItemUnequipPath = "/items/unequip";// 캐릭터 장비 해제
        public static readonly string getItemSellPath = "/items/sell";// 캐릭터 장비 판매
        public static readonly string getMatchRoomListPath = "/matches/room?";//page=0&size=10";// GET: 매치 방 리스트 조회
        public static readonly string getNewMatchRoomPath = "/matches/room";// POST: 매치 방 생성
        public static readonly string getRoomEnterPath = "/matches/room/enter";// 매치 방 입장

        public static readonly string getGreetingPath = "/greeting/";// 플레이어 방 입장 후 환영 메시지
        public static readonly string getReadyPath = "/ready/";// 준비
        public static readonly string getStartPath = "/start/";// 게임 시작
        public static readonly string getGameTurnPath = "/game/turn/";// 턴 수행(스킬 사용)
        public static readonly string getGameEndPath = "/game/end/";// 게임 종료시
        public static readonly string getGameSurrenderPath = "/game/surrender/";// 항복
        public static readonly string getGameQuitPath = "/game/quit/";// 돌아가기

        public static readonly string getSkillEffectsListPath = "/skills/effects";// 스킬 리스트 불러오기
        public static readonly string putSkillEffectPath = "/skills/effects";// 스킬 효과 수정
        public static readonly string getItemsListPath = "/items"; // 아이템 리스트 불러오기
        public static readonly string putItemPath = "/items"; // 아이템 수정
        public static readonly string getSkills = "/skills"; // 전체 스킬 리스트 조회
    }

    // 회원가입, 로그인 등을 담당할 서버
    public static class AuthServer
    {
        //http://127.0.0.1:5000
        //https://cebone.shop:443
        public static readonly string Server_URL = "http://cebone.shop";

        public static readonly string userLogInPath = "/users/login";// 로그인
        public static readonly string userJoinPath = "/users/join";// 회원가입
        public static readonly string userRegisterPath = "/users/character";// 회원가입 후 캐릭터 생성
    }
}
