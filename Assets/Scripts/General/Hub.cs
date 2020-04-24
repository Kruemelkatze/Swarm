﻿using System;
using System.Collections;
using System.Collections.Generic;
 using Extensions;
 using UnityEngine;

/// <summary>
/// This class can be used to hold objects that should be commonly available.
/// While Singletons can be achieved to do the same, this approach is more flexible on the long run.
/// </summary>
public class Hub : PersistentSingleton<Hub>
{
    [ReadOnly] public Dictionary<Type, object> registrations = new Dictionary<Type, object>();

    void Awake()
    {
        if (!InitSingletonInstance())
            return;
        
        // Register Stuff here via 
            Register<AudioController>(AudioController.Instance);
        // Or in the Awake() functions in the respective services
    }

    public static T Get<T>()
    {
        return (T)Instance.registrations[typeof(T)];
    }

    public static void Register<T>(T obj)
    {
        Instance.registrations[typeof(T)] = obj;
    }

    public static T Register<T>() where T : UnityEngine.Object
    {
        var obj = FindObjectOfType<T>();
        if (obj != null)
            Instance.registrations[typeof(T)] = obj;

        return obj;
    }
}
