using System;
using System.Collections.Generic;

namespace InfoWave.MonoGame.Common.Utils;

public static class DictionaryExtensions
{
    public static TValueSpecific GetOr<TKey, TValue, TValueSpecific>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValueSpecific> valueFactory) where TValueSpecific : TValue
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            return (TValueSpecific) value;
        }

        return (TValueSpecific) (dictionary[key] = valueFactory()); 
    }
}