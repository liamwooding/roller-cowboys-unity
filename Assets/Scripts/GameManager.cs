using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

  public static GameManager instance = null;

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
    
  }

  //Update is called every frame.
  void Update () {}
}