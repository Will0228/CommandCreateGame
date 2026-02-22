namespace Editor.ClassGenerator
{
    internal static class LayerTypeExtension
    {
        internal static string GetName(this LayerType type)
        {
            return type switch
            {
                LayerType.Presenter => "Presenter",
                LayerType.View   => "View",
                LayerType.UseCase   => "UseCase",
                LayerType.Service   => "Service",
                LayerType.Entity   => "Entity",
                LayerType.ValueObject  => "Value Object",
                LayerType.DataTransferObject  => "Data Transfer Object",
                LayerType.RepositoryInterface  => "Repository Interface",
                LayerType.RepositoryImplementation  => "Repository Impl",
                _ => "Unknown"
            };
        }
    }
}