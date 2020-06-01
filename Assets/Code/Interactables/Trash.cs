using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : Cabient {

    public AudioSource source;
    public AudioClip trash;

    /*
    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }*/

    void Update() {
    // plate accepts
    if (item && item.isMine) {
      item.RaiseEvent('s', true);
            //source.clip = trash;
            source.PlayOneShot(trash);
    }
  }

}
