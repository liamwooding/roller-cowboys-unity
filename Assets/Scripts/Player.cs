using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

  public Rigidbody bullet;
  public float bulletSpeed = 1f;
  public float kickSpeed = 1f;
  public float turnTime = 0.75f;
  public float brakingTime = 0.2f;
  public float drag = 3f;

  private Rigidbody rigidBody;
  private Vector3 dragStart;
  private int dragThreshold = 9;
  private Vector3 shot1;
  private Vector3 shot2;
  private float coastingTime;
  private int shotsDeclared = 0;
  public enum State {Ready, Waiting, Moving}
  public State state;

  // Use this for initialization
  void Start () {
    rigidBody = gameObject.GetComponent<Rigidbody>();
  }

  // Update is called once per frame
  void Update () {
    if (state == State.Ready) {
      if (Input.GetMouseButtonDown(0)) {
        HandleMouseDown();
      } else if (Input.GetMouseButtonUp(0)) {
        HandleMouseUp();
      }
    } else if (state == State.Moving) {
      AdjustCoast();
    }
  }

  void HandleMouseDown () {
    Debug.Log("Mouse down");
    dragStart = Input.mousePosition;
  }

  void HandleMouseUp () {
    Debug.Log("Mouse up");
    var dragVector = Input.mousePosition - dragStart;
    dragVector.Normalize();
    if (Vector3.Distance(dragStart, Input.mousePosition) < dragThreshold) {
      Debug.Log("No movement");
      return;
    }

    dragVector = Quaternion.Euler(90, 0, 0) * dragVector;

    DeclareShot(dragVector);
  }

  public void Ready () {
    state = State.Ready;
  }

  void DeclareShot (Vector3 shotVector) {
    if (shotsDeclared == 0) {
      Debug.Log("Declaring shot 1");
      shot1 = shotVector;
      shotsDeclared++;
    } else {
      Debug.Log("Declaring shot 2");
      shot2 = shotVector;
      TakeAction();
    }
  }

  void TakeAction () {
    state = State.Moving;
    FireShots();
    coastingTime = 0f;
    GameManager.instance.EndTurn();
  }

  void WaitForNewTurn () {
    Debug.Log("Player waiting for new turn");
    state = State.Waiting;
    GameManager.instance.StartTurn();
  }

  void FireShots () {
    FireBullet(shot1);
    FireBullet(shot2);
    ApplyKickback();
    shotsDeclared = 0;
  }

  void FireBullet (Vector3 velocity) {
    var bulletStart = transform.position + (velocity * 1.5f);
    Rigidbody bulletClone = (Rigidbody) Instantiate(bullet, bulletStart, transform.rotation);
    bulletClone.velocity = velocity * bulletSpeed;
  }

  void ApplyKickback () {
    Vector3 kickVector = -(shot1 + shot2);
    rigidBody.velocity = kickVector * kickSpeed;
  }

  void AdjustCoast () {
    if (state == State.Moving && rigidBody.IsSleeping()) {
      rigidBody.drag = drag;
      WaitForNewTurn();
      return;
    } 

    coastingTime += Time.deltaTime;
    if (coastingTime >= (turnTime - brakingTime)) {
      rigidBody.drag += (Time.deltaTime * 25);
    }
  }
}
