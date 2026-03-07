document.addEventListener("DOMContentLoaded", main);
window.addEventListener("resize", main);

function setTableColumnWidth() {
    let tableHeadings = Array.from(document.querySelectorAll("th"));
    let tableData = Array.from(document.querySelectorAll("td"));
    if (tableHeadings.length == 0 || tableData.length == 0) return;

    let highestWidth = tableData.map(element => element.getBoundingClientRect().width).reduce((highestValue, currentValue) => highestValue < currentValue ? currentValue : highestValue);

    tableHeadings.forEach(function (heading) {
        heading.style.width = `${Math.ceil(highestWidth)}px`;
    })
}
function setDateTableSizeAndPosition() {
    let tables = Array.from(document.querySelectorAll(".date-table"));
    let dateInputField = document.querySelector(".date-input-field");
    if (tables.length == 0 || !dateInputField) return;

    dateInputField = dateInputField.getBoundingClientRect();
    let dateInputFieldSize = [Math.ceil(dateInputField.width), Math.ceil(dateInputField.height)];
    let dateInputFieldPosition = [dateInputField.left, dateInputField.top];

    tables.forEach(function (table) {
        table.style.width = `${dateInputFieldSize[0]}px`;
    });
     
    let dayFields = Array.from(document.querySelectorAll(".date-table-days > *"));
    if (!dayFields) return;

    dayFields.forEach(function (day) {
        day.style.setProperty("--square-size", `${Math.ceil(day.getBoundingClientRect().width)}px`);
    });

    tables.forEach(function (table) {
        table.style.left = `${dateInputFieldPosition[0]}px`;
        table.style.top = `${dateInputFieldPosition[1] + dateInputFieldSize[1]}px`;
    });
}
function main() {
    setTableColumnWidth();
    setDateTableSizeAndPosition();
}
function onDateInputFocus(event) {
    event.preventDefault();
    let table = document.querySelector(".date-table");
    table.classList.add("active");
}
function onDateInputBlur(event) {
    event.preventDefault();
    let relatedTarget = event.relatedTarget;
    if (!relatedTarget) {
        let table = document.querySelector(".date-table");
        table.classList.remove("active");
    }
    else if ((relatedTarget.hasAttribute("date-value") || relatedTarget.hasAttribute("day-name") || relatedTarget.hasAttribute("month-year"))) {
        event.target.focus();
    }
    else {
        let table = document.querySelector(".date-table");
        table.classList.remove("active");
    }
}
function onDateDayClick(event) {
    event.stopPropagation();
    let dateInputField = document.querySelector(".date-input-field");
    dateInputField.value = event.target.getAttribute("date-value");
    dateInputField.dispatchEvent(new InputEvent("input"));
}
async function onDateInput(event) {
    const url = window.location.href;
    try {
        const response = await fetch(`${url}?handler=Date&Date=${event.target.value}`)
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const timeSlotOccupancy = await response.json();
        const timeSlotOptions = Array.from(document.getElementsByClassName('time-slot-option'));
        timeSlotOptions.forEach(function (timeSlot,i) {
            if (timeSlotOccupancy[i] > 20) {
                timeSlot.classList.add("hidden");
            }
            else {
                timeSlot.classList.remove("hidden");
            }
        });

        console.log(timeSlotOccupancy);
    } catch (error) {
        console.error(error.message);
    }
}
async function onTimeSlotInput(event) {
    const url = window.location.href;
    try {
        const dateInputValue = document.getElementsByClassName("date-input-field")[0].value;

        const response = await fetch(`${url}?handler=Time&Date=${dateInputValue}&TimeSlot=${event.target.value}`)
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const maxPeople = await response.json();

        const partySizeInput = document.getElementsByClassName("party-size-input")[0];
        const form = document.getElementsByClassName("create-form")[0];

        $(form).removeData('validator');
        $(form).removeData('unobtrusiveValidation');

        partySizeInput.setAttribute('data-val-range-max', `${maxPeople}`);
        partySizeInput.setAttribute('data-val-range', `The field PartySize must be between 0 and ${maxPeople}.`);

        $.validator.unobtrusive.parse(form);
        if (partySizeInput.value) $(partySizeInput).valid();
    } catch (error) {
        console.error(error.message);
    }
}
function onReservationClick(id) {
    const detailsButton = document.getElementById(`${id}`);
    detailsButton.click();
}