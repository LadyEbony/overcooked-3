using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using TMPro;

public class score : MonoBehaviour
{
    public TMP_Text Text;

    // Start is called before the first frame update
    void Start()
    {
        //Text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Text.text = "Score: " + ((int)Timer.score).ToString();
    }
}
