﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EntityNetwork;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public abstract class EntityUnit : MonoBehaviour {

  [Header("IDs")]
  public int entityID;

  [Header("Update Timers")]
  public float updateTimer = 0.1f;
  private float _nextUpdatetime;

  public bool isMine{
    get {
      return NetworkManager.isMaster;
    }
  }

  // Add this to all EntityUnits
  public static EntityUnit CreateEntity(int id){
    throw new System.Exception("Should never be called. This is just an example function.");
  }

  public static EntityUnit SetEntityHelper(int id, GameObject item){
    var comp = item.GetComponent<EntityUnit>();
    comp.entityID = id;
    return comp;
  }

  /// <summary>
  /// Called after <see cref="CreateEntity(int)"/> but before <see cref="Deserialize(Hashtable)"/>.
  /// </summary>
  public virtual void AwakeEntity() {}

  /// <summary>
  /// Called once after <see cref="Deserialize(Hashtable)"/>.
  /// </summary>
  public virtual void StartEntity() {}

  /// <summary>
  /// Called every frame by <see cref="UnitManager"/>.
  /// </summary>
  public virtual void UpdateEntity() {}

  /// <summary>
  /// Called once when destroyed by <see cref="UnitManager"/>.
  /// </summary>
  public virtual void DestroyEntity() {}

  /// <summary>
  /// Adds data to <paramref name="h"/>.
  /// </summary>
  /// <param name="h"></param>
  public virtual void Serialize(Hashtable h){
    h.Add(PhotonConstants.tpeChar, UnitManager.typeConversion[GetType()]);
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
