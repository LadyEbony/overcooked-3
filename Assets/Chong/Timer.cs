using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public Text text;
    public Font end;

    public float gameTime = 90f;

    float timer;
    public bool gameEnd;

    public bool isActive = false;

    //public PlayerController MC;

    [Header("Sound effects")]
    public AudioSource source;
    public AudioClip clock;
    public AudioClip ding;

    public static int score => OrderManager.score;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    IEnumerator Start() {
    while (!NetworkManager.gameReady) yield return null;

        timer = gameTime;
        isActive = true;
        gameEnd = false;
    }

    // Update is called once per frame
    void Update()
    {
      if (!isActive) return;

        timer -= Time.deltaTime;
        
        if (timer > 120)
        {
            text.text = ">2m";
        }
        else if (timer > 60)
        {
            text.text = ">1m";
        }
        else if (timer > 10)
        {
            text.fontSize = 150;
            text.text = ((int)timer).ToString();
        }
        else if (timer >= 0)
        {
            text.fontSize = 150;
            text.color = Color.red;
            text.text = " " + ((int)timer).ToString();

            if (!source.isPlaying)
            {
                source.PlayOneShot(clock, 2.0f);
            }
        }
        else if (timer < 0)
        {
            isActive = false;
            if (gameEnd == false)
            {
                source.Stop();
                if (!source.isPlaying)
                {
                    source.volume = 0.8f;
                    source.PlayOneShot(ding, 2.0f);
                }
                gameEnd = true;
            }

            text.fontSize = 90;
            text.font = end;
            text.color = Color.white;
            text.text = "End";
            
            if (score >= 6)
            {
              StartCoroutine(LoadScene("Ending"));
            }
            else
            {
              StartCoroutine(LoadScene("TimeUp"));
            }
        }
    }

    private IEnumerator LoadScene(string scene){
      yield return new WaitForSeconds(2f);
      Destroy(NetworkManager.instance.gameObject);
      SceneManager.LoadScene(scene);
    }
}
