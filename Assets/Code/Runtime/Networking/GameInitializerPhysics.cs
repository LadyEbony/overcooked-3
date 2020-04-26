using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializerPhysics : GameInitializer<PhysicsEntity, PhysicsEntityManager> {

  public override void ModifyLocalManager(PhysicsEntityManager manager) {
    base.ModifyLocalManager(manager);
    
    if (NetworkManager.isMaster) return;
    var entity = PhysicsEntity.CreateEntity() as PhysicsEntity;
    PhysicsEntityManager.Local.Register(entity);
  }

}
