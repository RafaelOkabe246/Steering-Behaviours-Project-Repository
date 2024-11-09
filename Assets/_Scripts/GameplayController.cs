using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public static GameplayController instance;
    public GameEvent startEvent;
    public GameEvent npcPego;

    public delegate void OnTimerEnded();
    public OnTimerEnded onTimerEnded;

    public delegate void OnGameEnded();
    public OnGameEnded onGameEnded;

    public float maxTimer = 60;
    [SerializeField]private float time = 0;

    private bool IsGameRunning;

    private void Awake()
    {
        instance = this;
    }

   

    private void Update()
    {
        if (!IsGameRunning)
            return;
        time += Time.deltaTime;
        if(time >= maxTimer)
        {
            onTimerEnded.Invoke();
        }

    }

    public void OnStartGameplay(string npcQuantity)
    {
        int _npcQuantity = int.Parse(npcQuantity);

        startEvent.TriggerListener(this, _npcQuantity);
        IsGameRunning = true;
    }

    public void TriggerEndGame()
    {
        onGameEnded.Invoke();
    }
}
