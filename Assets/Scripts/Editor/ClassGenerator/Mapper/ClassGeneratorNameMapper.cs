namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorNameMapper
    {
        internal string GetCompletedText(ComponentRoleType type, string className)
        {
            return type switch
            {
                ComponentRoleType.Presenter => className + "Presenter",
                ComponentRoleType.View => className + "View",
                ComponentRoleType.UseCase => className + "UseCase",
                ComponentRoleType.Service => className + "Service",
                ComponentRoleType.Entity => className + "Entity",
                ComponentRoleType.ValueObject => className + "Vo",
                ComponentRoleType.DataTransferObject => className + "Dto",
                ComponentRoleType.RepositoryInterface => "I" + className + "Repository",
                ComponentRoleType.RepositoryImplementation => className + "Repository",
                _ => "Unknown"
            };
        }
    }
}
