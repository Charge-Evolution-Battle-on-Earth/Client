using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static DataManager _instance;

    private List<SkillGetListResponse> _skillGetListResponse = new List<SkillGetListResponse>();
	private List<SkillGetListResponse> _filteredSkills = new List<SkillGetListResponse>();
	private List<ItemGetResponse> _itemGetResponse = new List<ItemGetResponse>();
	private List<ItemGetResponse> _filteredItems = new List<ItemGetResponse>();
	private string _sortStatus;

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

	public List<SkillGetListResponse> FiltererdSkills
	{
		get { return _filteredSkills; }
		set { _filteredSkills = value; }
	}

	public List<ItemGetResponse> FiltererdItems
	{
		get { return _filteredItems; }
		set { _filteredItems = value; }
	}

	public string SortStatus
    {
        get { return _sortStatus; }
		set { _sortStatus = value; }
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

[System.Serializable]
public class SkillGetListResponse
{
	public long skillId;
	public string skillNm;
	public long skillEffectId;
	public SkillEffectType skillEffectType; // DAMAGE, HEAL
	public int fixedValue;
	public StatRate statRate = new StatRate();
}

[System.Serializable]
public enum SkillEffectType
{
	DAMAGE,
	HEAL
}

[System.Serializable]
public class ItemGetResponse
{
	public long itemId;
	public long levelId;
	public long jobId;
	public long itemTypeId;
	public string itemNm;
	public int cost;
	public Stat stat = new Stat();
}

public class SkillPost
{
	public long skillEffectId;
	public int fixedValue;
	public StatRate statRate = new StatRate();
}

public class ItemPost
{
	public long itemId;
	public int cost;
	public Stat stat = new Stat();
}

[System.Serializable]
public class StatRate
{
	public int hpRate;
	public int atkRate;
	public int mpRate;
	public int spdRate;
}