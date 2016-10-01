using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
using Expanse;

public class VisLoggerController : Singleton<VisLoggerController>
{
    public VisLoggerPanel visLoggerPanel;
    public VisLoggerLog VisLoggerLogPrefab;

    int logCount;

	public void Awake()
    {
        VisLogger.OnAddLog(OnAddLog);
        VisLogger.OnClearLogs(OnClearLogs);
    }

    private void OnClearLogs()
    {
        visLoggerPanel.logParent.DestroyAllChildren();
    }

    private void OnAddLog(VisLog log)
    {
        VisLoggerLog newLogObject = Instantiate(VisLoggerLogPrefab);

        newLogObject.SetData(log);
        newLogObject.name = "LOG " + ++logCount;
        newLogObject.transform.SetParent(visLoggerPanel.logParent);
        newLogObject.transform.Reset();

        visLoggerPanel.scrollRect.verticalNormalizedPosition = 0;
    }
}
