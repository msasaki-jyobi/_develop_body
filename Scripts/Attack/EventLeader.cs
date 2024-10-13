using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventLeader : MonoBehaviour
{
    public event Action ShotEvent;
    public int DelayTime = 1000;

    private void Start()
    {
        Shot();
    }

    public async void Shot()
    {
        await UniTask.Delay(DelayTime);
        ShotEvent?.Invoke();
    }
}
