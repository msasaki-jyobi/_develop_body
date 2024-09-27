using System;
using System.Collections;
using UnityEngine;

namespace develop_body
{
    public enum ECommandType
    {
        Item,
        PlayerMotion,
        PlayerMove,
        CameraMove
    }

    [Serializable]
    public class CommandInfo
    {
        public ECommandType CommandType; // コマンドの種類

        // Common
        public float StartRecordTime; // 行動開始時間
        public string TargetParentName; // 親オブジェクト名

        // Common Motion
        public string StateName;
        public float FrameLate = 30;
        public bool ApplyRootMotion;

        // Item
        public string itemName;
        public Vector3 StartLocalPosition; // 目的地のローカル座標
        public Vector3 LocalRotation; // ローカルローテーション
        public Vector3 TargetPosition; // 目的地のローカル座標
        public float Size; // Vector3.one * Size
        public float LifeTime;
        public float HitSpan;
        public float DurationTime;
        public float MotionSpeed;
        public GameObject SummonedObject;
        public bool IsItemMotion;

        // 

        // 実行済みか？
        public bool IsPlayed;
        //public int StartKousokuValue = 15;
        //[Space(10)]
        //public bool IsHitMotion;
        //public string HitStateName;
        //[Space(10)]
        //public bool IsHitCommand;
        ////public CommandData HitCommandData;
        //[Space(10)]
        //public int HitDamage = 5;
        //[Space(10)]
        //[Space(10)]

        public void OnParameterSet(string addressableName, float startRecordTime, string serchParentName, Vector3 pos, Vector3 rot, float size, float lifeTime, float span, float duration, float enemyMotionSpeed)
        {
            itemName = addressableName;
            // 時間
            StartRecordTime = startRecordTime;
            // 親オブジェクト
            TargetParentName = serchParentName;
            // 目的地設定（ローカル）
            TargetPosition = pos;
            // 回転を設定
            LocalRotation = rot;
            // サイズ
            Size = size;
            // 生存時間
            LifeTime = lifeTime;
            // ヒットスパン
            HitSpan = span;
            // 接近時間
            DurationTime = duration;
            // 敵のモーション速度
            MotionSpeed = enemyMotionSpeed;
        }

        // 深いコピーを行うためのメソッド
        public CommandInfo Clone()
        {
            return new CommandInfo
            {
                itemName = this.itemName,
                StartRecordTime = this.StartRecordTime,
                TargetParentName = this.TargetParentName,
                StartLocalPosition = this.StartLocalPosition,
                TargetPosition = this.TargetPosition,
                LocalRotation = this.LocalRotation,
                Size = this.Size,
                LifeTime = this.LifeTime,
                HitSpan = this.HitSpan,
                DurationTime = this.DurationTime,
                MotionSpeed = this.MotionSpeed,
                SummonedObject = this.SummonedObject, // もし複製が必要なら別途コピー処理が必要
                
                IsPlayed = this.IsPlayed,

                IsItemMotion = this.IsItemMotion,
                StateName = this.StateName,
                FrameLate = this.FrameLate,
                ApplyRootMotion = this.ApplyRootMotion,
                
                //IsHitMotion = this.IsHitMotion,
                //HitStateName = this.HitStateName,

                //StartKousokuValue = this.StartKousokuValue,
                //IsHitCommand = this.IsHitCommand,
                //HitCommandData = this.HitCommandData,
                //HitDamage = this.HitDamage
            };
        }
    }

}
