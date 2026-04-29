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

        private readonly Dictionary<ClassKey, ClassGeneratorWordingSettingClassInfo> _cachedClassSettingDict = new();
        private readonly List<ClassGeneratorWordingSettingClassInfo> _cachedInfos = new(); // パフォーマンス考慮のため辞書から毎回リストを作らない
        
        // このタブを開いた瞬間に別タブの情報で更新をかけた時の購読
        private readonly Subject<IReadOnlyList<ClassGeneratorWordingSettingClassInfo>> _updateClassInfosSubject = new();
        public Observable<IReadOnlyList<ClassGeneratorWordingSettingClassInfo>> UpdateClassInfosAsObservable 
            => _updateClassInfosSubject.AsObservable();

        public ClassGeneratorWordingSettingTextAreaModel()
        {
            var implementationTextSb = new StringBuilder();
            implementationTextSb.AppendLine("以下のクラスを用いて【】を実装したいです。");
            implementationTextSb.AppendLine();
            implementationTextSb.AppendLine("実装した内容をJSON化して出力してください");
            _cachedImplementationDetailsInfo = new ClassGeneratorWordingSettingInfo("実装したい内容", implementationTextSb.ToString());
        }

        public void UpdateData(IReadOnlyList<ClassKey> classKeys)
        {
            // 消えている要素は辞書から削除し、追加されているクラスは辞書に追加する

            // 元々辞書に存在したが、今回の更新で消えたクラス
            // TODO
            // 実装正しいかわからないので問題があれば修正してください
            var rolesToRemove = _cachedClassSettingDict.Keys
                .Where(existingRole => !classKeys.Contains(existingRole))
                .ToList();

            foreach (var role in rolesToRemove)
            {
                _cachedClassSettingDict.Remove(role);
            }

            // 3. 追加・更新処理: 新しいリストにある要素を辞書に反映
            foreach (var classKey in classKeys)
            {
                if (!_cachedClassSettingDict.ContainsKey(classKey))
                {
                    _cachedClassSettingDict.Add(classKey, new ClassGeneratorWordingSettingClassInfo(new ClassGeneratorWordingSettingInfo(classKey.Id, "任せます"), classKey.ComponentRoleType));
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