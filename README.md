# UnityTimelineConditions
Experimental port of Animator conditions interface to Timeline events.

 * The track binds to an animator and uses it to populate all the current Parameters.
 * Each clip can bind to an object to receive events via ExecuteEvents/Interface.
 * User can configure conditions just like for mechanim transitions.
 * Conditions can be checked at Start/End of clip or every frame of its duration.
