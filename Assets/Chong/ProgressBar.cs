using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProgressBar : MonoBehaviour
{
    public Slider Pbar;
    //float timer;

    // Start is called before the first frame update
    void Start()
    {
        Pbar.maxValue = 10.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Pbar.value += Time.deltaTime;

        if (Pbar.value == Pbar.maxValue)
        {
            SceneManager.LoadScene("OvercookedLevel1");
        }
    }
}
