using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

  public float secondsToLive = 5f;
  public float explosionForce = 5f;
  public float explosionRadius = 10f;

    // Use this for initialization
  void Start () {
  }


  // Update is called once per frame
  void Update () {
    secondsToLive -= Time.deltaTime;
    if (secondsToLive <= 0) {
      Destroy(gameObject);
    }
  }

  void OnCollisionEnter (Collision col) {
    if (col.gameObject.tag == "Bullet") {
      Debug.Log("Making explosion");

      Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

      foreach (Collider hit in hitColliders) {
        Rigidbody hitRb = hit.gameObject.GetComponentInParent<Rigidbody>();
        if (hitRb) {
          hitRb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 0f, ForceMode.Force);
        }
      }
    }
    Destroy(gameObject);
  }
}
