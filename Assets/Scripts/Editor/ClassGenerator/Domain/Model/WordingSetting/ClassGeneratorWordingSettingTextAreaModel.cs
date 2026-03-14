using System.Text;

namespace Editor.ClassGenerator
{
    internal sealed class ClassGeneratorWordingSettingTextAreaModel
    {
        private string _implementationText;
        public string ImplementationText => _implementationText;

        internal void Initialize()
        {
            var implementationTextSb = new StringBuilder();
            implementationTextSb.AppendLine("以下のクラスを用いて【】を実装したいです。");
            implementationTextSb.AppendLine();
            implementationTextSb.AppendLine("実装した内容をJSON化して出力してください");
            _implementationText = implementationTextSb.ToString();
        }
    }
}