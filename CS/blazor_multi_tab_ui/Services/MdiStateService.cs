using blazor_multi_tab_ui.Components.MDI.Tabs;
using blazor_multi_tab_ui.Models;
using Microsoft.JSInterop;
using System.Runtime.Intrinsics.Arm;
using System.Text.Json;

namespace blazor_multi_tab_ui.Services
{
    public class MdiStateService(IJSRuntime _js)
    {
        private MdiStateModel state = new();
        private const string LOCAL_STORAGE_KEY = "MDI-Layout";
        private readonly Dictionary<string, Type> _stringToTypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["customers"] = typeof(Customers),
            ["orders"] = typeof(Orders),
            ["chart"] = typeof(Chart),
        };

        public bool TryGetType(string? key, out Type? type) => _stringToTypeMap.TryGetValue(key ?? "unknown", out type);
        public event Action? OnTabsChanged;

        public async Task<MdiStateModel> LoadState() 
        {
            var cachedState = await LoadStateFromLocalStorageAsync();
            if (cachedState != null) state = cachedState;
            else state = new MdiStateModel { Tabs = GetDefaultTabs(), ActiveTabIndex = 0 };
            return state;
        }
        public async Task SaveState(bool InvokeChanged = false)
        {
            var orderedTabs = state?.Tabs?.OrderBy(i => i.VisibleIndex).ToList();
            if (orderedTabs is null) return;
            for (int i = 0; i < orderedTabs.Count; i++) 
            {
                orderedTabs[i].VisibleIndex = i;
            }
            if (state is null) return;
            await SaveStateToLocalStorageAsync(state);
            if (InvokeChanged) OnTabsChanged?.Invoke();
        }
        public async Task SetActiveTabIndex(int newIndex) 
        {
            state.ActiveTabIndex = newIndex;
            await SaveState();
        }

        public async Task AddTab(string Text, string TypeName, Dictionary<string, object>? Parameters = null) 
        {
            state?.Tabs?.Add(new MdiTabModel { Text = Text, TabTypeName = TypeName, Parameters = Parameters, Visible = true, VisibleIndex = state.Tabs.Count });
            await SaveState(true);
        }
        public async Task SetAllTabsVisible(bool Visible) 
        {
            state?.Tabs?.ForEach(t => t.Visible = Visible);
            await SaveState(true);
        }
        public void SetTabVisible(int Index, bool Visible) 
        {
            var tab = state?.Tabs?.FirstOrDefault(tab => tab.VisibleIndex == Index);
            if (tab == null) { return; }
            tab.Visible = Visible;
        }

        public async Task RemoveTab(int Index) 
        {
            var tab = state?.Tabs?.FirstOrDefault(tab => tab.VisibleIndex == Index);
            if (tab == null) { return; }
            state?.Tabs?.Remove(tab);
            await SaveState(true);
        }
        public async Task RemoveAllTabs()
        {
            state?.Tabs?.Clear();
            await SaveState(true);
        }
        public async Task RemoveAllButTab(int Index)
        {
            var tab = state?.Tabs?.FirstOrDefault(tab => tab.VisibleIndex == Index);
            if (tab == null) { return; }
            state?.Tabs?.Clear();
            state?.Tabs?.Add(tab);
            await SaveState(true);
        }
        public void ReorderTabs(int fromIndex, int toIndex)
        {
            if (fromIndex == toIndex || fromIndex < 0 || toIndex < 0 ||
                fromIndex >= state?.Tabs?.Count || toIndex >= state?.Tabs?.Count)
                return;

            var orderedTabs = state?.Tabs?.OrderBy(i => i.VisibleIndex).ToList();
            if (orderedTabs is null) return;

            var tabToMove = orderedTabs[fromIndex];
            orderedTabs.RemoveAt(fromIndex);
            orderedTabs.Insert(toIndex, tabToMove);
            for (int i = 0; i < orderedTabs.Count; i++)
            {
                orderedTabs[i].VisibleIndex = i;
            }
            for (int i = 0; i < orderedTabs.Count; i++)
            {
                var originalTab = state?.Tabs?.First(x => x == orderedTabs[i]);
                if (originalTab != null) originalTab.VisibleIndex = orderedTabs[i].VisibleIndex;
            }
        }



        private List<MdiTabModel> GetDefaultTabs() 
        {
            List<MdiTabModel> result = new();
            result.Add(new MdiTabModel { Text = "Customers", Visible = true, VisibleIndex = 0, TabTypeName = "Customers" });
            return result;
        }
        private async Task SaveStateToLocalStorageAsync(MdiStateModel state)
        {
            try
            {
                var json = JsonSerializer.Serialize(state);
                await _js.InvokeVoidAsync("localStorage.setItem", LOCAL_STORAGE_KEY, json);
            }
            catch { return; }
        }

        private async Task<MdiStateModel?> LoadStateFromLocalStorageAsync()
        {
            try
            {
                var json = await _js.InvokeAsync<string>("localStorage.getItem", LOCAL_STORAGE_KEY);
                return JsonSerializer.Deserialize<MdiStateModel>(json);
            }
            catch { return null; }
        }
    }
}
