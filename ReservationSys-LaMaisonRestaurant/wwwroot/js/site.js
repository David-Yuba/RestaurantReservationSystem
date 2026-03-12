let IS_PRIVATE_DINING = false;

document.addEventListener("DOMContentLoaded", main);
window.addEventListener("resize", main);

function getHeaderHeight() {
    const headerElement = document.querySelector("header");
    document.body.style.setProperty("--header-height", `${headerElement.getBoundingClientRect().height}px`);
}
function getNavButtonHeight() {
    const navButton = document.querySelector(".mobile-navigation-button");
    if (!navButton) return;

    document.body.style.setProperty("--mobile-navigation-button-height", `${navButton.getBoundingClientRect().height}px`);
}
function positionLegendTableAndButton() {
    const legendTable = document.getElementsByClassName("legend-table")[0];
    const legendButton = document.getElementsByClassName("legend-button")[0];
    if (!legendTable || !legendButton) return;

    const buttonWidth = legendTable.getBoundingClientRect().width;
    const buttonHeight = legendTable.getBoundingClientRect().height;
    legendButton.style.width = `${buttonWidth}px`

    const tableTop = legendButton.getBoundingClientRect().top;
    const tableLeft = legendButton.getBoundingClientRect().left;
    legendTable.style.top = `${tableTop + legendButton.getBoundingClientRect().height}px`
    legendTable.style.left = `${tableLeft}px`;
    
}
function setTableColumnWidth() {
    const tableHeadings = Array.from(document.querySelectorAll("th"));
    const rightPadding = 32;
    const tableData = Array.from(document.querySelectorAll("td"));
    if (tableHeadings.length == 0 || tableData.length == 0) return;

    let columnWidths = [];
    for (let i = 0; i < tableHeadings.length ; i++){
        const tableHeadingWidth = tableHeadings[i].getBoundingClientRect().width;
        for (let j=i ; j<tableData.length ; j+=tableHeadings.length+1){
            const currentCellWidth = tableData[j].getBoundingClientRect().width;
            if (columnWidths.length == i) columnWidths[i] = currentCellWidth;
            else if (columnWidths[i] < currentCellWidth) columnWidths[i] = currentCellWidth;
        }
        if (columnWidths[i]<tableHeadingWidth)
            columnWidths[i] = tableHeadingWidth;
    }

    console.log(columnWidths);
    for (let i = 0; i < tableHeadings.length; i++)
        tableHeadings[i].style.width = `${Math.ceil(columnWidths[i]) + rightPadding}px`;

    for (let i=0 ; i<tableData.length ; i++)
        for (let j=i ; j<tableData.length ; j+=tableHeadings.length+1)
            tableData[j].style.width = `${Math.ceil(columnWidths[i]) + rightPadding}px`;
    
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
    getHeaderHeight();
    setTableColumnWidth();
    setDateTableSizeAndPosition();
    positionLegendTableAndButton();
    getNavButtonHeight();
}
function onMobileMenuClick() {
    const mobileMenu = document.getElementsByClassName("mobile-main-navigation")[0];
    const mobileMenuButton = document.getElementsByClassName("mobile-navigation-button")[0];
    mobileMenu.classList.toggle("active");
    mobileMenuButton.classList.toggle("active");
}
function onMobileMenuBlur() {
    const mobileMenu = document.getElementsByClassName("mobile-main-navigation")[0];
    const mobileMenuButton = document.getElementsByClassName("mobile-navigation-button")[0];
    mobileMenu.classList.remove("active");
    mobileMenuButton.classList.remove("active");
}
function onLegendClick() {
    const legendTable = document.getElementsByClassName("legend-table")[0];
    if (!legendTable) return;

    legendTable.classList.toggle("invisible");
}
function onLegendBlur(event) {
    const legendTable = document.getElementsByClassName("legend-table")[0];
    legendTable.classList.add("invisible");
}
function onDateInputFocus(event) {
    event.preventDefault();
    let table = document.querySelector(".date-table");
    setDateTableSizeAndPosition();
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
function onPrivateDiningInput() {
    const url = window.location.href;
    const timeSlotOptionEls = Array.from(document.getElementsByClassName("time-slot-option"));
    const dateInputEl = document.getElementsByClassName("date-input-field")[0];
    const timeSlotInputEl = document.getElementsByClassName("time-slot-input")[0];
    IS_PRIVATE_DINING = !IS_PRIVATE_DINING;

    if (IS_PRIVATE_DINING) {
        const partySizeInputEl = document.getElementsByClassName("party-size-input")[0];
        const form = document.getElementsByClassName("create-form")[0];

        $(form).removeData('validator');
        $(form).removeData('unobtrusiveValidation');

        partySizeInputEl.setAttribute('data-val-range-min', `${6}`);
        partySizeInputEl.setAttribute('data-val-range-max', `${12}`);
        partySizeInputEl.setAttribute('data-val-range', `The field PartySize must be between 6 and 12.`);

        $.validator.unobtrusive.parse(form);

        const privateDiningTimeSlots = ["18:00", "18:30", "19:00", "19:30", "20:00", "20:30", "21:00"]
        timeSlotOptionEls.forEach(function (el) {
            if (privateDiningTimeSlots.some((slot) => slot == el.value)) {
                el.classList.remove("hidden");
            }
            else el.classList.add("hidden")
        });
        updateView(url, partySizeInputEl, dateInputEl, timeSlotInputEl)
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
        const response = await fetch(`${url}?handler=Size&PartySize=${partySizeInputEl.value}&Date=${reformatDateString(dateInputEl.value)}&TimeSlot=${timeSlotInputEl.value}`)
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        let occupancyList = await response.json();
        
        const timeSlotOptionEls = Array.from(document.getElementsByClassName("time-slot-option"));

        timeSlotOptionEls.forEach(function (el) {
            let maxPeople = timeSlotIsNotFull(occupancyList, el, partySizeInputEl.value);

            if ((partySizeInputEl.value ? parseInt(partySizeInputEl.value) : 0) <= maxPeople) {
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
            isCurrentValueValid(timeSlotInputEl, occupancyList, partySizeInputEl.value);
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
    const selectedDay = new Date(reformatDateString(dateInputEl.value)).getDay();
    const privateDiningFieldEl = document.getElementsByClassName("private-dining-field")[0];
    if (selectedDay == 5 || selectedDay == 6) {
        privateDiningFieldEl.classList.remove("hidden");
    }
    else {
        const privateDiningFieldIn = document.getElementsByClassName("private-dining-input")[0];
        privateDiningFieldEl.classList.add("hidden");
        if (IS_PRIVATE_DINING) {
            privateDiningFieldIn.click();
        }
    }

if (IS_PRIVATE_DINING) {
    await updateView(url, partySizeInputEl, dateInputEl, timeSlotInputEl);

    return;
}

    try {
        const response = await fetch(`${url}?handler=Size&PartySize=${partySizeInputEl.value}&Date=${reformatDateString(dateInputEl.value)}&TimeSlot=${timeSlotInputEl.value}`)
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const occupancyList = await response.json();
        const timeSlotOptionEls = Array.from(document.getElementsByClassName("time-slot-option"));

        timeSlotOptionEls.forEach(function (el) {
            let maxPeople = timeSlotIsNotFull(occupancyList, el, partySizeInputEl.value);
            if ((parseInt(partySizeInputEl.value) ? parseInt(partySizeInputEl.value) : 0 ) <= maxPeople) {
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
            isCurrentValueValid(timeSlotInputEl, occupancyList, partySizeInputEl.value);
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
        const response = await fetch(`${url}?handler=Size&PartySize=${partySizeInputEl.value}&Date=${reformatDateString(dateInputEl.value) }&TimeSlot=${timeSlotInputEl.value}`)
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        const occupancyList = await response.json();

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
            isCurrentValueValid(timeSlotInputEl, occupancyList, partySizeInputEl.value);
        }
    } catch (error) {
        console.error(error.message);
    }
}

function reformatDateString(d) {
    const [date, month, year] = d.split("-");
    return `${month}-${date}-${year}`;
}
function reformatTimeString(time) {
    const [hours, minutes] = time.split(':');
    return `${hours}:${minutes}`
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
function isCurrentValueValid(el, occupancyList, partySize) {

if (IS_PRIVATE_DINING) {
    if (occupancyList.some(slot => reformatTimeString(slot) == el.value)) {
        el.value = "";
        return;
    }
    return;
}

    const timeSlot = occupancyList.find(occupancySlot => reformatTimeString(occupancySlot.timeSlot) == el.value);
    if (timeSlot) {
        let newMaxPartySize = 20 - timeSlot.partySize;
        if (newMaxPartySize < parseInt(partySize)) {
            el.value = "";
        }
    }
}

async function updateView(url, partySizeInputEl, dateInputEl, timeSlotInputEl) {
    try {
        const response = await fetch(`${url}?handler=PrivateDining&Date=${reformatDateString(dateInputEl.value) }&TimeSlot=${timeSlotInputEl.value}`)
        if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
        }
        let occupancyList = await response.json();
        const timeSlotOptionEls = Array.from(document.getElementsByClassName("time-slot-option"));

        const privateDiningTimeSlots = ["18:00:00", "18:30:00", "19:00:00", "19:30:00", "20:00:00", "20:30:00", "21:00:00"]
        timeSlotOptionEls.forEach(function (el) {
            if (occupancyList.some(slot => reformatTimeString(slot) == el.value)) {
                el.classList.add("hidden");
            }
            else if (privateDiningTimeSlots.some((slot) => slot == el.value))
                el.classList.remove("hidden");
        });

        if (partySizeInputEl.value) $(partySizeInputEl).valid();
        if (timeSlotInputEl.value) {
            isCurrentValueValid(timeSlotInputEl, occupancyList, 0);
        }
    } catch (error) {
        console.error(error.message);
    }
}
function reformatData() {
    const dateInputEl = document.getElementsByClassName("date-input-field")[0];
    if (dateInputEl.value == "") return;

    const [day, month, year] = dateInputEl.value.split('-');
    dateInputEl.value = `${month}-${day}-${year}`;
    setTimeout(function () {
        dateInputEl.value = `${day}-${month}-${year}`;
    }, 1)
}