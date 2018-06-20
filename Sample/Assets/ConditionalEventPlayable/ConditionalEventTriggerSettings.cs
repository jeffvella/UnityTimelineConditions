using System;

[Serializable]
public class ConditionalEventTriggerSettings
{
    public bool FireStartEvent = true;
    public bool FireEndEvent = true;
    public bool CheckConditionEveryFrame = true;
    public bool ResetTriggerOnStart = true;
}
