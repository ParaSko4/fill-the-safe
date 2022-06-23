using Cinemachine;
using Cysharp.Threading.Tasks;
using FillTheSafe.Gameplay;
using UnityEngine;
using Zenject;

namespace FillTheSafe.Scenes
{
    public class SceneBehaviour : MonoBehaviour
    {
        [SerializeField]
        private CinemachineVirtualCamera startCamera;
        [SerializeField]
        private CinemachineVirtualCamera gameplayCamera;
        [SerializeField]
        private float delayBeforeOpenCase;
        [SerializeField]
        private ParticleSystem[] confetti;

        private Safe safe;
        private CaseContainer caseContainer;
        private Case[] cases;

        [Inject]
        public void Construct(Safe safe, CaseContainer caseContainer)
        {
            this.safe = safe;
            this.caseContainer = caseContainer;
        }

        private void Awake()
        {
            cases = caseContainer.GetComponentsInChildren<Case>();
        }

        private void Start()
        {
            RunSceneScript();
        }

        private async void RunSceneScript()
        {
            await safe.OpenSafe();

            startCamera.Priority = 8;

            int delay = (int)(delayBeforeOpenCase * 1000f);

            await UniTask.Delay(delay);

            foreach (var valueCase in cases)
            {
                valueCase.OpenCase();

                await UniTask.Delay(delay);
            }

            safe.TakeContainer();
        }

        private async void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (safe.CurrentGridManager != null)
                {
                    safe.TakeContainer();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                startCamera.Priority = 11;
                caseContainer.CurrentSelectedCase.BackCase();

                foreach (var valueCase in cases)
                {
                    valueCase.CloseCase();
                }

                await safe.CloseSafe();

                foreach (var item in confetti)
                {
                    item.Play();
                }
            }
        }
    }
}