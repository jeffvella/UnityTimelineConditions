using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SomeEventHandler : MonoBehaviour, ITimelineEventHandler
{
    public void OnStart(ConditionalEventData data)
    {
        Debug.Log($"{data.Animator.transform.parent.name}.{data.TimelineClipName} Finished!");
    }

    public void OnStop(ConditionalEventData data)
    {
        Debug.Log($"{data.Animator.transform.parent.name}.{data.TimelineClipName} Started!");
    }

    public void OnConditionSuccess(ConditionalEventData data)
    {
        //var asset = data.Playable as ConditionalEventClip;
        
        Debug.Log($"{data.Animator.transform.parent.name}.{data.TimelineClipName} Condition Success!");
    }
}