using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

//[ExecuteInEditMode]
public class AnimatorUpdater : MonoBehaviour
{
    private Animator _animator;
    private PlayableDirector _director;
    private float _lastTime;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        //_director = GetComponent<PlayableDirector>();
    }

    void Update()
    {
        //var time = Time.realtimeSinceStartup;
        //var deltaTime = _lastTime - time;

        RaycastHit hit;
	    Physics.Raycast(transform.position, -Vector3.up, out hit);
	    _animator.SetFloat("GroundDistance", hit.distance);

	    //_animator.Update(deltaTime);
        //_lastTime = time;
    }


}
