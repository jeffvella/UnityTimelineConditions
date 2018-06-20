using System;
using System.Diagnostics.Tracing;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class ConditionalEventBehaviour : PlayableBehaviour
{
    public string ClipName { get; set; }
    public bool IsPlaying { get; set; }
    public bool IsTriggered { get; set; }

    public Animator BoundAnimator { get; set; }
    public PlayableDirector Director { get; set; }
    public ConditionEvaluator Conditions { get; set; }
    public GameObject Target { get; set; }
    public ConditionalEventTriggerSettings TriggerSettings { get; set; }

    public override void OnBehaviourPlay(Playable playable, FrameData frameData)
    {
        if (Director.state == PlayState.Playing)
        {
            // OnBehaviourPause is called while editing the conditions input fields
            // So we have to determine if its actually being played.
            IsPlaying = true;
        }

        if (TriggerSettings.ResetTriggerOnStart && IsTriggered)
        {
            IsTriggered = false;
        }
        
        if (TriggerSettings.FireStartEvent)
        {
            ExecuteEvents.Execute<ITimelineEventHandler>(Target ?? BoundAnimator.gameObject, null, (i, b)
                => i.OnStart(CreateEventArgs(playable, frameData)));

            CheckCondition(playable, frameData);
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData frameData)
    {
        if (!IsPlaying)
        {
            return;
        }        

        if (TriggerSettings.FireEndEvent)
        {          
            ExecuteEvents.Execute<ITimelineEventHandler>(Target ?? BoundAnimator.gameObject, null, (i, b)
                => i.OnStop(CreateEventArgs(playable, frameData)));

            CheckCondition(playable, frameData);
        }

        IsPlaying = false;
    }

    public override void ProcessFrame(Playable playable, FrameData frameData, object playerData)
    { 
        if (!IsPlaying)
        {
            return;
        }     

        if (TriggerSettings.CheckConditionEveryFrame)
        {
            CheckCondition(playable, frameData);
        }
    }

    private bool CheckCondition(Playable playable, FrameData frameData)
    {
        if (!IsTriggered && Conditions.Evaluate(BoundAnimator))
        {
            IsTriggered = true;
            ExecuteEvents.Execute<ITimelineEventHandler>(Target ?? BoundAnimator.gameObject, null, (i, b) 
                => i.OnConditionSuccess(CreateEventArgs(playable, frameData)));
            return true;
        }

        return false;
    }

    public ConditionalEventData CreateEventArgs(Playable playable, FrameData frameData)
    {
        return new ConditionalEventData
        {
            TimelineClipName = ClipName,
            Director = Director,
            Animator = BoundAnimator,
            Data = frameData,
            Playable = playable,
        };
    }
}