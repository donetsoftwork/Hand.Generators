using System;

namespace Hello;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class HelloGeneratorAttribute : Attribute
{
}
