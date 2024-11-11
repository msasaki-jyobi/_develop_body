using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.LowLevel;
using System.Collections.Generic;
using develop_common;
using UniRx;
using DG.Tweening;
using Unity.VisualScripting;

public class PartAttachment : MonoBehaviour
{
    public UnitInstance UnitInstance;

    public Transform PlayerRoot; // プレイヤー全体のルートオブジェクト
    public Transform Entity; // ルートオブジェクト
    public Sprite FaceImage;

    [Space(10)]
    // インスペクターで指定する部位（例：HeadColliderやHipsCollider）
    public Transform PartToAttach; // プレイヤーの特定の部位を設定

    // プレイヤー全体（ルートオブジェクト）
    //public UnitParameter UnitParameter; // ルートオブジェクト

    // ターゲットオブジェクト（例：敵の吸い込み口）
    public Transform TargetObject;

    //[Header("固定化参照用")]
    //public List<StringKeyGameObjectValuePair> BodyTargets = new List<StringKeyGameObjectValuePair>();


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

    // 固定化判定
    public bool IsPull;
    // 投げ判定用（ここじゃなくてもいいかも）
    public bool IsDown;
    private Tween rotationTween; // 回転アニメーションを管理するためのTweenオブジェクト

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

        // 固定化する必要があるのに、離れている場合
        if (IsPull)
        {
            var playerPos = PlayerRoot.transform.position;
            var targetPos = TargetObject.transform.position + PositionOffset;
            if (Vector3.Distance(playerPos, targetPos) >= 0.1f)
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
        foreach (var target in UnitInstance.InstanceBodys)
        {
            if (target.Key == AttachName)
                PartToAttach = target.Value.transform;
        }
        TargetObject = partToAttach;
        PositionOffset = positionOffset;
        RotationOffset = rotationOffset;

        IsPull = true;

        AttachPlayerToTarget().Forget();
        await UniTask.Delay(10);
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

    public GameObject GetBody(string keyName)
    {
        if(UnitInstance == null)
        {
            Debug.LogError($"{keyName}, UnitInstanceはNullです. ");
            return null;
        }
        foreach (var target in UnitInstance.InstanceBodys)
        {
            if (target.Key == keyName)
                return target.Value;
        }
        return null;
    }
    /// <summary>
    /// 親オブジェクトを解除する
    /// PullOFFの攻撃を受けたタイミングで呼ばれ、オブジェクトが解除される。
    /// 起き上がりをどうするか？
    /// </summary>
    public async void SetEntityParent()
    {
        if (PlayerRoot == null) return;
        if (Entity == null) return;

        IsPull = false;
        PlayerRoot.transform.parent = Entity.transform;

        // すでに回転中のアニメーションがある場合はキャンセル
        rotationTween?.Kill();

        // 目標の回転値を設定 (0, 現在のY軸, 0)
        Vector3 targetRotation = new Vector3(0, PlayerRoot.transform.rotation.eulerAngles.y, 0);

        // DoTweenで回転アニメーションを作成
        rotationTween = PlayerRoot.transform.DORotate(targetRotation, 0.3f)
            .SetEase(Ease.Linear)    // 線形補間
            .SetLink(PlayerRoot.gameObject);    // ゲームオブジェクトにリンク（破棄時に自動キャンセル）

        // アニメーションが完了するのを待機
        await rotationTween.AsyncWaitForCompletion();
    }

    // 敵の技を発動するメソッド（引数を使用）
    public void ActivateAbility(Transform playerHead, Transform enemyRoot, Transform hammer, Vector3 offset)
    {
        // B を Aにくっつける
        // 頭　を　掃除機　にくっつける
        var A = TargetObject;
        var B = PartToAttach;
        var C = PlayerRoot;

        A = playerHead;
        B = hammer;
        C = enemyRoot;

        Vector3 rot = C.transform.rotation.eulerAngles;

        // ターゲットオブジェクトの座標・回転を取得
        Vector3 targetPosition = A.position + A.TransformDirection(PositionOffset);
        Quaternion targetRotation = A.rotation * Quaternion.Euler(RotationOffset);

        // 基準となる部位（PartToAttach）の現在のワールド座標と回転を取得
        Vector3 partWorldPosition = B.position;
        Quaternion partWorldRotation = B.rotation;

        // PlayerRootの位置を移動させる際に、部位の位置がターゲットにぴったり合うように移動
        Vector3 positionDifference = targetPosition - partWorldPosition;
        C.position += positionDifference;
        C.position += UtilityFunction.LocalLookPos(PlayerRoot.transform, offset);

        // 部位の回転がターゲットの回転にぴったり合うように、PlayerRootの回転を更新
        Quaternion rotationDifference = targetRotation * Quaternion.Inverse(partWorldRotation);
        C.rotation = rotationDifference * C.rotation;
        C.rotation = Quaternion.Euler(0, C.transform.rotation.eulerAngles.y, 0);

        // ターゲットオブジェクトの子オブジェクトとしてセット
        //C.SetParent(A);

    }



















}
