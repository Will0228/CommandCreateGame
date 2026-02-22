namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorNameMapper
    {
        internal string GetCompletedText(LayerType type, string className)
        {
            return type switch
            {
                LayerType.Presenter => className + "Presenter",
                LayerType.View => className + "View",
                LayerType.UseCase => className + "UseCase",
                LayerType.Service => className + "Service",
                LayerType.Entity => className + "Entity",
                LayerType.ValueObject => className + "Vo",
                LayerType.DataTransferObject => className + "Dto",
                LayerType.RepositoryInterface => "I" + className + "Repository",
                LayerType.RepositoryImplementation => className + "Repository",
                _ => "Unknown"
            };
        }
    }
}
