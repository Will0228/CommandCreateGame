using Application.Attributes;
using Application.DI;
using Domain.Repository;

namespace Application.Test
{
    public sealed class TestUseCase
    {
        public ITestRepository Repository;
        
        [Inject]
        public TestUseCase(IResolver resolver)
        {
            Repository = resolver.Resolve<ITestRepository>();
        }
    }
}