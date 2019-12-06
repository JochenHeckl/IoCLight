using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


namespace IoCLight.Test
{
    public class ContainerTest
    {
        public class SimpleDefaultConstructor {};
        
        public class SimpleCustomConstructor
        {
            public SimpleCustomConstructor()
            {

            }
        };

        [Test]
        public void TestSimpleResolve()
        {
            var container = new Container();

            container.Register<SimpleDefaultConstructor>();
            container.Register<SimpleCustomConstructor>();

            Assert.IsNotNull( container.Resolve<SimpleDefaultConstructor>() );
            Assert.IsNotNull( container.Resolve<SimpleCustomConstructor>() );
        }

        //// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        //// `yield return null;` to skip a frame.
        //[UnityTest]
        //public IEnumerator NewTestScriptWithEnumeratorPasses()
        //{
        //    // Use the Assert class to test conditions.
        //    // Use yield to skip a frame.
        //    yield return null;
        //}
    }
}
