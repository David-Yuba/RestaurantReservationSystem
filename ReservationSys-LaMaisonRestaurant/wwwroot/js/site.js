let IS_PRIVATE_DINING = false;

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
function onReservationClick(id) {
    const detailsButton = document.getElementById(`${id}`);
    detailsButton.click();
}
function onPrivateDiningChange() {
    IS_PRIVATE_DINING = !IS_PRIVATE_DINING;
    const timeSlotOptionEls = Array.from(document.getElementsByClassName("time-slot-option"));

    if (IS_PRIVATE_DINING) {
        const partySizeInputEl = document.getElementsByClassName("party-size-input")[0];
        const form = document.getElementsByClassName("create-form")[0];

        $(form).removeData('validator');
        $(form).removeData('unobtrusiveValidation');

        partySizeInputEl.setAttribute('data-val-range-min', `${6}`);
        partySizeInputEl.setAttribute('data-val-range-max', `${12}`);
        partySizeInputEl.setAttribute('data-val-range', `The field PartySize must be between 6 and 12.`);

        $.validator.unobtrusive.parse(form);

        const privateDiningTimeSlots = ["6:00 PM", "6:30 PM", "7:00 PM", "7:30 PM", "8:00 PM", "8:30 PM", "9:00 PM"]
        timeSlotOptionEls.forEach(function (el) {
            if (privateDiningTimeSlots.some((slot) => slot == el.value)) {
                el.classList.remove("hidden");
            }
            else el.classList.add("hidden")
        });
        if (partySizeInputEl.value) $(partySizeInputEl).valid();
    }
    else {
        const partySizeInputEl = document.getElementsByClassName("party-size-input")[0];
        const form = document.getElementsByClassName("create-form")[0];

        $(form).removeData('validator');
        $(form).removeData('unobtrusiveValidation');

        partySizeInputEl.setAttribute('data-val-range-min', `${0}`);
        partySizeInputEl.setAttribute('data-val-range-max', `${10}`);
        partySizeInputEl.setAttribute('data-val-range', `The field PartySize must be between 0 and 10.`);

        $.validator.unobtrusive.parse(form);

        timeSlotOptionEls.forEach(function (el) {
            el.classList.remove("hidden"); 
        });
        if (partySizeInputEl.value) $(partySizeInputEl).valid();
    }
}

async function onPartySizeInput(event) {
    const url = window.location.href;
    const partySizeInputEl = document.getElementsByClassName("party-size-input")[0];
    const dateInputEl = document.getElementsByClassName("date-input-field")[0];
    const timeSlotInputEl = document.getElementsByClassName("time-slot-input")[0];
    if (!partySizeInputEl || !dateInputEl || !timeSlotInputEl) return;

if (IS_PRIVATE_DINING) {
    await updateView(url, partySizeInputEl, dateInputEl, timeSlotInputEl);

    return;
}

    try {
        const response = await fetch(`${url}?handler=Size&PartySize=${partySizeInputEl.value}&Date=${dateInputEl.value}&TimeSlot=${timeSlotInputEl.value}`)
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        let occupancyList = await response.json();

        const timeSlotOptionEls = Array.from(document.getElementsByClassName("time-slot-option"));

        timeSlotOptionEls.forEach(function (el) {
            if (occupancyList.some(slot => reformatStringDate(slot.timeSlot) == el.value)) {
                el.classList.add("hidden");
            }
            else el.classList.remove("hidden")
        });


        const newMaxPartySize = getNewMaxPartySize(occupancyList, timeSlotInputEl.value);
        const form = document.getElementsByClassName("create-form")[0];

        $(form).removeData('validator');
        $(form).removeData('unobtrusiveValidation');

        partySizeInputEl.setAttribute('data-val-range-min', `0`);
        partySizeInputEl.setAttribute('data-val-range-max', `${newMaxPartySize}`);
        partySizeInputEl.setAttribute('data-val-range', `The field PartySize must be between 0 and ${newMaxPartySize}.`);

        $.validator.unobtrusive.parse(form);

        if (partySizeInputEl.value) $(partySizeInputEl).valid();
        if (timeSlotInputEl.value) {
            isCurrentValueValid(timeSlotInputEl, occupancyList);
        }
    } catch (error) {
        console.error(error.message);
    }
}
async function onDateInput(event) {
    const url = window.location.href;
    const partySizeInputEl = document.getElementsByClassName("party-size-input")[0];
    const dateInputEl = document.getElementsByClassName("date-input-field")[0];
    const timeSlotInputEl = document.getElementsByClassName("time-slot-input")[0];

    if (!partySizeInputEl || !dateInputEl || !timeSlotInputEl) return;
    const selectedDay = new Date(dateInputEl.value).getDay();

    if (selectedDay == 5 || selectedDay == 6) {
        const privateDiningFieldEl = document.getElementsByClassName("private-dining-field")[0];
        privateDiningFieldEl.classList.remove("hidden");
    }
    else {
        const privateDiningFieldEl = document.getElementsByClassName("private-dining-field")[0];
        privateDiningFieldEl.classList.add("hidden");
        if (IS_PRIVATE_DINING) privateDiningFieldEl.click();
    }

if (IS_PRIVATE_DINING) {
    await updateView(url, partySizeInputEl, dateInputEl, timeSlotInputEl);

    return;
}

    try {
        const response = await fetch(`${url}?handler=Size&PartySize=${partySizeInputEl.value}&Date=${dateInputEl.value}&TimeSlot=${timeSlotInputEl.value}`)
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const occupancyList = await response.json();
        const timeSlotOptionEls = Array.from(document.getElementsByClassName("time-slot-option"));

        timeSlotOptionEls.forEach(function (el) {
            let maxPeople = timeSlotIsNotFull(occupancyList, el, partySizeInputEl.value);
            if (maxPeople > 0 && maxPeople < 20) {
                el.classList.remove("hidden");
            }
            else el.classList.add("hidden");
        });

        const newMaxPartySize = getNewMaxPartySize(occupancyList, timeSlotInputEl.value);
        const form = document.getElementsByClassName("create-form")[0];
        $(form).removeData('validator');
        $(form).removeData('unobtrusiveValidation');

        partySizeInputEl.setAttribute('data-val-range-min', `0`);
        partySizeInputEl.setAttribute('data-val-range-max', `${newMaxPartySize}`);
        partySizeInputEl.setAttribute('data-val-range', `The field PartySize must be between 0 and ${newMaxPartySize}.`);

        $.validator.unobtrusive.parse(form);

        if (partySizeInputEl.value) $(partySizeInputEl).valid();
        if (timeSlotInputEl.value) {
            isCurrentValueValid(timeSlotInputEl, occupancyList);
        }
    } catch (error) {
        console.error(error.message);
    }
}
async function onTimeSlotInput(event) {
    const url = window.location.href;
    const partySizeInputEl = document.getElementsByClassName("party-size-input")[0];
    const dateInputEl = document.getElementsByClassName("date-input-field")[0];
    const timeSlotInputEl = document.getElementsByClassName("time-slot-input")[0];

    if (!partySizeInputEl || !dateInputEl || !timeSlotInputEl) return;

if (IS_PRIVATE_DINING) {
    await updateView(url, partySizeInputEl, dateInputEl, timeSlotInputEl);

    return;
}

    try {
        const response = await fetch(`${url}?handler=Size&PartySize=${partySizeInputEl.value}&Date=${dateInputEl.value}&TimeSlot=${timeSlotInputEl.value}`)
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const occupancyList = await response.json();
        const timeSlotOptionEls = Array.from(document.getElementsByClassName("time-slot-option"));

        timeSlotOptionEls.forEach(function (el) {
            let maxPeople = timeSlotIsNotFull(occupancyList, el, partySizeInputEl.value);
            if (maxPeople > 0 && maxPeople < 20) {
                el.classList.remove("hidden");
            }
            else el.classList.add("hidden");
        });

        const newMaxPartySize = getNewMaxPartySize(occupancyList, timeSlotInputEl.value);
        const form = document.getElementsByClassName("create-form")[0];
        $(form).removeData('validator');
        $(form).removeData('unobtrusiveValidation');

        partySizeInputEl.setAttribute('data-val-range-min', `0`);
        partySizeInputEl.setAttribute('data-val-range-max', `${newMaxPartySize}`);
        partySizeInputEl.setAttribute('data-val-range', `The field PartySize must be between 0 and ${newMaxPartySize}.`);

        $.validator.unobtrusive.parse(form);

        if (partySizeInputEl.value) $(partySizeInputEl).valid();
        if (timeSlotInputEl.value) {
            isCurrentValueValid(timeSlotInputEl, occupancyList);
        }
    } catch (error) {
        console.error(error.message);
    }
}

function reformatTimeString(time) {
    const [hours, minutes] = time.split(':');
    return `${hours-12}:${minutes} PM`
}
function timeSlotIsNotFull(occupancyList, el, partySize) {
    let occupancySlot = occupancyList.find(slot => reformatTimeString(slot.timeSlot) == el.value);
    if (!occupancySlot || occupancySlot.partySize <= 10) return 10;

    let newMaxPartySize = 20 - occupancySlot.partySize;
    if (newMaxPartySize >= parseInt(partySize)) return newMaxPartySize;
    else return 0;
}
function getNewMaxPartySize(occupancyList, timeSlot) {
    let occupancySlot = occupancyList.find(slot => reformatTimeString(slot.timeSlot) == timeSlot);
    if (!occupancySlot || parseInt(occupancySlot.partySize) <= 10) return 10;

    return 20 - parseInt(occupancySlot.partySize);
}
function isCurrentValueValid(el, occupancyList) {

if (IS_PRIVATE_DINING) {
    if (occupancyList.some(slot => reformatTimeString(slot) == el.value)) {
        el.value = "";
        return;
    }
    return;
}

    const timeSlot = occupancyList.find(v => reformatTimeString(v.timeSlot) == el.value);
    if (timeSlot) {
        let newMaxPartySize = 20 - timeSlot.partySize;
        if (newMaxPartySize < parseInt(el.value)) {
            el.value = "";
        }
    }
}

async function updateView(url, partySizeInputEl, dateInputEl, timeSlotInputEl) {
    try {
        const response = await fetch(`${url}?handler=PrivateDining&Date=${dateInputEl.value}&TimeSlot=${timeSlotInputEl.value}`)
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        let occupancyList = await response.json();
        console.log(occupancyList);
        const timeSlotOptionEls = Array.from(document.getElementsByClassName("time-slot-option"));

        const privateDiningTimeSlots = ["6:00 PM", "6:30 PM", "7:00 PM", "7:30 PM", "8:00 PM", "8:30 PM", "9:00 PM"]
        timeSlotOptionEls.forEach(function (el) {
            if (occupancyList.some(slot => reformatTimeString(slot) == el.value)) {
                el.classList.add("hidden");
            }
            else if (privateDiningTimeSlots.some((slot) => slot == el.value))
                el.classList.remove("hidden");
        });

        if (partySizeInputEl.value) $(partySizeInputEl).valid();
        if (timeSlotInputEl.value) {
            isCurrentValueValid(timeSlotInputEl, occupancyList);
        }
    } catch (error) {
        console.error(error.message);
    }
}