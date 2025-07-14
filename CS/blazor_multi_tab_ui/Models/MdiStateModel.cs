namespace blazor_multi_tab_ui.Models
{
    public class MdiStateModel
    {
        public List<MdiTabModel>? Tabs { get; set; }
        public int ActiveTabIndex { get; set; }
    }
}
