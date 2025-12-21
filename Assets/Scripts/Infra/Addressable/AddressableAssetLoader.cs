using System.Collections.Generic;
using System.Linq;
using Common.ZLogger;
using Cysharp.Threading.Tasks;
using Domain.Addressable;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using ZLinq;

namespace Infra.Addressable
{
    /// <summary>
    /// Addressableを用いてアセットをロードするクラス
    /// </summary>
    public sealed class AddressableAssetLoader : IAddressableAssetLoader
    {
        private readonly Dictionary<string, AsyncOperationHandle> _loadedAssetHandles = new();
        private readonly Dictionary<GameObject, AsyncOperationHandle> _loadedInstanceHandles = new();
        
        public async UniTask<T> LoadAssetAsync<T>(string key) where T : class
        {
            // すでにロード中の場合は、そのハンドルを待つか、既存のものを返す
            if (_loadedAssetHandles.TryGetValue(key, out var existingHandle))
            {
                if (existingHandle.IsValid() && existingHandle.IsDone)
                {
                    ZLoggerUtility.LogDebug($"アセットはすでにロードされています : key = {key}. 既存のものを渡します");
                    return existingHandle.Result as T;
                    // return UniTask.FromResult(existingHandle.Result as T);
                }
                else if (existingHandle.IsValid() && !existingHandle.IsDone)
                {
                    await existingHandle.ToUniTask();
                    return existingHandle.Result as T;
                }
            }
            
            ZLoggerUtility.LogDebug($"アセットローディング: {key}");
            var handle = Addressables.LoadAssetAsync<T>(key);
            await handle.ToUniTask();
            
            _loadedAssetHandles[key] = handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }
            else
            {
                ZLoggerUtility.LogDebug($"アセットのロードに失敗しました: {key}. Error: {handle.OperationException}");
                Addressables.Release(handle);
                _loadedAssetHandles.Remove(key);
                return null;
            }
        }
        
        public async UniTask<T> LoadAssetAsync<T>(AssetReference assetReference, bool trackHandle = true)
            where T : class
        {
            var assetGUID = assetReference.AssetGUID;
            if (_loadedAssetHandles.TryGetValue(assetGUID, out var existingHandle))
            {
                if (existingHandle.IsValid() && existingHandle.IsDone)
                {
                    ZLoggerUtility.LogDebug($"アセットはすでにロードされています : key = {assetGUID}. 既存のオブジェクトを渡します");
                    return existingHandle.Result as T;
                }
                else if (existingHandle.IsValid() && !existingHandle.IsDone)
                {
                    await existingHandle.ToUniTask();
                    return existingHandle.Result as T;
                }
            }
            
            ZLoggerUtility.LogDebug($"アセットをインスタンス化します : {assetGUID}");
            var handle = assetReference.LoadAssetAsync<T>();
            await handle.ToUniTask();

            if (trackHandle)
            {
                _loadedAssetHandles[assetGUID] = handle;
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }
            else
            {
                ZLoggerUtility.LogDebug($"インスタンス化しようとしたアセット : {assetGUID}. エラー内容: {handle.OperationException}");
                Addressables.Release(handle);
                if (trackHandle)
                {
                    _loadedAssetHandles.Remove(assetGUID);
                }

                return null;
            }
        }

        public async UniTask<GameObject> InstantiateAssetAsync(string key, Transform parent = null, bool trackHandle = true)
        {
            if (_loadedAssetHandles.TryGetValue(key, out var existingHandle))
            {
                if (existingHandle.IsValid() && existingHandle.IsDone)
                {
                    ZLoggerUtility.LogDebug($"アセットはすでにロードされています : key = {key}. 既存のオブジェクトを渡します");
                    return existingHandle.Result as GameObject;
                }
                else if (existingHandle.IsValid() && !existingHandle.IsDone)
                {
                    await existingHandle.ToUniTask();
                    return existingHandle.Result as GameObject;
                }
            }
            
            ZLoggerUtility.LogDebug($"アセットをインスタンス化します : {key}");
            var handle = Addressables.InstantiateAsync(key, parent);
            await handle.ToUniTask();

            if (trackHandle)
            {
                _loadedAssetHandles[key] = handle;
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }
            else
            {
                ZLoggerUtility.LogDebug($"インスタンス化しようとしたアセット : {key}. エラー内容: {handle.OperationException}");
                Addressables.Release(handle);
                if (trackHandle)
                {
                    _loadedAssetHandles.Remove(key);
                }

                return null;
            }
        }

        public UniTask<GameObject> InstantiateAssetAsync(AssetReferenceGameObject assetReferenceGameObject, Transform parent = null,
            bool trackHandle = true)
        {
            throw new System.NotImplementedException();
        }

        public async UniTask<GameObject> InstantiateAssetAsync(AssetReferenceGameObject assetReferenceGameObject, Transform parent = null, bool trackHandle = true, bool isPoolAvailable = true)
        {
            var assetGUID = assetReferenceGameObject.AssetGUID;
            if (!isPoolAvailable && _loadedAssetHandles.TryGetValue(assetGUID, out var existingHandle))
            {
                if (existingHandle.IsValid() && existingHandle.IsDone)
                {
                    ZLoggerUtility.LogDebug($"アセットはすでにロードされています : key = {assetGUID}. 既存のオブジェクトを渡します");
                    return existingHandle.Result as GameObject;
                }
                else if (existingHandle.IsValid() && !existingHandle.IsDone)
                {
                    await existingHandle.ToUniTask();
                    return existingHandle.Result as GameObject;
                }
            }
            
            ZLoggerUtility.LogDebug($"アセットをインスタンス化します : {assetGUID}");
            var handle = assetReferenceGameObject.InstantiateAsync(parent);
            await handle.ToUniTask();

            if (trackHandle)
            {
                _loadedAssetHandles[assetGUID] = handle;
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }
            else
            {
                ZLoggerUtility.LogDebug($"インスタンス化しようとしたアセット : {assetGUID}. エラー内容: {handle.OperationException}");
                Addressables.Release(handle);
                if (trackHandle)
                {
                    _loadedInstanceHandles.Remove(handle.Result);
                }

                return null;
            }
        }

        public void ReleaseAsset<T>(T asset)
        {
            var entry = _loadedAssetHandles
                .AsValueEnumerable()
                .FirstOrDefault(x => x.Value.Result?.Equals(asset) ?? false);

            if (entry.Value.IsValid())
            {
                ZLoggerUtility.LogDebug($"リリースします : {entry.Key}");
                Addressables.Release(entry.Value);
                _loadedAssetHandles.Remove(entry.Key);
            }
            else
            {
                ZLoggerUtility.LogWarning($"管理していないアセットをリリースしようとしています : {entry.Key}");
            }
        }

        public void ReleaseInstance(GameObject instance)
        {
            if (!instance)
            {
                return;
            }

            if (_loadedInstanceHandles.TryGetValue(instance, out AsyncOperationHandle handle))
            {
                if (handle.IsValid())
                {
                    ZLoggerUtility.LogDebug($"インスタンス化されていたオブジェクトをリリースします : {instance.name}");
                    Addressables.ReleaseInstance(handle);
                    _loadedInstanceHandles.Remove(instance);
                }
            }
            else
            {
                ZLoggerUtility.LogWarning($"管理していないインスタンスをリリースしようとしています : {instance.name}");
                GameObject.Destroy(instance);
            }
        }

        public void ReleaseAll()
        {
            ZLoggerUtility.LogDebug("全てリリースします");

            foreach (var pair in _loadedAssetHandles.ToList())
            {
                if (pair.Value.IsValid())
                {
                    Addressables.Release(pair.Value);
                }
            }
            _loadedAssetHandles.Clear();

            foreach (var pair in _loadedInstanceHandles.ToList())
            {
                if (pair.Value.IsValid())
                {
                    Addressables.ReleaseInstance(pair.Value);
                }
            }
            _loadedInstanceHandles.Clear();
        }
    }
}