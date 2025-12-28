using Application.DI;
using Domain.Repository;
using Infra.Repository;

namespace Infra.Installer
{
    public sealed class TestInstaller
    {
        public void Install(IRegister register)
        {
            register.Register<ITestRepository, TestRepository>();
        }
    }
}