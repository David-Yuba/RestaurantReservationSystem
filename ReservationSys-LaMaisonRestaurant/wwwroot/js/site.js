document.addEventListener("DOMContentLoaded", main);
function setTableColumnWidth() {
    let tableHeadings = Array.from(document.querySelectorAll("th"));
    let tableData = Array.from(document.querySelectorAll("td"));
    let highestWidth = tableData.map(element => element.getBoundingClientRect().width).reduce((highestValue, currentValue) => highestValue < currentValue ? currentValue : highestValue);

    tableHeadings.forEach(function (heading) {
        heading.style.width = `${Math.ceil(highestWidth)}px`;
    })
}
function main() {
    setTableColumnWidth()
}