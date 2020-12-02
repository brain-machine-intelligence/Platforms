using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiDimDictList<K, T> : Dictionary<K, List<T>> 
{
    public void Add(K key, T addObject)
    {
        if (!ContainsKey(key)) Add(key, new List<T>());
        if (!base[key].Contains(addObject)) base[key].Add(addObject);
    }
}
