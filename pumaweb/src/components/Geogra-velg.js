import React, { useEffect, useState, useRef, useContext } from "react";
import "../App.css";
import style from "../App.css";

import TableNew from "./Table_New_kw.js";
import api from "../services/api.js";
import MottakerComponent from "./Mottaker_KW";
import loadingImage from "../assets/images/callbackActivityIndicator.gif";
import { NumberFormat } from "../common/Functions";
import { MainPageContext } from "../context/Context.js";
import {
  Utvalg,
  NewUtvalgName,
  criterias_KW,
  getAntall_KW,
  formatData,
  getAntallUtvalg,
} from "./KspuConfig";
import { MapConfig } from "../config/mapconfig";
import { KundeWebContext } from "../context/Context.js";

import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";
import Extent from "@arcgis/core/geometry/Extent";

function GeograVelg({ parentCallback }) {
  const rowStyle = {
    height: "1px",
    padding: "1px",
    backgroundColor: "#F6F5EB",
  };
  const { showBusiness, setShowBusiness } = useContext(KundeWebContext);
  const { showReservedHouseHolds, setShowReservedHouseHolds } =
    useContext(KundeWebContext);
  const { utvalgapiobject, setutvalgapiobject } = useContext(KundeWebContext);
  const { mapView } = useContext(MainPageContext);
  const [datalist, setData] = useState([]);
  const [datalist_dropdown, setData_dropdown] = useState([]);
  const [datalist_dropdown_budrute, setData_dropdown_budrute] = useState([]);
  const [selectedvalue, setselectedvalue] = useState("1");
  const [outputData, setOutputData] = useState([]);
  const [initialListValue, setInitialValue] = useState(
    "Det er ingen aktive utvalg"
  );
  const { householdcheckbox, sethouseholdcheckbox } =
    useContext(KundeWebContext);
  const { businesscheckbox, setbusinesscheckbox } = useContext(KundeWebContext);
  const { HouseholdSum, setHouseholdSum } = useContext(KundeWebContext);

  const [lagutvalgenable, setlagutvalgenable] = useState(true);
  const { BusinessSum, setBusinessSum } = useContext(KundeWebContext);
  const [selected, setselected] = useState({});
  const [record, setrecord] = useState([]);
  const [selectedrecord, setselectedrecord] = useState([]);
  const [HouseholdSum_tree, setHouseholdSum_tree] = useState(0);
  const [BusinessSum_tree, setBusinessSum_tree] = useState(0);

  const { Page, setPage } = useContext(KundeWebContext);
  const { selectedrecord_s, setselectedrecord_s } = useContext(KundeWebContext);
  const [reolID, setreolID] = useState(selectedrecord_s);

  const [dropdownvalue, setdropdownvalue] = useState("");

  const [dropdownrecord, setdropdownrecord] = useState([]);
  const { Page_P, setPage_P } = useContext(KundeWebContext);
  const [loading, setloading] = React.useState([false]);

  const { selectedKoummeIDs, setselectedKoummeIDs } =
    useContext(KundeWebContext);
  const [Budruterresult, setBudruterresult] = useState([]);
  const [btndisable, setbtndisable] = useState(true);
  const [melding, setmelding] = useState(false);
  const [melding2, setmelding2] = useState(false);
  const [errormsg2, seterrormsg2] = useState("");

  const { custNos, setcustNos } = useContext(KundeWebContext);
  const { avtaleData, setavtaleData } = useContext(KundeWebContext);
  const { username_kw, setusername_kw } = useContext(KundeWebContext);
  const { selectedRowKeys, setSelectedRowKeys } = useContext(KundeWebContext);

  const { CartItems, setCartItems } = useContext(KundeWebContext);
  const { utvalglistapiobject, setutvalglistapiobject } =
    useContext(KundeWebContext);

  const [vistogglevalue, setvistogglevalue] = useState(false);
  const [SelectedDataSet, setSelectedDataSet] = useState([]);
  const [TotalValue, settotalValue] = useState(0);
  const [detailText, setDetailText] = useState("");

  const [routeText, setRouteText] = useState("");
  const { ActiveMapButton, setActiveMapButton } = useContext(MainPageContext);
  const { routeUpdateEnabled, setRouteUpdateEnabled } =
    useContext(KundeWebContext);
  const [A, setA] = useState({});
  const resultBox = useRef();
  const inputEl = useRef();

  let Antall = [];

  useEffect(async () => {
    setloading(true);
    setRouteUpdateEnabled(true);
    setActiveMapButton("");
    mapView.activeTool = null;
    setutvalgapiobject({});
    setselectedKoummeIDs([]);
    setSelectedRowKeys([]);
    setselectedrecord_s([]);

    let j = mapView.graphics.items.length;
    for (let i = j; i > 0; i--) {
      if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
        mapView.graphics.remove(mapView.graphics.items[i - 1]);
      }
    }
    setHouseholdSum(0);
    setBusinessSum(0);
    setlagutvalgenable(true);
    setbtndisable(true);
    sethouseholdcheckbox(true);

    if (selectedKoummeIDs.length > 0 || selectedrecord_s.length > 0) {
      setlagutvalgenable(true);
      setbtndisable(false);
    }

    document.getElementById("resultBox").style.display = "none";

    await fetchData();
    await fetchData_dropdown();
    resultBox.current.style.display = "none";
    setloading(false);
  }, []);

  const dropdownselection = (e) => {
    setselectedvalue(e.target.value);
    inputEl.current.value = null;
    resultBox.current.style.display = "none";
  };

  const GotoMain = () => {
    setPage("");
    setutvalgapiobject({});
    setRouteUpdateEnabled(false);
    setActiveMapButton("");
    mapView.activeTool = null;
  };

  const createUtvalgObject = async (
    selectedDataSet,
    criteriaType,
    key,
    hus,
    bus
  ) => {
    let householdsum = hus !== 0 ? hus : HouseholdSum;
    let businesssum = bus !== 0 ? bus : BusinessSum;

    Antall = getAntallUtvalg(selectedDataSet);
    var A = Utvalg();
    A.hasReservedReceivers = false;
    A.name = NewUtvalgName();
    let total_antall = Budruterresult.map((item) => item.tot_anta).reduce(
      (prev, next) => prev + next
    );
    let household_res = Budruterresult.map((item) => item.hh_res).reduce(
      (prev, next) => prev + next
    );
    let household_value = Budruterresult.map((item) => item.hh).reduce(
      (prev, next) => prev + next
    );
    let Business_value = Budruterresult.map((item) => item.vh).reduce(
      (prev, next) => prev + next
    );
    Antall[0] = householdsum;
    Antall[1] = businesssum;
    if (householdcheckbox && !businesscheckbox) {
      A.totalAntall = householdsum;
    } else if (businesscheckbox && !householdcheckbox) {
      A.totalAntall = businesssum;
    } else if (householdcheckbox && businesscheckbox) {
      A.totalAntall = householdsum + businesssum;
    } else if (householdcheckbox) {
      A.totalAntall = householdsum;
    } else if (businesscheckbox) {
      A.totalAntall = businesssum;
    }
    if (A.receivers.length !== 0) {
      A.receivers.map((item) => {
        if (householdcheckbox) {
          if (item.receiverId !== 1) {
            A.receivers.push({ receiverId: 1, selected: true });
          }
        } else {
          let temp = A;
          temp = temp.receivers.filter((result) => {
            return result.receiverId !== 1;
          });
          A.receivers = temp;
        }
      });
    } else {
      if (householdcheckbox) {
        A.receivers.push({ receiverId: 1, selected: true });
      }
    }
    if (A.receivers.length !== 0) {
      A.receivers.map((item) => {
        if (businesscheckbox) {
          if (item.receiverId !== 4) {
            A.receivers.push({ receiverId: 4, selected: true });
          }
        } else {
          let temp = A;
          temp = temp.receivers.filter((result) => {
            return result.receiverId !== 4;
          });
          A.receivers = temp;
        }
      });
    } else {
      if (businesscheckbox) {
        A.receivers.push({ receiverId: 4, selected: true });
      }
    }

    // A.receivers = [{ ReceiverId: 1, selected: true }];
    if (showBusiness) A.receivers.push({ ReceiverId: 4, selected: true });
    if (showReservedHouseHolds)
      A.receivers.push({ ReceiverId: 5, selected: true });
    A.modifications = [];
    A.reoler = selectedDataSet;
    A.Business = Antall[1];
    A.ReservedHouseHolds = Antall[2];
    A.hush = Antall[0];
    A.criterias.push(criterias_KW(criteriaType, key));
    A.Antall = Antall;
    A.ordreReferanse = "";
    A.oldReolMapName = "";
    A.kundeNavn = username_kw;
    A.kundeNummer = custNos;
    A.avtalenummer = avtaleData;
    // setutvalgapiobject({});
    await setutvalgapiobject(A);
    setloading(false);
  };

  const getSelectedRoutes = (data) => {
    return data.reduce((acc, dt) => {
      if (!(dt.children === undefined)) {
        return acc.concat(getSelectedRoutes(dt.children));
      }

      return acc.concat(dt);
    }, []);
  };
  const updateUtvalgApiObj = async (selectedItems, hus, bus) => {
    if (selectedItems.length === 0) {
      // setutvalgapiobject({});
      setutvalgapiobject({});
      setselectedKoummeIDs([]);
      setSelectedRowKeys([]);
      setselectedrecord_s([]);

      let j = mapView.graphics.items.length;
      for (let i = j; i > 0; i--) {
        if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
          mapView.graphics.remove(mapView.graphics.items[i - 1]);
        }
      }
      setHouseholdSum(0);
      setBusinessSum(0);
      setloading(false);
    } else {
      let reolers = [];
      let k = selectedItems.map((element) => "'" + element + "'").join(",");
      let reolIDMap = [];
      let sql_geography = "";
      let reols = selectedItems;

      reolIDMap = reols.map((element) => "'" + element + "'").join(",");

      if (reols.length > 0 && selectedItems.length > 0) {
        sql_geography = `reol_id in (${k}) or reol_id in (${reolIDMap})`;
      } else if (selectedItems.length > 0) {
        sql_geography = `reol_id in (${k})`;
      } else if (reols.length > 0) {
        sql_geography = `reol_id in (${reolIDMap})`;
      }
      let BudruterUrl;

      let allLayersAndSublayers = mapView.map.allLayers.flatten(function (
        item
      ) {
        return item.layers || item.sublayers;
      });

      allLayersAndSublayers.items.forEach(function async(item) {
        if (item.title === "Budruter") {
          BudruterUrl = item.url;
        }
      });
      const kommuneName = await GetAllBurdruter();
      async function GetAllBurdruter() {
        if (sql_geography === "") {
          let j = mapView.graphics.items.length;
          for (let i = j; i > 0; i--) {
            if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
              mapView.graphics.remove(mapView.graphics.items[i - 1]);
            }
          }
          mapView.goTo(mapView.initialExtent);
        } else {
          let queryObject = new Query();
          queryObject.where = `${sql_geography}`;
          queryObject.returnGeometry = true;
          queryObject.outFields = MapConfig.budruterOutField;

          await query
            .executeQueryJSON(BudruterUrl, queryObject)
            .then(function async(results) {
              results.features.forEach(function async(feature) {
                reolers.push(formatData(feature.attributes));
              });
            });
        }
      }

      await createUtvalgObject(reolers, "Geografi: ", 100, hus, bus);
    }
  };
  const LagutvalgClick = async () => {
    setmelding(false);
    if (
      selectedKoummeIDs.length === 0 &&
      reolID.length === 0 &&
      mapView.graphics.items.length === 0
    ) {
      setmelding(true);
    } else {
      setPage_P("GeograVelg");
      setRouteUpdateEnabled(false);
      setPage("LagutvalgClick");
    }
  };

  const goback = () => {
    setPage("");
    setRouteUpdateEnabled(false);
    setutvalgapiobject({});
    //disable add remove rute widget
    setActiveMapButton("");
    mapView.activeTool = null;
    //set initial extent
    mapView.extent = new Extent(MapConfig.kundewebMapExtent);
    mapView.goTo(mapView.extent);
  };

  const handleBoxClick = async (e) => {
    if (selectedvalue === "1") {
      setloading(true);
      setbtndisable(false);
      setlagutvalgenable(true);
      setdropdownvalue(e.target.value);
      let result = datalist_dropdown.filter(
        (item) => item.kommuneName === e.target.value
      );
      let kummune_ID = result[0].kommuneID;
      let recordarray = [];
      for (let i = 0; i < datalist.length; i++) {
        if (datalist[i].key == result[0].fylkeID) {
          recordarray.push(datalist[i]);
        }
      }

      let selectedDataSet = [];
      inputEl.current.value = e.target.value;
      resultBox.current.style.display = "none";
      try {
        const { data, status } = await api.getdata(
          "Reol/GetReolsInKommune?kommuneId=" + kummune_ID
        );
        if (status === 200) {
          let antall = [];
          data.map(function (item) {
            if (!selectedRowKeys.includes(item.reolId.toString())) {
              antall.push(item.antall);
            }
          });

          let reolIDS = [];
          let reolID = data.map(function (item) {
            return reolIDS.push(item.reolId.toString());
          });

          let test = datalist.filter((e) => e.name == dropdownvalue);
          setdropdownrecord(test);
          let y = getUniqueListBy(selectedrecord_s, "key");

          reolIDS = [...new Set(reolIDS)];
          let finalReols = [...new Set(selectedRowKeys.concat(reolIDS))];

          setSelectedRowKeys(finalReols);

          if (
            test.length > 0 &&
            !selectedrecord_s.filter((e) => e.key === test.key).length > 0
          ) {
            let newvalue = getUniqueListBy(selectedrecord_s, "key");
            setSelectedRowKeys(finalReols);
          }

          setreolID(finalReols);

          let houshold_sum1 = antall.reduce(
            (accumulator, current) => accumulator + current.households,
            0
          );
          setHouseholdSum_tree(houshold_sum1);

          let Business_sum1 = antall.reduce(
            (accumulator, current) => accumulator + current.businesses,
            0
          );
          setBusinessSum_tree(Business_sum1);

          if (finalReols.length > 0) {
            await OppdaterClick(selectedRowKeys, finalReols);
            await updateUtvalgApiObj(
              finalReols,
              houshold_sum1 + HouseholdSum,
              Business_sum1 + BusinessSum
            );
            setHouseholdSum(houshold_sum1 + HouseholdSum);
            setBusinessSum(Business_sum1 + BusinessSum);
          } else {
            await OppdaterClick(selectedRowKeys, []);

            setHouseholdSum(HouseholdSum);
            setBusinessSum(BusinessSum);
          }
        } else {
          console.error("error : " + status);
        }
      } catch (error) {
        console.error("error : " + error);
      }
    } else if (selectedvalue === "2") {
      setloading(true);
      setbtndisable(false);
      setlagutvalgenable(true);
      setdropdownvalue(e.target.value);
      let result = datalist_dropdown_budrute.filter(
        (item) => item.name.toLowerCase() === e.target.value.toLowerCase()
      );
      let Reol_IDs = result[0].reolId;
      let recordarray = [];
      for (let i = 0; i < datalist.length; i++) {
        if (datalist[i].key == result[0].fylkeID) {
          recordarray.push(datalist[i]);
          setrecord(recordarray);
        }
      }

      let selectedDataSet = [];
      inputEl.current.value = e.target.value;
      resultBox.current.style.display = "none";
      try {
        const { data, status } = await api.getdata(
          "Reol/GetReolsFromReolIDString?ids=" + Reol_IDs
        );
        if (status === 200) {
          let antall = [];
          data.map(function (item) {
            if (!selectedRowKeys.includes(item.reolId.toString())) {
              antall.push(item.antall);
            }
          });
          let reolIDS = [];
          let reolID = data.map(function (item) {
            return reolIDS.push(item.reolId.toString());
          });

          reolIDS = [...new Set(reolIDS)];
          let finalReols = [...new Set(selectedRowKeys.concat(reolIDS))];

          let test = datalist.filter((e) => e.name == dropdownvalue);
          setdropdownrecord(test);

          let y = getUniqueListBy(selectedrecord_s, "key");

          setSelectedRowKeys(finalReols);

          if (
            test.length > 0 &&
            !selectedrecord_s.filter((e) => e.key === test.key).length > 0
          ) {
            let newvalue = getUniqueListBy(selectedrecord_s, "key");
            setSelectedRowKeys(finalReols);
          }

          setreolID(finalReols);
          let houshold_sum1 = antall.reduce(
            (accumulator, current) => accumulator + current.households,
            0
          );
          setHouseholdSum_tree(houshold_sum1);

          let Business_sum1 = antall.reduce(
            (accumulator, current) => accumulator + current.businesses,
            0
          );
          setBusinessSum_tree(Business_sum1);
          if (finalReols.length > 0) {
            await OppdaterClick(selectedRowKeys, finalReols);
            await updateUtvalgApiObj(
              finalReols,
              houshold_sum1 + HouseholdSum,
              Business_sum1 + BusinessSum
            );
            setHouseholdSum(houshold_sum1 + HouseholdSum);
            setBusinessSum(Business_sum1 + BusinessSum);
          } else {
            await OppdaterClick(selectedRowKeys, []);

            setHouseholdSum(HouseholdSum);
            setBusinessSum(BusinessSum);
          }
        } else {
          console.error("error : " + status);
        }
      } catch (error) {
        console.error("error : " + error);
      }
    }
  };

  const getUniqueListBy = (arr, key) => {
    return [...new Map(arr.map((item) => [item[key], item])).values()];
  };

  const format = (data) => {
    return data.reduce((acc, dt) => {
      if (!(dt.children === undefined)) {
        dt.children = format(dt.children);
      }
      return acc.concat(dt);
    }, []);
  };
  const sum = (arr) => {
    return arr.reduce(add, 0);
  };
  const add = (accumulator, a) => {
    return accumulator + a;
  };
  const callback = async (
    selectedrecord,
    SelectedKommunekeys,
    checkedRows,
    recordObject
  ) => {
    setloading(true);
    setActiveMapButton("");
    mapView.activeTool = null;

    if (SelectedKommunekeys?.length === 0) {
      setutvalgapiobject({});
      setselectedKoummeIDs([]);
      setSelectedRowKeys([]);
      setselectedrecord_s([]);

      let j = mapView.graphics.items.length;
      for (let i = j; i > 0; i--) {
        if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
          mapView.graphics.remove(mapView.graphics.items[i - 1]);
        }
      }
      setHouseholdSum(0);
      setBusinessSum(0);
    }
    setutvalgapiobject({});

    if (record.length > 0 && recordObject.name == record[0].name) {
      setreolID([]);
    }
    if (
      record.length > 0 &&
      recordObject.name == record[0].name &&
      SelectedKommunekeys.length == 0
    ) {
      setlagutvalgenable(true);
      setbtndisable(true);
    } else {
      setlagutvalgenable(true);
      setbtndisable(false);
    }

    let result2 = [];
    let resultBus = [];
    let result = [];
    let s = format(checkedRows);
    let selectRoutes = true;
    s.map((item) => {
      if (item.children) {
        selectRoutes = false;
        item.children.map((value) => {
          if (value.children) {
            let h1 = value.children.reduce(
              (accumulator, current) => accumulator + current.House,
              0
            );
          } else {
            result2.push(value.House);
            resultBus.push(value.Business);
          }
        });
      } else {
        result.push(item.House);
        if (selectRoutes) resultBus.push(item.Business);
      }
    });
    setHouseholdSum(sum(result));
    setBusinessSum(sum(resultBus));

    setselectedKoummeIDs([...SelectedKommunekeys]);
    setSelectedRowKeys([...SelectedKommunekeys]);
    setselectedrecord_s([]);
    await OppdaterClick(SelectedKommunekeys, []);
    await updateUtvalgApiObj(SelectedKommunekeys, sum(result), sum(resultBus));

    let g = getUniqueListBy(selectedrecord_s, "key");
    setreolID([]);
  };

  useEffect(() => {
    if (routeText.length > 0) {
      const getData = setTimeout(async () => {
        let url = "Reol/SearchReolByReolName?reolName=" + routeText;
        let resultSelect = document.getElementById("resultBox");
        try {
          const { data, status } = await api.getdata(url);
          if (status === 200) {
            resultSelect.options.length = 0;
            resultSelect.style.display = "block";
            setData_dropdown_budrute(data);
            for (let i = 0; i < data.length; i++) {
              let txt = data[i].name.toUpperCase();

              let newOption = new Option(txt, txt);
              resultSelect.add(newOption, resultSelect.length);
            }
          } else {
            console.error("error : " + status);
          }
        } catch (error) {
          console.error("er : " + error);
        }
      }, 600);
      return () => clearTimeout(getData);
    }
  }, [routeText]);

  const handlenextchange = async (e) => {
    if (selectedvalue === "1") {
      inputEl.current.value = e.target.value;
      let resultSelect = document.getElementById("resultBox");
      resultSelect.options.length = 0;
      resultSelect.style.display = "block";
      if (e.target.value == "") {
        if (resultSelect.options.length > 0)
          resultSelect.options.splice(resultSelect.options.length - 1, 0);
        return;
      }
      for (let i = 0; i < datalist_dropdown.length; i++) {
        let txt = datalist_dropdown[i].kommuneName.toUpperCase();

        if (txt.toLowerCase().startsWith(e.target.value.toLowerCase())) {
          let newOption = new Option(txt, txt);
          resultSelect.add(newOption, resultSelect.length);
        }
      }
    } else if (selectedvalue === "2") {
      setRouteText(e.target.value);
      //check th useEffect above. Used instead of debounce
    }
  };

  useEffect(() => {
    if (outputData.length > 0) {
      let sum = 0;
      let sum0 = 0;
      let sum1 = 0;
      let sum2 = 0;
      outputData.map((item, i) => {
        if (!item.children) {
          sum = sum + parseInt(item.Total);
          sum0 = sum0 + parseInt(item["Zone 0"]);
          sum1 = sum1 + parseInt(item["Zone 1"]);
          sum2 = sum2 + parseInt(item["Zone 2"]);
        }
      });
      settotalValue(sum);
      setDetailText(
        `Hush.: ${sum},  Sone 0: ${sum0},  Sone 1: ${sum1},  Sone 2: ${sum2}`
      );
      setInitialValue("Påbegynt utvalg");
      setvistogglevalue(true);
    } else {
      settotalValue(0);
      setDetailText(`Hush.: ${0},  Sone 0: ${0},  Sone 1: ${0},  Sone 2: ${0}`);
      setvistogglevalue(false);
    }
  }, [outputData]);

  const fetchData_dropdown = async () => {
    try {
      setloading(true);
      const { data, status } = await api.getdata("Kommune/GetAllKommunes");
      if (status === 200) {
        setloading(false);
        setData_dropdown(data);
      } else {
        setloading(false);
        console.error("error : " + status);
      }
    } catch (error) {
      setloading(false);
      // setloading(true);
      setmelding2(true);
      seterrormsg2("noe gikk galt. vennligst prøv etter en stund");
      console.error("er : " + error);
    }
  };

  const fetchData = async () => {
    try {
      setloading(true);
      const { data, status } = await api.getdata("Reol/GetAllReolJSON");
      if (status === 200) {
        setData(data);
        setloading(false);
      } else {
        setloading(false);

        console.error("error : " + status);
      }
    } catch (error) {
      setloading(false);
      setmelding2(true);
      seterrormsg2("noe gikk galt. vennligst prøv etter en stund");

      console.error("er : " + error);
    }
  };

  const columns = [
    {
      title: "Fylke\\Kommune\\Team\\Rute",
      dataIndex: "name",
      key: "key",
      sorter: (a, b) => {
        return;
      },
      sortOrder: "ascend",
      sortDirections: ["ASC", "DESC"],
    },
    {
      title: "Antall",
      dataIndex: "Total",
      key: "Total",
      align: "right",
      render: (Total) => NumberFormat(Total),
    },
  ];

  const OppdaterClick = async (selectedKommunes, reolIDs) => {
    // setlagutvalgenable(true);
    if (selectedKommunes.length === 0 && reolIDs.length === 0) {
      let j = mapView.graphics.items.length;
      for (let i = j; i > 0; i--) {
        if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
          mapView.graphics.remove(mapView.graphics.items[i - 1]);
        }
      }
    } else {
      let k = selectedKommunes.map((element) => "'" + element + "'").join(",");
      let reolIDMap = [];
      let sql_geography = "";
      reolIDMap = reolIDs.map((element) => "'" + element + "'").join(",");

      if (reolIDs.length > 0 && selectedKommunes.length > 0) {
        sql_geography = `reol_id in (${k}) or reol_id in (${reolIDMap})`;
      } else if (selectedKommunes.length > 0) {
        sql_geography = `reol_id in (${k})`;
      } else if (reolIDs.length > 0) {
        sql_geography = `reol_id in (${reolIDMap})`;
      }

      let BudruterUrl;

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
      const kommuneName = await GetAllBurdruter();
      async function GetAllBurdruter() {
        if (sql_geography === "") {
          mapView.removeAll();
          mapView.goTo(mapView.initialExtent);
        } else {
          let queryObject = new Query();
          queryObject.where = `${sql_geography}`;
          queryObject.returnGeometry = true;
          queryObject.outFields = ["tot_anta", "hh", "hh_res", "reol_id"];

          await query
            .executeQueryJSON(BudruterUrl, queryObject)
            .then(function (results) {
              if (results.features.length > 0) {
                let featuresGeometry = [];
                let selectedSymbol = {
                  type: "simple-fill", // autocasts as new SimpleFillSymbol()
                  color: [237, 54, 21, 0.25],
                  style: "solid",
                  outline: {
                    // autocasts as new SimpleLineSymbol()
                    color: [237, 54, 21],
                    width: 0.75,
                  },
                };

                let j = mapView.graphics.items.length;
                for (let i = j; i > 0; i--) {
                  if (
                    mapView.graphics.items[i - 1].geometry.type === "polygon"
                  ) {
                    mapView.graphics.remove(mapView.graphics.items[i - 1]);
                  }
                }
                results.features.map((item) => {
                  featuresGeometry.push(item.geometry);
                  let graphic = new Graphic(
                    item.geometry,
                    selectedSymbol,
                    item.attributes
                  );
                  mapView.graphics.add(graphic);
                });

                mapView.goTo(featuresGeometry);
              }
              results.features.forEach(function (feature) {
                Budruterresult.push(feature.attributes);
              });
            });
          setBudruterresult(Budruterresult);
        }
      }
    }
  };

  return (
    <div className={loading ? "col-5 pt-2 pt-2 blur" : "col-5 pt-2 pt-2 "}>
      {utvalglistapiobject.memberUtvalgs?.length && (
        <>
          <div className="padding_NoColor_B" style={{ cursor: "pointer" }}>
            <a
              id="uxHandlekurvSmall_uxLnkbtnHandlekurv"
              onClick={() => {
                if (CartItems.length > 0) {
                  setPage("cartClick_Component_kw");
                }
              }}
            >
              <div className="handlekurv handlekurvText pl-2">
                Du har{" "}
                {CartItems.length > 0
                  ? CartItems.length
                  : utvalglistapiobject.memberUtvalgs?.length}{" "}
                utvalg i bestillingen din.
              </div>
            </a>
          </div>
          <br />
        </>
      )}

      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <span className="title">Velg geografisk område</span>
      </div>
      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        <div
          id="uxGeografiAnalyse_uxHeader_lblDesc"
          className="lblAnalysisHeaderDesc"
        >
          {" "}
          Velg fylker, bruk plusstegnet for å komme til kommuner eller budruter.{" "}
          <p></p>
          Du kan også søke etter kommune eller budrute (nederst).<p></p>
          Noen ruter krysser kommunegrenser. Ruten blir da tilhørende i den
          kommunen det er den har flest postkasser.
          <p></p>
          Presis kommunedistribusjon er mulig om du kombinerer uadressert
          utsendelse med adresserte sendinger. Ta kontakt med kundeservice på
          04045 om du ønsker mer informasjon.
          <br />
          <p></p>
        </div>
      </div>
      {melding2 ? (
        <span className=" sok-Alert-text pl-1">{errormsg2}</span>
      ) : null}
      {melding2 ? <p></p> : null}
      {melding ? (
        <div className="pr-3">
          <div className="error WarningSign">
            <div className="divErrorHeading">Melding:</div>
            <div id="uxKjoreAnalyse_uxLblMessage" className="divErrorText_kw">
              merk av i en av avmerkingsboksene
            </div>
          </div>
          <br />
        </div>
      ) : null}

      <img
        src={loadingImage}
        style={{
          width: "20px",
          height: "20px",
          display: loading ? "block" : "none",
          position: "absolute",
          top: "170px",
          left: "250px",
          zindex: 100,
        }}
      />

      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0  scrolltablenew">
        {reolID.length > 0 ? (
          <TableNew
            columnsArray={columns}
            page={"Geogra"}
            defaultSelectedColumn={reolID}
            record_array={record}
            parentCallback={callback}
            data={datalist}
            setoutputDataList={setOutputData}
          />
        ) : (
          <TableNew
            columnsArray={columns}
            page={"Geogra"}
            defaultSelectedColumn={reolID}
            parentCallback={callback}
            data={datalist}
            setoutputDataList={setOutputData}
          />
        )}
      </div>
      <br />
      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 Kj-background-color-kw">
        <div className="col-lg-2 col-md-3 col-sm-3 col-xs-3 m-0 p-0">
          <span className="p-text pl-2">Søk etter</span>
        </div>
        <div className="col-lg-1 col-md-1 col-sm-1 col-xs-1 m-0 p-0 text-center">
          <span className={style.divErrorText}>:</span>
        </div>
        <div className="col-lg-3 col-md-4 col-sm-4 col-xs-4 m-0 p-0">
          <select
            id="uxDropDownListUtvalg"
            style={{
              height: "1.5rem",
            }}
            className=" p-text"
            // onClick={dropdownselection}
            onChange={dropdownselection}
            title="Begrens søket med"
          >
            <option value="1">Kommune</option>
            <option value="2">budrute</option>
          </select>
        </div>
        <div className="col-lg-6 col-md-4 col-sm-4 col-xs-4 m-0 p-0 pr-1">
          <input
            ref={inputEl}
            onChange={handlenextchange}
            className="KommunInputText-kw p-2 "
            type="text"
          />
        </div>
      </div>
      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 Kj-background-color-kw">
        <div className="col-lg-6 col-md-8 col-sm-8 col-xs-8 m-0 p-0"></div>
        <div className="col-lg-6 col-md-4 col-sm-4 col-xs-4 m-0 p-0">
          <select
            ref={resultBox}
            id="resultBox"
            size="5"
            onClick={handleBoxClick}
            className="float-right pr-1 KommunListbox_KW buttonHidden mt-1"
            multiple
          ></select>
        </div>
      </div>
      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 Kj-background-color-kw">
        &nbsp;{" "}
      </div>
      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 Kj-background-color-kw">
        <MottakerComponent
          householdvalue={HouseholdSum}
          Businessvalue={BusinessSum}
        />
      </div>
      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 text-right Kj-background-color-kw pt-2 pb-2"></div>
      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 pt-2 pb-2">
        <div className="col-lg-6 col-md-6 col-sm-6 col-xs-6 m-0 p-0">
          <input
            type="button"
            value="Tilbake"
            onClick={goback}
            className="KSPU_button_Gray"
          />
        </div>
        <div className="col-lg-6 col-md-6 col-sm-6 col-xs-6 m-0 p-0 text-right">
          <input
            type="button"
            value="Lag utvalg"
            onClick={LagutvalgClick}
            disabled={
              (selectedKoummeIDs.length === 0 &&
                reolID.length === 0 &&
                mapView.graphics.items.length === 0) ||
              utvalgapiobject?.reoler === undefined
            }
            className={
              (selectedKoummeIDs.length === 0 &&
                reolID.length === 0 &&
                mapView.graphics.items.length === 0) ||
              utvalgapiobject?.reoler === undefined
                ? "KSPU_button_Gray float-right"
                : "KSPU_button-kw float-right"
            }
          />
        </div>
      </div>

      <div className="row col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0 pt-2">
        <a className="KSPU_LinkButton_Url_KW pl-2" onClick={GotoMain}>
          Avbryt
        </a>
      </div>
      <br />
    </div>
  );
}

export default GeograVelg;
