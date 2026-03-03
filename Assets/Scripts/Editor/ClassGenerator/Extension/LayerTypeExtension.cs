namespace Editor.ClassGenerator
{
    internal static class LayerTypeExtension
    {
        internal static string GetName(this ComponentRoleType type)
        {
            return type switch
            {
                ComponentRoleType.Presenter => "Presenter",
                ComponentRoleType.View   => "View",
                ComponentRoleType.UseCase   => "UseCase",
                ComponentRoleType.Service   => "Service",
                ComponentRoleType.Entity   => "Entity",
                ComponentRoleType.ValueObject  => "Value Object",
                ComponentRoleType.DataTransferObject  => "Data Transfer Object",
                ComponentRoleType.RepositoryInterface  => "Repository Interface",
                ComponentRoleType.RepositoryImplementation  => "Repository Impl",
                _ => "Unknown"
            };
        }
    }
}