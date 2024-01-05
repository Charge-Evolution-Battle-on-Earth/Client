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
        // Dropdown�� ���� ����Ʈ �߰�
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (NationGetListResponse nation in nationList)
        {
            options.Add(new TMP_Dropdown.OptionData($"{nation.continentName} ({nation.continentCode})"));
        }

        dropdown.AddOptions(options);

        // Dropdown�� OnValueChanged �̺�Ʈ�� �޼��� ����
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // �ʱ� ���� �� ����
        OnDropdownValueChanged(dropdown.value);
    }

    void OnDropdownValueChanged(int value)
    {
        UserDataManager.Instance.SelectedNation = value;
        // ���õ� ���� �ش��ϴ� �ؽ�Ʈ ������Ʈ
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
