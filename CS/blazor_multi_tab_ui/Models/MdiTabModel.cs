using System.Text.Json;
using System.Text.Json.Serialization;

namespace blazor_multi_tab_ui.Models
{
    public class MdiTabModel
    {
        public MdiTabModel() => Id = Guid.NewGuid().ToString();
        public string Id { get; }
        public int VisibleIndex { get; set; }
        public bool Visible { get; set; }
        public string? Text { get; set; }
        public string? TabTypeName { get; set; }
        public Dictionary<string, JsonElement>? ParsedParameters { get; set; }
        private Dictionary<string, object>? _cachedParameters;

        [JsonIgnore]
        public Dictionary<string, object>? Parameters
        {
            get
            {
                if (_cachedParameters == null && ParsedParameters != null)
                {
                    _cachedParameters = ParsedParameters.ToDictionary(
                        p => p.Key,
                        p => (object)(p.Value.ValueKind switch
                        {
                            JsonValueKind.Number when p.Value.TryGetInt32(out var i) => i,
                            JsonValueKind.Number when p.Value.TryGetInt64(out var l) => l,
                            JsonValueKind.Number when p.Value.TryGetDouble(out var d) => d,
                            JsonValueKind.True => true,
                            JsonValueKind.False => false,
                            JsonValueKind.String => p.Value.GetString()!,
                            _ => p.Value.ToString()!
                        })
                    );
                }
                return _cachedParameters;
            }
            set
            {
                ParsedParameters = value?.ToDictionary(
                    p => p.Key,
                    p => JsonSerializer.SerializeToElement(p.Value)
                );
                _cachedParameters = value;
            }
        }
    }
}
