<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/955986368/24.2.1%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T1288385)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
[![](https://img.shields.io/badge/💬_Leave_Feedback-feecdd?style=flat-square)](#does-this-example-address-your-development-requirementsobjectives)
<!-- default badges end -->
# Blazor Tabs - Create a Dynamic Tabbed Interface

The example creates an interactive, multi-tab web interface using DevExpress Blazor [Tabs](https://docs.devexpress.com/Blazor/405074/components/layout/tabs), [Context Menu](https://docs.devexpress.com/Blazor/405060/components/navigation-controls/context-menu) components.


![Multi-Tab UI](images/blazor-tabbed-ui.png)

It illustrates how end users can create personalized workspaces and multitask effectively.

## Implementation Details

### Organize Content Into Tabs

The [MdiTabs](CS/blazor_multi_tab_ui/Components/MDI/MdiTabs.razor) component is based on [DxTabs](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxTabs) control. 
Obtain the tabs state and render tabs in a loop. Use [DxTabPage](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxTabPage) for each tab. Insert your custom Blazor components or content directly into each `DxTabPage`.

In this example the tab content is rendered dynamically: [DynamicComponent](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.components.dynamiccomponent?view=aspnetcore-9.0)

```razor
<DxTabs ActiveTabIndex=activeTabIndex
        ActiveTabIndexChanged="OnActiveTabIndexChanged"
        AllowTabReorder="true"
        TabReordering="OnTabReordering"
        TabClosing="OnTabClosing"
        RenderMode="TabsRenderMode.Default">
    @for (int i = 0; i < tabsCollection.Count; i++)
    {
        var tabModel = tabsCollection[i];
        <DxTabPage  AllowClose="true"
                    CssClass="@TabCssClass"
                    VisibleIndex="@tabModel.VisibleIndex"
                    Visible="@tabModel.Visible"
                    Text="@tabModel.Text">
            <ChildContent>
                @if (stateService.TryGetType(tabModel.TabTypeName, out var type))
                {
                    <DynamicComponent @key="@tabModel.Id" Type="type" Parameters="tabModel.Parameters" />
                }
                else
                {
                    <DynamicComponent @key="@tabModel.Id" Type="@typeof(Unknown)" />
                }

            </ChildContent>
        </DxTabPage>
    }
</DxTabs>
```

### Persist Tab State

Implement a custom `MdiTabModel` class ([MdiTabModel.cs](CS/blazor_multi_tab_ui/Models/MdiTabModel.cs)) to encapsulate properties associated with each individual tab. The `MdiStateModel` class ([MdiStateModel.cs](CS/blazor_multi_tab_ui/Models/MdiStateModel.cs)) represents the overall state of the [MdiTabs](CS/blazor_multi_tab_ui/Components/MDI/MdiTabs.razor) component — including visibility, display order, active tab index, and other properties of all tabs. The `VisibleIndex` property links an underlying object to its visual tab representation in the UI.

Bind these properties to the visual tab elements in the UI. To ensure that the state model accurately reflects the live interface, implement event handlers for `TabReorder`, `TabClosing`, and `ActiveTabIndexChanged`. These handlers listen for user actions and dynamically update the state to match the current tab layout.

The [MdiStateService](CS/blazor_multi_tab_ui/Services/MdiStateService.cs) service contains all methods required for managing state — including loading, saving, and updating properties. To maintain the tab layout across sessions, the state can be stored in local storage, session storage, cookies, or a database. In this example, `MdiStateService` stores the state in local storage. The service serializes the entire `MdiStateModel` to JSON and saves it to the browser's local storage whenever the UI layout changes. This preserves tab visibility, order, and the active tab even after the browser is closed and reopened. Tab state is restored in the `OnInitializedAsync` method of the `MdiTabs` component.

### Add Context Menu to Tabs

Add a context menu that lets users manage tabs more flexibly. The available operations are:

- **Close** the current tab.
- **Close** all tabs.
- **Close** all tabs except the current one.
- **Hide** the current tab.
- **Hide** all tabs.
- **Hide** all tabs except the current one.
- **Restore** previously hidden tabs.

Embed the [`TabsContextMenu`](CS/blazor_multi_tab_ui/Components/MDI/TabsContextMenu.razor) component inside [`MdiTabs`](CS/blazor_multi_tab_ui/Components/MDI/MdiTabs.razor).  
`TabsContextMenu` renders a [DxContextMenu](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxContextMenu) with one [DxContextMenuItem](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxContextMenuItem) per action:

```razor
<DxContextMenu @ref="menu">
    <Items>
        <DxContextMenuItem Text="Hide">
            <Items>
                <DxContextMenuItem Click="HideTab"       Text="This"       />
                <DxContextMenuItem Click="HideAllTabs"   Text="All Tabs"   />
                <DxContextMenuItem Click="HideOtherTabs" Text="Other Tabs" />
            </Items>
        </DxContextMenuItem>

        <DxContextMenuItem Click="RestoreHiddenTabs" Text="Restore Hidden Tabs" />

        <DxContextMenuItem Text="Close">
            <Items>
                <DxContextMenuItem Click="CloseTab"       Text="This"       />
                <DxContextMenuItem Click="CloseAllTabs"   Text="All Tabs"   />
                <DxContextMenuItem Click="CloseOtherTabs" Text="Other Tabs" />
            </Items>
        </DxContextMenuItem>
    </Items>
</DxContextMenu>
```

Use the `TabSelector` parameter to find the `MdiTabs` elemnt and its tab-header elements to target.  
At runtime, the companion script [`TabsContextMenu.razor.js`](CS/blazor_multi_tab_ui/Components/MDI/TabsContextMenu.razor.js) performs the following:

- Finds the matching elements via the selector.
- Suppresses the browser’s default context menu.
- Captures the mouse position and invokes a `[JSInvokable]` .NET method that opens the DevExpress menu at the pointer location.

When a menu item is clicked, the handler calls the corresponding method of [`MdiStateService`](CS/blazor_multi_tab_ui/Services/MdiStateService.cs) to update the tab state (close, hide, or restore) and persist the change if required.


## Files to Review

- [`MdiTabs.razor`](CS/blazor_multi_tab_ui/Components/MDI/MdiTabs.razor) – main tabbed UI component
- [`MdiTabModel.cs`](CS/blazor_multi_tab_ui/Models/MdiTabModel.cs) – defines individual tab structure
- [`MdiStateModel.cs`](CS/blazor_multi_tab_ui/Models/MdiStateModel.cs) – represents the overall state of all tabs
- [`MdiStateService.cs`](CS/blazor_multi_tab_ui/Services/MdiStateService.cs) – manages tab state, persistence, and updates
- [`TabsContextMenu.razor`](CS/blazor_multi_tab_ui/Components/MDI/TabsContextMenu.razor) – context menu UI and logic
- [`TabsContextMenu.razor.js`](CS/blazor_multi_tab_ui/Components/MDI/TabsContextMenu.razor.js) – JS interop for right-click and menu activation
- [`Tabs`](CS/blazor_multi_tab_ui/Components/MDI/MdiTabs.razor/Tabs) – folder with tab content components (one per tab type)

## Documentation

- [DxTabs Class](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxTabs)
- [DxTabPage Class](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxTabPage)
- [DxContextMenu Class](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxContextMenu)
- [DxContextMenuItem Class](https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxContextMenuItem)

## More Examples

- [Form Layout for Blazor - Tabbed Wizard](https://github.com/DevExpress-Examples/Form-Layout-for-Blazor-Tabbed-Wizard)

<!-- feedback -->
## Does this example address your development requirements/objectives?

[<img src="https://www.devexpress.com/support/examples/i/yes-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=blazor-multi-tab-ui&~~~was_helpful=yes) [<img src="https://www.devexpress.com/support/examples/i/no-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=blazor-multi-tab-ui&~~~was_helpful=no)

(you will be redirected to DevExpress.com to submit your response)
<!-- feedback end -->
