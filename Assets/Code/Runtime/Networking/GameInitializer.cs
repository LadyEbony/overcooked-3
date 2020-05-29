using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;

public class GameInitializer: MonoBehaviour{

  public static GameInitializer Instance { get; private set; }

  public Dictionary<int, UnitEntityManager> managers;

  public GameObject playerPrefab;
  public GameObject platePrefab;
  public GameObject foodPrefab;

  public Transform[] spawnPoints;

  public T Entity<T>(int actor, int id) where T: UnitEntity{
    UnitEntityManager manager;
    if (managers.TryGetValue(actor, out manager)){
      return manager.Entity<T>(id);
    }
    return null;
  }

  private void Awake() {
    Instance = this;
    managers = new Dictionary<int, UnitEntityManager>();
  }

  private void OnEnable() {
    NetworkManager.onJoin += OnPlayerConnected;
    NetworkManager.onLeave += OnPlayerLeaved;
  }

  private void OnDisable() {
    NetworkManager.onJoin -= OnPlayerConnected;
    NetworkManager.onLeave -= OnPlayerLeaved;
  }

  // Start is called before the first frame update
  IEnumerator Start() {
    while (!NetworkManager.expectedState) yield return null;

    DoubleDictionary<PlayerEntity, ItemEntity>.Create();
    DoubleDictionary<Cabient, ItemEntity>.Create();

    if (NetworkManager.inRoom){
      var players = NetworkManager.net.CurrentRoom.Players;

      foreach(var player in players.Values){
        var id = player.ID;
        var manager = CreateManager(id);

        AddUnitManager(id, manager);
        if (player.IsLocal) ModifyLocalManager(manager);
        if (player.IsMasterClient) ModifyServerManager(manager);
      }

    } else {
      var id = -1;
      var manager = CreateManager(id);

      AddUnitManager(id, manager);
      ModifyLocalManager(manager);
    }

  }

  private UnitEntityManager CreateManager(int id){
    var obj = new GameObject("Unit Manager", typeof(UnitEntityManager));
    var manager = obj.GetComponent<UnitEntityManager>();
    manager.EntityID = id;
    manager.authorityID = id;
    manager.Register();

    return manager;
  }

  public void ModifyServerManager(UnitEntityManager manager){
  
  }

  public virtual void ModifyLocalManager(UnitEntityManager manager) {
		UnitEntityManager.Local = manager;

    var player = PlayerEntity.CreateEntity() as PlayerEntity;
    player.controller.SetPosition(spawnPoints[manager.authorityID % spawnPoints.Length].position);
    manager.Register(player);
	}

	private void AddUnitManager(int actor, UnitEntityManager manager){
    managers.Add(actor, manager);
  }

  private void RemoveUnitManager(int actor){
    managers.Remove(actor);
  }

  private void OnPlayerConnected(EventData data) {
    var id = (int)data.Parameters[ParameterCode.ActorNr];
    if (id != PlayerProperties.localPlayer.ID){
      var manager = CreateManager(id);
      AddUnitManager(id, manager);
    }
  }

  private void OnPlayerLeaved(EventData data) {
    var id = (int)data.Parameters[ParameterCode.ActorNr];

    UnitEntityManager manager;
    if(managers.TryGetValue(id, out manager)){
      Destroy(manager.gameObject);
      RemoveUnitManager(id);
    }
    
  }
}
