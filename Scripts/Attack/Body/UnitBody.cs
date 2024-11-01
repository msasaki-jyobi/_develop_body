using develop_common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace develop_body
{
    public class UnitBody : MonoBehaviour
    {
        public List<BodyCollider> colliders = new List<BodyCollider>();

        public bool IsBodyDamage;
        public AnimatorStateController AnimatorStateController;
        public string AdditiveDamageStateName;




        public void AddBodyCollider(BodyCollider collider)
        {
            colliders.Add(collider);
        }
        /// <summary>
        /// pos から一番近いボディオブジェクトを返す
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public BodyCollider GetDistanceBody(Vector3 pos)
        {
            BodyCollider closestCollider = null;
            float minDistance = float.MaxValue;

            foreach (var collider in colliders)
            {
                float distance = Vector3.Distance(pos, collider.transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCollider = collider;
                }
            }

            return closestCollider;
        }
        /// <summary>
        /// 対象のBodyを返す
        /// </summary>
        /// <param name="targetBody"></param>
        /// <returns></returns>
        public BodyCollider GetBody(EBodyType targetBody)
        {
            foreach (var collider in colliders)
            {
                if (collider.BodyType == targetBody)
                    return collider;
            }
            return null;
        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    if (!IsBodyDamage) return;
        //    OnHit(other.gameObject);
        //}

        //private void OnCollisionEnter(Collision collision)
        //{
        //    if (!IsBodyDamage) return;
        //    OnHit(collision.gameObject);
        //}

        //public void OnHit(GameObject hit)
        //{
        //    if(hit.TryGetComponent<TargetShot>(out var targetJump))
        //    {
        //        targetJump.IsShot = false;
        //        AnimatorStateController.AnimatorLayerPlay(1, AdditiveDamageStateName, 0f);
                
        //        hit.transform.parent = GetDistanceBody(hit.transform.position).gameObject.transform;
        //        hit.transform.localPosition = Vector3.zero;
        //    }
        //}
    }
}