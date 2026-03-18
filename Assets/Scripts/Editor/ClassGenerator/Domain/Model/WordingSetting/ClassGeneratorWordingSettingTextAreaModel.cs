using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using R3;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorWordingSettingTextAreaModel : IDisposable
    {
        private readonly ClassGeneratorWordingSettingInfo _cachedImplementationDetailsInfo;
        public ClassGeneratorWordingSettingInfo ImplementationDetailsInfo => _cachedImplementationDetailsInfo;

        private readonly Dictionary<ClassId, ClassGeneratorWordingSettingClassInfo> _cachedClassSettingDict = new();
        private readonly List<ClassGeneratorWordingSettingClassInfo> _cachedInfos = new(); // パフォーマンス考慮のため辞書から毎回リストを作らない
        
        // このタブを開いた瞬間に別タブの情報で更新をかけた時の購読
        private readonly Subject<IReadOnlyList<ClassGeneratorWordingSettingClassInfo>> _updateClassInfosSubject = new();
        public Observable<IReadOnlyList<ClassGeneratorWordingSettingClassInfo>> UpdateClassInfosAsObservable 
            => _updateClassInfosSubject.AsObservable();

        internal ClassGeneratorWordingSettingTextAreaModel()
        {
            var implementationTextSb = new StringBuilder();
            implementationTextSb.AppendLine("以下のクラスを用いて【】を実装したいです。");
            implementationTextSb.AppendLine();
            implementationTextSb.AppendLine("実装した内容をJSON化して出力してください");
            _cachedImplementationDetailsInfo = new ClassGeneratorWordingSettingInfo("実装したい内容", implementationTextSb.ToString());
        }

        internal void UpdateData(IReadOnlyDictionary<ClassId, ComponentRoleType> dict)
        {
            // 消えている要素は辞書から削除し、追加されているクラスは辞書に追加する

            // 元々辞書に存在したが、今回の更新で消えたクラス
            var rolesToRemove = _cachedClassSettingDict.Keys
                .Where(existingRole => !dict.Keys.Contains(existingRole))
                .ToList();

            foreach (var role in rolesToRemove)
            {
                _cachedClassSettingDict.Remove(role);
            }

            // 3. 追加・更新処理: 新しいリストにある要素を辞書に反映
            foreach (var kvp in dict)
            {
                if (!_cachedClassSettingDict.ContainsKey(kvp.Key))
                {
                    _cachedClassSettingDict.Add(kvp.Key, new ClassGeneratorWordingSettingClassInfo(new ClassGeneratorWordingSettingInfo(kvp.Key.Value, "任せます"), kvp.Value));
                }
            }
            
            // 辞書からリストを作成せずにキャッシュされているリストに値を移し替える
            _cachedInfos.Clear();
            foreach (var kvp in _cachedClassSettingDict)
            {
                _cachedInfos.Add(kvp.Value);
            }
            
            _updateClassInfosSubject.OnNext(_cachedInfos);
        }

        void IDisposable.Dispose()
        {
            _updateClassInfosSubject.Dispose();
        }
    }
}