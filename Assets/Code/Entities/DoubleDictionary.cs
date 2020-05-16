using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DoubleDictionary<Key, Value>{

  static Dictionary<Key, Value> keys;
  static Dictionary<Value, Key> values;

  static DoubleDictionary(){
    Create();
  }

  public static void Create(){
    keys = new Dictionary<Key, Value>();
    values = new Dictionary<Value, Key>();
  }

  public static void Set(Key key, Value value){
    keys.Add(key, value);
    values.Add(value, key);
  }

  public static void Remove(Key key){
    Value value;
    if (keys.TryGetValue(key, out value)){
      keys.Remove(key);
      values.Remove(value);
    }
  }

  public static void Remove(Value value){
    Key key;
    if (values.TryGetValue(value, out key)){
      keys.Remove(key);
      values.Remove(value);
    }
  }

  public static void Remove(Key key, Value value){
    keys.Remove(key);
    values.Remove(value);
  }

  public static Key Get(Value value){
    Key key;
    return values.TryGetValue(value, out key) ? key : default;
  }

  public static Value Get(Key key){
    Value value;
    return keys.TryGetValue(key, out value) ? value : default;
  }

}
