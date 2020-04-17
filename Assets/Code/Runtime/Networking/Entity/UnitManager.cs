using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

using EntityNetwork;
using ExitGames.Client.Photon;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UnitManager : EntityBase, IMasterOwnsUnclaimed {

  public Dictionary<int, EntityUnit> entities;

  public GameObject platePrefab;

  public static UnitManager Instance { get; private set; }

  public static Dictionary<System.Type, int> typeConversion;
  public static Dictionary<int, MethodInfo> createConversion;

  static UnitManager(){
    typeConversion = new Dictionary<System.Type, int>();
    createConversion = new Dictionary<int, MethodInfo>();

    // all entity unit types
    var types = typeof(EntityUnit).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(EntityUnit)) && !t.IsAbstract);
    foreach(var t in types){
      // to be lazy, the type string should return a unique int value
      var value = t.ToString().GetStableHashCode();
      // and get the method info
      var method = t.GetMethod("CreateEntity", BindingFlags.Public | BindingFlags.Static);
      typeConversion.Add(t, value);
      createConversion.Add(value, method);
    }

  }

  public override void Awake() {
    Instance = this;

    entities = new Dictionary<int, EntityUnit>();
    Register();
  }

  IEnumerator Start(){
    while(!NetworkManager.expectedState) yield return null;

    // just for testing
    if (isMine){
      var item = Instantiate(platePrefab);
      Register(item.GetComponent<EntityUnit>());
    }
  }

  void Update(){
    if (!NetworkManager.expectedState) return;

    foreach(var e in entities.Values){
      e.UpdateEntity();
    }

    if (NetworkManager.inRoom && isMine){
      UpdateNow();
    }
  }

  public T Entity<T>(int entityId) where T: EntityUnit{
    EntityUnit item;
    return entities.TryGetValue(entityId, out item) ? item as T : null;
  } 



  public void Register(EntityUnit unit){
    // all entities here are server owned
    // int is what 2^32, this number is pretty much unique
    var id = Random.Range(int.MinValue, int.MaxValue);
    while(entities.ContainsKey(id)){
      Debug.Log("Congratuations!. This message has a 0.0000000002% chance of appearing.");
      id = Random.Range(int.MinValue, int.MaxValue);
    }
    unit.entityID = id;
    entities.Add(id, unit);

    unit.AwakeEntity();
    unit.StartEntity();
  }

  public override void Serialize(Hashtable h) {
    base.Serialize(h);

    foreach(var e in entities.Values){
      var temp = new Hashtable();
      // we only send the full entity data when their auto timers are triggered
      // otherwise send empty
      if (e.UpdateReady){
        e.Serialize(temp);
      }
      h.Add(e.entityID, temp);
    }
  }

  public override void Deserialize(Hashtable h) {
    base.Deserialize(h);

    var cur = new HashSet<int>(entities.Keys);

    foreach(var e in h){
      if (e.Key is int){
        var id = (int)e.Key;
        var hashtable = (Hashtable)e.Value;

        EntityUnit item;
        if (!entities.TryGetValue(id, out item)){
          // a new id, create
          var typeID = (int)hashtable[PhotonConstants.tpeChar];
          var createMethod = createConversion[typeID];
          item = createMethod.Invoke(null, new object[] { id }) as EntityUnit;
          item.AwakeEntity();
          item.Deserialize(hashtable);

          // add to hashset
          entities.Add(item.entityID, item);
          item.StartEntity();
        } else {
          // not new
          item.Deserialize(hashtable);
          cur.Remove(id);
        }
      }
    }

    // remove all entities that wasn't included previously
    foreach(var id in cur){
      EntityUnit item;
      if (entities.TryGetValue(id, out item)){
        item.DestroyEntity();

        // we cannot delete immediately
        // in case they still get data in the future
        var obj = item.gameObject;
        obj.SetActive(false);
        Destroy(obj, 2.5f);

        entities.Remove(id);
      }
      
    }

  }

}
