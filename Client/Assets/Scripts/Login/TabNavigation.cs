using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabNavigation : MonoBehaviour
{
    public TMP_InputField[] inputFields;
    public Button loginButton;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                SelectPreviousInputField();
            }
            else
            {
                SelectNextInputField();
            }
        }
        if(Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            TMP_InputField currentInputField = GetCurrentSelectedInputField();
            if (currentInputField != null)
            {
                int currentIndex = System.Array.IndexOf(inputFields, currentInputField);

                if (currentIndex == 0)
                {
                    int nextIndex = (currentIndex + 1) % inputFields.Length;

                    inputFields[nextIndex].Select();
                    inputFields[nextIndex].ActivateInputField();
                }
                else
                {
                    loginButton.onClick.Invoke();
                }
            }
        }
    }

    void SelectNextInputField()
    {
        TMP_InputField currentInputField = GetCurrentSelectedInputField();

        if (currentInputField != null)
        {
            int currentIndex = System.Array.IndexOf(inputFields, currentInputField);
            int nextIndex = (currentIndex + 1) % inputFields.Length;

            inputFields[nextIndex].Select();
        }
    }

    void SelectPreviousInputField()
    {
        TMP_InputField currentInputField = GetCurrentSelectedInputField();

        if (currentInputField != null)
        {
            int currentIndex = System.Array.IndexOf(inputFields, currentInputField);
            int previousIndex = (currentIndex - 1 + inputFields.Length) % inputFields.Length;

            inputFields[previousIndex].Select();
        }
    }

    TMP_InputField GetCurrentSelectedInputField()
    {
        for (int i = 0; i < inputFields.Length; i++)
        {
            if (inputFields[i].isFocused)
            {
                return inputFields[i];
            }
        }

        return null;
    }
}
