﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;

public abstract class GameInitializer<S, T> : MonoBehaviour where T: UnitManager<S> where S: Unit{

  public static GameInitializer<S, T> Instance { get; private set; }

  public Dictionary<int, T> managers;

  public GameObject playerPrefab;
  public GameObject aiPrefab;
  public GameObject gunPrefab;
  public GameObject bulletPrefab;
  public GameObject grenadePrefab;

  private void Awake() {
    Instance = this;
    managers = new Dictionary<int, T>();
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

  private T CreateManager(int id){
    var obj = new GameObject("Manager", typeof(T));
    var manager = obj.GetComponent<T>();
    manager.EntityID = id;
    manager.authorityID = id;
    manager.Register();

    return manager;
  }

  public void ModifyServerManager(T manager){
    
  }

  public virtual void ModifyLocalManager(T manager) {
		UnitManager<S>.Local = manager;
	}

	private void AddUnitManager(int actor, T manager){
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

    T manager;
    if(managers.TryGetValue(id, out manager)){
      Destroy(manager.gameObject);
      RemoveUnitManager(id);
    }
    
  }
}
