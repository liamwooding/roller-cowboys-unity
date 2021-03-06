﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

  public static GameManager instance = null;
  public GameObject playerGO;
  public GameObject enemyGO;

  public GameObject player;
  private Player playerScript;
  private static float playTime = 3;
  private float remainingPlayTime = playTime;

  void Awake () {
    if (instance == null) {
      instance = this;
    } else if (instance != this) {
      //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
      Destroy(gameObject);
    }
    //Sets this to not be destroyed when reloading scene
    DontDestroyOnLoad(gameObject);

    //Call the InitGame function to initialize the first level 
    InitGame();
  }

  void InitGame () {
    player = (GameObject) Instantiate(playerGO, new Vector3(0, 0, 0), Quaternion.identity);
    playerScript = (Player) player.GetComponent(typeof(Player));
    player.transform.position = playerScript.startPosition;
    playerScript.Ready();
  }

  //Update is called every frame.
  void Update () {
    if (remainingPlayTime > 0) {
      remainingPlayTime -= Time.deltaTime;
    }
  }

  public void StartTurn() {
    Debug.Log("New turn starts ===============");
    remainingPlayTime = playTime;
    playerScript.Ready();
  }

  public void EndTurn () {
    Debug.Log("Turn ended ++++++++++++++++++++");
    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

    foreach (GameObject enemy in enemies) {
      Enemy enemyScript = (Enemy) enemy.GetComponent("Enemy");
      enemyScript.DecideAction();
    }
    if (enemies.Length < 2) {
      Instantiate(enemyGO, GetRandomSpawnPosition(), Quaternion.identity);
    }
  }

  Vector3 GetRandomSpawnPosition () {
    float x = Random.Range(-16, 16);
    float z = Random.Range(-11, 11);
    return new Vector3(x, 1, z);
  }
}