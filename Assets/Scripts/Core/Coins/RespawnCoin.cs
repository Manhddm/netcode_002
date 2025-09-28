using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnCoin : Coin
{
    public event Action<RespawnCoin> OnCollected;
    private Vector3 prevPosition;

    private void Update()
    {
        if (transform.position != prevPosition)
        {
            ShowCoin(true);
        }
        prevPosition = transform.position;
    }

    public override int Collect()
    {
        
        if (!IsServer)
        {
            ShowCoin(false);
            return 0;
        }
        if (alreadyCollected) return 0;
        alreadyCollected = true;    
        OnCollected?.Invoke(this);
        return coinValue;
        
    }

    public void Reset()
    {
        alreadyCollected = false;
    }
}
