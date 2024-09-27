using develop_body;
using develop_common;
using System.Collections;
using System.Collections.Generic;
using TNRD;
using UniRx;
using UnityEngine;

namespace develop_body
{
    public class CommandController : SingletonMonoBehaviour<CommandController>
    {
        // Inspecter
        public UnitBody UnitBody;
        public AnimatorStateController AnimatorStateController;
        public Camera Camera;
        public string BodyTagName = "Body";
        public GameObject ClickEffect;

        // Create Info
        public SerializableInterface<ICommand> SelectCommand;
        // Timer
        private ReactiveProperty<float> CommandTimer = new ReactiveProperty<float>();
        public ReactiveProperty<float> RecordTimer = new ReactiveProperty<float>();
        //public float CommandTimer() => _commandTimer.Value;
        //public float RecordTimer() => _recordTimer.Value;

        // Flg
        public ReactiveProperty<bool> IsCommandPlay = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> IsStartRecord = new ReactiveProperty<bool>();

        // Command List (Auto)
        public List<CommandInfo> CommandInfos = new List<CommandInfo>();


        private void Start()
        {

        }


        private void Update()
        {
            if (IsCommandPlay.Value)
                CommandTimer.Value += Time.deltaTime;
            if (IsStartRecord.Value)
                RecordTimer.Value += Time.deltaTime;

            if (Input.GetMouseButtonDown(0))
                OnItemCommandCreate();
        }

        public void OnItemCommandCreate()
        {
            BodyCollider bodyCollider = null;
            var hit = UtilityFunction.CreateScreenClickObject(Camera.main);
            if (hit != null)
            {
                if (hit.transform.parent.CompareTag(BodyTagName))
                {
                    bodyCollider = UnitBody.GetDistanceBody(hit.transform.position);
                    if (bodyCollider != null)
                        Debug.Log($"{bodyCollider}�Ƀ^�b�`. Type:{bodyCollider.bodyType}, Value:{bodyCollider.BodyParameter}");
                    else
                    {
                        Debug.LogError("bodyCollider��Null�ł�");
                        return;
                    }
                }
                else
                {
                    Debug.Log($"BodyCollider�ł͂���܂���");
                    return;
                }
            }

            // Command Create
            CommandInfo command = new CommandInfo();

            // ���ݑI������Ă�ru
            var commandName = "";

            // Parent
            var parentBody = bodyCollider.transform.parent;
            var parentName = parentBody.name;

            // Parameter Set
            hit.transform.rotation = Camera.transform.rotation;
            float recordeTime = RecordTimer.Value;
            Vector3 clickPosition = hit.transform.position;
            Vector3 localRot = hit.transform.localEulerAngles;

            // UI Parameter Set
            var localScale = 1f;
            var lifeTime = 1f;
            var hitSpan = 1f;
            var durationTime = 1f;
            var motionSpeed = 1f;

            // CommandList Insert
            command.OnParameterSet(
                commandName, recordeTime, parentName,
                clickPosition, localRot, localScale,
                lifeTime, hitSpan, durationTime, motionSpeed);
            command.StartLocalPosition = GetCameraPositionFromObject(hit.transform.parent, Camera);
            CommandInfos.Add(command);
        }

        public void OnMotionCommandCreate()
        {

        }


        public void OnCommandPlay()
        {
            if (!IsCommandPlay.Value) return;

            foreach (var command in CommandInfos)
            {
                if (CommandTimer.Value >= command.StartRecordTime)
                {
                    if (command.IsPlayed) return;
                    command.IsPlayed = true;

                    // �R�}���h�̎�ށu�A�C�e���v�u���[�V�����Đ��v�u�v���C���[�ʒu�ړ��u�J�����ʒu�v
                    // �G�ꂽ��_���[�W�� or �������Ń^�C�~���O�悭���͂��鎮�c�B
                    // ���[�V�����I��ŃX�y�[�X�L�[�ōĐ��Ƃ�

                    switch (command.CommandType)
                    {
                        case ECommandType.Item:
                            ItemPlay(command);
                            break;
                        case ECommandType.PlayerMotion:
                            PlayerMotionPlay(command);
                            break;
                        case ECommandType.PlayerMove:
                            PlayerMovePlay(command);
                            break;
                        case ECommandType.CameraMove:
                            CameraMovePlay(command);
                            break;
                    }
                }
            }
        }

        private void ItemPlay(CommandInfo command)
        {

        }

        private void PlayerMotionPlay(CommandInfo command)
        {
            AnimatorStateController.StatePlay("State1", EStatePlayType.SinglePlay, true);
        }

        private void PlayerMovePlay(CommandInfo command)
        {

        }

        private void CameraMovePlay(CommandInfo command)
        {

        }

        public void OnReset(bool allReset = false)
        {

        }

        public void OnRecordStart()
        {

        }

        public void OnLastCommandDelete()
        {

        }

        public void OnCommandSave()
        {

        }



        public Vector3 GetCameraPositionFromObject(Transform objectTransform, Camera camera)
        {
            // �J�����̃��[���h���W���擾
            Vector3 cameraWorldPosition = camera.transform.position;

            // �I�u�W�F�N�g���猩���J�����̍��W���v�Z
            Vector3 relativePosition = objectTransform.InverseTransformPoint(cameraWorldPosition);

            // ���΍��W��Ԃ�
            return relativePosition;
        }

        private void TestRaySample()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var hit = UtilityFunction.GetScreenClickObjectPosition(Camera.main, ClickEffect);
                if (hit != null)
                {
                    var bodyCollider = UnitBody.GetDistanceBody(hit);
                    if (bodyCollider != null)
                        Debug.Log($"{bodyCollider}�Ƀ^�b�`. Type:{bodyCollider.bodyType}, Value:{bodyCollider.BodyParameter}");
                }
            }
        }
    }

}
