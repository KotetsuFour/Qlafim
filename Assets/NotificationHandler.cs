using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class NotificationHandler : MonoBehaviour
{
    public int[] allowTriggers;
    public string[] allowCode;

    public int[] reactTriggers;
    public string[] reactCode;

    public int codeIdx;

    public void allowNotification(GameNotification note)
    {
        try
        {
            for (codeIdx = allowTriggers[(int)note.nature]; codeIdx < allowTriggers.Length && allowCode[codeIdx] != "DONE"; codeIdx++)
            {
                processLine(allowCode);
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error at line {codeIdx}: {ex.Message}");
        }
    }
    public void react(GameNotification note)
    {
        try
        {
            for (codeIdx = reactTriggers[(int)note.nature]; codeIdx < reactTriggers.Length && reactCode[codeIdx] != "DONE"; codeIdx++)
            {
                processLine(allowCode);
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error at line {codeIdx}: {ex.Message}");
        }
    }
    public abstract void processLine(string[] codeList);
}
