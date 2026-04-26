using System;
using R3;

namespace Editor.ClassGenerator
{
    [Flags]
    internal enum AppLayerType
    {
        None = 0,
        Presentation = 1 << 8, // プレゼンテーション層
        Application = 1 << 9, // アプリケーション層
        Domain = 1 << 10, // ドメイン層
        Infrastructure = 1 << 11, // インフラ層
    }
    
    [Flags]
    internal enum ComponentRoleType
    {
        None = 0,
        
        // プレゼンテーション層
        Presenter = (1 << 8) | (1 << 0),
        View  = (1 << 8) | (1 << 1),
        
        // アプリケーション層
        UseCase = (1 << 9) | (1 << 0),
        Service = (1 << 9) | (1 << 1),
        
        // ドメイン層
        Entity = (1 << 10) | (1 << 0),
        ValueObject = (1 << 10) | (1 << 1),
        DataTransferObject = (1 << 10) | (1 << 2),
        RepositoryInterface = (1 << 10) | (1 << 3),
        
        // インフラ層
        RepositoryImplementation = (1 << 11) | (1 << 0),
        
        // 層判定用のマスク
        PresentationMask = (1 << 8),
        ApplicationMask = (1 << 9),
        DomainMask = (1 << 10),
        InfrastructureMask = (1 << 11),
    }
    
    internal sealed class ClassGeneratorCommonModel
    {
        private readonly ReactiveProperty<int> _selectedCategoryIndexProp = new();
        public ReadOnlyReactiveProperty<int> SelectedCategoryIndexProp => _selectedCategoryIndexProp;
        public int SelectedCategoryIndex => _selectedCategoryIndexProp.Value;
        
        public void SaveCategoryIndex(int index) => _selectedCategoryIndexProp.Value = index;
    }
}