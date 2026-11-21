using System;

namespace JH.IoCLight
{
    public class CircularDependencyException : InvalidOperationException
    {
        public CircularDependencyException(string message) : base(message)
        {
        }
    }
}
