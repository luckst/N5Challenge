using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace N5.Challenge.IntegrationTests
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TestPriorityAttribute : Attribute
    {
        public int Priority { get; }

        public TestPriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }
}