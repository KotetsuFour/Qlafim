using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputRestricter : MonoBehaviour
{
    [SerializeField] private int maxCharacters;
    private TMP_InputField field;
    private void Start()
    {
        field = GetComponent<TMP_InputField>();
    }
    public void keepValid()
    {
        if (field.text.Length > maxCharacters)
        {
            field.text = field.text.Substring(0, maxCharacters);
        }
    }

    public bool isValid()
    {
        return field.text != null && field.text.Trim() != "";
    }
}
