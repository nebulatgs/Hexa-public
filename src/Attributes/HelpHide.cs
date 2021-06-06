using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharp​Plus.CommandsNext.Attributes;

namespace Hexa.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class HelpHideAttribute : Attribute
    {
    }
}