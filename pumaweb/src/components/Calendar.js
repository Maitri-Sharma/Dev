import React, { useEffect, useState, useContext, useMountEffect } from "react";
import DayPicker, { DateUtils } from "react-day-picker";
import Helmet from "react-helmet";
import "../App.css";
import YearMonthForm from "./YearMonthForm";
import moment from "moment";
import api from "../services/api.js";
import loadingImage from "../assets/images/callbackActivityIndicator.gif";
import "moment/locale/nb";
import MomentLocaleUtils from "react-day-picker/moment";
import { KSPUContext, MainPageContext } from "../context/Context.js";
import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";
import * as query from "@arcgis/core/rest/query";
import ModelComponent1 from "./ShowRouteDetails.js";

const currentYear = new Date().getFullYear();
const WEEKDAYS_SHORT = ["Søn", "Man", "Tir", "Ons", "Tor", "Fre", "Lør"];
const monthNames = [
  "January",
  "February",
  "March",
  "April",
  "May",
  "June",
  "July",
  "August",
  "September",
  "October",
  "November",
  "December",
];

const d = new Date();
const fromMonth_0 = monthNames[d.getMonth()];

const fromMonth = fromMonth_0.concat(" ").concat(currentYear).toString();

function Calender(props) {
  const [flag, setflag] = useState(true);
  const [month, setmonth] = useState("");

  const [monthSelected, setmonthSelected] = useState(new Date());
  const [Finn, setFinn] = useState("");

  const [fontvalue, setfontvalue] = useState("");
  const [calwidth, setcalwidth] = useState("");

  const [selectedDaysToDisable, setselectedDaysToDisable] = useState([]);
  //const WEEKDAYS_SHORT = ["Sø", "Ma", "Ti", "On", "To", "Fr", "Lø"];
  const [lastday, setlastday] = useState("");
  const [firstday, setfirstday] = useState("");
  const [highlighteddays, sethighlighteddays] = useState([]);
  const [highlighteddays3, sethighlighteddays1] = useState([]);
  const [Range, setRange] = useState({});
  const [daylogic, setdaylogic] = useState(false);
  const [fromDate, setfromDate] = useState("");
  const [todate, setToDate] = useState("");
  const [loading, setloading] = useState(false);
  const [defaultshow, setdefaultshow] = useState(false);
  const [capacityAlert, setCapacityAlert] = useState(false);
  const [capacityFullyBookedAlert, setCapacityFullyBookedAlert] =
    useState(false);
  const [routeData, setRouteData] = useState([]);
  const [changeYear, setchangeYear] = useState(currentYear);
  const [disableall, setdisabelall] = useState(false);
  const [rendercalendar, setrendercalendar] = useState(false);
  const [changeable, setchangeable] = useState(false);
  const [showList, setShowList] = useState("");
  const [bookedReolID, setBookedReolId] = useState([]);
  const [highlighteddaysArray, sethighlighteddaysArray] = useState([]);
  const [localeUtil, setlocaleUtil] = useState([
    {
      getfullvalue: fromMonth,
    },
  ]);
  const { activUtvalglist, activUtvalg, utvalglistcheck, showHousehold } =
    useContext(KSPUContext);
  const { mapView } = useContext(MainPageContext);
  let dataValue = [];
  //checking the list of selected days should not in first seven days
  const CheckSevenDays = (selectablestring) => {
    let setOfFourDays = [];
    if (props.Calendar === "normalCalendar") {
      for (let i = 1; i <= 8; i++) {
        let nextday = moment()
          .add(i, "days")
          .format("ddd MMM DD yyyy")
          .toString();
        setOfFourDays.push(nextday);
      }
    } else {
      for (let i = 1; i <= 1; i++) {
        let nextday = moment()
          .add(i, "days")
          .format("ddd MMM DD yyyy")
          .toString();
        setOfFourDays.push(nextday);
      }
    }

    for (let i = 0; i < selectablestring.length; i++) {
      if (setOfFourDays.length > 0) {
        for (let j = 0; j < setOfFourDays.length; j++) {
          if (
            Date.parse(setOfFourDays[j]) === Date.parse(selectablestring[i])
          ) {
            selectablestring = selectablestring.filter(
              (item) => item !== selectablestring[i]
            );
          }
        }
      }
    }
    let selectabledays = [];
    if (selectablestring.length > 1) {
      let todayDate = new Date(selectablestring[0]);
      let nextDay = new Date(todayDate);
      nextDay.setDate(todayDate.getDate() + 1);

      let firstDay = new Date(todayDate.getFullYear(), todayDate.getMonth(), 1);

      if (
        selectablestring[1].toLocaleString() !== nextDay.toDateString() &&
        selectablestring[0].toLocaleString() !== firstDay.toDateString()
      ) {
        if (selectablestring.length % 2 !== 0) {
          for (let i = 1; i < selectablestring.length; i++) {
            selectabledays.push(selectablestring[i]);
          }
          return selectabledays;
        } else {
          return selectablestring;
        }
      } else {
        return selectablestring;
      }
    } else {
      if (selectablestring.length % 2 !== 0) {
        for (let i = 0; i < selectablestring.length; i++) {
          selectabledays.push(selectablestring[i]);
        }
        return selectabledays;
      } else {
        return selectablestring;
      }
    }
  };

  const handleYearMonthChange = async (
    month,
    test2,
    monthvalue,
    changeable
  ) => {
    setCapacityAlert(false);
    setCapacityFullyBookedAlert(false);
    setdisabelall(false);
    setrendercalendar(true);
    setchangeYear(test2);
    setmonth(month);
    let mon = monthvalue;
    let yer = test2; //current year
    let h = new Date(test2, mon, 1); //this is first day of current month
    setfirstday(moment(h).format("MM-DD-YYYY"));
    let lastday = new Date(yer, mon + 1, 0); //this is last day of current month
    setlastday(moment(lastday).format("MM-DD-YYYY"));

    setmonthSelected(new Date(month));
    setFinn(test2);
    await fetchData(month, test2);
    //}
  };
  const NumberOfDaysLeftForNextMonth = () => {
    let dateToday = new Date();
    let lastDayOfMonth = new Date(
      dateToday.getFullYear(),
      dateToday.getMonth() + 1,
      0
    ).getDate();
    let daysUntilEndOfMonth = lastDayOfMonth - dateToday.getDate();
    return daysUntilEndOfMonth;
  };
  const FindingWeekNumber = (currentdate) => {
    let weeknumber = moment(currentdate, "MM-DD-YYYY").week();

    return weeknumber;
  };
  const removeFullyBookRoute = (e) => {
    if (e.target.checked) {
      if (!utvalglistcheck) {
        let reolerArray = activUtvalg?.reoler;
        reolerArray = reolerArray.filter(function (item) {
          return bookedReolID.indexOf(item.reolId.toString()) === -1;
        });
        activUtvalg.reoler = reolerArray;
        // console.log("active Utvalg", activUtvalg, reolerArray);
      }

      props.parentCallback(fromDate, false, routeData);
    } else {
      props.parentCallback(fromDate, true, routeData);
    }
  };
  const showRoutesList = () => {
    setShowList("ViewMaximizer");
  };
  const delay = (ms) => new Promise((resolve) => setTimeout(resolve, ms));
  const showFullRouteToMap = async (Reolids, colorFlag) => {
    if (Reolids.length > 0) {
      setloading(true);
      let m = mapView.graphics.items.length;
      for (let n = m; n > 0; n--) {
        if (mapView.graphics.items[n - 1].geometry.type === "polygon") {
          if (
            Reolids.includes(mapView.graphics.items[n - 1].attributes.reol_id)
          ) {
            mapView.graphics.remove(mapView.graphics.items[n - 1]);
          }
        }
      }
      let k = Reolids.map((element) => "'" + element + "'").join(",");
      let reolsWhereClause = `reol_id in (${k})`;
      let BudruterUrl;

      // await delay(15000);

      let allLayersAndSublayers = mapView.map.allLayers.flatten(function (
        item
      ) {
        return item.layers || item.sublayers;
      });

      allLayersAndSublayers.items.forEach(function (item) {
        if (item.title === "Budruter") {
          BudruterUrl = item.url;
        }
      });

      //Get ObjectIDs
      const queryOIDs = new Query();
      queryOIDs.where = `${reolsWhereClause}`;
      let oids = await query.executeForIds(BudruterUrl, queryOIDs);

      //build query block for more than 2000 ObjectIDs
      let times = null;
      let quotient = Math.floor(oids.length / 2000);
      let remainder = oids.length % 2000;
      let startIndex = 0;
      let endIndex = 2000;
      let promise = [];

      const queryExtent = new Query();
      queryExtent.where = `${reolsWhereClause}`;
      let doubleCoveragefeaturesGeometryExtent = await query.executeForExtent(
        BudruterUrl,
        queryExtent
      );

      if (quotient < 1) {
        times = 1;
        endIndex = oids.length;
      } else {
        if (remainder === 0) {
          times = quotient;
        } else {
          times = quotient + 1;
        }
      }

      for (let i = 0; i < times; i++) {
        if (i > 0) {
          startIndex = startIndex + 2000;
          endIndex = endIndex + 2000;
        }
        if (i === times - 1) {
          endIndex = oids.length;
        }

        let objectsIds = oids.slice(startIndex, endIndex);
        // let calCulateMaxOffset = 0;
        // if (oids.length > 2000) {
        //   calCulateMaxOffset = 2000;
        // }

        const queryResults = new Query();

        queryResults.outFields = ["tot_anta", "hh", "hh_res", "reol_id"];
        queryResults.where = "OBJECTID IN (" + objectsIds.join(",") + ")";
        queryResults.outSpatialReference = mapView.spatialReference;
        //queryResults.maxAllowableOffset = calCulateMaxOffset;
        queryResults.returnGeometry = true;

        promise[i] = query.executeQueryJSON(BudruterUrl, queryResults);
      }

      Promise.all(promise).then((values) => {
        for (let i = 0; i < values.length; i++) {
          for (let j = 0; j < values[i].features.length; j++) {
            let selectedSymbol = {
              type: "simple-fill", // autocasts as new SimpleFillSymbol()
              color: colorFlag ? "#ffff00" : [237, 54, 21, 0.25],
              style: "solid",
              outline: {
                // autocasts as new SimpleLineSymbol()
                color: [237, 54, 21],
                width: 0.75,
              },
            };

            let graphic = new Graphic(
              values[i].features[j].geometry,
              selectedSymbol,
              values[i].features[j].attributes
            );
            mapView.graphics.add(graphic);
          }
        }
        setloading(false);
      });

      mapView.watch("updating", function (evt) {
        if (evt) {
          setloading(true);
        } else {
          setloading(false);
        }
      });
    }
  };
  const handleDayClick = async (day, modifiers = {}) => {
    setCapacityAlert(false);
    setCapacityFullyBookedAlert(false);
    let buttonFlag = false;
    let dataReolerItem = [];
    if (modifiers.disabled) {
      return;
    } else if (highlighteddays3.length > 0) {
      highlighteddays3.map((item) => {
        if (
          Date.parse(item.toDateString()) === Date.parse(day.toDateString())
        ) {
          buttonFlag = true;
        }
      });
    }
    if (buttonFlag) {
      let selectedDate = moment(day).format("MM-DD-YYYY");
      let queryRequest = [];
      let routeInfoAPI = `GetPrsCalendarAdminDetails/GetRuteinfo?id=${props.UtvalgID}&type=${props.type}&vekt=${props.weight}&distribusjonstype=${props.selection}&valgtDato=${selectedDate}&thickness=${props.thickness}`;
      try {
        const { data, status } = await api.postdata(routeInfoAPI, queryRequest);
        if (status === 200) {
          if (data) {
            let routeCount = 0;
            setRouteData(data);
            dataReolerItem = [];
            data.ruteInfo.forEach(function (reolItem) {
              dataReolerItem.push(reolItem.ruteId.toString());
            });
            setBookedReolId(dataReolerItem);

            if (!utvalglistcheck) {
              routeCount =
                data.totaltAntallBudruter / activUtvalg?.reoler.length;
              routeCount = routeCount * 100;

              if (routeCount <= 50) {
                setCapacityAlert(true);
                setCapacityFullyBookedAlert(false);
              } else {
                setCapacityAlert(false);
                setCapacityFullyBookedAlert(true);
              }

              showFullRouteToMap(dataReolerItem, true);
            } else {
              routeCount = data.totaltAntallMottakere / activUtvalglist?.antall;
              routeCount = routeCount * 100;

              if (routeCount <= 50) {
                setCapacityAlert(true);
                setCapacityFullyBookedAlert(false);
              } else {
                setCapacityAlert(false);
                setCapacityFullyBookedAlert(true);
              }

              // setCapacityAlert(true);
            }
          }
        }
      } catch {}
    } else {
      if (bookedReolID.length > 0 && !utvalglistcheck) {
        showFullRouteToMap(bookedReolID, false);
      }
    }
    setdefaultshow(false);

    // alert(day);

    let startDay = new Date(day);
    let startDayweeknumber = await FindingWeekNumber(startDay);

    let highlighteddays1 = highlighteddaysArray.filter((item) => {
      return item.toDateString() !== startDay.toDateString();
    });

    let ToDATE = "";
    for (let i = 0; i < highlighteddays1.length; i++) {
      let ToDATEWeekNumber = await FindingWeekNumber(highlighteddays1[i]);
      if (ToDATEWeekNumber === startDayweeknumber) {
        ToDATE = highlighteddays1[i];
        break;
      }
    }

    if (Date.parse(day) > Date.parse(ToDATE)) {
      setfromDate(ToDATE);
      setToDate(day);
    } else {
      setfromDate(day);
      setToDate(ToDATE);
    }

    setdaylogic(true);

    if (Date.parse(day) > Date.parse(ToDATE)) {
      if (ToDATE) {
        props.parentCallback(ToDATE, buttonFlag, routeData, dataReolerItem);
        setRange({ from: new Date(ToDATE), to: day });
      } else {
        props.parentCallback(day, buttonFlag, routeData, dataReolerItem);
        setRange({ from: new Date(ToDATE), to: day });
      }
    } else {
      if (day) {
        props.parentCallback(day, buttonFlag, routeData, dataReolerItem);
        setRange({ from: day, to: new Date(ToDATE) });
      } else {
        props.parentCallback(ToDATE, buttonFlag, routeData, dataReolerItem);
        setRange({ from: day, to: new Date(ToDATE) });
      }
    }
    // props.parentCallback(day);
  };
  // const getDaysInMonth = (month, year) => {
  //   let date = new Date(year, month, 1);
  //   let days = [];
  //   while (date.getMonth() === month) {
  //     days.push(new Date(date));
  //     date.setDate(date.getDate() + 1);
  //   }
  //   return days;
  // };

  const getDifference = (a, b) => {
    return a.filter((element) => {
      return !b.includes(element);
    });
  };

  const fetchData = async (monvaluedate, yer) => {
    let mon = monvaluedate.getMonth();
    let firstdayvalue = new Date(
      monvaluedate.getFullYear(),
      monvaluedate.getMonth(),
      1
    );
    let lastdayvalue = new Date(
      monvaluedate.getFullYear(),
      monvaluedate.getMonth() + 1,
      0
    );
    firstdayvalue = moment(firstdayvalue).format("MM-DD-YYYY");
    lastdayvalue = moment(lastdayvalue).format("MM-DD-YYYY");

    setloading(true);

    setchangeable(false);
    let disabledays = [];
    let selectabledays = [];
    let partialSelectabledays = [];

    let adminUrl = `GetPrsCalendarAdminDetails/GetPrsAdminData?id=${props.UtvalgID}&type=${props.type}&vekt=${props.weight}&distribusjonstype=${props.selection}&startDato=${firstdayvalue}&sluttDato=${lastdayvalue}&thickness=${props.thickness}`;

    const { data, status } = await api.getdata(adminUrl);
    if (status === 200) {
      if (data.kapasitet.length > 0) {
        dataValue = data?.kapasitet;
      }
    }

    let dataResult = [...dataValue];
    if (dataResult.length > 0) {
      dataResult.map((item) => {
        if (item.isSelectable) {
          selectabledays.push(new Date(item.dato));
          if (item?.isFullyBokked && props.Calendar === "normalCalendar") {
            if (showHousehold && props.type === "U") {
              partialSelectabledays.push(new Date(item.dato));
            } else if (props.type === "L") {
              partialSelectabledays.push(new Date(item.dato));
            }
          }
        } else {
          disabledays.push(new Date(item.dato).toString());
        }
      });
      sethighlighteddaysArray(selectabledays);
      let currentmonth = new Date().getMonth();
      // let alldisabledays = getDaysInMonth(mon, yer);
      // let date = new Date();
      let firstDay = new Date(yer, mon, 1);

      let endTime = new Date(yer, mon + 1, 0);
      let arrTime = [];

      for (let q = firstDay; q <= endTime; q.setDate(q.getDate() + 1)) {
        arrTime.push(q.toDateString());
      }

      // alldisabledays = alldisabledays.map((item) => {
      //   return item.toDateString();
      // });
      // alldisabledays = [...arrTime, ...alldisabledays];
      selectabledays = selectabledays.map((item) => {
        return item.toDateString();
      });
      if (partialSelectabledays.length > 0) {
        partialSelectabledays = partialSelectabledays.map((item) => {
          return item.toDateString();
        });
      }
      //if the current month is selected then we need to  remove the days previous to today date
      if (mon == currentmonth && yer == currentYear) {
        let removabledays = [];
        let firstDay = new Date(yer, mon, 1);

        let endTime = new Date();

        for (let q = firstDay; q <= endTime; q.setDate(q.getDate() + 1)) {
          removabledays.push(q.toDateString());
        }
        selectabledays = selectabledays.filter(
          (x) => !removabledays.includes(x)
        );
      }

      selectabledays = await CheckSevenDays(selectabledays);
      if (partialSelectabledays.length > 0) {
        partialSelectabledays = selectabledays.filter((item) =>
          partialSelectabledays.includes(item)
        );
        // partialSelectabledays = await CheckSevenDays(partialSelectabledays);
      }
      // console.log("partialSelectabledays", partialSelectabledays);
      // console.log("selectabledays", selectabledays);
      if (
        (selectabledays.length == 0 && flag) ||
        NumberOfDaysLeftForNextMonth < 4
      ) {
        setflag(false);

        setchangeable(true);
        setloading(true);
        return true;
      }

      let difference = getDifference(arrTime, selectabledays);

      let tomorrow = new Date();
      tomorrow.setDate(tomorrow.getDate() + 1);

      let day1 = tomorrow;
      let tomorrow1 = new Date();

      tomorrow1.setDate(tomorrow1.getDate() + 2);

      if (
        selectabledays.includes(day1.toDateString()) &&
        selectabledays.includes(tomorrow1.toDateString())
      ) {
        difference = difference;
      } else {
        difference.push(day1);
      }

      difference = difference.map((item) => {
        return new Date(item);
      });
      selectabledays = selectabledays.map((item) => {
        return new Date(item);
      });
      if (partialSelectabledays.length > 0) {
        partialSelectabledays = partialSelectabledays.map((item) => {
          return new Date(item);
        });
      }
      // if (props.defaultDate.toDateString() !== "") {
      //   selectabledays.push(new Date(props.defaultDate));
      // }

      setselectedDaysToDisable(difference);

      // console.log("new idea2", selectabledays);
      // console.log("new idea3", partialSelectabledays);
      if (partialSelectabledays.length > 0) {
        sethighlighteddays1(partialSelectabledays);
      } else {
        sethighlighteddays1([]);
      }

      sethighlighteddays(selectabledays);
    } else {
      setdisabelall(true);
    }

    setloading(false);
  };

  useEffect(() => {
    if (
      Date.parse(props.defaultDate) &&
      Date.parse(props.defaultDate) > Date.parse(new Date())
    ) {
      let yr = moment(props.defaultDate).format("YYYY");
      let mt = moment(props.defaultDate).format("M") - 1;
      fetchData(new Date(yr, mt), yr);
      handleYearMonthChange(new Date(yr, mt), yr, mt);
    }
    if (props.newselectedReolId.length > 0) {
      showFullRouteToMap(props.newselectedReolId, false);
    }
  }, []);

  useEffect(() => {
    setCapacityAlert(false);
    setCapacityFullyBookedAlert(false);

    if (!rendercalendar) {
      if (props.defaultDate !== "") {
        setdefaultshow(true);
      }
      setfromDate("");
      setToDate("");

      let date = new Date(),
        y = date.getFullYear(),
        mon = date.getMonth();
      let yer = date.getFullYear(); //current year
      let h = new Date(yer, mon, 1); //this is first day of current month
      setfirstday(moment(h).format("MM-DD-YYYY"));
      let lastday = new Date(yer, mon + 1, 0); //this is last day of current month

      setlastday(moment(lastday).format("MM-DD-YYYY"));
      let element = document.getElementById("test");
      if (!element) {
        const para = document.createElement("aar");
        para.id = "test";
        para.setAttribute("title", "weekday");
        para.setAttribute("className", "weekno");

        const node = document.createTextNode("Uke");
        para.appendChild(node);
        const element = document.getElementsByClassName("DayPicker-Weekday");

        element[0].appendChild(para);
      }
      if (props.page == "DTPage") {
        setfontvalue("100%");
        setcalwidth("20rem");
      } else if (props.page == "DTPage1") {
        setfontvalue("100%");
        setcalwidth("350px");
      } else if (props.page == "DTPage_2") {
        setfontvalue("100%");
        setcalwidth("70px");
      } else {
        setfontvalue("10px");
        setcalwidth("350px");
      }
      if (
        Date.parse(props.defaultDate) &&
        Date.parse(props.defaultDate) > Date.parse(new Date())
      ) {
        let yr = moment(props.defaultDate).format("YYYY");
        let mt = moment(props.defaultDate).format("M") - 1;
        fetchData(new Date(yr, mt), yr);
      } else {
        fetchData(new Date(), y);
      }
    } else {
      handleYearMonthChange(month, changeYear, month);
    }
  }, [props.selection]);

  const modifiers = {
    fromDate: fromDate,
    todate: todate,
    newHighlightedDay: highlighteddays3,
    highlightedday1: highlighteddays,
  };

  return (
    <div>
      <div className="_center">
        <Helmet>
          <style>
            {` .DayPicker-Weekday abbr[data-original-title],
                        abbr[title] {
                            text-decoration: none;
                            font-family: PostenSans-Light;
                            font-weight:normal;
                            font-size: 100%;
                            width:0px !important;
                        }
                        
                       
                       
                        .DayPicker-Day DayPicker-Day--fromDate DayPicker-Day--outside {
                          background: "white" !important;
                          
                        }
                                                
                        .DayPicker-Day DayPicker-Day--ToDate DayPicker-Day--outside {
                          background: "white" !important;
                        }
                        .DayPicker-Day--selected:not(.DayPicker-Day--start):not(.DayPicker-Day--end):not(.DayPicker-Day--outside) {
                          background-color:#7bc144 !important;
                          color:#353839;
                          font-weight :bold;
                          border-radius:0px;
                        
                         
                        }
                       
                        
                      .DayPicker-Month {
                      margin: 0 1em;
                      margin-top: 1em;
                      border-spacing: 0;
                      border-collapse: collapse;
                      -webkit-user-select: none;
                      -ms-user-select: none;
                      user-select: none;
                      width:${calwidth}
                      
                     
                    }
                   

                      }
                        .DayPicker-Caption {
                            margin-bottom: 0.5em;
                            padding: 0px;
                            padding-top: 0px;
                            padding-right: 0px;
                            padding-bottom: 0px;
                            padding-left: 0px;
                            text-align: left;
                            font-size: ${fontvalue};
                            font-family: PostenSans-Light;
                        }
    
                        .DayPicker-Week {
                            border: 1px solid #dddddd;;
                            font-size: ${fontvalue};
                            font-family: PostenSans-Light;
                        }
                        .DayPicker-Weekday {
                            background-color: #002f19;
                            color: White;
                            underlined: none;
                            font-size: 1vw !important;
                            font-weight: bold;
                            border: 1px solid #dddddd;
                            font-family: PostenSans-Light;
                            padding:9px 0px 9px 0px !important;
                            height:30px;
                            align-items:center;
                            justify-content:center;
                            flex-direction:row;
                            width:30vw;
                            font-weight:normal;
                        }
                        .DayPicker-Day--selected {
                                color:black
                                border-radius:0px; 
                                font-family: Posten Sans;
                        }
                       
                        .DayPicker-Day--newHighlightedDay{
                          background-color: yellow;
    position: relative;
    border-radius: 0px;
                          color: red !important;
                          
                        }
                        .DayPicker {
                          display: inline-block;                            
                          color: #7bc144;
                          font-size: ${fontvalue};
                          background-color : #ffffff
                      }
                        .DayPicker-Day DayPicker-Day--selected {
                         
                          color : darkgreen
                          font-weight : bold !important
                        }
                       
                       
                        .DayPicker-Day--disabled {
                          color: #353839
                         
                        }
                        .DayPicker-WeekNumber {
                            border: 1px solid #dddddd;
                            width:0rem;
                            padding:10px !important;
                            // font-size: ${fontvalue};
                            font-size: 1vw !important;
                            color: #00643a !important;
                            font-family: Posten Sans;
                            
                            background-color:#f1f7e9
                        }
                        .DayPicker-NavButton--prev {
                            display: block;
                        }
                        .DayPicker-Day {
                            border: 1px solid #dddddd;
                            font-family : Posten Sans;
                            font-size: 1vw !important;
                            padding:7px !important;
                        }

                        .DayPicker-NavButton--next {
                            display: block;
                        }
                        .DayPicker-NavBar {
                            display: none;
                        }
                        .DayPicker-Month {
                          // width :${calwidth}
                          height : 5rem
                          font-family: Posten Sans;
                          margin: 0px !important;
                        }
                        .DayPicker:not(.DayPicker--interactionDisabled) .DayPicker-Day:not(.DayPicker-Day--disabled):not(.DayPicker-Day--selected):not(.DayPicker-Day--outside):hover {
                          border-radius:0px !important;
                        }
                       
                         `}
          </style>
        </Helmet>

        {loading ? (
          <img
            src={loadingImage}
            style={{
              width: "20px",
              height: "20px",
              position: "absolute",
              left: "210px",
              zindex: 100,
            }}
          />
        ) : null}
        <DayPicker
          showWeekNumbers
          localeUtils={MomentLocaleUtils}
          locale={"nb"}
          month={month}
          disabledDays={
            !disableall
              ? ([{ before: new Date() }], selectedDaysToDisable)
              : [{ after: new Date() }]
          }
          weekdaysShort={WEEKDAYS_SHORT}
          modifiers={modifiers}
          selectedDays={defaultshow ? props.defaultDate : [fromDate, todate]}
          onDayClick={handleDayClick}
          captionElement={({ date, localeUtils }) => (
            <YearMonthForm
              date={date}
              localeUtils={localeUtil}
              onChange={handleYearMonthChange}
              Finn={Finn}
              Fromdate={fromMonth}
              changeyear={changeYear}
              Calendar={props.Calendar}
              selection={props.selection}
              changeable={changeable}
            />
          )}
        />
      </div>

      {capacityAlert ? (
        <div className="calendarError calendarWarningSign">
          <div className="Distribution_label pl-5">
            <span>
              {routeData?.totaltAntallBudruter} budruter (
              {routeData?.totaltAntallMottakere} husholdninger) er fullbooket i
              denne perioden
            </span>
          </div>
          <div className="Distribution_Text">
            <span className="mb-2">
              Disse rutene er markert med gul i kartet. Klikk på dem for mer
              informasjon eller
              <a
                id="routeListId"
                href=""
                class="KSPU_LinkButton"
                data-toggle="modal"
                data-target="#showRouteList"
                onClick={showRoutesList}
              >
                {" "}
                se oversikt over rutene.
              </a>
            </span>
            <br></br>
            <input
              type="checkbox"
              id="removeFullyBookedRoutesId"
              value=""
              className=""
              onChange={(e) => {
                removeFullyBookRoute(e);
              }}
            />

            <span>
              Jeg godtar at de fullbookede rutene fjernes fra bestillingen.
            </span>
            <br></br>
            <br></br>
            <span>
              Ønsker du ikke å utelate disse budrutene må du endre
              distribusjonsperiode.
            </span>
          </div>
        </div>
      ) : null}

      {capacityFullyBookedAlert ? (
        <div className="calendarError calendarWarningSign">
          <div className="Distribution_Text pt-3">
            <span className="ml-5">
              I den distribusjonsperioden du har valgt er det ledig kapasitet
              for under halvparten av de rutene du har i området ditt.
            </span>
            <br></br>
            <span className="">Du må finne en annen distribusjonsperiode</span>
          </div>
        </div>
      ) : null}
      {showList === "ViewMaximizer" && routeData?.ruteInfo?.length > 0 ? (
        <ModelComponent1
          title={"Oversikt over fullbookede budruter"}
          id={"showRouteList"}
          routeData={routeData}
        />
      ) : null}
    </div>
  );
}
export default Calender;
