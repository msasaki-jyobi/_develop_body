using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace develop_tps
{
    public class UnitBody : MonoBehaviour
    {
        public List<BodyCollider> colliders = new List<BodyCollider>();

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
    }
}