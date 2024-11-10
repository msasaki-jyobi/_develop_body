using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace develop_body
{


    public enum EShakeType
    {
        SPRING,
        EASING
    }

    public enum EShakeMoveType
    {
        Normal = 0,
        Move = 1,
        TimeMove = 2
    }

    public class ShakeController : MonoBehaviour
    {
        // カメラシェイク用
        [Header(":::Shakeするオブジェクトにアタッチ:::")]
        // Shakeタイプを指定
        public EShakeType ShakeType;
        // 座標移動の仕方を指定
        public EShakeMoveType ShakeMoveType;
        // Shakeの動きを許可
        public bool ShakeFlg;
        // 大きくすると振幅が増える
        public float Spring = 0.05f;
        // 小さくすると振動する時間が短くなる(減衰しやすく）
        public float Damping = 0.95f;
        public float Easing = 0.35f;
        public float Distance = 0.02f;

        [Header(":::ShakeFlg OFF：Controllerへ追従：ShakeFlg ON：戻る）:::")]
        public GameObject Controller;
        public float FollowSpeed = 5f; // 追従速度

        // 座標移動用
        private Vector3 _v;
        private GameObject _moveTargetObject;
        private float _moveSpanTime;
        private float _moveTimer;

        [Header("ShakeActionMove() 衝突など")]
        public bool LKeyDebug;
        public Vector3 TranslateDirectionMin;
        public Vector3 TranslateDirectionMax;
        public float DirectionTime = 0.2f;

        private Vector3 _defaultPosition;
        private Tween currentTween;

        public MonoBehaviour DynamicBone;

        private void Start()
        {
            _defaultPosition = transform.localPosition;
        }


        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                if (LKeyDebug)
                    ShakeActionMove();
            }

            // 移動上限
            Vector3 pos = transform.localPosition;
            pos.x = Mathf.Clamp(pos.x, _defaultPosition.x - Distance, _defaultPosition.x + Distance);
            pos.y = Mathf.Clamp(pos.y, _defaultPosition.y - Distance, _defaultPosition.y + Distance);
            pos.z = Mathf.Clamp(pos.z, _defaultPosition.z - Distance, _defaultPosition.z + Distance);
            transform.localPosition = pos;

            if (!ShakeFlg)
            {
                // 徐々にControllerの位置を追いかける
                transform.position = Vector3.Lerp(transform.position, Controller.transform.position, FollowSpeed * Time.deltaTime);
                return;
            }

            // Shake
            switch (ShakeType)
            {
                case EShakeType.SPRING:
                    SpringShake();
                    break;
                case EShakeType.EASING:
                    EasingShake();
                    break;
            }

            //　移動タイプに合わせて移動する
            switch (ShakeMoveType)
            {
                case EShakeMoveType.TimeMove:
                    TypeMoveTime();
                    break;
                case EShakeMoveType.Move:
                    TypeMove();
                    break;
            }




            // デバッグ用 Shake
            //if(Input.GetKey(KeyCode.A))
            //{
            //    transform.Translate(translateDirection * Time.deltaTime);
            //}
            //if (Input.GetKeyDown(KeyCode.D))
            //{
            //    transform.Translate(translateDirection * Time.deltaTime);
            //    if (!scaleOption) return;
            //    transform.localScale = changeScale;
            //}
        }



        public void SpringShake()
        {
            if (!ShakeFlg) return;

            Vector3 diff = _defaultPosition - transform.localPosition;
            _v += diff * Spring;
            _v *= Damping;
            transform.localPosition += _v;
        }

        public void EasingShake()
        {
            if (!ShakeFlg) return;

            Vector3 diff = _defaultPosition - transform.localPosition;
            _v = diff * Easing;
            transform.localPosition += _v;
        }

        /// <summary>
        /// 一定時間おきに座標移動
        /// </summary>
        private void TypeMove()
        {
            if (!ShakeFlg) return;
            transform.localPosition = _moveTargetObject.transform.localPosition;
        }

        /// <summary>
        /// 常に座標移動します
        /// </summary>
        private void TypeMoveTime()
        {
            if (!ShakeFlg) return;
            _moveTimer += Time.deltaTime;
            if (_moveTimer <= _moveSpanTime)
            {

                //transform.position = moveTargetObject.transform.position;
                //transform.position = Vector3.MoveTowards(transform.position, moveTargetObject.transform.position, 1f * Time.deltaTime);

                Vector3 diff = transform.localPosition - _moveTargetObject.transform.localPosition;
                transform.Translate((diff * 3) * Time.deltaTime);
            }
            if (_moveTimer >= _moveSpanTime * 2)
            {
                _moveTimer = 0;
            }
        }




        /// <summary>
        /// 衝突した時の挙動
        /// </summary>
        public void ShakeActionMove()
        {
            //float x = Random.Range(-TranslateDirectionMin.x, TranslateDirectionMax.x);
            //float y = Random.Range(-TranslateDirectionMin.y, TranslateDirectionMax.y);
            //float z = Random.Range(-TranslateDirectionMin.z, TranslateDirectionMax.z);
            //Vector3 puru = new Vector3(x, y, z);
            //transform.Translate(puru * Time.deltaTime);
            MoveToOffset(TranslateDirectionMin, TranslateDirectionMax, DirectionTime);
        }

        /// <summary>
        /// 移動先のオブジェクト座標を設定します
        /// </summary>
        public void ShakeTargetObjectSet(GameObject _obj)
        {
            _moveTargetObject = _obj;
        }

        /// <summary>
        /// 一定時間おきに指定座標へ移動するタイマーを設定します。（ループモーション用
        /// </summary>
        /// <param name="time"></param>
        public void ShakeMoveTimeSet(float time)
        {
            _moveSpanTime = time;
        }

        /// <summary>
        /// 動き方を変更します
        /// </summary>
        public void ShakeTypeChange(int i)
        {
            switch (i)
            {
                case 0:
                    ShakeMoveType = EShakeMoveType.Normal;
                    break;
                case 1:
                    ShakeMoveType = EShakeMoveType.Move;
                    break;
                case 2:
                    ShakeMoveType = EShakeMoveType.TimeMove;
                    break;
            }
        }

        /// <summary>
        /// サイズを変更します
        /// </summary>
        /// <param name="size"></param>
        public void ShakeScaleChange(float size)
        {
            transform.localScale = new Vector3(size, size, size);
        }

        /// <summary>
        /// プルプルの機能をON/OFFします
        /// </summary>
        /// <param name="flg"></param>
        public void ShakeFlgChange(bool flg)
        {
            ShakeFlg = flg;
        }

        /// <summary>
        /// 親オブジェクトを切り替えます
        /// </summary>
        public void ShakeSetParentObject(GameObject obj)
        {
            transform.parent = obj.transform;
        }
        /// <summary>
        /// 移動状態を初期位置に戻す
        /// </summary>
        public async void Reset()
        {
            ShakeFlg = true;
            await UniTask.Delay(10);
            ShakeFlg = false;
        }

        public async void MoveToOffset(Vector3 min, Vector3 max, float time)
        {
            // 現在のTweenがあれば停止
            if (currentTween != null && currentTween.IsActive())
            {
                currentTween.Kill();
            }

            if (DynamicBone != null)
                DynamicBone.enabled = false;

            // 現在のローカル座標を取得
            Vector3 currentPosition = _defaultPosition;

            // minかmaxのどちらかをランダムに選択し、現在の座標に加算
            Vector3 targetPosition = currentPosition + (Random.value < 0.5f ? min : max);

            // 新しいTweenを開始し、変数に格納
            currentTween = transform.DOLocalMove(targetPosition, time).SetEase(Ease.Linear);

            await UniTask.Delay((int)(1000 * time));
            
            if (DynamicBone != null)
                DynamicBone.enabled = true;

        }
    }
}