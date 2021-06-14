using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayer : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform[] spawnpoints;

    private void Start()
    {
        Vector3 randomPosition = spawnpoints[Random.Range(0, spawnpoints.Length)].position;
        PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
    }
}
