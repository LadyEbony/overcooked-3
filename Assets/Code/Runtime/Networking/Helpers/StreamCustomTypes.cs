using System;
using System.IO;
using ExitGames.Client.Photon;
using UnityEngine;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public static class StreamCustomTypes {
	public static void Register() {
		PhotonPeer.RegisterType(typeof(Vector2), (byte)'V', SerializeVector2, DeserializeVector2);
		PhotonPeer.RegisterType(typeof(Vector3), (byte)'W', SerializeVector3, DeserializeVector3);
		PhotonPeer.RegisterType(typeof(Quaternion), (byte)'Q', SerializeQuaternion, DeserializeQuaternion);
    PhotonPeer.RegisterType(typeof(Color), (byte)'C', SerializeColor, DeserializeColor);
		PhotonPeer.RegisterType(typeof(char), (byte)'c', SerializeChar, DeserializeChar);

    PhotonPeer.RegisterType(typeof(PhysicsEntityManager.EntityState), (byte)'E', SerializeEntityState, DeserializeEntityState);
	}

	private static short SerializeVector2(StreamBuffer outStream, object customObj) {
		var vo = (Vector2)customObj;

		var ms = new MemoryStream(2 * 4);

		ms.Write(BitConverter.GetBytes(vo.x), 0, 4);
		ms.Write(BitConverter.GetBytes(vo.y), 0, 4);

		outStream.Write(ms.ToArray(), 0, 2 * 4);
		return 2 * 4;
	}

	private static object DeserializeVector2(StreamBuffer inStream, short length) {
		var bytes = new Byte[2 * 4];
		inStream.Read(bytes, 0, 2 * 4);
		return new 
			Vector2(
				BitConverter.ToSingle(bytes, 0),
				BitConverter.ToSingle(bytes, 4));

		// As best as I can tell, the new Protocol.Serialize/Deserialize are written around WP8 restrictions
		// It's not worth the pain.

		//int index = 0;
		//float x, y;
		//Protocol.Deserialize(out x, bytes, ref index);
		//Protocol.Deserialize(out y, bytes, ref index);

		//return new Vector2(x, y);
	}

	private static short SerializeVector3(StreamBuffer outStream, object customObj) {
		Vector3 vo = (Vector3)customObj;

		var ms = new MemoryStream(3 * 4);

		ms.Write(BitConverter.GetBytes(vo.x), 0, 4);
		ms.Write(BitConverter.GetBytes(vo.y), 0, 4);
		ms.Write(BitConverter.GetBytes(vo.z), 0, 4);

		outStream.Write(ms.ToArray(), 0, 3 * 4);
		return 3 * 4;
	}

	private static object DeserializeVector3(StreamBuffer inStream, short length) {
		var bytes = new byte[3 * 4];

		inStream.Read(bytes, 0, 3 * 4);

		return new 
			Vector3(
				BitConverter.ToSingle(bytes, 0),
				BitConverter.ToSingle(bytes, 4),
				BitConverter.ToSingle(bytes, 8));
	}

	private static short SerializeQuaternion(StreamBuffer outStream, object customObj) {
		Quaternion vo = (Quaternion)customObj;

		var ms = new MemoryStream(4 * 4);

		ms.Write(BitConverter.GetBytes(vo.x), 0, 4);
		ms.Write(BitConverter.GetBytes(vo.y), 0, 4);
		ms.Write(BitConverter.GetBytes(vo.z), 0, 4);
		ms.Write(BitConverter.GetBytes(vo.w), 0, 4);

		outStream.Write(ms.ToArray(), 0, 4 * 4);
		return 4 * 4;
	}

	private static object DeserializeQuaternion(StreamBuffer inStream, short length) {
		var bytes = new byte[4 * 4];

		inStream.Read(bytes, 0, 4 * 4);

		return new 
			Quaternion(
				BitConverter.ToSingle(bytes, 0),
				BitConverter.ToSingle(bytes, 4),
				BitConverter.ToSingle(bytes, 8),
				BitConverter.ToSingle(bytes, 12));
	}

  	private static short SerializeColor(StreamBuffer outStream, object customObj) {
		Color vo = (Color)customObj;

		var ms = new MemoryStream(4 * 4);

		ms.Write(BitConverter.GetBytes(vo.r), 0, 4);
		ms.Write(BitConverter.GetBytes(vo.g), 0, 4);
		ms.Write(BitConverter.GetBytes(vo.b), 0, 4);
    ms.Write(BitConverter.GetBytes(vo.a), 0, 4);

		outStream.Write(ms.ToArray(), 0, 4 * 4);
		return 4 * 4;
	}

	private static object DeserializeColor(StreamBuffer inStream, short length) {
		var bytes = new byte[4 * 4];

		inStream.Read(bytes, 0, 4 * 4);

		return new 
			Color(
				BitConverter.ToSingle(bytes, 0),
				BitConverter.ToSingle(bytes, 4),
				BitConverter.ToSingle(bytes, 8),
        BitConverter.ToSingle(bytes, 12));
	}

	private static short SerializeChar(StreamBuffer outStream, object customObj) {
		outStream.Write(new[]{ (byte)((char)customObj) }, 0, 1);
		return 1;
	}

	private static object DeserializeChar(StreamBuffer inStream, short Length) {
		var bytes = new Byte[1];
		inStream.Read(bytes, 0, 1);

		return (char)bytes[0];
	}

  private static short SerializeEntityState(StreamBuffer outstream, object customObj){
    var vo = (PhysicsEntityManager.EntityState)customObj;

    MemoryStream ms = new MemoryStream(5);
    ms.WriteByte(vo.mask);
    ms.Write(BitConverter.GetBytes(vo.id), 0, 4);

    // player
    if (vo.mask == 1){
      ms.Capacity = 5 + 8 + 4 + 8 + 4;

      ms.Write(BitConverter.GetBytes(vo.position.x), 0, 4);             // 8
      ms.Write(BitConverter.GetBytes(vo.position.z), 0, 4);

      ms.Write(BitConverter.GetBytes(vo.rotation.eulerAngles.y), 0, 4); // 4

      ms.Write(BitConverter.GetBytes(vo.velocity.x), 0, 4);             // 8
      ms.Write(BitConverter.GetBytes(vo.velocity.z), 0, 4);

      ms.Write(BitConverter.GetBytes(vo.angularVelocity.y), 0, 4);      // 4
    } 
    // default
    else {
      ms.Capacity = 5 + 12 + 16 + 12 + 12;

      ms.Write(BitConverter.GetBytes(vo.position.x), 0, 4);         // 12
      ms.Write(BitConverter.GetBytes(vo.position.y), 0, 4);
      ms.Write(BitConverter.GetBytes(vo.position.z), 0, 4);

      ms.Write(BitConverter.GetBytes(vo.rotation.x), 0, 4);         // 12
      ms.Write(BitConverter.GetBytes(vo.rotation.y), 0, 4);
      ms.Write(BitConverter.GetBytes(vo.rotation.z), 0, 4);
      ms.Write(BitConverter.GetBytes(vo.rotation.w), 0, 4);

      ms.Write(BitConverter.GetBytes(vo.velocity.x), 0, 4);         // 12
      ms.Write(BitConverter.GetBytes(vo.velocity.y), 0, 4);
      ms.Write(BitConverter.GetBytes(vo.velocity.z), 0, 4);

      ms.Write(BitConverter.GetBytes(vo.angularVelocity.x), 0, 4);  // 12
      ms.Write(BitConverter.GetBytes(vo.angularVelocity.y), 0, 4);
      ms.Write(BitConverter.GetBytes(vo.angularVelocity.z), 0, 4);
    }

    var l = ms.Length;
    outstream.Write(ms.ToArray(), 0, (int)l);
    return (short)l;
  }

  private static object DeserializeEntityState(StreamBuffer inStream, short Length) {
    var mask = inStream.ReadByte();

    // player
    if (mask == 1){
      var bytes = new byte[4 + 8 + 4 + 8 + 4];
      inStream.Read(bytes, 0, 4 + 8 + 4 + 8 + 4);

      return new 
			PhysicsEntityManager.EntityState{
        id = BitConverter.ToInt32(bytes, 0),
        mask = (byte)mask,
        position = new Vector3(BitConverter.ToSingle(bytes, 4), 0f, BitConverter.ToSingle(bytes, 8)),
        rotation = Quaternion.Euler(0f, BitConverter.ToSingle(bytes, 12), 0f),
        velocity = new Vector3(BitConverter.ToSingle(bytes, 16), 0f, BitConverter.ToSingle(bytes, 20)),
        angularVelocity = new Vector3(0f, BitConverter.ToSingle(bytes, 24), 0f),
      };
    } 
    // default
    else {
      var bytes = new byte[4 + 12 + 16 + 12 + 12];
		  inStream.Read(bytes, 0, 4 + 12 + 16 + 12 + 12);

      return new 
			PhysicsEntityManager.EntityState{
        id = BitConverter.ToInt32(bytes, 0),
        mask = (byte)mask,
        position = new Vector3(BitConverter.ToSingle(bytes, 4), BitConverter.ToSingle(bytes, 8), BitConverter.ToSingle(bytes, 12)),
        rotation = new Quaternion(BitConverter.ToSingle(bytes, 16), BitConverter.ToSingle(bytes, 20), BitConverter.ToSingle(bytes, 24), BitConverter.ToSingle(bytes, 28)),
        velocity = new Vector3(BitConverter.ToSingle(bytes, 32), BitConverter.ToSingle(bytes, 36), BitConverter.ToSingle(bytes, 40)),
        angularVelocity = new Vector3(BitConverter.ToSingle(bytes, 44), BitConverter.ToSingle(bytes, 48), BitConverter.ToSingle(bytes, 52)),
      };
    }


    

		
	}
}
