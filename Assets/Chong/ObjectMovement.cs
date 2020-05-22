using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    public GameObject[] wayPoints;
    int tarPosition;
    public float speed;
    float WPradius = 1;

    [Header("Sound effects")]
    public AudioSource source;
    public AudioClip horn;
    public AudioClip truck;
    public float waitTime;


    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(-90, 0, -126.3f);
        waitTime = 15.0f;
    }

    // Update is called once per frame
    void Update()
    {
        waitTime -= Time.deltaTime;

        if (Vector3.Distance(wayPoints[tarPosition].transform.position, transform.position) < WPradius)
        {
            tarPosition++;
            if (tarPosition >= wayPoints.Length)
            {
                tarPosition = 0;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, wayPoints[tarPosition].transform.position, Time.deltaTime * speed);
        //transform.rotation = transform.rotation * Quaternion.Euler(0,0,-0.12f);
        transform.RotateAround(transform.position, new Vector3(0,1,0), -8.05f * Time.deltaTime);

        if (!source.isPlaying)
        {
            source.clip = truck;
            source.volume = Random.Range(0.8f, 1);
            source.pitch = Random.Range(0.5f, 0.7f);
            source.Play();
        }

        if (waitTime < 0.0f)
        {
            source.priority = 60;
            source.volume = 0.03f;
            source.pitch = 0.5f;
            source.PlayOneShot(horn, 2.0f);
            waitTime = 60.0f;
        }
        


    }
    
}
