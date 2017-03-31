//========= Copyright 2017, HTC Corporation. All rights reserved. ===========

using System;
using System.Reflection;
using System.Collections.Generic;
using HTC.UnityPlugin.ViveShare;

public static partial class ViveShare_Event
{
    // event lookup table
    private class EventTableEntry
    {
        public object eventObj;
        public MethodInfo method;
    }

    private static Dictionary<string, EventTableEntry> eventTable = new Dictionary<string, EventTableEntry>();
    public static void RegistorEventEntry()
    {
        // do nothing if already registered
        if (eventTable.Count > 0)
            return;

        FieldInfo[] fileds = typeof(ViveShare_Event).GetFields();

        foreach (FieldInfo fieldInfo in fileds)
        {
            object obj = fieldInfo.GetValue(null);
            Type objType = obj.GetType();
            ViveEventBase eventBase = (ViveEventBase)obj;

            MethodInfo[] methods = objType.GetMethods();
            foreach (MethodInfo method in methods)
            {
                if (method.Name == "Invoke")
                {
                    EventTableEntry tableEntry = new EventTableEntry();
                    tableEntry.eventObj = obj;
                    tableEntry.method = method;

                    eventTable.Add(eventBase.eventID, tableEntry);

                    break;
                }
            }
        }
    }

    // event invoke by event id
    public static void InvokeEventByID(string eventID, params object[] args)
    {
        EventTableEntry evnt = eventTable[eventID];
        if (evnt != null)
        {
            evnt.method.Invoke(evnt.eventObj, args);
        }
    }
}

//-----------------------------------------------------------------------------
// event type definition
//-----------------------------------------------------------------------------

public class ViveEventBase
{
    public string eventID { get; private set; }

    public ViveEventBase(string id)
    {
        eventID = id;
    }
}

// event with no arguments
public class ViveEvent : ViveEventBase
{
    private Action eventListeners;

    public ViveEvent(string id) : base(id)
    {
    }

    public void Listen(Action listener)
    {
        if (listener == null) { return; }
        eventListeners += listener;
    }

    public void Remove(Action listener)
    {
        if (listener == null) { return; }
        eventListeners -= listener;
    }

    public void Invoke()
    {
        if (eventListeners != null)
        {
            eventListeners.Invoke();
        }
    }

    public void InvokeRemote()
    {
        ViveShare_MessageHandler.Instance.AddEventMessage(eventID, null);
    }
}

// event with 1 argument
public class ViveEvent<T> : ViveEventBase
{
    private Action<T> eventListeners;

    public ViveEvent(string id) : base(id)
    {
    }

    public void Listen(Action<T> listener)
    {
        if (listener == null) { return; }
        eventListeners += listener;
    }

    public void Remove(Action<T> listener)
    {
        if (listener == null) { return; }
        eventListeners -= listener;
    }

    public void Invoke(T arg)
    {
        if (eventListeners != null)
        {
            eventListeners.Invoke(arg);
        }
    }

    public void InvokeRemote(T arg)
    {
        object[] objects = { arg };
        ViveShare_MessageHandler.Instance.AddEventMessage(eventID, objects);
    }
}

// event with 2 arguments
public class ViveEvent<T1, T2> : ViveEventBase
{
    private Action<T1, T2> eventListeners;

    public ViveEvent(string id) : base(id)
    {
    }

    public void Listen(Action<T1, T2> listener)
    {
        if (listener == null) { return; }
        eventListeners += listener;
    }

    public void Remove(Action<T1, T2> listener)
    {
        if (listener == null) { return; }
        eventListeners -= listener;
    }

    public void Invoke(T1 arg1, T2 arg2)
    {
        if (eventListeners != null)
        {
            eventListeners.Invoke(arg1, arg2);
        }
    }

    public void InvokeRemote(T1 arg1, T2 arg2)
    {
        object[] objects = { arg1, arg2 };
        ViveShare_MessageHandler.Instance.AddEventMessage(eventID, objects);
    }
}

// event with 3 arguments
public class ViveEvent<T1, T2, T3> : ViveEventBase
{
    private Action<T1, T2, T3> eventListeners;

    public ViveEvent(string id) : base(id)
    {
    }

    public void Listen(Action<T1, T2, T3> listener)
    {
        if (listener == null) { return; }
        eventListeners += listener;
    }

    public void Remove(Action<T1, T2, T3> listener)
    {
        if (listener == null) { return; }
        eventListeners -= listener;
    }

    public void Invoke(T1 arg1, T2 arg2, T3 arg3)
    {
        if (eventListeners != null)
        {
            eventListeners.Invoke(arg1, arg2, arg3);
        }
    }

    public void InvokeRemote(T1 arg1, T2 arg2, T3 arg3)
    {
        object[] objects = { arg1, arg2, arg3 };
        ViveShare_MessageHandler.Instance.AddEventMessage(eventID, objects);
    }
}

// event with 4 arguments
public class ViveEvent<T1, T2, T3, T4> : ViveEventBase
{
    private Action<T1, T2, T3, T4> eventListeners;

    public ViveEvent(string id) : base(id)
    {
    }

    public void Listen(Action<T1, T2, T3, T4> listener)
    {
        if (listener == null) { return; }
        eventListeners += listener;
    }

    public void Remove(Action<T1, T2, T3, T4> listener)
    {
        if (listener == null) { return; }
        eventListeners -= listener;
    }

    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        if (eventListeners != null)
        {
            eventListeners.Invoke(arg1, arg2, arg3, arg4);
        }
    }

    public void InvokeRemote(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        object[] objects = { arg1, arg2, arg3, arg4 };
        ViveShare_MessageHandler.Instance.AddEventMessage(eventID, objects);
    }
}