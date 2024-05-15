using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FontChanger : MonoBehaviour
{
    [SerializeField]
    public TMP_FontAsset FontAsset;
}

#if UNITY_EDITOR
[CustomEditor(typeof(FontChanger))]
public class TMP_FontChangerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Change Font!"))
        {
            TMP_FontAsset fontAsset = ((FontChanger)target).FontAsset;

            foreach (TextMeshPro textMeshPro3D in GameObject.FindObjectsOfType<TextMeshPro>(true))
            {
                textMeshPro3D.font = fontAsset;
            }
            foreach (TextMeshProUGUI textMeshProUi in GameObject.FindObjectsOfType<TextMeshProUGUI>(true))
            {
                textMeshProUi.font = fontAsset;
            }
        }
    }
}
#endif
