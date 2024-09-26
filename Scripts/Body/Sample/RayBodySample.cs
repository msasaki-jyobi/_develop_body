using develop_body;
using develop_common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayBodySample : MonoBehaviour
{
    public UnitBody UnitBody;
    public GameObject ClickEffect;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var hit = UtilityFunction.GetScreenClickObjectPosition(Camera.main, ClickEffect);
            if (hit != null)
            {
                var bodyCollider = UnitBody.GetDistanceBody(hit);
                if (bodyCollider != null)
                    Debug.Log($"{bodyCollider}‚Éƒ^ƒbƒ`. Type:{bodyCollider.bodyType}, Value:{bodyCollider.BodyParameter}");
            }
        }
           
    }
}
