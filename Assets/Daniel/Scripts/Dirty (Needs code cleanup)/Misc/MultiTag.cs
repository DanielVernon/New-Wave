using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class MultiTag : MonoBehaviour
{
    
    [Flags] public enum Tags
    {
        ReflectAlongX,
        Other1,
        other2,
    }
    [EnumFlag]
    public Tags tags = 0;

    public void AddTag(Tags T)
    {
        tags |= T;
    }

    public void SetTags(Tags T)
    {
        tags = T;
    }

    public bool CheckTag(Tags T)
    {
        Tags checkTag = T & tags;
        if (checkTag == T)
            return true;
        else
            return false;
    }

    public void ToggleTag(Tags T)
    {
        tags ^= T;
    }

    //private void Update()
    //{
    //    if (CheckTag(Tags.ReflectAlongX) && CheckTag(Tags.Other1))
    //    {
    //        Debug.Log("Yes" + Random.Range(0, 99));
    //    }
    //}


    //public static T AddEnums<T>(T enum1, T enum2)
    //{
        
    //    return (enum1 | enum2);
    //}
    /// <summary>
    /// Checks if enum a contains b, cast enums to int to get this to work
    /// </summary>
    /// <returns></returns>
    public static bool CheckEnum(Enum enum1, Enum enum2)
    {
        ulong enum1Val = Convert.ToUInt64(enum1);
        ulong enum2Val = Convert.ToUInt64(enum2);
        return enum2Val == (enum1Val & enum2Val);
    }
}