using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace develop_body
{
    public class TargetShot : MonoBehaviour
    {
        public float JumpSpeed = 5f;
        public EventLeader Leader;
        public bool IsRandom;
        [Space(10)]
        public UnitBody PlayerBody;
        public EBodyType TargetBody;
        public bool IsShot;
        // Start is called before the first frame update
        void Start()
        {
            if (Leader != null)
                Leader.ShotEvent += Jump;

            if (PlayerBody == null)
                PlayerBody = GameObject.FindObjectOfType<UnitBody>();

            if (IsRandom)
            {
                int random = Random.Range(0, 18);
                TargetBody = (EBodyType)random;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Jump();
            }

            if (IsShot)
            {
                transform.Translate(0, 0, JumpSpeed * Time.deltaTime);
            }
        }

        public void Jump()
        {
            transform.LookAt(PlayerBody.GetBody(TargetBody).gameObject.transform.position);
            IsShot = true;
        }
    }

}
