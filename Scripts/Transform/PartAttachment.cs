using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.LowLevel;
using System.Collections.Generic;
using develop_common;
using UniRx;
using Common.Unit;

public class PartAttachment : MonoBehaviour
{
    // インスペクターで指定する部位（例：HeadColliderやHipsCollider）
    public Transform PartToAttach; // プレイヤーの特定の部位を設定

    // プレイヤー全体（ルートオブジェクト）
    public Transform PlayerRoot; // プレイヤー全体のルートオブジェクト
    public Transform Entity; // ルートオブジェクト
    //public UnitParameter UnitParameter; // ルートオブジェクト

    // ターゲットオブジェクト（例：敵の吸い込み口）
    public Transform TargetObject;

    public List<StringKeyGameObjectValuePair> BodyTargets = new List<StringKeyGameObjectValuePair>();


    // 座標と回転のオフセット（インスペクターで調整可能）
    public Vector3 PositionOffset;
    public Vector3 RotationOffset;

    // デバッグ用フラグ
    public bool IsDebugK;

    // 連打対策のためのフラグ
    private bool isAttaching = false;
    private CancellationTokenSource cancellationTokenSource;

    // 2回目の処理のためのディレイ（ms単位）
    public int delayBetweenSteps = 10;

    void OnDisable()
    {
        cancellationTokenSource?.Cancel();
    }
    private void Start()
    {

    }

    private void Update()
    {
        if (IsDebugK && Input.GetKeyDown(KeyCode.K))
        {
            AttachPlayerToTarget().Forget();
        }

        //if (transform.parent != Entity &&
        //    UnitParameter.KousokuValue.Value > 0 && UnitParameter.ParentTimer <= 0)
        //{
        //    // 離れてたら再実行
        //    if (TargetObject != null)
        //        if (PartToAttach != null)
        //            if (Vector3.Distance(TargetObject.transform.position, PartToAttach.transform.position) < (PartToAttach.transform.position + PositionOffset).magnitude)
        //                AttachPlayerToTarget().Forget();
        //}
        //else if (UnitParameter.ParentTimer > 0)
        //{
        //    UnitParameter.SetEntityParent();
        //}

        //if (transform.parent != null && transform.parent.gameObject == null)
        //{
        //    // 親オブジェクトが破壊された場合に、自分を切り離す
        //    transform.SetParent(Entity);
        //    Debug.Log("Parent destroyed, detaching A object.");
        //}


    }

    /// <summary>
    /// オブジェクトを上書きしてアタッチする
    /// </summary>
    public async void AttachTarget(Transform partToAttach, string AttachName, Vector3 positionOffset = default, Vector3 rotationOffset = default)
    {
        foreach (var target in BodyTargets)
        {
            if (target.Key == AttachName)
                PartToAttach = target.Value.transform;
        }
        TargetObject = partToAttach;
        PositionOffset = positionOffset;
        RotationOffset = rotationOffset;

        AttachPlayerToTarget().Forget();
        await UniTask.Delay(10);
        AttachPlayerToTarget().Forget();
    }

    public void ForgetAttach()
    {
        AttachPlayerToTarget().Forget();
    }

    // プレイヤーをターゲットにくっつけるメソッド
    public async UniTask AttachPlayerToTarget()
    {
        if (isAttaching) return; // 連打防止
        isAttaching = true;
        cancellationTokenSource = new CancellationTokenSource();

        // 1回の処理で2回分のアタッチ処理を行う
        await AttachInTwoStepsWithDelay(cancellationTokenSource.Token);

        isAttaching = false;
    }

    // 2回の処理を行い、間にディレイを入れる
    private async UniTask AttachInTwoStepsWithDelay(CancellationToken token)
    {
        // Step 1: 最初の位置合わせ
        AttachDirectly();

        // 少しのディレイを入れる（ここでは100ms）
        await UniTask.Delay(delayBetweenSteps, cancellationToken: token);

        // Step 2: 再度位置と回転を修正
        AttachDirectly();
    }

    private void AttachDirectly()
    {
        // ターゲットオブジェクトの座標・回転を取得
        Vector3 targetPosition = TargetObject.position + TargetObject.TransformDirection(PositionOffset);
        Quaternion targetRotation = TargetObject.rotation * Quaternion.Euler(RotationOffset);

        // 基準となる部位（PartToAttach）の現在のワールド座標と回転を取得
        Vector3 partWorldPosition = PartToAttach.position;
        Quaternion partWorldRotation = PartToAttach.rotation;

        // PlayerRootの位置を移動させる際に、部位の位置がターゲットにぴったり合うように移動
        Vector3 positionDifference = targetPosition - partWorldPosition;
        PlayerRoot.position += positionDifference;

        // 部位の回転がターゲットの回転にぴったり合うように、PlayerRootの回転を更新
        Quaternion rotationDifference = targetRotation * Quaternion.Inverse(partWorldRotation);
        PlayerRoot.rotation = rotationDifference * PlayerRoot.rotation;

        // ターゲットオブジェクトの子オブジェクトとしてセット
        PlayerRoot.SetParent(TargetObject);
    }

    //// インスペクターでの調整時にすぐに結果が見えるようにするため
    //private void OnValidate()
    //{
    //    if (PlayerRoot != null && TargetObject != null && PartToAttach != null)
    //    {
    //        AttachDirectly(); // インスペクターでの調整時に即座に反映
    //    }
    //}

    public GameObject GetBody(string attachName)
    {
        foreach (var target in BodyTargets)
        {
            if (target.Key == attachName)
                return target.Value;
        }
        return null;
    }

}
