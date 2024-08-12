import React, { useState, useContext, useEffect } from "react";
import { KundeWebContext, KSPUContext } from "../context/Context.js";
import TableNew from "./Table_New_kw.js";
import api from "../services/api.js";
import { MainPageContext, UtvalgContext } from "../context/Context.js";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";

import Kommun from "./Kommun.js";
import Graphic from "@arcgis/core/Graphic";
import loadingImage from "../assets/images/callbackActivityIndicator.gif";
import { GetData, groupBy } from "../Data";

import { MapConfig } from "../config/mapconfig";

import {
  Utvalg,
  NewUtvalgName,
  criterias_KW,
  getAntall,
  formatData,
  getAntallUtvalg,
} from "./KspuConfig";

function Demogra_Velg_Click() {
  const [searchData, setSearchData] = useState([]);
  const { showBusiness, setShowBusiness } = useContext(KundeWebContext);
  const { showReservedHouseHolds, setShowReservedHouseHolds } =
    useContext(KundeWebContext);
  const { Page, setPage } = useContext(KundeWebContext);
  const { mapView } = useContext(MainPageContext);
  const [datalist, setData] = useState([]);
  const [outputData, setOutputData] = useState([]);
  const [selectedhush, setselectedhush] = useState(0);
  const [selectedrecord, setselectedrecord] = useState([]);
  const { HouseholdSum_Demo, setHouseholdSum_Demo } =
    useContext(KundeWebContext);
  const { BusinessSum, setBusinessSum } = useContext(KundeWebContext);
  const { selecteddemografiecheckbox, setselecteddemografiecheckbox } =
    useContext(KundeWebContext);
  const [nomessagediv, setnomessagediv] = useState(false);
  const { selectedKoummeIDs, setselectedKoummeIDs } =
    useContext(KundeWebContext);
  const [reolID, setreolID] = useState([]);
  const [Kommuneresult, setKommuneresult] = useState([]);
  const [Fylkeresult, setFylkeresult] = useState([]);
  const [Budruterresult, setBudruterresult] = useState([]);
  const [loading, setloading] = useState(false);
  const { utvalgapiobject, setutvalgapiobject } = useContext(KundeWebContext);
  const { Demograresultarray, setDemograresultarray } =
    useContext(KundeWebContext);
  const { Antallvalue, setAntallvalue } = useContext(KundeWebContext);
  const [demograficParam, setDemograficParam] = useState([]);
  const { selecteddemografiecheckbox_c, setselecteddemografiecheckbox_c } =
    useContext(KundeWebContext);
  const { resultData, setResultData } = useContext(KundeWebContext);
  const [melding2, setmelding2] = useState(false);
  const [errormsg2, seterrormsg2] = useState("");
  const { custNos, setcustNos } = useContext(KundeWebContext);
  const { avtaleData, setavtaleData } = useContext(KundeWebContext);
  const { username_kw, setusername_kw } = useContext(KundeWebContext);
  const { selectedRowKeys, setSelectedRowKeys } = useContext(KundeWebContext);
  const { ActiveUtvalgObject, setActiveUtvalgObject } =
    useContext(KundeWebContext);

  const { ResultOutputData, setResultOutputData } =
    React.useContext(KundeWebContext);
  const { householdcheckbox, sethouseholdcheckbox } =
    useContext(KundeWebContext);
  const { businesscheckbox, setbusinesscheckbox } = useContext(KundeWebContext);
  const { globalBilType, setGlobalBilType } = useContext(KundeWebContext);
  const { criteriaObject, setCriteriaObject } = useContext(KundeWebContext);

  let routesData = [];
  let currentReoler = [];
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
  const createUtvalgObject = (
    selectedDataSet,
    criteriaType,
    key,
    totallantallvalue
  ) => {
    let routes = getSelectedRoutes(selectedDataSet);
    Antall = getAntall(selectedDataSet);
    Antall[0] = totallantallvalue;
    var a = Utvalg();
    a.hasReservedReceivers = showReservedHouseHolds ? true : false;
    a.name = NewUtvalgName();
    a.totalAntall = totallantallvalue;
    a.receivers = [{ ReceiverId: 1, selected: true }];
    if (showBusiness) a.receivers.push({ ReceiverId: 4, selected: true });
    if (showReservedHouseHolds)
      a.receivers.push({ ReceiverId: 5, selected: true });
    a.modifications = [];
    a.reoler = routesData;
    a.Business = Antall[1];
    a.ReservedHouseHolds = Antall[2];
    a.hush = resultData.totalAntall;
    a.criterias.push(criterias_KW(criteriaType, key));
    a.Antall = Antall;
    a.ordreReferanse = "";
    a.oldReolMapName = "";
    a.kundeNavn = username_kw;
    a.kundeNummer = custNos;
    a.avtalenummer = avtaleData;
    setutvalgapiobject({});
    // setutvalgapiobject(a);
    setActiveUtvalgObject(a);
  };

  function extractValue(arr, prop) {
    // extract value from property
    let extractedValue = arr.map((item) => item[prop]);

    return extractedValue;
  }

  const GotoMain = () => {
    setPage("");
  };

  const goback = () => {
    let j = mapView.graphics.items.length;
    for (let i = j; i > 0; i--) {
      if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
        mapView.graphics.remove(mapView.graphics.items[i - 1]);
      }
    }

    setPage("Demografivelg");
  };
  const VelgAntallClick = async () => {
    if (ResultOutputData.length == 0) {
      setnomessagediv(true);
    } else {
      setloading(true);
      let lengh = criteriaObject?.demograCheckedItems?.length;
      let temp = criteriaObject?.demograCheckedItems;
      let r = [];

      for (let i = 0; i < temp.length; i++) {
        if (temp[i].includes("-")) {
          let p = temp[i].replace("-", "_");
          r.push(p);
        } else {
          r.push(temp[i]);
        }
      }
      let stringwithcomma = r?.map((element) => {
        return element + ">=0" + " " + "OR" + " ";
      });
      let lastelement = stringwithcomma[stringwithcomma.length - 1];
      let lastelement1 = lastelement.slice(0, -4);
      stringwithcomma = stringwithcomma.slice(0, -1);
      stringwithcomma.push(lastelement1);

      let k = selectedKoummeIDs.map((element) => "'" + element + "'").join(",");
      let sql_geography = `kommuneID in (${k})`;
      let sql_geography1 = `AND (main.KOMMUNEID IN (${k}))`;
      stringwithcomma = stringwithcomma.map((element) => element);

      let sql_param_where_clause = `(${stringwithcomma})`;

      let r1 = r.map((el) => {
        if (globalBilType === true) {
          return "indeks." + el + "+";
        } else {
          return "main." + el + "+";
        }
      });

      let sqlOrderby = `ORDER BY( (${r1.join("").slice(0, -1)})/${lengh})DESC`;
      let indexFieldSelected = r.map((el) => {
        if (globalBilType === true) {
          return "indeks." + el + "+";
        } else {
          return "main." + el + "+";
        }
      });

      let API_PARAM = {
        options: {
          MaxAntall: 9999999,
          SQLWhereClause: sql_param_where_clause.replaceAll(",", ""),
          SQLWhereClauseGeography: sql_geography1,
          sqlOrderby: sqlOrderby,
          indexFieldSelected: indexFieldSelected,
          sqlOrderby: sqlOrderby,
        },
        utvalg: null,
      };
      try {
        const { data, status } = await api.postdata(
          `Reol/GetReolerByDemographySearch?isFromKundeWeb=${true}`,
          API_PARAM
        );
        if (status == 200) {
          routes(data.reoler);
          let selectedDataSet = [];
          // setDemograficParam(data.reoler);
          let ReolDataRow = groupBy(
            data.reoler,
            "",
            0,
            householdcheckbox,
            businesscheckbox,
            showReservedHouseHolds,
            []
          );
          selectedDataSet.push(ReolDataRow[0]);
          await setResultData(data);
          createUtvalgObject(
            selectedDataSet,
            "Demografi",
            20,
            data.totalAntall
          );

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
          let k = data.reoler
            .map((element) => "'" + element.reolId + "'")
            .join(",");
          let sql_geographyInMap = `reol_id in (${k})`;
          const kommuneName = await GetAllBurdruter();
          async function GetAllBurdruter() {
            let queryObject = new Query();

            if (globalBilType === true) {
              queryObject.where = `${sql_geographyInMap}`;
            } else {
              queryObject.where = `${sql_geographyInMap} AND ${sql_param_where_clause.replaceAll(
                ",",
                ""
              )}`;
            }

            queryObject.returnGeometry = true;
            queryObject.outFields = ["*"];

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
                  currentReoler.push(formatData(feature.attributes));
                });
              });
            //createUtvalgObject(currentReoler, "Demografi", 20);

            setBudruterresult(Budruterresult);
          }
        }
        // console.log(Budruterresult, "kommube result==total antall");
        // let toal_antall = Budruterresult.map((item) => item.tot_anta).reduce(
        //   (prev, next) => prev + next
        // );
        // let houshold_sum1 = Budruterresult.map((item) => item.hh).reduce(
        //   (prev, next) => prev + next
        // );
        // let household_res = Budruterresult.map((item) => item.hh_res).reduce(
        //   (prev, next) => prev + next
        // );
        // setHouseholdSum_Demo(houshold_sum1);
        // utvalgapiobject.totalAntall = houshold_sum1;
        // setAntallvalue(parseInt(houshold_sum1) + parseInt(household_res));

        // let lengh = selecteddemografiecheckbox.length;
        // let temp = selecteddemografiecheckbox;
        // let r = [];

        // for (let i = 0; i < temp.length; i++) {
        //   if (temp[i].includes("-")) {
        //     let p = temp[i].replace("-", "_");
        //     r.push(p);
        //   } else {
        //     r.push(temp[i]);
        //   }
        // }
        // let stringwithcomma = r.map((element) => {
        //   return "main." + element + ">=0" + " " + "OR" + " ";
        // });
        // lastelement = stringwithcomma[stringwithcomma.length - 1];
        // let lastelement1 = lastelement.slice(0, -4);
        // stringwithcomma = stringwithcomma.slice(0, -1);
        // stringwithcomma.push(lastelement1);
        // let k = selectedKoummeIDs.map((element) => "'" + element + "'").join(",");
        // let sql_geography = `AND (main.kommuneID in (${k}))`;
        // stringwithcomma = stringwithcomma.map((element) => element);
        // let sql_param_where_clause = ` (${stringwithcomma})`;

        // lastelement = stringwithcomma[stringwithcomma.length - 1];
        // console.log(lastelement.slice(0, -5), "firstlastelemetn");
        // lastelement1 = lastelement.slice(0, -4);
        // console.log(lastelement1, "lastelement===>>>");
        // stringwithcomma = stringwithcomma.slice(0, -1);
        // console.log(stringwithcomma, "firststringwithcomma");
        // stringwithcomma.push(lastelement1);
        // console.log(stringwithcomma, "string with comma===>>>");
        // k = selectedKoummeIDs.map((element) => "'" + element + "'").join(",");
        // sql_geography = `AND (main.KOMMUNEID IN (${k}))`;
        // stringwithcomma = stringwithcomma.map((element) => element);
        // sql_param_where_clause = `(${stringwithcomma})`;
        // console.log(sql_param_where_clause.replaceAll(",", ""), "woww");
        // let r1 = r.map((el) => {
        //   return "main." + el + "+";
        // });
        setPage("Demogra_velg_antall_click");
        setloading(true);
      } catch (error) {
        setloading(false);
        setmelding2(true);
        seterrormsg2(
          "De valgte verdiene finnes ikke i systemet. vennligst prøv med forskjellige verdier"
        );
        console.error("er : " + error);
      }
    }
  };

  useEffect(async () => {
    setselectedKoummeIDs([]);
    setSelectedRowKeys([]);

    let j = mapView.graphics.items.length;
    for (var i = j; i > 0; i--) {
      if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
        mapView.graphics.remove(mapView.graphics.items[i - 1]);
      }
    }
    setloading(true);
    fetchData();
    setloading(false);
    let kommuneUrl;
    let fylkeUrl;

    let Finalresult = [];

    let allLayersAndSublayers = mapView.map.allLayers.flatten(function (item) {
      return item.layers || item.sublayers;
    });

    allLayersAndSublayers.items.forEach(function (item) {
      if (item.title === "Kommune") {
        kommuneUrl = item.url;
      }
      if (item.title === "Fylke") {
        fylkeUrl = item.url;
      }
    });

    const kommuneName = await GetAllKommunes();
    const fylkesName = await getAllFylkes();

    async function getAllFylkes() {
      let queryObject = new Query();
      queryObject.where = "1=1";
      queryObject.returnGeometry = false;
      queryObject.outFields = ["fylke", "fylke_id"];

      await query
        .executeQueryJSON(fylkeUrl, queryObject)
        .then(async function (results) {
          await results.features.forEach(function (feature) {
            Fylkeresult.push(feature.attributes);
          });
        });
      setFylkeresult(Fylkeresult);
    }

    async function GetAllKommunes() {
      let queryObject = new Query();
      queryObject.where = "1=1";
      queryObject.returnGeometry = false;
      queryObject.outFields = ["kommune", "komm_id", "fylke_id"];

      await query
        .executeQueryJSON(kommuneUrl, queryObject)
        .then(async function (results) {
          await results.features.forEach(function (feature) {
            Kommuneresult.push(feature.attributes);
          });
        });
      setKommuneresult(Kommuneresult);
    }
    // Finalresult = Kommuneresult.map((item, i) =>
    //   Object.assign({}, item, Fylkeresult[i])
    // );
    if (Fylkeresult.length !== 0 && Kommuneresult.length !== 0) {
      Finalresult = Kommuneresult.map((t1) => ({
        ...t1,
        ...Fylkeresult.find((t2) => t2.fylke_id === t1.fylke_id),
      }));
    }

    let result = [];
    let s = [];
    Finalresult.map((u) => {
      if (!s.includes(u.fylke_id)) {
        result.push({
          ID: u.fylke_id,
          name: u.fylke,
          key: u.fylke_id,
          children: [
            {
              ID: u.kommune,
              key: u.komm_id,
              name: u.kommune,
            },
          ],
        });

        s.push(u.fylke_id);
      } else {
        let index = result.findIndex((element) => element.ID === u.fylke_id);
        result[index].children.push({
          ID: u.komm_id,
          key: u.komm_id,
          name: u.kommune,
        });
      }
    });
    result.sort((a, b) => (a.name > b.name ? 1 : -1));
    setDemograresultarray(result);
    setloading(false);
  }, []);
  const columns = [
    {
      title: "",
      dataIndex: "name",
      key: "key",
      sorter: (a, b) => {
        return;
      },
      sortOrder: "ascend",
      sortDirections: ["ASC", "DESC"],
    },
  ];
  let AllSelectedIDS = [];
  const callback = (
    selectedrecord,
    SelectedKommunekeys,
    checkedRows,
    recordObject
  ) => {
    setnomessagediv(false);
    // setselectedKoummeIDs([]);
    let reolID = [];
    if (selectedrecord) {
      reolID = checkedRows.map(function (item) {
        if (item.hasOwnProperty("children")) {
          return extractValue(item.children, "key");
        } else {
          return item.key;
        }
      });

      let s = [];
      // let SelectedKoummneID = s.concat.apply(s, reolID);
      let SelectedKoummneID = reolID.filter((element) => {
        return element !== undefined;
      });
      if (selectedKoummeIDs.length > 0) {
        selectedKoummeIDs.map((item) => {
          SelectedKoummneID.push(item);
        });
      }
      // SelectedKoummneID.push(selectedKoummeIDs);
      // setselectedKoummeIDs([...new Set(reolID.flat(1))]);
      setselectedKoummeIDs(SelectedKommunekeys);
    }
    setselectedrecord(checkedRows);
  };

  const fetchData = async () => {
    try {
      const { data, status } = await api.getdata("Kommune/GetAllKommunes");
      if (status === 200) {
        let s = [];
        let result = [];

        data.map((u) => {
          if (!s.includes(u.fylkeID)) {
            result.push({
              ID: u.fylkeID,
              name: u.fylkeName,
              key: u.fylkeID,
              children: [
                {
                  ID: u.kommuneID,
                  key: u.kommuneID,
                  name: u.kommuneName,
                },
              ],
            });

            s.push(u.fylkeID);
          } else {
            let index = result.findIndex((element) => element.ID === u.fylkeID);
            result[index].children.push({
              ID: u.kommuneID,
              key: u.kommuneID,
              name: u.kommuneName,
            });
          }
        });
        result.sort((a, b) => (a.name > b.name ? 1 : -1));
        setDemograresultarray(result);
        // setData(result)

        // let s =data.map(item=>{
        // 	return {
        // 		ID : item.fylkeID,
        // 		name :item.fylkeName,
        // 		children : [{
        // 		ID : item.kommuneID,
        // 		name : item.kommuneName,
        // 		}]
        // 		}
        // })
      } else {
        console.error("error : " + status);
      }
    } catch (error) {
      console.error("er : " + error);
    }
  };

  return (
    <div className={loading ? "col-5 p-2 blur" : "col-5 p-2"}>
      <div className="paddingBig_NoColor_B">
        <span className=" title ">Velg geografisk område</span>

        <div className="padding_NoColor_T">
          <p
            id="DemografiAnalyse1_uxHeader_lblDesc"
            className="lblAnalysisHeaderDesc"
          >
            Utvalget lages med utgangspunkt i de budrutene hvor dine valgte
            variabler er best representert.
          </p>
        </div>
        <div className="">
          <p className="lblAnalysisHeaderStep ">
            {" "}
            1. Velg kjennetegn for målgruppen din <br />
            <b> 2. Velg geografisk område</b> <br />
            3. Velg antall mottakere
          </p>
        </div>
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

        {nomessagediv ? (
          <div className="pr-3">
            <div className="error WarningSign">
              <div className="divErrorHeading">Melding:</div>
              <span
                id="uxKjoreAnalyse_uxLblMessage"
                className="divErrorText_kw"
              >
                Ingen segmenter er valgt. Velg minst ett segment.
              </span>
            </div>
          </div>
        ) : null}
        <br />
        <div className="lblAnalysisHeaderStep ">
          <p>Velg fylker eller klikk på pluss-tegnene for å velge kommuner.</p>
        </div>

        {melding2 ? (
          <span className=" sok-Alert-text pl-1">{errormsg2}</span>
        ) : null}
        {melding2 ? <p></p> : null}

        {}

        {/* <div style={{ backgroundColor: "#E6E6E6" }}> */}
        <div>
          <div
            style={{
              width: "350px",
              height: "300px",
              overflow: "auto",
            }}
          >
            <TableNew
              columnsArray={columns}
              data={Demograresultarray}
              page={"Demogra"}
              defaultSelectedColumn={reolID}
              parentCallback={callback}
              setoutputDataList={setOutputData}
            />
          </div>
          <br />
        </div>
        <div className="div_left">
          <input
            type="submit"
            name="DemografiAnalyse1$uxFooter$uxBtForrige"
            value="Tilbake"
            onClick={goback}
            className="KSPU_button_Gray"
          />
          <div className="padding_NoColor_T">
            <a className="KSPU_LinkButton_Url_KW pl-2" onClick={GotoMain}>
              Avbryt
            </a>
          </div>
        </div>

        <div className="float-right">
          <div>
            <input
              type="submit"
              name="DemografiAnalyse1$uxFooter$uxBtnNeste"
              value="Velg antall "
              onClick={VelgAntallClick}
              className="KSPU_button-kw"
            />
          </div>
        </div>
      </div>
    </div>
  );
}

export default Demogra_Velg_Click;
