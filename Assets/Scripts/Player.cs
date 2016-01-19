using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

  public Rigidbody bullet;
  public float bulletSpeed = 1f;
  public float kickSpeed = 1f;

  private Vector3 dragStart;
  private int dragThreshold = 9;
  private Vector3 shot1;
  private Vector3 shot2;
  private int shotsDeclared = 0;

  // Use this for initialization
  void Start () {}

  // Update is called once per frame
  void Update () {
    if (Input.GetMouseButtonDown(0)) {
      OnMouseDown();
    } else if (Input.GetMouseButtonUp(0)) {
      OnMouseUp();
    }
  }

  void OnMouseDown () {
    dragStart = Input.mousePosition;
  }

  void OnMouseUp () {
    var dragVector = Input.mousePosition - dragStart;
    dragVector.Normalize();
    if (Vector3.Distance(dragStart, Input.mousePosition) < dragThreshold) {
      Debug.Log("No movement");
      return;
    }

    dragVector = Quaternion.Euler(90, 0, 0) * dragVector;

    DeclareShot(dragVector);
  }

  void DeclareShot (Vector3 shotVector) {
    if (shotsDeclared == 0) {
      shot1 = shotVector;
      shotsDeclared++;
    } else {
      shot2 = shotVector;
      shotsDeclared = 0;
      FireShots();
    }
  }

  void FireShots () {
    FireBullet(shot1);
    FireBullet(shot2);
    ApplyKickback();
  }

  void FireBullet (Vector3 velocity) {
    var bulletStart = transform.position + (velocity * 1.5f);
    Rigidbody bulletClone = (Rigidbody) Instantiate(bullet, bulletStart, transform.rotation);
    bulletClone.velocity = velocity * bulletSpeed;
  }

  void ApplyKickback () {
    Vector3 kickVector = -(shot1 + shot2);
    Rigidbody rigidBody = gameObject.GetComponent<Rigidbody>();
    rigidBody.velocity = kickVector * kickSpeed;
  }
}
