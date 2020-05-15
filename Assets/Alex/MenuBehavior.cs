using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBehavior : MonoBehaviour
{

    private char userStartKey = 'j';
    public Text menuText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        menuText.text = "Malding Cheetahs Present:\n\nKrazy Kitchen\n\nHit J to Join the Server!";

        if (Input.GetKeyDown(KeyCode.J)) {
            menuText.text = "";
        }
    }
}
