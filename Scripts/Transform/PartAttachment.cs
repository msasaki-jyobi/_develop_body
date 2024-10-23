using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class PartAttachment : MonoBehaviour
{
    // インスペクターで指定する部位（例：HeadColliderやHipsCollider）
    public Transform PartToAttach; // プレイヤーの特定の部位を設定

    // プレイヤー全体（ルートオブジェクト）
    public Transform PlayerRoot; // プレイヤー全体のルートオブジェクト

    // ターゲットオブジェクト（例：敵の吸い込み口）
    public Transform TargetObject;

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

    private void Update()
    {
        if (IsDebugK && Input.GetKeyDown(KeyCode.K))
        {
            AttachPlayerToTarget().Forget();
        }
    }

    /// <summary>
    /// オブジェクトを上書きしてアタッチする
    /// </summary>
    public void AttachTarget()
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
}
