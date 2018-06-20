using System.Reflection;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.AI;

[TrackColor(0.92f, 0.16f, 0.22f)]
[TrackClipType(typeof(ConditionalEventClip))]
[TrackBindingType(typeof(Animator))]
public class ConditionalEventTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var playable = ScriptPlayable<ConditionalEventMixerBehaviour>.Create(graph, inputCount);
        InitializeClips(go);
        return playable;
    }

    private void InitializeClips(GameObject go)
    {
        var director = go.GetComponent<PlayableDirector>();
        var trackTargetObject = director.GetGenericBinding(this) as Animator;

        foreach (var clip in GetClips())
        {
            var playableAsset = clip.asset as ConditionalEventClip;
            if (playableAsset)
            {
                playableAsset.Director = director;
                playableAsset.BoundAnimator = trackTargetObject;
                playableAsset.Clip = clip;
            }
        }
    }
}

