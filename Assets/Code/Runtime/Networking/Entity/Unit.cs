using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour {
    
  [Header("IDs")]
  public int entityID;
  public int authorityID;

  public abstract bool isMine { get; }
  public bool isMaster => NetworkManager.isMaster;

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

}
