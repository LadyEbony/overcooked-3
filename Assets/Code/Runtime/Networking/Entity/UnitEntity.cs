using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EntityNetwork;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public abstract class UnitEntity : Unit {

  [Header("Update Timers")]
  public float updateTimer = 0.1f;
  private float _nextUpdatetime;

  public override bool isMine => authorityID == UnitEntityManager.Local.authorityID;

  // Add this to all EntityUnits
  public static UnitEntity CreateEntity(){
    throw new System.Exception("Should never be called. This is just an example function.");

    // Example: replace null with your prefab
    // return SetEntityHelper(null);
  }

  public static UnitEntity CreateEntityHelper(GameObject prefab){
    var obj = Instantiate(prefab);
    var comp = obj.GetComponent<UnitEntity>();
    comp.AwakeEntity();
    return comp;
  }

  /// <summary>
  /// Adds data to <paramref name="h"/>.
  /// </summary>
  /// <param name="h"></param>
  public virtual void Serialize(Hashtable h){
    h.Add(PhotonConstants.tpeChar, UnitManager<UnitEntity>.typeConversion[GetType()]);
  }

  /// <summary>
  /// Interprets data from <paramref name="h"/>.
  /// </summary>
  /// <param name="h"></param>
  public virtual void Deserialize(Hashtable h) {}

  public virtual bool UpdateReady {
    get{
      if (Time.time >= _nextUpdatetime){
        _nextUpdatetime = Time.time + updateTimer;
        return true;
      }
      return false;
    }
  }

}
