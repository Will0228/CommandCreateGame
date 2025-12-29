using Application.Attributes;
using Application.DI;
using Domain.Repository;
using UnityEngine;

namespace Infra.Repository
{
    public sealed class TestRepository : ITestRepository
    {
        [Inject]
        public TestRepository(IResolver resolver)
        {
            
        }
        
        public void Test()
        {
            Debug.Log("Inject OK");
        }
    }
}