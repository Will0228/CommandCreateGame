using Application.DI;
using Application.Test;
using Domain.Repository;
using Infra.Repository;

namespace Infra.Installer
{
    public sealed class TestInstaller
    {
        public void Install(IRegister register)
        {
            register.Register<TestUseCase>();
            register.Register<ITestRepository, TestRepository>();
        }
    }
}