using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntegerRegister
{
    public int value;
    private int permanentAlteration;
    private int temporaryAlteration;
    private int minValue;
    public IntegerRegister(int value)
    {
        this.value = value;
        this.minValue = int.MinValue;
    }
    public int effectiveValue()
    {
        return Mathf.Max(minValue, value + permanentAlteration + temporaryAlteration);
    }
    public void changePermanently(int amount)
    {
        permanentAlteration += amount;
    }
    public void changeTemporarily(int amount)
    {
        temporaryAlteration += amount;
    }
    public void setMinValue(int min)
    {
        minValue = min;
    }
}
