using System;

namespace Hexa.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class CategoryAttribute : Attribute
    {
        public CategoryAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}