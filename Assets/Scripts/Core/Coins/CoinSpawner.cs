using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] private RespawnCoin respawnCoinPrefab;
    [SerializeField] private int maxCoins = 20;
    [SerializeField] private int coinValue = 10;
    [SerializeField] private Vector2 xSpawnRange;
    [SerializeField] private Vector2 ySpawnRange;
    [SerializeField] private LayerMask layerMask;
    private float coinRadius;
    private Collider2D[] coinBuffer = new Collider2D[1];
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        coinRadius = respawnCoinPrefab.GetComponent<CircleCollider2D>().radius;
        for (int i = 0; i < maxCoins; i++)
        {
            SpawnCoin();
        }
    }
    public override void OnNetworkDespawn()
    {
        
    }

    private void SpawnCoin()
    {
        RespawnCoin coinInstance = Instantiate(respawnCoinPrefab, GetSpawnPosition(), Quaternion.identity);
        coinInstance.SetCoinValue(coinValue);
        coinInstance.GetComponent<NetworkObject>().Spawn();
        coinInstance.OnCollected += HandleCoinCollected;
    }

    private void HandleCoinCollected(RespawnCoin obj)
    {
        obj.transform.position = GetSpawnPosition();
        obj.Reset();
    }

    private Vector2 GetSpawnPosition()
    {
        float x = 0;
        float y = 0;
        while (true)
        {
            x = Random.Range(xSpawnRange.x, xSpawnRange.y);
            y = Random.Range(ySpawnRange.x, ySpawnRange.y);
            Vector2 spawnPosition = new Vector2(x, y);
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPosition, coinRadius, coinBuffer, layerMask);
            if (numColliders == 0) return spawnPosition;
        }
    }
}
