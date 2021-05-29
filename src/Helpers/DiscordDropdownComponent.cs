// using System;
// using DSharpPlus.Entities;
// using Newtonsoft.Json;

// namespace Hexa.Helpers
// {
//     public class DiscordDropdownComponent : DiscordComponent
//     {
//         [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
//         internal new ComponentType Type { get; set; } = (ComponentType)3; // Discord likes to throw 400. //

//         // /// <summary>
//         // /// The style of the dropdown.
//         // /// </summary>
//         // [JsonProperty("style", NullValueHandling = NullValueHandling.Ignore)]
//         // public DropdownStyle Style { get; set; }

//         /// <summary>
//         /// The text to apply to the dropdown. If this is not specified <see cref="Emoji"/> becomes required.
//         /// </summary>
//         [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
//         public string Label { get; set; }

//         /// <summary>
//         /// Whether this dropdown can be pressed.
//         /// </summary>
//         [JsonProperty("disabled", NullValueHandling = NullValueHandling.Ignore)]
//         public bool Disabled { get; set; }

//         /// <summary>
//         /// The emoji to add to the dropdown. Can be used in conjunction with a label, or as standalone. Must be added if label is not specified.
//         /// </summary>
//         [JsonProperty("emoji", NullValueHandling = NullValueHandling.Ignore)]
//         public DiscordComponentEmoji Emoji { get; set; }

//         /// <summary>
//         /// Constructs a new <see cref="DiscordDropdownComponent"/>.
//         /// </summary>
//         public DiscordDropdownComponent() { }

//         /// <summary>
//         /// Constructs a new dropdown with the specified options.
//         /// </summary>
//         /// <param name="style">The style/color of the dropdown.</param>
//         /// <param name="customId">The Id to assign to the dropdown. This is sent back when a user presses it.</param>
//         /// <param name="label">The text to display on the dropdown, up to 80 characters. Can be left blank if <paramref name="emoji"/>is set.</param>
//         /// <param name="disabled">Whether this dropdown should be initialized as being disabled. User sees a greyed out dropdown that cannot be interacted with.</param>
//         /// <param name="emoji">The emoji to add to the dropdown. This is required if <paramref name="label"/> is empty or null.</param>
//         public DiscordDropdownComponent(string customId, string label, bool disabled = false, DiscordComponentEmoji emoji = null)
//         {
//             // this.Style = style;
//             this.Label = label;
//             // this.CustomId = customId;
//             this.Disabled = disabled;
//             this.Emoji = emoji;
//             this.Type = (ComponentType)3;
//         }
//     }
// }
