using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabandDrop : MonoBehaviour
{
    public GameObject item;
    public GameObject mainCharactor;
    public Transform MC;
    public Vector3 offset;


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
        mainCharactor = GameObject.Find("FPSController");
        MC = mainCharactor.transform;
        offset = new Vector3(0.5f, 0.0f, 0.0f);
        hold = status.notHolding;
        item.GetComponent<Rigidbody>().useGravity = true;
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
            item.transform.position = MC.transform.position + offset;
            item.transform.rotation = MC.transform.rotation;
            item.transform.parent = mainCharactor.transform;
            hold = status.holding;
        }

        if (Input.GetKey(KeyCode.Q) && (hold == status.holding))
        {
            item.GetComponent<Rigidbody>().useGravity = true;
            item.GetComponent<Rigidbody>().isKinematic = false;
            item.transform.position = MC.transform.position + offset;
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

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out theItem, 1.5f))
        {
            //selectedItem = theItem.transform.gameObject.name;


            selection = theItem.transform;
            //var selection = theItem.transform;

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

