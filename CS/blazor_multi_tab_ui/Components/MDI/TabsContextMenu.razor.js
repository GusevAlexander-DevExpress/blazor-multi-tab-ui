export function addContextMenuHandler(tabSelector, dotNetObject) {
    var tabElements = document.querySelectorAll(tabSelector);
    if (!tabElements) return;

    tabElements.forEach(tabElement => {

        tabElement.addEventListener('contextmenu', (event) => {
            event.preventDefault();
            let eventArgs = {
                clientX: event.clientX,
                clientY: event.clientY,
                screenX: event.screenX,
                screenY: event.screenY,
                offsetX: event.offsetX,
                offsetY: event.offsetY,
                pageX: event.pageX,
                pageY: event.pageY,
                button: event.button,
                buttons: event.buttons,
                ctrlKey: event.ctrlKey,
                shiftKey: event.shiftKey,
                altKey: event.altKey,
                metaKey: event.metaKey,
                detail: event.detail,
                type: event.type
            };
            var index = parseInt(tabElement.getAttribute("index"));
            dotNetObject.invokeMethodAsync("ShowContextMenu", eventArgs, index);
        });
    });
};