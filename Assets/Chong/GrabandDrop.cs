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
  public Vector3 interactionOffset1;
  public Vector3 interactionOffset2;
  public float interactionRadius = 1f;

  //Variables for sound effects
  [Header("Sound effects")]
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
    //OnItemDrop();
  }

  void OnInteracting(){
    // allows you to make any script 'interactable'
    if (Input.GetKeyDown(KeyCode.E)){
      if (interactingBase != null){
        source.PlayOneShot(pickUp, 2.0f);
        interactingBase.Activate(player);
      } else if (holding) {
        // Jose: moved drop to E if nothing is being interacted with
        source.PlayOneShot(drop, 1f);
        held.RaiseEvent('d', true, NetworkManager.ServerTimeFloat);
      }
    } 

    if (Input.GetKeyDown(KeyCode.F)){
      if (interactingAlt != null){
        interactingAlt.ActivateAlt(player);
      } else if (holding) {
        // Jose: moved throw to F if nothing is being interacted with
        source.PlayOneShot(throwItem, 2.0f);
        held.RaiseEvent('t', true, NetworkManager.ServerTimeFloat);
      }
      
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
      if (interactingAlt != null && !interactingAlt.Equals(null)) interactingAlt.OnDeselectAlt(player);

      // search all colliders for an iinteractable interface
      GetInteractionBox(out var point1, out var point2, out var radius);
      var hitColliders = Physics.OverlapCapsule(point1, point2, radius, interactionLayerMask);

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
            var sqrdis = SqrMagnitudeXZ(transform.position, hitCol.transform.position);
            if (pri > basepri) goto alt;                            // higher priority. don't try
            else if (pri == basepri && sqrdis >= basedis) goto alt; // same priority, check distance

            basedis = sqrdis;
            basepri = pri;
            basehit = basetemp;
          }

          alt:;

          var alttemp = interact as IInteractableAlt;
          if (alttemp != null){
            var pri = alttemp.IsInteractableAlt(player);
            var sqrdis = SqrMagnitudeXZ(transform.position, hitCol.transform.position);;
            if (pri > altpri) goto done;                           // higher priority. don't try
            else if (pri == altpri && sqrdis >= altdis) goto done; // same priority, check distance

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
      interactingAlt?.OnSelectAlt(player);
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

  private void GetInteractionBox(out Vector3 point1, out Vector3 point2, out float size){
    point1 = transform.TransformPoint(interactionOffset1);
    point2 = transform.TransformPoint(interactionOffset2);
    size = interactionRadius;
  }

  public void OnDrawGizmosSelected() {
    GetInteractionBox(out var point1, out var point2, out var radius);
    Gizmos.color = Color.blue;
    Gizmos.DrawWireSphere(point1, radius);
    Gizmos.DrawWireSphere(point2, radius);
    Gizmos.DrawWireSphere((point1 + point2) / 2f, radius);
  }
}

