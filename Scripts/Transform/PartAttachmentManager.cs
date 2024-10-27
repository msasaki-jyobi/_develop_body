using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PartAttachmentManager : SingletonMonoBehaviour<PartAttachmentManager>
{
    //public PartAttachment UnitA;
    public ReactiveProperty<GameObject> ParentEnemy = new ReactiveProperty<GameObject>();

    public void SetParentEnemy(GameObject enemy)
    {
        ParentEnemy.Value = enemy;
    }

}
