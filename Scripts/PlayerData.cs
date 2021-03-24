using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData 
{
    public int health;
    public float[] position;
    public bool hasKilledSkeleton, hasKilledMonster, hasKilledMoss, hasPlayedIntroduction, hasPlayedSaveW0rd;

    public PlayerData(Player player)
    {
        health = 5;
        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
        hasKilledSkeleton = GameManager.Instance.hasKilledSkeleton;
        hasKilledMonster = GameManager.Instance.hasKilledMonster;
        hasKilledMoss = GameManager.Instance.hasKilledMoss;
        hasPlayedIntroduction = GameManager.Instance.hasPlayedIntroduction;
        hasPlayedSaveW0rd = GameManager.Instance.hasPlayedSaveWord;
    }
}
