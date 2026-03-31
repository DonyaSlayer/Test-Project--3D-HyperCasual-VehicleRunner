using UnityEngine;
using Zenject;
public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<GameStateController>()
            .AsSingle()
            .NonLazy();
        Container.BindInterfacesAndSelfTo<InputHandler>()
            .AsSingle()
            .NonLazy();
        Container.Bind<ObjectPoolManager>()
            .FromComponentInHierarchy()
            .AsSingle();
    }
}
