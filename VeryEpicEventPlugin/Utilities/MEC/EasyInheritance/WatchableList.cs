#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace VeryEpicEventPlugin.Utilities.MEC.EasyInheritance;

/// <summary>
/// The watchable list defining methods for simple events.
/// </summary>
/// <typeparam name="T"></typeparam>
public class WatchableList<T> : List<T>
{
    /// <summary>
    /// Action executed when element is added.
    /// </summary>
    public Action<T>? OnAdd { get; set; }

    /// <summary>
    /// Action executed when element is removed
    /// </summary>
    public Action<T>? OnRemove { get; set; }

    /// <summary>
    /// Action executed when list is cleared (every element calls OnRemove)
    /// </summary>
    public Action? OnClear { get; set; }

    /// <summary>
    /// Empty constructor for reflection and advanced stuff.
    /// </summary>
    public WatchableList()
    {
        
    }

    /// <summary>
    /// Constructor creating WatchableList object out of actions linked to events.
    /// </summary>
    /// <param name="onAdd">Action{T} object</param>
    /// <param name="onRemove">Action{T} object</param>
    /// <param name="onClear">Action object</param>
    public WatchableList(Action<T> onAdd, Action<T> onRemove, Action onClear)
    {
        OnAdd = onAdd;
        OnRemove = onRemove;
        OnClear = onClear;
    }
    
    /// <summary>
    /// Method override with new for Add. Executed the event before adding.
    /// </summary>
    /// <param name="item"></param>
    public new void Add(T item)
    {
        try
        {
            OnAdd?.Invoke(item);
        }
        catch (Exception ex) 
        {
            Log.Error(ex);
        }
        
        base.Add(item);
    }
    
    public new bool Remove(T item)
    {
        try
        {
            OnRemove?.Invoke(item);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
        
        return base.Remove(item);
    }

    public new void Clear()
    {
        try
        {
            OnClear?.Invoke();
            foreach (var i in this)
            {
                OnRemove?.Invoke(i);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
        
        base.Clear();
    }

    /// <summary>
    /// Removes all objects by preficator.
    /// </summary>
    /// <param name="match"></param>
    public new void RemoveAll(Predicate<T> match)
    {
        try
        {
            foreach (var item in this)
            {
                if (match(item)) OnRemove?.Invoke(item);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
        
        base.RemoveAll(match);
    }
}