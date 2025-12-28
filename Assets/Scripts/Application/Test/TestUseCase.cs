using Application.Attributes;
using Application.DI;
using Domain.Repository;

namespace Application.Test
{
    public sealed class TestUseCase
    {
        public ITestRepository Repository;
        
        // [Inject]
        // public TestUseCase(ITestRepository testRepository)
        // {
        //     Repository = testRepository;
        // }

        [Inject]
        public void Test(ITestRepository testRepository, IResolver resolver)
        {
            Repository = resolver.Resolve<ITestRepository>();
            Repository.Test();
        }
    }
}