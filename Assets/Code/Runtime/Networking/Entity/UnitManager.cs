using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

using EntityNetwork;
using ExitGames.Client.Photon;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public abstract class UnitManager<T> : EntityBase, IMasterOwnsUnclaimed where T: Unit{

  public Dictionary<int, T> entities;

  public static UnitManager<T> Local;
  public static Dictionary<System.Type, int> typeConversion;
  public static Dictionary<int, MethodInfo> createConversion;

  static UnitManager(){
    typeConversion = new Dictionary<System.Type, int>();
    createConversion = new Dictionary<int, MethodInfo>();

    // all entity unit types
    var types = typeof(T).Assembly.GetTypes().Where(t => (t.IsSubclassOf(typeof(T)) || t == typeof(T)) && !t.IsAbstract);
    foreach(var t in types){
      // to be lazy, the type string should return a unique int value
      var value = t.ToString().GetStableHashCode();
      // and get the method info
      var method = t.GetMethod("CreateEntity", BindingFlags.Public | BindingFlags.Static);
      Debug.Log(t);
      Debug.Log(method);
      typeConversion.Add(t, value);
      createConversion.Add(value, method);
    }

  }

  public override void Awake(){
    base.Awake();
    entities = new Dictionary<int, T>();
  }

  void Update(){
    if (!NetworkManager.expectedState) return;

    var items = entities.Values.ToArray();
    foreach(var e in items){
      e.UpdateEntity();
    }

    if (NetworkManager.inRoom && isMine){
      UpdateNow();
    }
  }

  public S Entity<S>(int entityId) where S: T{
    T item;
    return entities.TryGetValue(entityId, out item) ? item as S : null;
  } 

  public void Register(T unit){
    // all entities here are server owned
    // int is what 2^32, this number is pretty much unique
    var id = Random.Range(int.MinValue, int.MaxValue);
    while(entities.ContainsKey(id)){
      Debug.Log("Congratuations!. This message has a 0.0000000002% chance of appearing.");
      id = Random.Range(int.MinValue, int.MaxValue);
    }
    unit.entityID = id;
    unit.authorityID = authorityID;
    entities.Add(id, unit);

    unit.StartEntity();
  }

  public void Deregister(UnitEntity unit){
    entities.Remove(unit.entityID);
  } 

  protected void DeserializeHelper(Hashtable h, System.Action<T, Hashtable> deserialize){
    var cur = new HashSet<int>(entities.Keys);

    foreach(var e in h){
      if (e.Key is int){
        var id = (int)e.Key;
        var hashtable = (Hashtable)e.Value;
        T item;
        if (!entities.TryGetValue(id, out item) && hashtable.Count > 0){
          // a new id, create
          var typeID = (int)hashtable[PhotonConstants.tpeChar];
          var createMethod = createConversion[typeID];
          item = createMethod.Invoke(null, new object[] { }) as T;
          item.entityID = id;
          item.authorityID = authorityID;

          deserialize.Invoke(item, hashtable);

          // add to hashset
          entities.Add(item.entityID, item);
          item.StartEntity();
        } else {
          // not new
          if (hashtable.Count > 0)
            deserialize.Invoke(item, hashtable);
          cur.Remove(id);
        }
      }
    }

    // remove all entities that wasn't included previously
    foreach(var id in cur){
      T item;
      if (entities.TryGetValue(id, out item)){
        item.DestroyEntity();

        // we cannot delete immediately
        // in case other scripts need this
        var obj = item.gameObject;
        obj.SetActive(false);
        Destroy(obj, 1f);

        entities.Remove(id);
      }
      
    }

  }

  void OnDestroy(){
    EntityManager.DeRegister(this);

    foreach(var item in entities.Values){
      var obj = item.gameObject;
      obj.SetActive(false);
      Destroy(obj, 1f);
    }
  }

}
