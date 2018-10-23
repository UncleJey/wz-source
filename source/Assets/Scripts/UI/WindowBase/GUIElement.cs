using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GUIElement<TElementType> : GUIElementBase
    where TElementType : GUIElement<TElementType>
{
    public static event Action<TElementType> OnOpened;
    public static event Action<TElementType> OnClosed;

    

    public static readonly Dictionary<Type, TElementType> Existing = new Dictionary<Type, TElementType>();
    public static readonly List<TElementType> Opened = new List<TElementType>();

    public static void InitElements( MonoBehaviour _parent )
    {
        _parent.StartCoroutine(Initor(_parent));
    }

    private static IEnumerator Initor(MonoBehaviour _parent)
    {
        var children = _parent.GetComponentsInChildren<TElementType>(true);
        foreach (var elem in children)
        {
            var type = elem.GetType();
#if UNITY_EDITOR
            if (Existing.ContainsKey(type))
            {
                TElementType wnd = Existing[type];
                Debug.LogError( "Window with type " + type + " already exists (" + wnd.name + "). Replacing with "+elem.name );
                Existing.Remove( type );
            }
#endif
            Existing.Add( type, elem );
            elem.Initialize();
            elem.gameObject.SetActive(true);
        }

        yield return null;
        foreach (var elem in children)
        {
            if (!Opened.Contains(elem))
                elem.gameObject.SetActive(false);           
        }
    }

    public static T GetElement<T>()
        where T : TElementType
    {
        return (T) GetElement(typeof(T));
    }

    public static TElementType GetElement( Type _type )
    {
        TElementType elem;
        if (!Existing.TryGetValue(_type, out elem))
        {
            Debug.LogError("GUI Element of type " + _type.Name + " not found!");
        }
        return elem;
    }

    protected virtual void Initialize()
    {
        
    }

    protected void MarkOpened()
    {
        var self = this as TElementType;
        if (!Opened.Contains(self))
            Opened.Add(self);
        onThisOpened.Execute();
        OnOpened.Execute(self);
    }

    protected void MarkClosed()
    {
        var self = this as TElementType;
        if (Opened.Remove(this as TElementType))
        {
            onThisClosed.Execute();
            OnClosed.Execute(self);
        }
    }
}

public abstract class GUIElementBase : MonoBehaviour
{
    protected Action onThisOpened;
    protected Action onThisClosed;

    public event Action OnThisOpened
    {
        add { onThisOpened += value; }
        remove { onThisOpened -= value; }
    }
    public event Action OnThisClosed
    {
        add { onThisClosed += value; }
        remove { onThisClosed -= value; }
    }
}
