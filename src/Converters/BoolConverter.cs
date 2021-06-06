using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;

namespace Hexa.Converters
{
    public class BoolConverter : IArgumentConverter<bool>
    {
        public Task<Optional<bool>> ConvertAsync(string value, CommandContext ctx)
        {
            if (bool.TryParse(value, out var boolean))
            {
                return Task.FromResult(Optional.FromValue(boolean));
            }

            switch (value.ToLower())
            {
                case "yes":
                case "y":
                case "t":
                case "enable":
                case "on":
                case "true":
                case "1":
                case "one":
                    return Task.FromResult(Optional.FromValue(true));

                case "no":
                case "n":
                case "f":
                case "disable":
                case "off":
                case "false":
                case "0":
                case "zero":
                    return Task.FromResult(Optional.FromValue(false));

                default:
                    return Task.FromResult(Optional.FromNoValue<bool>());
            }
        }
    }
}