using System;

namespace JH.IoCLight
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public sealed class InjectAttribute : Attribute { }
}
