using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameplayController : MonoBehaviour
{
    public static GameplayController instance;
    public GameEvent startEvent;
    public GameEvent endTimer;
    //public GameEvent onGameEnded;

    public float maxTimer = 60;
    [SerializeField]private float time = 0;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI winnerText;

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
            endTimer.TriggerListener(this, "");
            time = 0;
        }
        timerText.text = $"Time: {time}";
    }

    public void OnStartGameplay(string npcQuantity)
    {
        int _npcQuantity = int.Parse(npcQuantity);

        startEvent.TriggerListener(this, _npcQuantity);
        IsGameRunning = true;
    }

    public void TriggerEndGame()
    {
        //onGameEnded.TriggerListener(this,"");
        winnerText.text = "Winner: " + NpcManager.currentPegador.name;


    }
}
