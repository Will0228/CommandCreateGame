using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Domain.Addressable
{
    public interface IAddressableAssetLoader
    {
        UniTask<T> LoadAssetAsync<T>(string key) where T : class;
        UniTask<T> LoadAssetAsync<T>(AssetReference assetReference, bool trackHandle = true) where T : class;
        UniTask<GameObject> InstantiateAssetAsync(string key, Transform parent = null, bool trackHandle = true);
        UniTask<GameObject> InstantiateAssetAsync(AssetReferenceGameObject assetReferenceGameObject, Transform parent = null, bool trackHandle = true, bool isPoolAvailable = true);
        void ReleaseAsset<T>(T asset);
        void ReleaseInstance(GameObject instance);
        void ReleaseAll();
    }
}