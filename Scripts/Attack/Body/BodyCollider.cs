using develop_common;
using System.Collections;
using Unity.VisualScripting;
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
        public EBodyType BodyType;
        public GameObject RootObject;
        public UnitInstance UnitInstance;
        public GameObject ParentObject;



        [Header("使わない")]
        public UnitBody UnitBody;
        public int BodyParameter;

        private void Awake()
        {
            if (ParentObject != null)
                transform.parent = ParentObject.transform;
        }

        private void Start()
        {
            if (UnitBody != null)
                UnitBody.AddBodyCollider(this);

            if (UnitInstance != null)
            {
                //var pair = new StringKeyGameObjectValuePair(BodyType.ToString(), gameObject);
                var pair = new StringKeyGameObjectValuePair();
                pair.SetPair(BodyType.ToString(), gameObject);
                UnitInstance.InstanceBodys.Add(pair);
            }
        }
    }
}