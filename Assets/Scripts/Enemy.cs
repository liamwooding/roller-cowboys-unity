﻿using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

  public Rigidbody bullet;
  public float bulletSpeed = 1f;
  public GameObject player;

  void Start () {
    player = GameManager.instance.player;
  }

  void Update () {}

  public void DecideAction () {
    FireBullet(GetShotDirection());
  }

  Vector3 GetShotDirection () {
    Vector3 shotDirection = player.transform.position - transform.position;
    shotDirection.Normalize();
    return shotDirection;
  }

  void FireBullet (Vector3 velocity) {
    var bulletStart = transform.position + (velocity * 1.5f);
    Rigidbody bulletClone = (Rigidbody) Instantiate(bullet, bulletStart, transform.rotation);
    bulletClone.velocity = velocity * bulletSpeed;
  }
}
