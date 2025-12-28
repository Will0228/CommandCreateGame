using Domain.Repository;
using Infra.DI;
using Infra.Repository;

namespace Infra.Installer
{
    public sealed class TestInstaller
    {
        public void Install(DIContainer container)
        {
            container.Register<ITestRepository, TestRepository>();
        }
    }
}