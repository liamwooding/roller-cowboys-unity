using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, IShootable {

  public Rigidbody bullet;
  public float bulletSpeed = 1f;
  public float kickSpeed = 1f;
  public float turnTime = 0.75f;
  public float brakingTime = 0.2f;
  public float drag = 3f;
  public Vector3 startPosition = new Vector3(0, 1, 0);
  public enum State {Ready, Aiming, Waiting, Moving}
  public State state;

  private LineRenderer rightHandAim;
  private LineRenderer leftHandAim;
  private Rigidbody rb;
  private Vector3 dragStart;
  private int dragThreshold = 9;
  private Vector3 shot1;
  private Vector3 shot2;
  private float coastingTime;
  private int shotsDeclared = 0;

  // Use this for initialization
  void Start () {
    rb = gameObject.GetComponent<Rigidbody>();
    leftHandAim = GameObject.Find("Left Hand Aim").GetComponent<LineRenderer>();
    rightHandAim = GameObject.Find("Right Hand Aim").GetComponent<LineRenderer>();
  }

  // Update is called once per frame
  void Update () {
    if (state == State.Aiming) {
      if (Input.GetMouseButtonUp(0)) {
        HandleMouseUp();
      } else {
        DrawAimLine();
      }
    } else if (state == State.Ready && Input.GetMouseButtonDown(0)) {
      HandleMouseDown();
    } else if (state == State.Moving) {
      AdjustCoast();
    }
  }

  void HandleMouseDown () {
    dragStart = Input.mousePosition;
    state = State.Aiming;
  }

  void HandleMouseUp () {
    state = State.Ready;
    var dragVector = Input.mousePosition - dragStart;
    dragVector.Normalize();
    if (Vector3.Distance(dragStart, Input.mousePosition) < dragThreshold) {
      Debug.Log("No movement");
      return;
    }

    dragVector = Quaternion.Euler(90, 0, 0) * dragVector;
    DeclareShot(dragVector);
  }

  void OnCollisionEnter (Collision col) {
    if (col.gameObject.tag == "Bullet") {
      GetShot(col.gameObject);
    }
  }

  public void Ready () {
    state = State.Ready;
  }

  public void GetShot (GameObject bullet) {
    transform.position = startPosition;
    rb.velocity = new Vector3(0, 0, 0);
  }

  void DrawAimLine () {
    var dragVector = Quaternion.Euler(90, 0, 0) * (Input.mousePosition - dragStart);
    LineRenderer aimLine;
    if (shotsDeclared == 0) {
      aimLine = leftHandAim;
    } else {
      aimLine = rightHandAim;
    }
    aimLine.SetVertexCount(2);
    aimLine.SetPosition(0, transform.position);
    aimLine.SetPosition(1, dragVector - transform.position);
  }

  void ClearAimLines () {
    rightHandAim.SetVertexCount(0);
    leftHandAim.SetVertexCount(0);
  }

  void DeclareShot (Vector3 shotVector) {
    if (shotsDeclared == 0) {
      shot1 = shotVector;
      shotsDeclared++;
    } else {
      shot2 = shotVector;
      TakeAction();
    }
  }

  void TakeAction () {
    ClearAimLines();
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
    rb.velocity = kickVector * kickSpeed;
  }

  void AdjustCoast () {
    if (state == State.Moving && rb.IsSleeping()) {
      rb.drag = drag;
      WaitForNewTurn();
      return;
    } 

    coastingTime += Time.deltaTime;
    if (coastingTime >= (turnTime - brakingTime)) {
      rb.drag += (Time.deltaTime * 50);
    }
  }
}
