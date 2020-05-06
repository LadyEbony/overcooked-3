using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabandDrop : MonoBehaviour
{
    public GameObject item;
    public Transform MC;
    public Transform holdSlot;
    public Vector3 fw;
    public float speed;

    public Transform selection;
    public Transform curSelection;
    public RaycastHit theItem;
    public Transform curItem;

    enum status { holding, notHolding };

    status hold;

    [SerializeField] private string selectableTag = "Selectable";
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;



    // Start is called before the first frame update
    void Start()
    {
        MC = this.transform;
        holdSlot = MC.transform.Find("holdSlot");
        speed = 350.0f;
        hold = status.notHolding;
    }

    // Update is called once per frame
    void Update()
    {
        IndicateSelections();
        GrabAndDrop();
    }
    
    //###########################################################################
    void GrabAndDrop()
    {
        if (Input.GetKey(KeyCode.E) && (hold == status.notHolding) && (curSelection != null))
        {
            item = curSelection.transform.gameObject;
            item.GetComponent<Rigidbody>().useGravity = false;
            item.GetComponent<Rigidbody>().isKinematic = true;
            item.transform.position = holdSlot.transform.position;
            item.transform.rotation = holdSlot.transform.rotation;
            item.transform.parent = holdSlot.transform;
            hold = status.holding;
        }

        if (Input.GetKey(KeyCode.Q) && (hold == status.holding))
        {
            item.GetComponent<Rigidbody>().useGravity = true;
            item.GetComponent<Rigidbody>().isKinematic = false;
            item.transform.parent = null;
            hold = status.notHolding;
        }

        if (Input.GetKey(KeyCode.R) && (hold == status.holding)){
            fw = holdSlot.transform.forward;
            item.GetComponent<Rigidbody>().useGravity = true;
            item.GetComponent<Rigidbody>().isKinematic = false;
            item.GetComponent<Rigidbody>().AddForce(fw * speed);
            item.transform.parent = null;
            hold = status.notHolding;
        }

    }
 
    //###################################################################
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

