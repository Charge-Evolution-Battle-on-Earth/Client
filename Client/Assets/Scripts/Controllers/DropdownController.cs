using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class DropdownController : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public TextMeshProUGUI selectedValueText;

    public void AddItemsToDropdown(List<NationGetListResponse> nationList)
    {
        dropdown.ClearOptions();
        // Dropdown에 나라 리스트 추가
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (NationGetListResponse nation in nationList)
        {
            options.Add(new TMP_Dropdown.OptionData($"{nation.continentName} ({nation.continentCode})"));
        }

        dropdown.AddOptions(options);

        // Dropdown의 OnValueChanged 이벤트에 메서드 연결
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // 초기 선택 값 설정
        OnDropdownValueChanged(dropdown.value);
    }

    void OnDropdownValueChanged(int value)
    {
        UserDataManager.Instance.SelectedNation = value;
        // 선택된 값에 해당하는 텍스트 업데이트
        string selectedValue = dropdown.options[value].text;
        selectedValueText.text = selectedValue;
    }
}

[System.Serializable]
public class NationGetListResponse
{
    public string continentName;
    public string continentCode;
}
