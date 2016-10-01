using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Expanse;
using TMPro;

public class VisLoggerLog : MonoBehaviour
{
    public Image panel;
    public TextMeshProUGUI logText;
    public TextMeshProUGUI groupText;
    public TextMeshProUGUI timeText;

    public void SetData(VisLog log)
    {
        LogColor = log.color;
        LogText = log.message;
        GroupText = log.group.ToString();
        TimeText = log.dateTime.ToLongTimeString();
    }

    public Color LogColor
    {
        set
        {
            Color newColor = value;
            newColor.a = panel.color.a;
            panel.color = newColor;
        }
    }

    public string LogText
    {
        set
        {
            logText.SetText(value);
        }
    }

    public string GroupText
    {
        set
        {
            groupText.SetText(value);
        }
    }

    public string TimeText
    {
        set
        {
            timeText.SetText(value);
        }
    }
}
