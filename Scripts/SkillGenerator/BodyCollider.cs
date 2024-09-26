using System.Collections;
using UnityEngine;

namespace develop_body
{
    public enum EBodyType
    {
        Hips,
        Spine,
        Chest,
        UpperChest,
        Neck,
        Head,
        //RIGT
        RightShoulder,
        RightArm,
        RightHand,
        RightUpperLeg,
        RightLowerLeg,
        RightFoot,
        // LEFT
        LeftShoulder,
        LeftArm,
        LeftHand,
        LeftUpperLeg,
        LeftLowerLeg,
        LeftFoot,
    }
    public class BodyCollider : MonoBehaviour
    {
        public EBodyType bodyType;
        public GameObject ParentObject;
        public UnitBody UnitBody;
        public int BodyParameter;

        private void Start()
        {
            if (ParentObject != null)
                transform.parent = ParentObject.transform;
            else
                Debug.LogError($"ParentObjectは見つかりません：{gameObject.name}");

            //if(UnitBody != null)
            //    UnitBody.AddBodyCollider(this);
            //else
            //    Debug.LogError($"UnitBodyは見つかりません:{gameObject.name}");
        }
    }
}