using System;
using UnityEngine;

public class TimeDispatcher : MonoBehaviour
{
    public delegate void OnTime(DateTime dateTime);

    private DateTime? _lastDateTime;

    public OnTime OnSecond { get; set; } = null;
    public OnTime OnMinute { get; set; } = null;

    private async void Update()
    {
        var now = Clock.GetInstance().Now();
        if ((_lastDateTime == null || _lastDateTime?.Minute != now.Minute) && OnMinute != null) OnMinute(now);

        if ((_lastDateTime == null || _lastDateTime?.Second != now.Second) && OnSecond != null) OnSecond(now);

        _lastDateTime = now;
    }
}