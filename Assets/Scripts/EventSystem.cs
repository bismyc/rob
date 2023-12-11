using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is event manager class that creates a maintains and dictionary of events and delegates.
/// </summary>
public class EventSystem
{
    private Dictionary<Type, List<Delegate>> listners = new Dictionary<Type, List<Delegate>>();

    /// <summary>
    /// Registers a generic type event
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="handler"></param>
    public void RegisterEvent<T>(Action<T> handler)
    {
        Type type = typeof(T);
        if (!listners.ContainsKey(type))
        {
            listners.Add(type, new List<Delegate>() { handler });
        }
        else
        {
            listners[type].Add(handler);
        }
    }

    /// <summary>
    /// invokes the event delegates with parameters
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameters"></param>
    public void SendEvent<T>(T parameters)
    {
        Type type = typeof(T);

        if (listners.ContainsKey(type))
        {
            List<Delegate> delegates = listners[type];
            for (int i = 0; i < delegates.Count; i++)
            {
                delegates[i].DynamicInvoke(parameters);
            }
        }
    }

    public void SendEvent<T>()
    {
        SendEvent<T>(default(T));
    }

    /// <summary>
    /// Unregister the event
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="handler"></param>
    public void UnRegisterEvent<T>(Action<T> handler)
    {
        Type type = typeof(T);
        if (listners.ContainsKey(type))
        {
            listners[type].Remove(handler);
        }
    }
}
/// <summary>
/// List of all events
/// </summary>
namespace Events
{
    public class OnTapPlane { public Vector3 worldPos; public Vector3 screenPos; }
    public class OnSelectGameAsset { public int id; }
    public class OnActivateGameAsset { public int id; }
    public class OnInteractionActionBegin{ public Action action; }
    public class OnInteractionActionEnd { public Action action; }

}


