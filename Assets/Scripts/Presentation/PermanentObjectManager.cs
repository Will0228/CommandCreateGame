using UnityEngine;

namespace Presenter
{
    /// <summary>
    /// DontDestroyOnLoadで永久的に残したいオブジェクトを管理するクラス
    /// </summary>
    public sealed class PermanentObjectManager : MonoBehaviour
    {
        [Header("永久的に残したいオブジェクト")]
        [SerializeField] private GameObject[] _objects;
        
        private void Awake()
        {
            foreach (var obj in _objects)
            {
                DontDestroyOnLoad(obj);
            }
        }
    }
}