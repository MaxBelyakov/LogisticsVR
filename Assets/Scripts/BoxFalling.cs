using UnityEngine;
using System.Collections;

public class BoxFalling : MonoBehaviour
{
    public AudioClip clip;

    void OnCollisionEnter(Collision collision)
    {
        // Check collision speed if fall fast play sound
        if (collision.relativeVelocity.magnitude > 3f)
            this.GetComponent<AudioSource>().PlayOneShot(clip);
    }

    void OnTriggerEnter(Collider collider)
    {
        // Destroy box with time delay when it in unloading reticle zone
        if (collider.gameObject.tag == "unloading reticle")
            StartCoroutine(DestroyBox());
    }

    IEnumerator DestroyBox()
    {
        yield return new WaitForSeconds(2f);

        // Destroy box
        Destroy(gameObject);

        // Send counter to info desk
        GameObject.FindGameObjectWithTag("info desk").GetComponent<InfoDesk>().delivered++;
    }
}