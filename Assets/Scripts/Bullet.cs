using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

  public float secondsToLive = 5f;

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
    Destroy(gameObject);
  }
}
