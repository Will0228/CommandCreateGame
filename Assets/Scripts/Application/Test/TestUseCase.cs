using Application.Attributes;
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
        public void Test(ITestRepository testRepository)
        {
            Repository = testRepository;
            Repository.Test();
        }
    }
}