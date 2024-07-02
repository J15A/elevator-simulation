using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeInputField : MonoBehaviour
{
    public TMP_InputField[] inputFields;

    public int InputSelected = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            InputSelected++;
            if (InputSelected > inputFields.Length - 1) InputSelected = 0;
            SelectInputField();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            InputSelected--;
            if (InputSelected < 0) InputSelected = inputFields.Length - 1;
            SelectInputField();
        }

        void SelectInputField()
        {
            inputFields[InputSelected].Select();
        }
    }
    public void inputField0Selected() => InputSelected = 0;
    public void inputField1Selected() => InputSelected = 1;
    public void inputField2Selected() => InputSelected = 2;
    public void inputField3Selected() => InputSelected = 3;
    public void inputField4Selected() => InputSelected = 4;
    public void inputField5Selected() => InputSelected = 5;
}
