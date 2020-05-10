using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

using Type = System.Type;

public abstract class Unit : MonoBehaviour {
    
  [Header("IDs")]
  public int entityID;
  public int authorityID;

  public bool isMine => UnitEntityManager.Local.authorityID == authorityID;
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

  public void RaiseEvent(char c, bool includeLocal, params object[] parameters){
    UnitEntityManager.Local.RaiseEvent(this, c, includeLocal, parameters);
  }

  	/// <summary>
	/// Invoke a labeled character event. You should never need to use this method manually.
	/// </summary>
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public void InternallyInvokeEvent(char c, params object[] parameters) {
		tokenHandler[c].Invoke(this, parameters);
	}

	private static Dictionary<Type, Dictionary<char, MethodInfo>> handlers = new Dictionary<Type, Dictionary<char, MethodInfo>>();

	private Dictionary<char, MethodInfo> _tokenHandler;
	protected Dictionary<char, MethodInfo> tokenHandler {
		get {
			if (_tokenHandler != null) return _tokenHandler;

			var T = GetType();

      // get from reflection cache
			if (handlers.ContainsKey(T))
				return _tokenHandler = handlers[T];

      // create from scratch
			BuildTokenList(T);
			return _tokenHandler = handlers[T];
		}
	}

  static void BuildTokenList(Type T) {
		if (!T.IsSubclassOf(typeof(Unit)))
			throw new System.Exception("Cannot build a token list for a class that doesn't derive Unit");

    var th = new Dictionary<char, MethodInfo>();
    handlers.Add(T, th);

		var methods = T.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		foreach(MethodInfo methodInfo in methods) {
			var netEvent = methodInfo.GetCustomAttributes(typeof(EntityBase.NetEvent),true).FirstOrDefault() as EntityBase.NetEvent;
			if (netEvent == null) continue;

			th.Add(netEvent.token, methodInfo);
		}

    

	}

}
