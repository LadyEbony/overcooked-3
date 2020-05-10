using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrabandDrop : MonoBehaviour {

  public PlayerEntity player;

  public ItemEntity held => DoubleDictionary<PlayerEntity, ItemEntity>.Get(player);
  public bool holding => held != null;

  public IInteractableBase interactingBase;
  public IInteractableAlt interactingAlt;

  [Header("Interaction")]
  public LayerMask interactionLayerMask;
  public float interactionOffset = 1.5f;
  public float interactionDistance = 1f;

  //Variables for sound effects
  public AudioSource source;
  public AudioClip pickUp;
  public AudioClip drop;
  public AudioClip throwItem;

    /*
      public GameObject item;
      public Transform MC;
      public Transform holdSlot;
      public Vector3 fw;
      public float speed;

      public Transform selection;
      public Transform curSelection;
      public RaycastHit theItem;
      public Transform curItem;
      public Material defaultMaterial;

      enum status { holding, notHolding };

      status hold;

      [SerializeField] private string selectableTag = "Selectable";
      [SerializeField] private Material highlightMaterial;
      //[SerializeField] private Material defaultMaterial;

      */

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
      player = GetComponent<PlayerEntity>();

      /*
        MC = this.transform;
        holdSlot = MC.transform.Find("holdSlot");
        speed = 350.0f;
        hold = status.notHolding;

      */
    }
    
  // player entity calls the scripts now
  public void LocalUpdate(){
    IndicateSelections();
    OnInteracting();
    OnItemDrop();
  }

  void OnInteracting(){
    // allows you to make any script 'interactable'
    if (interactingBase != null && Input.GetKeyDown(KeyCode.E)){
      source.PlayOneShot(pickUp, 2.0f);
      interactingBase.Activate(player);
    } 

    if (interactingAlt != null && Input.GetKeyDown(KeyCode.F)){
      interactingAlt.ActivateAlt(player);
    } 
  }

    //###########################################################################
    void OnItemDrop()
    {
        if (Input.GetKey(KeyCode.Q) && holding)
        {
            source.PlayOneShot(drop, 1f);
            held.RaiseEvent('d', true, NetworkManager.ServerTimeFloat);
        }

        if (Input.GetKey(KeyCode.R) && holding){
            source.PlayOneShot(throwItem, 2.0f);
            held.RaiseEvent('t', true, NetworkManager.ServerTimeFloat);
        }

    }
 
    //###################################################################
    // Jose:
    // Change so the current selection points to the itementity script
    // and probably put the selection code in there
    void IndicateSelections()
    {
      // generic approach to deselect
      // extra steps cause Unity
      if (interactingBase != null && !interactingBase.Equals(null)) interactingBase.OnDeselect(player);
      if (interactingAlt != null && !interactingAlt.Equals(null)) interactingAlt.OnDeselect(player);

      // search all colliders for an iinteractable interface
      GetInteractionBox(out var center, out var size);
      var hitColliders = Physics.OverlapBox(center, size * 0.5f, Quaternion.identity, interactionLayerMask);

      IInteractableBase basehit = null;
      float basedis = float.MaxValue;
      int basepri = int.MaxValue - 1;

      IInteractableAlt althit = null;
      float altdis = float.MaxValue;
      int altpri = int.MaxValue - 1;

      foreach (Collider hitCol in hitColliders) {
        // moved interactable check here
        var interact = hitCol.transform.GetComponentInParent<IInteractable>();
        if (interact != null){
          var basetemp = interact as IInteractableBase;
          if (basetemp != null){
            var pri = basetemp.IsInteractable(player);
            float sqrdis;
            if (pri > basepri) goto alt;  // higher priority. don't try
            else if (pri == basepri){
              // same priority, don't try if farther away
              sqrdis  = SqrMagnitudeXZ(transform.position, hitCol.transform.position);
              if (sqrdis >= basedis) goto alt;
            } else {
              sqrdis = float.MaxValue;
            }

            basedis = sqrdis;
            basepri = pri;
            basehit = basetemp;
          }

          alt:;

          var alttemp = interact as IInteractableAlt;
          if (alttemp != null){
            var pri = alttemp.IsInteractableAlt(player);
            float sqrdis;
            if (pri > altpri) goto done;  // higher priority. don't try
            else if (pri == altpri){
              // same priority, don't try if farther away
              sqrdis  = SqrMagnitudeXZ(transform.position, hitCol.transform.position);
              if (sqrdis >= altdis) goto done;
            } else {
              sqrdis = float.MaxValue;
            }

            altdis = sqrdis;
            altpri = pri;
            althit = alttemp;
          }

          done:;

        }
      }

      interactingBase = basehit;
      interactingAlt = althit;

      // generic approach to select
      interactingBase?.OnSelect(player);
      interactingAlt?.OnSelect(player);
    }

  // normalize Y. we only care about XZ
  // this is in response to the previous bug where
  // if there an item on a cutting board
  // the cutting board could be selected for Alt
  // but a neighboring item would be selected for Base instead of the item
  // probably revisit this later
  private float SqrMagnitudeXZ(Vector3 a, Vector3 b){
    a.y = 0f;
    b.y = 0f;
    return Vector3.SqrMagnitude(a - b);
  }

  private void GetInteractionBox(out Vector3 center, out Vector3 size){
    center = transform.position + transform.forward * interactionOffset;
    size = Vector3.one * interactionDistance;
  }

  public void OnDrawGizmosSelected() {
    GetInteractionBox(out var center, out var size);
    Gizmos.color = Color.blue;
    Gizmos.DrawWireCube(center, size);
  }
}

