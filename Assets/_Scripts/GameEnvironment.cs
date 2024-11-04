using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnvironment 
{
    private static readonly GameEnvironment instance = new GameEnvironment();
    private static GameObject[] hidingSpots;
    static GameEnvironment()
    {
        hidingSpots = GameObject.FindGameObjectsWithTag("HideSpot");
    }
    private GameEnvironment() { }
    public static GameEnvironment GetInstance { get { return instance; } }
    public GameObject[] GetHidingSpots() { return hidingSpots; }
}
