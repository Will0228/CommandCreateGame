using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using R3;

namespace Editor.ClassGenerator
{x
    internal sealed class ClassGeneratorWordingSettingTextAreaModel : IDisposable
    {
        private readonly ClassGeneratorWordingSettingInfo _cachedImplementationDetailsInfo;
        public ClassGeneratorWordingSettingInfo ImplementationDetailsInfo => _cachedImplementationDetailsInfo;

        private readonly Dictionary<ComponentRoleType, string> _cachedContentTextByRoleTypes = new();
        
        // このタブを開いた瞬間に別タブの情報で更新をかけた時の購読
        private readonly Subject<IReadOnlyDictionary<ComponentRoleType, string>> _updateContentTextTypeSubject = new();
        public Observable<IReadOnlyDictionary<ComponentRoleType, string>> UpdateContentTextTypeAsObservable 
            => _updateContentTextTypeSubject.AsObservable();

        internal ClassGeneratorWordingSettingTextAreaModel()
        {
            var implementationTextSb = new StringBuilder();
            implementationTextSb.AppendLine("以下のクラスを用いて【】を実装したいです。");
            implementationTextSb.AppendLine();
            implementationTextSb.AppendLine("実装した内容をJSON化して出力してください");
            _cachedImplementationDetailsInfo = new ClassGeneratorWordingSettingInfo("実装したい内容", implementationTextSb.ToString());
        }

        internal void UpdateData(IReadOnlyList<ClassGeneratorModel.LayerSettings> settingsList)
        {
            // 消えている要素は辞書から削除し、追加されているクラスは辞書に追加する
            var types = settingsList
                .Select(s => s.Type)
                .ToList();

            // 元々辞書に存在したが、今回の更新で消えたクラス
            var rolesToRemove = _cachedContentTextByRoleTypes.Keys
                .Where(existingRole => !types.Contains(existingRole))
                .ToList();

            foreach (var role in rolesToRemove)
            {
                _cachedContentTextByRoleTypes.Remove(role);
            }

            // 3. 追加・更新処理: 新しいリストにある要素を辞書に反映
            foreach (var type in types)
            {
                if (!_cachedContentTextByRoleTypes.ContainsKey(type))
                {
                    _cachedContentTextByRoleTypes.Add(type, "任せます");
                }
            }
            
            _updateContentTextTypeSubject.OnNext(_cachedContentTextByRoleTypes);
        }

        void IDisposable.Dispose()
        {
            _updateContentTextTypeSubject.Dispose();
        }
    }
}