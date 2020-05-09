using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabandDrop : MonoBehaviour {

  public ItemEntity held;
  public PlayerEntity player;

  public bool holding => held != null;

  public IInteractable interacting;

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
    if (Input.GetKey(KeyCode.E) && interacting != null){
      //playing pick up sound
      source.PlayOneShot(pickUp, 2.0f);
      interacting.Activate(player);
    }
  }

    //###########################################################################
    void OnItemDrop()
    {
        if (Input.GetKey(KeyCode.Q) && holding)
        {
            source.PlayOneShot(drop, 1f);
            held.RaiseEvent('d', true, NetworkManager.ServerTimeFloat, player.authorityID, player.entityID);
        }

        if (Input.GetKey(KeyCode.R) && holding){
            source.PlayOneShot(throwItem, 2.0f);
            held.RaiseEvent('t', true, NetworkManager.ServerTimeFloat, player.authorityID, player.entityID);
        }

    }
 
    //###################################################################
    // Jose:
    // Change so the current selection points to the itementity script
    // and probably put the selection code in there
    void IndicateSelections()
    {
      // generic approach to deselect
      if (interacting != null){
        interacting.OnDeselect(player);
      }

      // generic approach to select
      // should result an array of hits, and pick the closest hit that's interactable

      //RaycastHit hit;
      GetInteractionBox(out var center, out var size);
      var hitColliders = Physics.OverlapBox(center, size * 0.5f, Quaternion.identity, interactionLayerMask);

      IInteractable hit = null;
      float minDis = float.MaxValue;
      foreach (Collider hitCol in hitColliders) {
        // moved interactable check here
        var interact = hitCol.transform.GetComponentInParent<IInteractable>();
        if (interact != null && interact.IsInteractable(player)){
          var sqrdis = Vector3.SqrMagnitude(transform.position - hitCol.transform.position);
          if (sqrdis < minDis){
            minDis = sqrdis;
            hit = interact;
          }
        }

        /*
          var distance = Vector3.Distance(this.transform.position, hitCol.transform.position);

          if (distance < minDis)
          {
              minDis = distance;
              hit = hitCol;
          }
        */
      }
      interacting = hit;

      if (interacting != null) { 
        interacting.OnSelect(player);
      }
     
        // I moved the renderer to ItemEntity

        /*

          if (curSelection != null)
          {
              var selectionRenderer = curSelection.GetComponent<Renderer>();
              selectionRenderer.material = defaultMaterial;
              curSelection = null;
          }

          if (Physics.Raycast(this.transform.position, this.transform.TransformDirection(Vector3.forward), out theItem, 1.5f) && (hold == status.notHolding))
          {
              selection = theItem.transform;

              if (selection.CompareTag(selectableTag))
              {
                  var selectionRender = selection.GetComponent<Renderer>();

                  if (selectionRender != null)
                  {
                      //New
                      defaultMaterial = selection.GetComponent<Renderer>().material;
                      selectionRender.material = highlightMaterial;
                  }
                  curSelection = selection;
              }
          }
          else
          {
              Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.magenta);
              Debug.Log("Did not Hit");
          }

        */
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

