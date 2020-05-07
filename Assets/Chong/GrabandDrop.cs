using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabandDrop : MonoBehaviour {

  public ItemEntity held;
  public PlayerEntity player;

  public bool holding => held != null;

  public IInteractable interacting;
  public float interactionDistance = 1.5f;
  public LayerMask interactionLayerMask;

    public Collider[] hitColliders;
    public float minDis;

    

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
      interacting.Activate(player);
    }
  }

    //###########################################################################
    void OnItemDrop()
    {
        if (Input.GetKey(KeyCode.Q) && holding)
        {
            UnitEntityManager.Local.Drop(player, held);
        }

        if (Input.GetKey(KeyCode.R) && holding){
            UnitEntityManager.Local.Throw(player, held);
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
        interacting = null;
      }

        // generic approach to select
        // should result an array of hits, and pick the closest hit that's interactable

        //RaycastHit hit;
        Collider hit = null;

        hitColliders = Physics.OverlapBox(this.transform.position + this.transform.forward * 2, transform.localScale);
        minDis = float.MaxValue;

        foreach (Collider hitCol in hitColliders)
        {
            var distance = Vector3.Distance(this.transform.position, hitCol.transform.position);

            if (distance < minDis)
            {
                minDis = distance;
                hit = hitCol;
            }
        }

        if (hit != null) { 
      //if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, interactionDistance, interactionLayerMask)){
        interacting = hit.transform.GetComponent<IInteractable>();
        if (interacting != null && interacting.IsInteractable(player)){
          interacting.OnSelect(player);
        } else {
          interacting = null;
        }
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
}

