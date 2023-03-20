using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform SpawnPos;
    public Transform DroppedBlockParent;
    public Transform ReadyBlockParent;
    public GameObject BlockPrefab;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void SpawnBlock()
    {
        //Spawn next block here
        if(ReadyBlockParent.transform.childCount == 0)
        {
            GameObject go = Instantiate(BlockPrefab, SpawnPos.position, Quaternion.identity);
            //Keep blocks value as name and formatted value inside text component
            go.name = GetNextSpawnValue().ToString();
            go.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = GetFormattedValue(go.name);
            go.transform.parent = ReadyBlockParent;
        }
        else
        {
            Debug.Log("Already block in queue drop it first");
        }
    }
    void DropTheBlock()
    {

    }

    public int GetNextSpawnValue()
    {
        int spawnValue = 0;
        return spawnValue;
    }
    public string GetFormattedValue(string value)
    {
        string rval = "";
        return rval;
    }

}
