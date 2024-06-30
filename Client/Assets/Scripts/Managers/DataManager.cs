using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static DataManager _instance;

    private List<SkillGetListResponse> _skillGetListResponse = new List<SkillGetListResponse>();
	private List<ItemGetResponse> _itemGetResponse = new List<ItemGetResponse>();

	public static DataManager Instance
    {
		get
        {
			if(_instance == null)
            {
				GameObject dataManagerObject = new GameObject("DataManager");
				_instance = dataManagerObject.AddComponent<DataManager>();
				DontDestroyOnLoad(dataManagerObject);
			}
			return _instance;
        }
    }

	public List<SkillGetListResponse> SkillGetListResponse
    {
		get { return _skillGetListResponse; }
		set { _skillGetListResponse = value; }
    }

	public List<ItemGetResponse> itemGetResponse
    {
		get { return _itemGetResponse; }
		set { _itemGetResponse = value; }
	}
}

public class SkillGetListResponse
{
	public long skillId;
	public string skillNm;
	public long skillEffectId;
	public SkillEffectType skillEffectType; // DAMAGE, HEAL
	public int fixedValue;
	public int hpRate;
	public int atkRate;
	public int mpRate;
	public int spdRate;
}

public enum SkillEffectType
{
	DAMAGE,
	HEAL
}

public class ItemGetResponse
{
	public long itemId;
	public long levelId;
	public long jobId;
	public long itemTypeId;
	public string itemNm;
	public int cost;
	public Stat stat;
}

public class SkillPost
{
	public long skillId;
	public long skillEffectId;
	public int fixedValue;
	public int hpRate;
	public int atkRate;
	public int mpRate;
	public int spdRate;
}

public class ItemPost
{
	public long itemId;
	public int cost;
	public Stat stat;
}