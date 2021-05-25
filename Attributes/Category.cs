using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharp​Plus.CommandsNext.Attributes;

namespace Hexa.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = false)]
    public class CategoryAttribute : Attribute
    {
        public CategoryAttribute(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }
}