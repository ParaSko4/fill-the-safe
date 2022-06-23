using FillTheSafe.Gameplay;
using UnityEngine;
using Zenject;

namespace FillTheSafe.DI
{
    public class SceneInstaller : MonoInstaller
    {
        [SerializeField]
        private Camera mainCamera;
        [SerializeField]
        private Safe safe;
        [SerializeField]
        private CaseContainer caseContainer;

        public override void InstallBindings()
        {
            Container.Bind<Camera>().FromInstance(mainCamera).AsSingle().NonLazy();
            Container.Bind<Safe>().FromInstance(safe).AsSingle().NonLazy();
            Container.Bind<CaseContainer>().FromInstance(caseContainer).AsSingle().NonLazy();
        }
    }
}