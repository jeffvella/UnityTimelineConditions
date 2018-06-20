using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class AnimationCondition
{
    public string Parameter;
    public ConditionMode ConditionMode;
    public float Threshold;
}

[Serializable]
public class ConditionEvaluator
{
    public List<AnimationCondition> Items = new List<AnimationCondition>();

    public ParameterCollection Parameters { get; set; }

    public void Initialize(Animator animator)
    {
        if ((Application.isEditor && !Application.isPlaying || !IsInitialized) && (animator.isInitialized || animator.hasBoundPlayables))
        {
            Parameters = new ParameterCollection(animator.parameters);
        }
    }

    public bool IsInitialized => Parameters != null;

    public bool Evaluate(Animator animator)
    {
        if (animator == null)
        {
            return false;
        };
        
        if (!IsInitialized)
        {
            Initialize(animator);
        }

        if (Parameters == null || Parameters.Items.Length == 0)
        {
            return false;
        }

        foreach (var condition in Items)
        {
            var p = Parameters[condition.Parameter];            
            switch (p?.type)
            {
                case AnimatorControllerParameterType.Bool:
                    if (!animator.GetBool(p.nameHash))
                        return false;
                    break;
                case AnimatorControllerParameterType.Float:
                    switch (condition.ConditionMode)
                    {
                        case ConditionMode.Greater:
                            if (animator.GetFloat(p.nameHash) <= condition.Threshold)
                                return false;
                            break;
                        case ConditionMode.Less:
                            if (animator.GetFloat(p.nameHash) >= condition.Threshold)
                                return false;
                            break;
                    }
                    break;
                case AnimatorControllerParameterType.Int:
                    switch (condition.ConditionMode)
                    {
                        case ConditionMode.Greater:
                            if (condition.Threshold <= animator.GetInteger(p.nameHash))
                                return false;
                            break;
                        case ConditionMode.Less:
                            if (condition.Threshold >= animator.GetInteger(p.nameHash))
                                return false;
                            break;
                        case ConditionMode.Equals:
                            if ((int)condition.Threshold != animator.GetInteger(p.nameHash))
                                return false;
                            break;
                        case ConditionMode.NotEqual:
                            if ((int)condition.Threshold == animator.GetInteger(p.nameHash))
                                return false;
                            break;
                    }
                    break;
                case AnimatorControllerParameterType.Trigger:
                    if (animator.GetInteger(p.nameHash) != 1)
                        return false;
                    break;
                case null:
                    Debug.LogWarning($"Parameter doesnt exist {condition.Parameter} in Animator {animator.name}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        return true;
    }


}

public class ParameterCollection
{
    public ParameterCollection(AnimatorControllerParameter[] paramsArray)
    {
        _paramsDict = paramsArray.ToDictionary(k => k.name, v => v);
        Items = paramsArray;
    }

    private Dictionary<string, AnimatorControllerParameter> _paramsDict;

    public AnimatorControllerParameter[] Items { get; set; }

    public AnimatorControllerParameter this[string s] => _paramsDict.ContainsKey(s) ? _paramsDict[s] : null;

    public AnimatorControllerParameter this[int i] => Items[i];

    public int IndexOfParameter(string name)
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i].name == name)
                return i;
        }
        return -1;
    }
}

public enum ConditionMode
{
    If = 1,
    IfNot = 2,
    Greater = 3,
    Less = 4,
    Equals = 6,
    NotEqual = 7,
}