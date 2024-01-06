using TMPro;
using UnityEngine;
using System;
using System.Collections.Generic;

public class DropdownController : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public TextMeshProUGUI selectedValueText;
    private List<NationGetListResponse> nation;
    public void AddItemsToDropdown(List<NationGetListResponse> nationList)
    {
        nation = nationList;
        dropdown.ClearOptions();
        // Dropdown에 나라 리스트 추가
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (NationGetListResponse nation in nationList)
        {
            options.Add(new TMP_Dropdown.OptionData($"{nation.nationNm} {nation.nationId}"));
        }

        dropdown.AddOptions(options);

        // Dropdown의 OnValueChanged 이벤트에 메서드 연결
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // 초기 선택 값 설정
        OnDropdownValueChanged(dropdown.value);
    }

    void OnDropdownValueChanged(int value)
    {
        // 선택된 값에 해당하는 텍스트 업데이트
        string selectedValue = dropdown.options[value].text;
        selectedValueText.text = selectedValue;
        UserDataManager.Instance.NationId = Convert.ToInt64(nation[value].nationId);
    }
}

[System.Serializable]
public class NationGetListResponse
{
    public string nationNm;
    public long nationId;
}
