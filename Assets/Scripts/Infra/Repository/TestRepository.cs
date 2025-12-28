using Domain.Repository;
using UnityEngine;

namespace Infra.Repository
{
    public sealed class TestRepository : ITestRepository
    {
        public void Test()
        {
            Debug.Log("Inject OK");
        }
    }
}