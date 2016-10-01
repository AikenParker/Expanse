using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Expanse;
using TMPro;

public class VisLoggerPanel : MonoBehaviour
{
    public RectTransform logParent;
    public ScrollRect scrollRect;

    public void OnClearPressed()
    {
        VisLogger.ClearLogs();
    }
}
