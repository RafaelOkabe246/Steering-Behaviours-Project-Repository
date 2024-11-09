using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcManager : MonoBehaviour
{
    public static NpcManager instance;
    public NpcController npcPrefab;

    public static NpcController currentPegador;

    private List<NpcController> npcs = new List<NpcController>();

    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        GameplayController.instance.onTimerEnded += EliminateCurrentPegador;
        GameplayController.instance.onTimerEnded += CheckNPCs;
    }

    private void OnDisable()
    {
        GameplayController.instance.onTimerEnded -= EliminateCurrentPegador;
        GameplayController.instance.onTimerEnded -= CheckNPCs;

    }

    public void ChangePegador(NpcController newPegador)
    {
        if (currentPegador == newPegador)
            return;

        currentPegador.ChangeState(NpcState.Fugitivo);

        currentPegador = newPegador;

        currentPegador.ChangeState(NpcState.Pegador);
    }

    public void SetNpcs(Component sender, object data)
    {
        int npcQuantity = (int)data;

        for (int i = 0; i < npcQuantity; i++)
        {
            SpawnNpc();
        }

        SetInitialPegador();
    }

    private void SetInitialPegador()
    {
        int randomIndex = Random.Range(0, npcs.Count-1);

        NpcController pegador = npcs[randomIndex];
        foreach (NpcController npc in npcs)
        {
            if(npc != pegador)
            {
                npc.ChangeState(NpcState.Fugitivo);
            }
        }
        pegador.ChangeState(NpcState.Pegador);

        currentPegador = pegador;

    }

    private void SpawnNpc()
    {
        NpcController newNpc = Instantiate(npcPrefab);
        npcs.Add(newNpc);
        newNpc.gameObject.name += " "+  npcs.Count;
        newNpc.isPlaying = true;
    }

    private void EliminateCurrentPegador()
    {
        currentPegador.SetIsPlaying(false);
    }

    private void CheckNPCs()
    {
        for (int i = 0; i < npcs.Count; i++)
        {
            if (npcs[i].isPlaying == true)
            {
                return;
            }
        }

        GameplayController.instance.TriggerEndGame();
    }
}
