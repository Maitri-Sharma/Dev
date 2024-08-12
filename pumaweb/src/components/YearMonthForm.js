import React, { useEffect, useState } from "react";
import DayPicker from "react-day-picker";
import "react-day-picker/lib/style.css";

var currentYear = new Date().getFullYear();
const fromMonth = new Date(currentYear, 0);
const toMonth = fromMonth;

var currentmonth = new Date().getMonth();

function YearMonthForm({
  date,
  localeUtils,
  onChange,
  Finn,
  Fromdate,
  changeyear,
  Calendar,
  selection,
  changeable,
}) {
  const [Display, setDisplay] = useState(false);
  const [YearValue, setYearValue] = useState(currentYear);
  const [MonthValues, setMonthValues] = useState(currentmonth);

  const months1 = [
    "Januar",
    "Februar",
    "Mars",
    "April",
    "Mai",
    "Juni",
    "Juli",
    "August",
    "September",
    "Oktober",
    "November",
    "Desember",
    "Januar",
    "Februar",
    "Mars",
    "April",
    "Mai",
    "Juni",
    "Juli",
    "August",
    "September",
    "Oktober",
    "November",
  ];

  useEffect(() => {
    if (changeable) {
      onChange(
        new Date(currentYear, currentmonth + 1),
        currentYear,
        currentmonth + 1
      );
    }
  }, []);

  let months = [];
  const years = [];
  if (Calendar === "normalCalendar") {
    //  if (year.value == currentYear) {
    // if(currentmonth < 9 )
    // {
    // for (let i = currentmonth; i < currentmonth + (12 - currentmonth); i++) {
    //   months.push(months1[i]);
    // }
    // const years = [];
    if (currentmonth >= 10) {
      for (
        let i = fromMonth.getFullYear();
        i <= toMonth.getFullYear() + 1;
        i += 1
      ) {
        years.push(i);
      }
    } else {
      for (
        let i = fromMonth.getFullYear();
        i <= toMonth.getFullYear();
        i += 1
      ) {
        years.push(i);
      }
    }

    if (changeyear == currentYear) {
      if (currentmonth >= 10) {
        for (
          let i = currentmonth;
          i < currentmonth + (12 - currentmonth);
          i++
        ) {
          months.push(months1[i]);
        }
      } else {
        for (let i = currentmonth; i < currentmonth + 3; i++) {
          months.push(months1[i]);
        }
      }
    } else if (changeyear == currentYear + 1) {
      for (let i = 0; i < 2 - (11 - currentmonth); i++) {
        months.push(months1[i]);
      }
    }
    // else {
    //   for (let i = 0; i < 12; i++) {
    //     months.push(months1[i]);
    //   }
    // }
    // }
    //   else{

    //   }

    // for (
    //   let i = fromMonth.getFullYear();
    //   i <= toMonth.getFullYear() + 1;
    //   i += 1
    // ) {
    //   years.push(i);
    // }
  } else {
    if (currentmonth == 0) {
      for (
        let i = fromMonth.getFullYear();
        i <= toMonth.getFullYear() + 1;
        i += 1
      ) {
        years.push(i);
      }
    } else {
      for (
        let i = fromMonth.getFullYear();
        i <= toMonth.getFullYear() + 2;
        i += 1
      ) {
        years.push(i);
      }
    }

    if (changeyear == currentYear) {
      for (let i = currentmonth; i < currentmonth + (12 - currentmonth); i++) {
        months.push(months1[i]);
      }
    } else if (changeyear == currentYear + 2) {
      for (let i = 0; i <= currentmonth - 1; i++) {
        months.push(months1[i]);
      }
    } else {
      for (let i = 0; i < 12; i++) {
        months.push(months1[i]);
      }
    }
  }

  const handleChange = (e) => {
    // e.preventDefault();
    const { year, month } = e.target.form;

    if (Calendar !== "normalCalendar") {
      if (
        month.value >= currentmonth &&
        year.value == years[years.length - 1]
      ) {
        month.value = 0;
        onChange(new Date(year.value, 0), year.value, 0);
      } else if (month.value == 0 && year.value == currentYear) {
        onChange(new Date(year.value, currentmonth), year.value, currentmonth);
      } else if (month.value < currentmonth && year.value == currentYear) {
        onChange(
          new Date(year.value, months.length),
          year.value,
          months.length
        );
      } else {
        month.value = month.value;
        onChange(new Date(year.value, month.value), year.value, month.value);
      }
    } else {
      if (month.value > months.length - 1 && year.value == currentYear + 1) {
        month.value = 0;
        onChange(new Date(year.value, 0), year.value, 0);
      } else if (month.value == 0 && year.value == currentYear) {
        onChange(new Date(year.value, currentmonth), year.value, currentmonth);
      } else if (month.value < currentmonth && year.value == currentYear) {
        // month.value = currentmonth;
        onChange(
          new Date(year.value, months.length),
          year.value,
          months.length
        );
      } else {
        month.value = month.value;
        onChange(new Date(year.value, month.value), year.value, month.value);
      }

      setYearValue(year.value);
      setMonthValues(month.value);
    }
  };

  return (
    <form className="DayPicker-Caption" style={{ marginTop: "10px" }}>
      {currentYear == changeyear ? (
        <select name="month" onChange={handleChange} value={date.getMonth()}>
          {months.map((month, i) => (
            <option key={month} value={i + new Date().getMonth()}>
              {month}
            </option>
          ))}
        </select>
      ) : (
        <select name="month" onChange={handleChange} value={date.getMonth()}>
          {months.map((month, i) => (
            <option key={month} value={i}>
              {month}
            </option>
          ))}
        </select>
      )}
      <select name="year" onChange={handleChange} value={date.getFullYear()}>
        {years.map((year) => (
          <option key={year} value={year}>
            {year}
          </option>
        ))}
      </select>
    </form>
  );
}

export default YearMonthForm;
