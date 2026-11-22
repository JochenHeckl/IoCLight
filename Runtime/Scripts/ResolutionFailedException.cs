using System;

namespace JH.IoCLight
{
  // Thrown when a type, or a constructor's dependencies, are not registered. Constructor
  // selection treats only this exception as a reason to try the next constructor.
  public class ResolutionFailedException : InvalidOperationException
  {
    public ResolutionFailedException(string message)
      : base(message) { }
  }
}
