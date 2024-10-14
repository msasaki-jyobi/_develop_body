using develop_common;
using develop_timeline;
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
        public string AdditiveDamageStateName = "DAMAGE00";
        public float HitSpan = 1f;
        public ClipData HitClip;
        public GameObject HitEffect;
        public string DamageVoiceKind = "痛がる";
        [Space(10)]
        public UnitBody PlayerBody;
        public EBodyType TargetBody;
        public bool IsShot;

        private bool _isBodyHit;
        private float _hitTimer;

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
            //if (Input.GetKeyDown(KeyCode.F))
            //{
            //    Jump();
            //}

            if (IsShot)
            {
                transform.Translate(0, 0, JumpSpeed * Time.deltaTime);
            }

            _hitTimer -= Time.deltaTime;
        }

        public void Jump()
        {
            transform.LookAt(PlayerBody.GetBody(TargetBody).gameObject.transform.position);
            IsShot = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            OnHit(other.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnHit(collision.gameObject);
        }

        public void OnHit(GameObject hit)
        {
            if (hit.TryGetComponent<AnimatorStateController>(out var animatorStateController))
                animatorStateController.AnimatorLayerPlay(1, AdditiveDamageStateName, 0f);

            if (hit.TryGetComponent<UnitBody>(out var unitBody))
            {
                if (!unitBody.IsBodyDamage) return;

                if (_hitTimer <= 0)
                {
                    _hitTimer = HitSpan;
                    AudioManager.Instance.PlayOneShotClipData(HitClip);
                    UtilityFunction.PlayEffect(gameObject, HitEffect);
                    // ダメージボイス
                    InstanceManager.Instance.UnitVoice.PlayVoice(DamageVoiceKind);

                }
                if (_isBodyHit) return;
                _isBodyHit = true;
                IsShot = false;
                transform.parent = unitBody.GetDistanceBody(transform.position).gameObject.transform;
                transform.localPosition = Vector3.zero;
            }
        }
    }

}
