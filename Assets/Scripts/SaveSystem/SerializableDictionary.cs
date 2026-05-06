using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    //Unity 的序列化系统不支持直接序列化字典，因此我们使用两个列表分别存储键和值。
    //序列化字典时，会把字典中的键和值填充到这两个列表；反序列化时，再根据这两个列表重建字典。
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();

    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
            Debug.LogError("键和值的数量不匹配！");

        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);//根据反序列化后的键和值列表，重建字典。
        }
    }

    public void OnBeforeSerialize()
    {
        //在序列化之前，先清空键和值的列表，以确保它们与当前字典中的数据保持一致。
        keys.Clear();
        values.Clear();

        //遍历字典，将键和值分别添加到对应的列表中。
        foreach (KeyValuePair<TKey, TValue> pairs in this)
        {
            keys.Add(pairs.Key);
            values.Add(pairs.Value);
        }
    }
}
