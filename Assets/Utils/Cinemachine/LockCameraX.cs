using Cinemachine;
using UnityEngine;

namespace ItemTrader.Utils.Cinemachine
{
    [ExecuteInEditMode][SaveDuringPlay][AddComponentMenu("")]
    public class LockCameraX : CinemachineExtension
    {
        [Tooltip("Lock the camera's Z position to this value")]
        public float xPosition = 0;

        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                var pos = state.RawPosition;
                pos.x = xPosition;
                state.RawPosition = pos;
            }
        }
    }
}