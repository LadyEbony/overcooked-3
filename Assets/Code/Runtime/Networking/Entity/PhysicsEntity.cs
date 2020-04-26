using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsEntity : Unit {

  [Header("Rigidbody")]
  public new Rigidbody rigidbody;
  public float speed = 4f;

  public struct ClientState{
    public Vector3 position;
    public Quaternion rotation;
  }

  public struct InputState{
    public byte input;
  }

  public struct InputMessage{
    public int tick;
    public byte input;
  }

  public struct StateMessage{
    public int tick;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public Vector3 angularVelocity;
  }

  private ClientState[] clientStateBuffer;
  private InputState[] inputStateBuffer;

  private InputMessage inputMessage;
  private StateMessage stateMessage;
  private Queue<InputMessage> inputMessages;
  private Queue<StateMessage> stateMessages;

  [Header("Lock Step")]
  public int tick_number;
  private float timer;

  public override bool isMine => authorityID == PhysicsEntityManager.Local.authorityID;

  // Add this to all EntityUnits
  public static PhysicsEntity CreateEntity(){
    return CreateEntityHelper(GameInitializerPhysics.Instance.playerPrefab);

    // Example: replace null with your prefab
    // return SetEntityHelper(null);
  }

  public static PhysicsEntity CreateEntityHelper(GameObject prefab){
    var obj = Instantiate(prefab);
    var comp = obj.GetComponent<PhysicsEntity>();
    comp.AwakeEntity();
    return comp;
  }

  public override void AwakeEntity() {
    base.AwakeEntity();

    clientStateBuffer = new ClientState[512];
    inputStateBuffer = new InputState[512];

    inputMessages = new Queue<InputMessage>();
    stateMessages = new Queue<StateMessage>();
  }

  public override void StartEntity() {
    base.StartEntity();

    rigidbody = GetComponent<Rigidbody>();
    timer = 0f;
  }

  public override void UpdateEntity() {
    base.UpdateEntity();

    if (isMaster) MasterUpdate();
    else ClientUpdate();
  }

  void MasterUpdate(){
    if (isMine){
      inputMessage.input = GetInput;
      inputMessage.tick = tick_number;
      inputMessages.Enqueue(inputMessage);
    }

    if (inputMessages.Count > 0){
      var message = inputMessages.Dequeue();

      ApplyInput(message.input);
      Physics.Simulate(Time.fixedDeltaTime);

      stateMessage.position = rigidbody.position;
      stateMessage.rotation = rigidbody.rotation;
      stateMessage.velocity = rigidbody.velocity;
      stateMessage.angularVelocity = rigidbody.angularVelocity;
      stateMessage.tick = message.tick + 1;
    }
  }

  void ClientUpdate(){
    // client decides inputs
    if (isMine){
      var input = GetInput;
      inputMessage.input = input;
      inputMessage.tick = tick_number;

      var buffer = tick_number % 512;
      inputStateBuffer[buffer].input = input;
      clientStateBuffer[buffer].position = rigidbody.position;
      clientStateBuffer[buffer].rotation = rigidbody.rotation;

      ApplyInput(input);
      Physics.Simulate(Time.fixedDeltaTime);
      ++tick_number;

      while(stateMessages.Count > 0){
        var message = stateMessages.Dequeue();

        buffer = message.tick % 512;
        var err = (message.position - clientStateBuffer[buffer].position).sqrMagnitude;

        if (err > 0.001f){
          // error, rewind
          rigidbody.position = message.position;
          rigidbody.rotation = message.rotation;
          rigidbody.velocity = message.velocity;
          rigidbody.angularVelocity = message.angularVelocity;

          var rewind = message.tick;
          while(rewind < tick_number){
            buffer = rewind % 512;
            input = inputStateBuffer[buffer].input;
            clientStateBuffer[buffer].position = rigidbody.position;
            clientStateBuffer[buffer].rotation = rigidbody.rotation;

            ApplyInput(input);
            Physics.Simulate(Time.fixedDeltaTime);
            ++rewind;
          }

        }
      }
    }

    
  }

  void ApplyInput(byte input){
    var left = (input & 1) > 0;
    var right = (input & 2) > 0;
    var up = (input & 4) > 0;
    var down = (input & 8) > 0;
    var jump = (input & 16) > 0;

    var velocity = rigidbody.velocity;
    if (left && !right) velocity.x = -speed;
    if (right && !left) velocity.x = speed;
    if (up && !down) velocity.z = speed;
    if (down && !up) velocity.z = -speed;
    if (jump) velocity.y = speed;
    rigidbody.velocity = velocity;
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

  public virtual void SerializeClient(Hashtable h){
    h.Add(PhotonConstants.tpeChar, PhysicsEntityManager.typeConversion[GetType()]);
    h.Add('t', inputMessage.tick);
    h.Add('i', inputMessage.input);
  }

  public virtual void SerializeMaster(Hashtable h){
    h.Add(PhotonConstants.tpeChar, PhysicsEntityManager.typeConversion[GetType()]);
    h.Add('t', stateMessage.tick);
    h.Add('p', stateMessage.position);
    h.Add('r', stateMessage.rotation);
    h.Add('v', stateMessage.velocity);
    h.Add('a', stateMessage.angularVelocity);
  }

  public virtual void DeserializeClient(Hashtable h){
    var tick = (int)h['t'];
    var pos = (Vector3)h['p'];
    var rot = (Quaternion)h['r'];
    var vel = (Vector3)h['v'];
    var ang = (Vector3)h['a'];

    stateMessages.Enqueue(new StateMessage{ tick = tick, position = pos, rotation = rot, velocity = vel, angularVelocity = ang });
  }

  public virtual void DeserializeMaster(Hashtable h){
    var tick = (int)h['t'];
    var input = (byte)h['i'];

    inputMessages.Enqueue(new InputMessage{ tick = tick, input = input });
  }

}
