import React, { useState, useContext, useEffect } from "react";
import { UtvalgContext } from "../context/Context.js";
import { KSPUContext } from "../context/Context.js";
import { MainPageContext } from "../context/Context.js";
import { GetData, groupBy } from "../Data";
import loadingImage from "../assets/images/callbackActivityIndicator.gif";
import {
  Utvalg,
  NewUtvalgName,
  criterias,
  getAntall,
  formatData,
  getAntallUtvalg,
} from "./KspuConfig";
import api from "../services/api.js";
import SaveUtvalg from "./SaveUtvalg";
import Query from "@arcgis/core/rest/support/Query";
import * as query from "@arcgis/core/rest/query";
import Graphic from "@arcgis/core/Graphic";

function Submit_Button({
  tabvalue,
  setnomessagediv,
  setdemografikmsg,
  setDemografikAntalMsg,
  demografikeSubmitValue,
  segmenterSubmitValue,
  setVelgvalue,
  currentStep,
  setCurrentStep,
  page,
}) {
  const { searchData, setSearchData } = useContext(UtvalgContext);
  const { isPickList } = useContext(UtvalgContext);
  const { searchURL, setSearchURL } = useContext(KSPUContext);
  const { picklistData, setPicklistData } = useContext(KSPUContext);
  const [steps, setSteps] = useState([]);
  const { activUtvalg, setActivUtvalg } = useContext(KSPUContext);
  const [loading, setloading] = useState(false);
  const {
    showBusiness,
    showHousehold,
    setvalue,
    setAktivDisplay,
    setRuteDisplay,
    setAdresDisplay,
    setDemografieDisplay,
    setSegmenterDisplay,
    setAddresslisteDisplay,
    setGeografiDisplay,
    setKjDisplay,
    setpagekeysseg,
    setpagekeys,
    globalBilTypeIW,
    setGlobalBilTypeIW,
    demoIndexArray,
    setGeograErrMsg,
  } = useContext(KSPUContext);
  const { showReservedHouseHolds, setShowReservedHouseHolds } =
    useContext(KSPUContext);

  const { selectedKoummeIDs, setselectedKoummeIDs } = useContext(KSPUContext);
  const { selectedsegment, setselectedsegment, selectedName, setSelectedName } =
    useContext(KSPUContext);
  const { selectedRowKeys, setSelectedRowKeys } = React.useContext(KSPUContext);
  const { selecteddemografiecheckbox, setselecteddemografiecheckbox } =
    useContext(KSPUContext);
  const { antallValue, setAntallValue } = useContext(KSPUContext);
  const { resultData, setResultData } = useContext(KSPUContext);

  const [segmenterParam, setSegmenterParam] = useState([]);
  const [demograficParam, setDemograficParam] = useState([]);
  const [segmentIDsExisting, setSegmentIDsExisting] = useState([]);
  const [demograficIDsExisting, setDemograficIDsExisting] = useState([]);
  const [Large, setLarge] = useState(" ");
  const { mapView } = useContext(MainPageContext);
  const { demografikagemsg, setdemografikagemsg } = useContext(KSPUContext);
  const { demografikmsg } = useContext(KSPUContext);
  const { mapattribute, setmapattribute } = useContext(KSPUContext);

  let routesData = [];
  let newobjTemp = {};

  selecteddemografiecheckbox.map((item) => {
    newobjTemp[item] = 0;
  });

  const routes = (data) => {
    data.map((item) => {
      routesData.push(item);
    });
  };
  let Antall = [];

  const getSelectedRoutes = (data) => {
    return data.reduce((acc, dt) => {
      if (!(dt.children === undefined)) {
        return acc.concat(getSelectedRoutes(dt.children));
      }
      return acc.concat(dt);
    }, []);
  };

  useEffect(async () => {
    let selectedDataSet = [];
    let ReolDataRow = [];

    if (parseInt(tabvalue) == 20) {
      ReolDataRow = groupBy(
        demograficParam,
        "",
        0,
        showHousehold,
        showBusiness,
        showReservedHouseHolds,
        []
      );
      setResultData(ReolDataRow);
    } else if (parseInt(tabvalue) == 30) {
      ReolDataRow = groupBy(
        segmenterParam,
        "",
        0,
        showHousehold,
        showBusiness,
        showReservedHouseHolds,
        []
      );
      setResultData(ReolDataRow);
    } else {
      ReolDataRow = groupBy(
        activUtvalg?.reoler,
        "",
        0,
        showHousehold,
        showBusiness,
        showReservedHouseHolds,
        []
      );
      setResultData(ReolDataRow);
    }
    //let selectedDataSet = await GetData(searchURL+searchData, '',parseInt(steps[0])-1, showBusiness,showReservedHouseHolds, picklistData);
  }, [showBusiness]);

  useEffect(async () => {
    let selectedDataSet = [];
    let ReolDataRow = [];
    if (parseInt(tabvalue) == 20) {
      ReolDataRow = groupBy(
        demograficParam,
        "",
        0,
        showHousehold,
        showBusiness,
        showReservedHouseHolds,
        []
      );
      setResultData(ReolDataRow);
    } else if (parseInt(tabvalue) == 30) {
      ReolDataRow = groupBy(
        segmenterParam,
        "",
        0,
        showHousehold,
        showBusiness,
        showReservedHouseHolds,
        []
      );
      setResultData(ReolDataRow);
    } else {
      ReolDataRow = groupBy(
        activUtvalg?.reoler,
        "",
        0,
        showHousehold,
        showBusiness,
        showReservedHouseHolds,
        []
      );
      setResultData(ReolDataRow);
    }
  }, [showReservedHouseHolds]);

  useEffect(async () => {
    let ReolDataRow = [];
    if (parseInt(tabvalue) == 20) {
      ReolDataRow = groupBy(
        demograficParam,
        "",
        0,
        showHousehold,
        showBusiness,
        showReservedHouseHolds,
        []
      );
      setResultData(ReolDataRow);
    } else if (parseInt(tabvalue) == 30) {
      ReolDataRow = groupBy(
        segmenterParam,
        "",
        0,
        showHousehold,
        showBusiness,
        showReservedHouseHolds,
        []
      );
      setResultData(ReolDataRow);
    } else {
      ReolDataRow = groupBy(
        activUtvalg?.reoler,
        "",
        0,
        showHousehold,
        showBusiness,
        showReservedHouseHolds,
        []
      );
      setResultData(ReolDataRow);
    }
  }, [showHousehold]);

  const createUtvalgObject = async (
    selectedDataSet,
    key,
    criteriaType,
    FromDemografie,
    segmenterFlag
  ) => {
    let routes = getSelectedRoutes(selectedDataSet);
    let reolerArray = [];
    routes.map((item) => {
      if (routesData.length > 0) {
        reolerArray.push(routesData.filter((x) => x.reolId == item.key)[0]);
      } else {
        reolerArray.push(routes.filter((x) => x.reolId == item.key)[0]);
      }
    });

    Antall = await getAntallUtvalg(selectedDataSet);
    var a = Utvalg();
    a.hasReservedReceivers = showReservedHouseHolds ? true : false;
    a.name = NewUtvalgName();
    a.totalAntall =
      (showHousehold ? Antall[0] : 0) +
      (showBusiness ? Antall[1] : 0) +
      (showReservedHouseHolds ? Antall[2] : 0);

    if (showHousehold) a.receivers.push({ receiverId: 1, selected: true });
    if (showBusiness) a.receivers.push({ receiverId: 4, selected: true });
    if (showReservedHouseHolds)
      a.receivers.push({ receiverId: 5, selected: true });
    a.modifications = [];
    a.reoler = segmenterFlag ? selectedDataSet : reolerArray;
    a.Business = Antall[1];
    a.ReservedHouseHolds = Antall[2];
    a.hush = Antall[0];
    a.Antall = Antall;
    if (parseInt(tabvalue) == 30) {
      let segmentCriteria = [];
      selectedsegment?.map((item) => {
        if (item === "A") {
          segmentCriteria.push("Senior Ordinær");
        } else if (item === "B") {
          segmentCriteria.push("Senior Aktiv");
        } else if (item === "C1") {
          segmentCriteria.push("Urban Ung");
        } else if (item === "C2") {
          segmentCriteria.push("Urban Moden");
        } else if (item === "D") {
          segmentCriteria.push("Ola og Kari Tradisjonell");
        } else if (item === "E") {
          segmentCriteria.push("Ola og Kari Individualist");
        } else if (item === "F") {
          segmentCriteria.push("Barnefamilie Velstand og Kultur ");
        } else if (item === "G") {
          segmentCriteria.push("Barnefamilie Barnerik");
        } else if (item === "H") {
          segmentCriteria.push("Barnefamilie Prisbevisst");
        } else {
          segmentCriteria.push("Barnefamilie Moderne Aktiv");
        }
      });
      let k = selectedKoummeIDs.map((element) => element).join(", ");
      let str = segmentCriteria?.map((element) => element).join(", ");
      let newString = str + " Kommuner: " + k;
      a.criterias.push(criterias(criteriaType, newString));
    } else if (parseInt(tabvalue) === 20) {
      let segmentCriteria = [];
      let k =
        demograficIDsExisting?.length > 0
          ? demograficIDsExisting.map((element) => element).join(", ")
          : selectedKoummeIDs.map((element) => element).join(", ");

      selecteddemografiecheckbox?.map((item) => {
        if (item === "land_rover") {
          segmentCriteria.push("Land Rover");
        } else {
          segmentCriteria.push(item);
        }
      });

      let str = segmentCriteria?.map((element) => element).join(", ");
      let newString = str + " Kommuner: " + k;
      a.criterias.push(criterias(criteriaType, newString));
    } else {
      a.criterias.push(
        criterias(
          criteriaType,
          key === ""
            ? " "
            : parseInt(tabvalue) === 9
            ? "Geografiplukkliste"
            : key + searchData.join()
        )
      );
    }

    await setActivUtvalg(a);
  };

  const DemografikePageSubmit = async () => {
    if (demografikeSubmitValue) {
      let demograficIDsExisting = [];
      activUtvalg.reoler.map((data) => {
        demograficIDsExisting.push(data.kommuneId);
        setDemograficIDsExisting(demograficIDsExisting);
      });

      let lengh = selecteddemografiecheckbox.length;
      let temp = selecteddemografiecheckbox;
      let r = [];

      for (let i = 0; i < temp.length; i++) {
        if (temp[i].includes("-")) {
          let p = temp[i].replace("-", "_");
          r.push(p);
        } else {
          r.push(temp[i]);
        }
      }

      let stringwithcomma = temp.map((element) => {
        if (globalBilTypeIW) {
          return element + ">=0" + " " + "OR" + " ";
        } else {
          return "main." + element + ">=0" + " " + "OR" + " ";
        }
      });
      let lastelement = stringwithcomma[stringwithcomma.length - 1];

      let lastelement1 = lastelement.slice(0, -4);

      stringwithcomma = stringwithcomma.slice(0, -1);

      stringwithcomma.push(lastelement1);

      let k = demograficIDsExisting
        .map((element) => "'" + element + "'")
        .join(",");
      let sql_geography = `AND (main.KOMMUNEID IN (${k}))`;
      stringwithcomma = stringwithcomma.map((element) => element);
      let sql_param_where_clause = `(${stringwithcomma})`;

      let r1 = r.map((el) => {
        if (globalBilTypeIW === true) {
          return "indeks." + el + "+";
        } else {
          return "main." + el + "+";
        }
      });
      let sqlOrderby = `ORDER BY( (${r1.join("").slice(0, -1)})/${lengh})DESC`;
      let indexFieldSelected = r.map((el) => {
        if (globalBilTypeIW === true) {
          return "indeks." + el;
        } else {
          return "main." + el;
        }
      });
      try {
        let API_PARAM = {
          options: {
            maxAntall: antallValue > 0 ? antallValue : 0,
            sQLWhereClause: sql_param_where_clause.replaceAll(",", ""),
            sqlOrderby: sqlOrderby,
            sQLWhereClauseGeography: sql_geography,
            indexFieldSelected: indexFieldSelected,
          },
          utvalg: Object.keys(activUtvalg).length === 0 ? null : activUtvalg,
        };

        // GetReolerByIndexedDemographySearch

        const { data, status } = await api.postdata(
          `Reol/GetReolerByIndexedDemographySearch`,
          API_PARAM
        );
        if (status == 200) {
          routes(data.reoler);
          setDemograficParam(data.reoler);
          let ReolDataRow = await groupBy(
            data.reoler,
            "",
            0,
            showHousehold,
            showBusiness,
            showReservedHouseHolds,
            []
          );
          setResultData(ReolDataRow);
          return ReolDataRow;
        }
      } catch (error) {
        console.error("er : " + error);
      }
    } else {
      let lengh = selecteddemografiecheckbox.length;
      let temp = selecteddemografiecheckbox;
      let r = [];

      for (let i = 0; i < temp.length; i++) {
        if (temp[i].includes("-")) {
          let p = temp[i].replace("-", "_");
          r.push(p);
        } else {
          r.push(temp[i]);
        }
      }
      let stringwithcomma = r.map((element) => {
        if (globalBilTypeIW) {
          return element + ">=0" + " " + "OR" + " ";
        } else {
          return "main." + element + ">=0" + " " + "OR" + " ";
        }
      });

      let KommuneIDsRowKeys =
        selectedKoummeIDs.length > 0 ? selectedKoummeIDs : selectedRowKeys;
      let lastelement = stringwithcomma[stringwithcomma.length - 1];

      let lastelement1 = lastelement.slice(0, -4);

      stringwithcomma = stringwithcomma.slice(0, -1);

      stringwithcomma.push(lastelement1);

      let k = KommuneIDsRowKeys.map((element) => "'" + element + "'").join(",");
      let sql_geography = `AND (main.KOMMUNEID IN (${k}))`;
      stringwithcomma = stringwithcomma.map((element) => element);
      let sql_param_where_clause = `(${stringwithcomma})`;

      let r1 = r.map((el) => {
        if (globalBilTypeIW === true) {
          return "indeks." + el + "+";
        } else {
          return "main." + el + "+";
        }
      });
      let sqlOrderby = `ORDER BY( (${r1.join("").slice(0, -1)})/${lengh})DESC`;
      let indexFieldSelected = r.map((el) => {
        if (globalBilTypeIW === true) {
          return "indeks." + el;
        } else {
          return "main." + el;
        }
      });

      let API_PARAM = {
        options: {
          maxAntall: antallValue == null || antallValue == "" ? 0 : antallValue,
          sQLWhereClause: sql_param_where_clause.replaceAll(",", ""),
          sqlOrderby: sqlOrderby,
          sQLWhereClauseGeography: sql_geography,
          indexFieldSelected: indexFieldSelected,
        },
        utvalg: Object.keys(activUtvalg).length === 0 ? null : null,
      };

      try {
        const { data, status } = await api.postdata(
          `Reol/GetReolerByIndexedDemographySearch`,
          API_PARAM
        );
        if (status == 200) {
          routes(data.reoler);

          setDemograficParam(data.reoler);
          let ReolDataRow = await groupBy(
            data.reoler,
            "",
            0,
            showHousehold,
            showBusiness,
            showReservedHouseHolds,
            []
          );
          setResultData(ReolDataRow);
          return ReolDataRow;
        }
      } catch (error) {
        console.error("er : " + error);
      }
    }
  };

  const removeItemAll = (arr, value) => {
    var i = 0;
    while (i < arr.length) {
      if (arr[i] === value) {
        arr.splice(i, 1);
      } else {
        ++i;
      }
    }
    return arr;
  };

  const segmentPageSubmit = async () => {
    if (segmenterSubmitValue) {
      let segmentIDsExisting = [];
      activUtvalg.reoler.map((data) => {
        segmentIDsExisting.push(data.kommuneId);
        setSegmentIDsExisting(segmentIDsExisting);
      });

      // let s_ID = selectedsegment.join(',')
      let stringwithcomma = selectedsegment
        .map((element) => "'" + element + "'")
        .join(",");

      let k = segmentIDsExisting
        .map((element) => "'" + element + "'")
        .join(",");

      let sql_param_where_clause = `SEGMENT IN (${stringwithcomma})`;
      let sql_geography = `(kommuneID in (${k}))`;

      let API_PARAM = {
        MaxAntall: "-1",
        SQLWhereClause: sql_param_where_clause,
        SQLWhereClauseGeography: sql_geography,
        indexFieldSelected: [],
        sqlOrderby: "",
      };

      try {
        const { data, status } = await api.postdata(
          `Reol/GetReolerBySegmenterSearch`,
          API_PARAM
        );
        if (status == 200) {
          routes(data.reoler);

          setSegmenterParam(data.reoler);
          let ReolDataRow = await groupBy(
            data.reoler,
            "",
            0,
            showHousehold,
            showBusiness,
            showReservedHouseHolds,
            []
          );

          setResultData(ReolDataRow);
          // createUtvalgObject(ReolDataRow, "Segment: ", 2);
          return ReolDataRow;
        }
      } catch (error) {
        console.error("er : " + error);
      }
    } else {
      // let s_ID = selectedsegment.join(',')
      let stringwithcomma = selectedsegment
        .map((element) => "'" + element + "'")
        .join(",");
      let k = selectedKoummeIDs.map((element) => "'" + element + "'").join(",");
      let sql_param_where_clause = `SEGMENT IN (${stringwithcomma})`;
      let sql_geography = `(kommuneID in (${k}))`;
      let API_PARAM = {
        MaxAntall: "-1",
        SQLWhereClause: sql_param_where_clause,
        SQLWhereClauseGeography: sql_geography,
        indexFieldSelected: [],
        sqlOrderby: "",
      };

      try {
        const { data, status } = await api.postdata(
          `Reol/GetReolerBySegmenterSearch`,
          API_PARAM
        );
        if (status == 200 && data !== null) {
          routes(data.reoler);
          setSegmenterParam(data.reoler);
          let ReolDataRow = await groupBy(
            data.reoler,
            "",
            0,
            showHousehold,
            showBusiness,
            showReservedHouseHolds,
            []
          );
          setResultData(ReolDataRow);
          return data.reoler;
        }
      } catch (error) {}
    }
  };

  const handlenextclick = async () => {
    let newArray = [];
    let selectedDataSet = [];
    let ReolDataRow = [];

    let j = mapView.graphics.items.length;
    for (var i = j; i > 0; i--) {
      if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
        mapView.graphics.remove(mapView.graphics.items[i - 1]);
        //j++;
      }
    }

    // Write Criteria Based on tabvalue value
    switch (parseInt(tabvalue)) {
      case 1:
        await Promise.all(
          searchData.map(async (element) => {
            ReolDataRow = await GetData(
              searchURL + element,
              "fylke",
              0,
              showHousehold,
              showBusiness,
              showReservedHouseHolds,
              picklistData,
              routes
            );

            selectedDataSet.push(ReolDataRow[0]);
          })
        );

        // selectedDataSet = await GetData(searchURL+searchData, 'fylke',0, showBusiness,showReservedHouseHolds, picklistData);
        if (isPickList) {
          setResultData(selectedDataSet);
          setVelgvalue(9);
          setCurrentStep(2);
        } else {
          createUtvalgObject(selectedDataSet, "Fylke: ", 3);
          setResultData(selectedDataSet);
          setVelgvalue(10);
          setCurrentStep(3);
        }

        newArray = [...steps, "1"];
        setSteps(newArray);
        return;
      case 2:
        if (searchData?.length > 0) {
          setGeograErrMsg(false);
          await Promise.all(
            searchData.map(async (element) => {
              ReolDataRow = await GetData(
                searchURL + element,
                "Kommune",
                1,
                showHousehold,
                showBusiness,
                showReservedHouseHolds,
                picklistData,
                routes
              );
              selectedDataSet.push(ReolDataRow[0]);
            })
          );

          if (isPickList) {
            setResultData(selectedDataSet);
            setVelgvalue(9);
            setCurrentStep(2);
          } else {
            createUtvalgObject(selectedDataSet, "Kommune : ", 4);
            setResultData(selectedDataSet);
            setVelgvalue(10);
            setCurrentStep(3);
          }
          newArray = [...steps, "2"];
          setSteps(newArray);
          return;
        } else {
          setGeograErrMsg(true);
          return false;
        }

      case 3:
        ReolDataRow = await GetData(
          searchURL,
          "Team",
          2,
          showHousehold,
          showBusiness,
          showReservedHouseHolds,
          picklistData,
          routes,
          1,
          true,
          searchData
        );
        ReolDataRow.map((item) => {
          selectedDataSet.push(item);
        });
        if (isPickList) {
          setResultData(selectedDataSet);
          setVelgvalue(9);
          setCurrentStep(2);
        } else {
          createUtvalgObject(selectedDataSet, "TeamNr : ", 5);
          setResultData(selectedDataSet);
          setVelgvalue(10);
          setCurrentStep(3);
        }
        newArray = [...steps, "3"];
        setSteps(newArray);
        return;
      case 4:
        await Promise.all(
          searchData.map(async (element) => {
            ReolDataRow = await GetData(
              searchURL + element,
              "Route",
              2,
              showHousehold,
              showBusiness,
              showReservedHouseHolds,
              picklistData,
              routes
            );

            selectedDataSet.push(ReolDataRow[0]);
          })
        );

        createUtvalgObject(selectedDataSet, "RouteId : ", 21);

        setVelgvalue(10);
        setCurrentStep(3);
        newArray = [...steps, "4"];
        setSteps(newArray);
        return;
      case 5:
        setloading(true);
        ReolDataRow = await GetData(
          searchURL + searchData,
          "Postnr",
          2,
          showHousehold,
          showBusiness,
          showReservedHouseHolds,
          picklistData,
          routes,
          2
        );
        ReolDataRow.map((item) => {
          selectedDataSet.push(item);
        });
        if (isPickList) {
          setResultData(selectedDataSet);
          setVelgvalue(9);
          setCurrentStep(2);
        } else {
          createUtvalgObject(selectedDataSet, "Postnr : ", 6);
          setResultData(selectedDataSet);
          setVelgvalue(10);
          setCurrentStep(3);
        }

        newArray = [...steps, "5"];
        setloading(false);
        setSteps(newArray);

        return;
      case 9:
        if (parseInt(steps[0]) === 3) {
          ReolDataRow = await GetData(
            searchURL,
            "Team",
            2,
            showHousehold,
            showBusiness,
            showReservedHouseHolds,
            picklistData,
            routes,
            1,
            true,
            searchData
          );
          ReolDataRow.map((item) => {
            selectedDataSet.push(item);
          });
        } else if (parseInt(steps[0]) === 5) {
          ReolDataRow = await GetData(
            searchURL + searchData,
            "Postnr",
            2,
            showHousehold,
            showBusiness,
            showReservedHouseHolds,
            picklistData,
            routes,
            2
          );
          ReolDataRow.map((item) => {
            selectedDataSet.push(item);
          });
        } else {
          await Promise.all(
            searchData.map(async (element) => {
              ReolDataRow = await GetData(
                searchURL + element,
                "Postnr",
                parseInt(steps[0]) === 5 ? 0 : parseInt(steps[0]) - 1,
                showHousehold,
                showBusiness,
                showReservedHouseHolds,
                picklistData,
                routes
              );

              ReolDataRow.map((item) => {
                selectedDataSet.push(item);
              });
            })
          );
        }

        createUtvalgObject(selectedDataSet, "Geografiplukkliste", 19);
        setResultData(selectedDataSet);
        setVelgvalue(10);
        setCurrentStep(3);
        newArray = [...steps, "9"];
        setSteps(newArray);
        return;
      case 20:
        let selectedvalue = [
          "ald19_23",
          "ald24_34",
          "ald35_44",
          "ald45_54",
          "ald55_64",
          "ald65_74",
          "ald75_84",
          "ald85_o",
        ];
        let mencheckbox = ["menn", "Kvinner"];

        const found = selecteddemografiecheckbox.some((r) =>
          selectedvalue.includes(r)
        );
        let k = selecteddemografiecheckbox;
        let arr = [];

        if (selecteddemografiecheckbox.length > 0) {
          let count = 0;
          let i = 0;
          for (i = 0; i < 8; i++) {
            if (demoIndexArray?.includes(i)) {
              count += 1;
              break;
            }
          }
          for (i = 8; i < 13; i++) {
            if (demoIndexArray?.includes(i)) {
              count += 1;
              break;
            }
          }
          for (i = 30; i < 36; i++) {
            if (demoIndexArray?.includes(i)) {
              count += 1;
              break;
            }
          }
          for (i = 46; i < 56; i++) {
            if (demoIndexArray?.includes(i)) {
              count += 1;
              break;
            }
          }
          for (i = 36; i < 46; i++) {
            if (demoIndexArray?.includes(i)) {
              count += 1;
              break;
            }
          }
          for (i = 13; i < 24; i++) {
            if (demoIndexArray?.includes(i)) {
              count += 1;
              break;
            }
          }
          for (i = 24; i < 30; i++) {
            if (demoIndexArray?.includes(i)) {
              count += 1;
              break;
            }
          }
          for (i = 56; i < 82; i++) {
            if (demoIndexArray?.includes(i)) {
              setGlobalBilTypeIW(true);
              count += 1;
              break;
            } else {
              setGlobalBilTypeIW(false);
            }
          }
          for (i = 82; i < 95; i++) {
            if (demoIndexArray?.includes(i)) {
              count += 1;
              break;
            }
          }

          if (count > 1) {
            setdemografikagemsg(true);
            return false;
          }
          if (k.includes("Menn") || k.includes("Kvinner")) {
            if (!found) {
              setdemografikagemsg(true);
              return;
            }
          }
          if (k.includes("Menn") && k.includes("Kvinner")) {
            k.map((item) => {
              if (selectedvalue.includes(item)) {
                let ItemtoAdd = item.substring(3);
                let ItemtoAddMen = "";
                let ItemtoAddWomen = "";
                ItemtoAddMen = "C" + ItemtoAdd + "_" + "MEN";
                ItemtoAddWomen = "C" + ItemtoAdd + "_" + "KVI";
                arr.push(ItemtoAddMen);
                arr.push(ItemtoAddWomen);
              } else {
                arr.push(item);
              }
            });
          } else if (k.includes("Menn")) {
            k.map((item) => {
              if (selectedvalue.includes(item)) {
                let ItemtoAdd = item.substring(3);

                ItemtoAdd = "C" + ItemtoAdd + "_" + "MEN";
                arr.push(ItemtoAdd);
              } else {
                arr.push(item);
              }
            });
          } else if (k.includes("Kvinner")) {
            k.map((item) => {
              if (selectedvalue.includes(item)) {
                let ItemtoAdd = item.substring(3);

                ItemtoAdd = "C" + ItemtoAdd + "_" + "KVI";
                arr.push(ItemtoAdd);
              } else {
                arr.push(item);
              }
            });
          } else {
            arr = selecteddemografiecheckbox;
          }

          arr = removeItemAll(arr, "Menn");
          arr = removeItemAll(arr, "Kvinner");
          setselecteddemografiecheckbox(arr);
        }

        if (selecteddemografiecheckbox.length === 0) {
          setdemografikmsg(true);
          return;
        } else if (antallValue === "" && antallValue == null) {
          setDemografikAntalMsg(true);
        }

        if (currentStep === 1) {
          setCurrentStep(2);
        } else {
          if (antallValue > 1) {
            let selectedReolData = await DemografikePageSubmit();
            // let selectedReolData1 = await DemografieMapApi();
            await createUtvalgObject(selectedReolData, "Demografi: ", 12, true);
            await setResultData(selectedReolData);
            await setmapattribute(selectedReolData);

            setCurrentStep(3);
          } else {
            let selectedReolData = await DemografikePageSubmit();
            // let selectedReolData1 = await DemografieMapApi();
            createUtvalgObject(selectedReolData, "Demografi: ", 12, true);
            setmapattribute(selectedReolData);
            setResultData(selectedReolData);
            setCurrentStep(3);
          }
        }

        newArray = [...steps, "20"];
        setSteps(newArray);
        return;

      case 30:
        if (selectedsegment.length < 1) {
          setnomessagediv(true);
          return;
        }
        if (currentStep == 1) {
          if (!segmenterSubmitValue) {
            setpagekeysseg([]);
            setCurrentStep(2);
          } else {
            let selectedReolData = await segmentPageSubmit();

            await createUtvalgObject(
              selectedReolData,
              "Segment: ",
              2,
              false,
              false
            );

            setCurrentStep(3);
          }
        } else {
          let selectedReolData = await segmentPageSubmit();

          await createUtvalgObject(
            selectedReolData,
            "Segment: ",
            2,
            false,
            true
          );

          setCurrentStep(3);
        }
        newArray = [...steps, "30"];
        setSteps(newArray);
        return;
      case 60:
        if (currentStep === 1) {
          setCurrentStep(2);
        }
        newArray = [...steps, "30"];
        setSteps(newArray);
        return;
      default:
        return;
    }
  };

  const handleprevclick = async (e) => {
    let selectedDataSet = [];
    let ReolDataRow = [];
    setPicklistData([]);
    if (isPickList) {
      setCurrentStep(parseInt(currentStep) - 1);
    } else {
      setCurrentStep(1);
    }

    let step = steps;
    let prevstep = step.pop();
    setSteps(step);
    switch (parseInt(prevstep)) {
      case 1:
      case 3:
      case 2:
      case 4:
      case 5:
        setVelgvalue(prevstep);
        // setCurrentStep(prevstep);
        setSearchData([]);
        return;
      case "9":
      case 9:
        await Promise.all(
          searchData.map(async (element) => {
            ReolDataRow = await GetData(
              searchURL + element,
              "Route",
              parseInt(steps[0]) === 5 ? 0 : parseInt(steps[0]) - 1,
              showHousehold,
              showBusiness,
              showReservedHouseHolds,
              [],
              routes
            );
            selectedDataSet.push(ReolDataRow[0]);
          })
        );
        //selectedDataSet = await GetData(searchURL+searchData, 'Route',2, showBusiness,showReservedHouseHolds, picklistData);
        setResultData(selectedDataSet);
        setVelgvalue(prevstep);
        // setCurrentStep(prevstep);

        return;
      case 20:
        if (demografikeSubmitValue) {
          if (parseInt(tabvalue) == 20) {
            if (currentStep === 2) {
              setpagekeysseg([]);
            }
            setCurrentStep(currentStep - 1);
            setActivUtvalg({});
          }
        } else if (!demografikeSubmitValue) {
          if (parseInt(tabvalue) == 20) {
            if (currentStep === 2) {
              setpagekeysseg([]);
            }
            setCurrentStep(currentStep - 1);
            setActivUtvalg({});
          }
        } else {
          setCurrentStep(currentStep - 2);
        }
        // setselectedKoummeIDs(selectedKoummeIDs);
        return;
      case 30:
        if (!segmenterSubmitValue) {
          if (parseInt(tabvalue) == 30) {
            if (currentStep === 2) {
              setSelectedName([]);
              setselectedsegment([]);
            }
            setCurrentStep(currentStep - 1);
            setActivUtvalg({});

            // setselectedsegment([]);
            // setSelectedName([]);
          }
        } else {
          setCurrentStep(currentStep - 2);
        }
        // setselectedKoummeIDs(selectedKoummeIDs);
        return;
    }
    window.scrollTo(0, 0);
  };
  const handleCancel = (e) => {
    setResultData([]);
    setPicklistData([]);
    // setCurrentStep(1);
    setSearchData([]);
    // if (parseInt(tabvalue) != 20) {
    //   setCurrentStep(1);
    // }
    // if (parseInt(tabvalue) != 30) {
    //   setCurrentStep(1);
    // }
    // window.scrollTo(0, 0);
    setActivUtvalg({});
    setselecteddemografiecheckbox([]);
    setselectedsegment([]);
    setselectedKoummeIDs([]);
    setSelectedRowKeys([]);
    setSelectedName([]);
    setAktivDisplay(false);
    setpagekeysseg([]);
    setpagekeys([]);
    // setDenknDisplay(false);
    // setMapDisplay(true);
    setRuteDisplay(false);
    setAdresDisplay(false);
    setDemografieDisplay(false);
    setSegmenterDisplay(false);
    setAddresslisteDisplay(false);
    setGeografiDisplay(false);
    setKjDisplay(false);
    setvalue(true);
  };
  const showLarge = (e) => {
    setLarge("Save_Large");
  };

  return (
    <>
      {Large == "Save_Large" ? <SaveUtvalg id={"uxBtnLagre12"} /> : null}
      <div className="row col-12 m-0 p-0 mt-2 mb-2">
        {/* <div
          className="col-2 text-right "
          style={{
            visibility: currentStep > 1 ? "visible" : "hidden",
          }}
        > */}
        <input
          type="submit"
          id="uxBtForrige"
          className="KSPU_button"
          value="<< Forrige"
          onClick={handleprevclick}
          style={{
            visibility: currentStep > 1 ? "visible" : "hidden",
            text: "",
            float: "left",
          }}
        />
        {/* </div> */}
        {/* <div className="col-2 text-right "> */}
        <input
          type="submit"
          id="uxBtnAvbryt"
          className="KSPU_button"
          value="Avbryt"
          onClick={handleCancel}
          style={{ text: "Avbryt", marginLeft: "auto" }}
        />
        {loading ? (
          <img
            src={loadingImage}
            style={{
              width: "20px",
              height: "20px",
              position: "absolute",
              left: "300px",
              zindex: 100,
            }}
          />
        ) : null}
        {tabvalue === 20 && currentStep == 2 ? (
          <input
            type="submit"
            id="uxBtnNeste"
            className="KSPU_button float-right"
            value="Neste >>"
            onClick={handlenextclick}
            disabled={antallValue > 0 && currentStep == 2 ? false : true}
            style={{
              display: currentStep < 3 ? "block" : "none",
              float: "right",
              marginLeft: "auto",
            }}
          />
        ) : (
          <input
            type="submit"
            id="uxBtnNeste"
            className="KSPU_button float-right"
            value="Neste >>"
            onClick={handlenextclick}
            style={{
              display: currentStep < 3 ? "block" : "none",
              float: "right",
              marginLeft: "auto",
            }}
          />
        )}

        <input
          type="submit"
          id="uxBtnLagre"
          className="KSPU_button float-right"
          value="Lagre"
          data-toggle="modal"
          data-target="#uxBtnLagre12"
          onClick={showLarge}
          style={{
            display: currentStep === 3 ? "block" : "none",
            text: "Lagre",
            marginLeft: "auto",
            float: "right",
          }}
        />
      </div>
    </>
  );
}
export default Submit_Button;
