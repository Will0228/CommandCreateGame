namespace Editor.ClassGenerator
{
    /// <summary>
    /// クラスを一意に識別できるKey
    /// </summary>
    /// <param name="Value">{クラス名}{ComponentRoleごとのSuffix}</param>
    public sealed record ClassId(string Value){}
}