using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayThunk : MonoBehaviour
{
    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        source.volume = collision.relativeVelocity.magnitude * 2f;
        source.Play();
    }
}
