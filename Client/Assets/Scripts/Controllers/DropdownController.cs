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
        // Dropdown�� ���� ����Ʈ �߰�
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (NationGetListResponse nation in nationList)
        {
            options.Add(new TMP_Dropdown.OptionData($"{nation.nationNm} {nation.nationId}"));
        }

        dropdown.AddOptions(options);

        // Dropdown�� OnValueChanged �̺�Ʈ�� �޼��� ����
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // �ʱ� ���� �� ����
        OnDropdownValueChanged(dropdown.value);
    }

    void OnDropdownValueChanged(int value)
    {
        // ���õ� ���� �ش��ϴ� �ؽ�Ʈ ������Ʈ
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
