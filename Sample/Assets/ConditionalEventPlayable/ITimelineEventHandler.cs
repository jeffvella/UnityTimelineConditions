using UnityEngine.EventSystems;

public interface ITimelineEventHandler : IEventSystemHandler
{
    void OnStart(ConditionalEventData data);
    void OnStop(ConditionalEventData data);
    void OnConditionSuccess(ConditionalEventData data);
}