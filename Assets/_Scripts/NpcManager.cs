using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcManager : MonoBehaviour
{
    public static NpcManager instance;
    public NpcController npcPrefab;

    private void Awake()
    {
        instance = this;
    }

    public void SetNpcs(Component sender, object data)
    {
        int npcQuantity = (int)data;

        for (int i = 0; i < npcQuantity; i++)
        {
            SpawnNpc();
        }
    }

    private void SpawnNpc()
    {
        NpcController newNpc = Instantiate(npcPrefab);

        Debug.Log("Spawn npc");
    }
}
