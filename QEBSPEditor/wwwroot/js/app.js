function attachTooltip(ref,text) {
    return new bootstrap.Tooltip(ref, {
        title: text
    });
}

function updateTooltip(tooltip, text) {
    tooltip.setContent(text);
}

function setTextareaSelection(element, pos) {
    element.selectionStart = pos;
    element.selectionEnd = pos;

    element.blur();
    element.focus();
}

function clickElement(element) {
    element.click();
}