using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class ConditionalEventClip : PlayableAsset, ITimelineClipAsset
{
    [HideInInspector]
    public ConditionalEventBehaviour template = new ConditionalEventBehaviour();

    public ConditionEvaluator Conditions = new ConditionEvaluator();

    public ExposedReference<GameObject> EventReceiver;

    public ConditionalEventTriggerSettings TriggerSettings = new ConditionalEventTriggerSettings();

    public PlayableDirector Director { get; set; }

    public TimelineClip Clip { get; set; }

    public ClipCaps clipCaps => ClipCaps.None;

    private Animator _boundAnimator;
    public Animator BoundAnimator
    {
        get { return _boundAnimator; }
        set
        {
            _boundAnimator = value;
            OnAnimatorAssigned();
        }
    }

    private void OnAnimatorAssigned()
    {
        // There is some funky behavior with when when the parameters are available in various edit/play modes
        // Animator is not yet initialized here (and so its parameters collection is empty) when in play mode.

        if (_boundAnimator == null)
        {
            Conditions.Parameters = null;
        }
        else
        {
            Conditions.Initialize(_boundAnimator);
        }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        if (BoundAnimator == null)
            return Playable.Null;

        if (Application.isEditor)
        {
            Conditions.Initialize(BoundAnimator);
        }

        var playable = ScriptPlayable<ConditionalEventBehaviour>.Create (graph, template);
        ConditionalEventBehaviour clone = playable.GetBehaviour();   
        clone.Target = EventReceiver.Resolve(graph.GetResolver());
        clone.BoundAnimator = BoundAnimator;
        clone.Director = Director;
        clone.Conditions = Conditions;
        clone.TriggerSettings = TriggerSettings;
        clone.ClipName = Clip.displayName;     
                
        return playable;
    }


}