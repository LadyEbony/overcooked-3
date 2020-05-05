using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Options = ExitGames.Client.Photon.LoadBalancing.RaiseEventOptions;

public class PhysicsEntityManager : UnitManager<PhysicsEntity> {

  public static PhysicsEntityManager Instance;

  public struct ClientState{
    public byte input;
    public Vector3 position;
    public Quaternion rotation;
  }

  public struct EntityState{
    public int id;
    public byte mask;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;
  }

  public struct InputMessage{
    public int tick;
    public byte input;
  }

  public struct StateMessage{
    public int tick;
    public EntityState[] entities;
  }

  [Header("Lock Step")]
  public int tick_number;
  public int tick_message;
  private float timer;

  public const int BUFFER_SIZE = 256;

  private Queue<StateMessage> stateMessageBuffer;

  private Dictionary<int, byte> lastPlayerInputs;             // for client predictation
  private Dictionary<int, Queue<InputMessage>> inputBuffer;   // for server

  private void Start() {
    tick_number = 0;
    timer = 0f;

    stateMessageBuffer = new Queue<StateMessage>();

    lastPlayerInputs = new Dictionary<int, byte>();
    inputBuffer = new Dictionary<int, Queue<InputMessage>>();

    // testing
    var physics = FindObjectsOfType<PhysicsEntity>();
    foreach(var e in physics){
      entities.Add(e.entityID, e);
      e.AwakeEntity();
      e.StartEntity();
    }
  }

  void Update(){
    if (!NetworkManager.expectedState) return;

    timer += Time.deltaTime;
    while(timer >= Time.fixedDeltaTime){
      timer -= Time.fixedDeltaTime;

      // simulate current state
      // client and server
      if (NetworkManager.isMaster){
        
        var actors = new List<(int actor, int tick)>();
        foreach(var e in entities){
          var entity = e.Value;

          var input = PopInput(entity.authorityID);
          if (input.tick >= 0) actors.Add((entity.authorityID, input.tick));
          entity.ApplyInput(input.input);
        }

        Physics.Simulate(Time.fixedDeltaTime);

        // every 4 messages. should reduce traffic
        if (tick_message >= 4){
          var entitiesResponse = entities.Select(e => e.Value.GetEntityState()).ToArray();

          // send response
          foreach(var pair in actors){
            var actor = pair.actor;
            var tick = pair.tick;

            var h = new Hashtable();
            Serialize(h);
            h.Add('t', pair.tick + 1);
            h.Add('e', entitiesResponse);

            var op = Options.Default;
            op.TargetActors = new int[] { actor };
            NetworkManager.netMessage(PhotonConstants.EntityUpdateCode, h, true, op);

          }

          tick_message -= 4;
        }

        ++tick_number;
        ++tick_message;

      } else {
        // simulate client
        var localinput = GetInput;
        var buffer = tick_number % BUFFER_SIZE;

        // apply input and save predicated state
        foreach(var e in entities){
          var entity = e.Value;

          var input = entity.isMine ? localinput : GetLastPlayerInput(entity.authorityID);
          entity.ApplyInput(input);
          entity.SaveClientState(buffer, input);
        }

        Physics.Simulate(Time.fixedDeltaTime);

        // send input
        var h = new Hashtable();
        Serialize(h);
        h.Add('t', tick_number);
        h.Add('i', localinput);

        // sending input to everyone so they recieve the most recent last input
        NetworkManager.netMessage(PhotonConstants.EntityUpdateCode, h, true, Options.Default);

        ++tick_number;

        // check for error states
        // client only
        while(stateMessageBuffer.Count > 0){
          var message = stateMessageBuffer.Dequeue();

          buffer = message.tick % BUFFER_SIZE;

          // get error amount
          float error = 0f;
          foreach(var e in message.entities){
            var entity = entities[e.id];
            entity.StorePrevEntityState(e);
            error += entity.GetError(buffer);
          }

          if (error > 0.001f){

            // restore to what the message said
            foreach(var e in entities){
              var entity = e.Value;
              entity.CapturePrevState();
              entity.RestorePrevEntityState();
            }

            // do the client predicated inputs again
            var rewind = message.tick;
            while(rewind < tick_number){

              buffer = rewind % BUFFER_SIZE;
              foreach(var e in entities){
                var entity = e.Value;

                var input = entity.isMine ? entity.clientStateBuffer[buffer].input : GetLastPlayerInput(entity.authorityID);
                entity.ApplyInput(input);
                entity.SaveClientState(buffer, input);
              }

              Physics.Simulate(Time.fixedDeltaTime);
              ++rewind;
            }

            // amount of error check
            foreach(var e in entities){
              e.Value.ComparePrevState();
            }
          }
        }

        // interpolate
        foreach(var e in entities){
          e.Value.UpdateEntity();
        }
      }

      
    }
  }

  private byte GetInput {
    get {
      var left = Input.GetKey(KeyCode.A);
      var right = Input.GetKey(KeyCode.D);
      var up = Input.GetKey(KeyCode.W);
      var down = Input.GetKey(KeyCode.S);
      var space = Input.GetKey(KeyCode.Space);

      byte input = 0;
      if (left)   input |= 1;
      if (right)  input |= 2;
      if (up)     input |= 4;
      if (down)   input |= 8;
      if (space)  input |= 16;
      return input;
    }
  }

  public void SetLastPlayerInput(int actor, byte input){
    if (lastPlayerInputs.ContainsKey(actor)){
      lastPlayerInputs[actor] = input;
    } else {
      lastPlayerInputs.Add(actor, input);
    }
  }

  public byte GetLastPlayerInput(int actor){
    byte input;
    return lastPlayerInputs.TryGetValue(actor, out input) ? input : (byte)0;
  }

  public void AddInput(int actor, int tick, byte input){
    Queue<InputMessage> queue;
    if (!inputBuffer.TryGetValue(actor, out queue)){
      queue = new Queue<InputMessage>();
      inputBuffer.Add(actor, queue);
    }
    queue.Enqueue(new InputMessage{ tick = tick, input = input});
    SetLastPlayerInput(actor, input);
  }

  public InputMessage PopInput(int actor){
    Queue<InputMessage> queue;
    if (!inputBuffer.TryGetValue(actor, out queue)){
      queue = new Queue<InputMessage>();
      inputBuffer.Add(actor, queue);
    }

    if (actor == NetworkManager.masterID){
      return new InputMessage{ input = GetInput, tick = tick_number };
    }

    if (queue.Count <= 1){
      return new InputMessage{ input = GetLastPlayerInput(actor), tick = -1 };
    }

    return queue.Dequeue();
  }

  public override void Deserialize(Hashtable h) {
    base.Deserialize(h);

    // input message
    if (h.ContainsKey('i')){
      var input = (byte)h['i'];
      var actor = (int)h[PhotonConstants.actChar];

      if (NetworkManager.isMaster){
        var tick = (int)h['t'];
        AddInput(actor, tick, input);
      } else {
        SetLastPlayerInput(actor, input);
      }
    } 
    // entities message
    else {
      var tick = (int)h['t'];
      var states = (EntityState[])h['e'];

      StateMessage state;
      state.tick = tick;
      state.entities = states;

      stateMessageBuffer.Enqueue(state);
    }
  }

}
