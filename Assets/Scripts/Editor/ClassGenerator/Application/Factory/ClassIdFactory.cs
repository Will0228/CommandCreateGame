namespace Editor.ClassGenerator
{
    internal sealed class ClassIdFactory
    {
        internal ClassId Create(ClassGeneratorModel.LayerSettings settings)
        {
            return new ClassId($"{settings.ClassNames}{settings.Suffix}");
        }
    }
}