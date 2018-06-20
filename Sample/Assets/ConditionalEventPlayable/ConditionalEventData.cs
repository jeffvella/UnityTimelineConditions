using UnityEngine;
using UnityEngine.Playables;

public class ConditionalEventData
{
    public string TimelineClipName { get; set; }
    public Animator Animator { get; set; }
    public PlayableDirector Director { get; set; }
    public Playable Playable { get; set; }
    public FrameData Data { get; set; }
}