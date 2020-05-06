using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabandDrop : MonoBehaviour {

  public ItemEntity held;
  public PlayerEntity player;

  public bool holding => held != null;

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



    // Start is called before the first frame update
    void Start()
    {
      player = GetComponent<PlayerEntity>();

        MC = this.transform;
        holdSlot = MC.transform.Find("holdSlot");
        speed = 350.0f;
        hold = status.notHolding;
    }
    
  // player entity calls the scripts now
  public void LocalUpdate(){
    IndicateSelections();
    GrabAndDrop();
  }

    //###########################################################################
    void GrabAndDrop()
    {
        if (Input.GetKey(KeyCode.E) && !holding && (curSelection != null))
        {
          UnitEntityManager.Local.Pickup(player, curSelection.GetComponent<ItemEntity>());
        }

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
    }
}

