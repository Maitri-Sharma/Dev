import React, { useState, useEffect, useContext, useRef } from "react";
import style from "../App.css";
import { KundeWebContext } from "../context/Context.js";
import TableNew from "./Table_New_kw";
import api from "../services/api.js";
import MottakerComponent from "./Mottaker_KW.js";
import LagutvalgClick from "./Lagutvalgclick.js";
import http from "../services/http";
import { MainPageContext } from "../context/Context.js";

import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";
import loadingImage from "../assets/images/callbackActivityIndicator.gif";
import Color from "@arcgis/core/Color";
import SimpleLineSymbol from "@arcgis/core/symbols/SimpleLineSymbol";
import SimpleFillSymbol from "@arcgis/core/symbols/SimpleFillSymbol";
import FeatureLayer from "@arcgis/core/layers/FeatureLayer";

import {
  Utvalg,
  NewUtvalgName,
  criterias_KW,
  getAntall,
  formatData,
  getAntallUtvalg,
} from "./KspuConfig";
import { MapConfig } from "../config/mapconfig";
import { GetData, groupBy } from "../Data";

function VeglGeografiskOmrade_kw(props) {
  const [SelectedDataSet, setSelectedDataSet] = useState([]);
  const { utvalgapiobject, setutvalgapiobject } = useContext(KundeWebContext);
  const { criteriaObject, setCriteriaObject } = useContext(KundeWebContext);
  const { showBusiness, setShowBusiness } = useContext(KundeWebContext);
  const { showReservedHouseHolds, setShowReservedHouseHolds } =
    useContext(KundeWebContext);
  const [loading, setloading] = useState(false);
  const [reolID, setreolID] = useState([]);
  const [datalist, setData] = useState([]);
  const [outputData, setOutputData] = useState([]);
  const { Page, setPage } = useContext(KundeWebContext);
  const { HouseholdSum_seg, setHouseholdSum_seg } = useContext(KundeWebContext);
  const { BusinessSum, setBusinessSum } = useContext(KundeWebContext);
  const { segmenterresultarray, setsegmenterresultarray } =
    useContext(KundeWebContext);
  const [selectedrecord, setselectedrecord] = useState([]);
  const [HouseholdSum_tree, setHouseholdSum_tree] = useState(0);
  const [BusinessSum_tree, setBusinessSum_tree] = useState(0);
  const [selectedhush, setselectedhush] = useState(0);
  const [Budruterresult, setBudruterresult] = useState([]);
  const { mapView } = useContext(MainPageContext);
  const { Antallvalue, setAntallvalue } = useContext(KundeWebContext);
  const { selectedKoummeIDs, setselectedKoummeIDs } =
    useContext(KundeWebContext);
  const { selectedsegment, setselectedsegment } = useContext(KundeWebContext);
  const [nomessagediv, setnomessagediv] = useState(false);
  const { Page_P, setPage_P } = useContext(KundeWebContext);
  const { selectedrecord_s, setselectedrecord_s } = useContext(KundeWebContext);
  const { ResultOutputData, setResultOutputData } =
    React.useContext(KundeWebContext);
  const { householdcheckbox, sethouseholdcheckbox } =
    useContext(KundeWebContext);
  const { businesscheckbox, setbusinesscheckbox } = useContext(KundeWebContext);
  const { custNos, setcustNos } = useContext(KundeWebContext);
  const { avtaleData, setavtaleData } = useContext(KundeWebContext);
  const { username_kw, setusername_kw } = useContext(KundeWebContext);
  const { selectedRowKeys, setSelectedRowKeys } = useContext(KundeWebContext);

  useEffect(() => {
    let j = mapView.graphics.items.length;
    for (var i = j; i > 0; i--) {
      if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
        mapView.graphics.remove(mapView.graphics.items[i - 1]);
        //j++;
      }
    }
  }, []);

  let routesData = [];
  let currentReoler = [];
  const routes = (data) => {
    data.map((item) => {
      routesData.push(item);
    });
  };

  function extractValue(arr, prop) {
    // extract value from property
    let extractedValue = arr.map((item) => item[prop]);

    return extractedValue;
  }
  const callback = (
    selectedrecord,
    SelectedKommunekeys,
    checkedRows,
    recordObject
  ) => {
    // setlagutvalgenable(true)
    setnomessagediv(false);
    setselectedKoummeIDs([]);
    let reolID = [];
    if (checkedRows.length > 0) {
      reolID = checkedRows.map(function (item) {
        if (item.hasOwnProperty("children")) {
          return extractValue(item.children, "key");
        } else {
          return item.key;
        }
      });
      // let s = [];
      // let SelectedKoummneID = s.concat.apply(s, reolID);
      // SelectedKoummneID = SelectedKoummneID.filter((element) => {
      //   return element !== undefined;
      // });

      // SelectedKoummneID.push(selectedKoummeIDs);
      // setselectedKoummeIDs(reolID.flat(1));
      setselectedKoummeIDs(SelectedKommunekeys);

      // setreolID(reolID)
    }
    // if(selectedrecord.length == 0){
    //     setselectedrecord(datalist)
    // }
    // else{
    setselectedrecord([]);
    setselectedrecord_s([]);

    // }

    // let houshold_sum = selectedrecord.reduce((accumulator, current) => accumulator + current.House, 0);
    // let Business_sum = selectedrecord.reduce((accumulator, current) => accumulator + current.Business, 0);
    //        setHouseholdSum_tree(parseInt(houshold_sum)+parseInt(selectedhush))
    //        setHouseholdSum(parseInt(houshold_sum)+parseInt(selectedhush))
    //        setBusinessSum_tree(parseInt(Business_sum))
    //        setBusinessSum(parseInt(Business_sum))
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
  const createUtvalgObject = (selectedDataSet, criteriaType, key) => {
    sethouseholdcheckbox(true);
    setbusinesscheckbox(false);
    let routes = getSelectedRoutes(selectedDataSet);
    // let routesData =
    //   segmenterParam.length > 0 ? segmenterParam : demograficParam;
    Antall = getAntallUtvalg(selectedDataSet);
    var a = Utvalg();
    a.hasReservedReceivers = showReservedHouseHolds ? true : false;
    a.name = NewUtvalgName();
    // a.totalAntall =
    //   Antall[0] +
    //   (showBusiness ? Antall[1] : 0) +
    //   (showReservedHouseHolds ? Antall[2] : 0);
    a.receivers = [{ ReceiverId: 1, selected: true }];
    if (showBusiness) a.receivers.push({ ReceiverId: 4, selected: true });
    if (showReservedHouseHolds)
      a.receivers.push({ ReceiverId: 5, selected: true });
    a.modifications = [];
    a.totalAntall = Antall[0];
    a.reoler = selectedDataSet;
    a.Business = Antall[1];
    a.ReservedHouseHolds = Antall[2];
    a.hush = Antall[0];
    a.criterias.push(criterias_KW(criteriaType, key));
    a.Antall = Antall;
    a.ordreReferanse = "";
    a.oldReolMapName = "";
    a.kundeNavn = username_kw;
    a.kundeNummer = custNos;
    a.avtalenummer = avtaleData;
    setutvalgapiobject({});
    setutvalgapiobject(a);
  };

  const goback = () => {
    // remove previous highlighted feature
    // let j = mapView.graphics.items.length;
    //     var k = 0;
    //     k = j;
    //     for (var i = j; i > 0; i--) {
    //       if (mapView.graphics.items[i-1].geometry.type === "polygon") {
    //         mapView.graphics.remove(mapView.graphics.items[i-1]);
    //         //j++;
    //       }
    //     }
    // mapView.graphics.removeAll();
    let j = mapView.graphics.items.length;
    for (var i = j; i > 0; i--) {
      if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
        mapView.graphics.remove(mapView.graphics.items[i - 1]);
        //j++;
      }
    }
    //move to initial extent
    // mapView.goTo(mapView.initialExtent);
    setPage("Segmentervelg");
  };
  const GotoMain = () => {
    // remove previous highlighted feature
    // let j = mapView.graphics.items.length;
    //     var k = 0;
    //     k = j;
    //     for (var i = j; i > 0; i--) {
    //       if (mapView.graphics.items[i-1].geometry.type === "polygon") {
    //         mapView.graphics.remove(mapView.graphics.items[i-1]);
    //         //j++;
    //       }
    //     }
    // mapView.graphics.removeAll();
    let j = mapView.graphics.items.length;
    for (var i = j; i > 0; i--) {
      if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
        mapView.graphics.remove(mapView.graphics.items[i - 1]);
        //j++;
      }
    }
    //move to initial extent
    // mapView.goTo(mapView.initialExtent);
    setPage("");
  };
  const velgGeogra = async () => {
    if (selectedKoummeIDs.length == 0) {
      setnomessagediv(true);
    } else {
      setloading(true);
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

      let url = "Reol/GetReolerBySegmenterSearch";
      try {
        const { data, status } = await api.postdata(
          `Reol/GetReolerBySegmenterSearch`,
          API_PARAM
        );
        if (status == 200) {
          // if (data.reoler && data.reoler.length > 0) {
          //   routes(data.reoler);
          //   let selectedDataSet = [];
          //   // setSegmenterParam(data.reoler);
          //   let ReolDataRow = groupBy(
          //     data.reoler,
          //     "",
          //     0,
          //     // showBusiness,
          //     true,
          //     // showReservedHouseHolds,
          //     true,
          //     []
          //   );
          //   selectedDataSet.push(ReolDataRow[0]);
          // }
          //  else {
          let stringwithcomma1 = selectedsegment
            .map((element) => "'" + element + "'")
            .join(",");
          let k1 = selectedKoummeIDs
            .map((element) => "'" + element + "'")
            .join(",");
          let segmentWhereClause1 = `SEGMENT IN (${stringwithcomma1})`;
          let kommuneWhereClause1 = `kommuneID in (${k1})`;

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
            let queryObject = new Query();

            queryObject.where = `${kommuneWhereClause1} AND ${segmentWhereClause1}`;
            queryObject.returnGeometry = true;
            queryObject.outFields = MapConfig.budruterOutField;

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

                  // remove previous highlighted feature
                  // let j = mapView.graphics.items.length;
                  // var k = 0;
                  // k = j;
                  // for (var i = j; i > 0; i--) {
                  //   if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
                  //     mapView.graphics.remove(mapView.graphics.items[i - 1]);
                  //     //j++;
                  //   }
                  // }
                  // mapView.graphics.removeAll();
                  let j = mapView.graphics.items.length;
                  for (var i = j; i > 0; i--) {
                    if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
                      mapView.graphics.remove(mapView.graphics.items[i - 1]);
                      //j++;
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
            createUtvalgObject(currentReoler, "Segment", 30);
            setBudruterresult(Budruterresult);
          }
          if (Budruterresult.length > 0) {
            let total_antall = Budruterresult.map(
              (item) => item.tot_anta
            ).reduce((prev, next) => prev + next);
            let household_res = Budruterresult.map(
              (item) => item.hh_res
            ).reduce((prev, next) => prev + next);
            let household_value = Budruterresult.map((item) => item.hh).reduce(
              (prev, next) => prev + next
            );
            // utvalgapiobject.totalAntall = household_value;
            setAntallvalue(household_value);
            setHouseholdSum_seg(household_value);
          } else {
            let total_antall = 0;
            let household_value = 0;
            // utvalgapiobject.totalAntall = 0;
            setAntallvalue(household_value);
            setHouseholdSum_seg(household_value);
          }
          // setAntallvalue(household_value);
          // setHouseholdSum_seg(household_value);
          setPage_P(Page);
          sethouseholdcheckbox(true);
          setbusinesscheckbox(false);

          setPage("LagutvalgClick");



          setloading(false);

          //     let stringwithcomma=selectedsegment.map(element=>("'"+element)+"'").join(",");
          //     let k = selectedKoummeIDs.map(element=>("'"+element)+"'").join(",");
          //     let sql_param_where_clause = `SEGMENT IN (${stringwithcomma})`
          //    let sql_geography = `(kommuneID in (${k}))`
          //     let API_PARAM = { "MaxAntall":"-1",
          //      "SQLWhereClause":sql_param_where_clause,
          //       "SQLWhereClauseGeography":sql_geography,
          //       "indexFieldSelected": [],
          //       "sqlOrderby": "",
          //     }
          //     let url = 'Reol/GetReolerBySegmenterSearch'
          //     try{
          //       const {data, status} = await api.postdata(`Reol/GetReolerBySegmenterSearch`,API_PARAM)
          //       if(status == 200){
          //       let reoler = data.reoler;
          //       let s =[];
          //       if(reoler.length > 0){
          //       let k = reoler.map(item =>
          //       {
          //         if(item !== null){
          //           s.push(item.antall)
          //         }
          //         return s
          //       })
          //     }
          //       var sum = 0;
          //       for (var i = 0; i < s.length; i++) {
          //         sum += s[i].households + s[i].householdsReserved
          //       }
          //       let HouseholdSum_sum = sum;
          //       setHouseholdSum_seg(HouseholdSum_sum)
          //       setPage_P(Page)
          //       setPage("LagutvalgClick");
          //       }
          //     }
          //     catch (error) {
          //       console.error('er : ' + error);
          //     }
          //   }
          // }
          // const fetchData = async () => {
          // try {
          // const {data , status} = await api.getdata('Kommune/GetAllKommunes');
          // if(status === 200)
          // {
          // setData(data);
          // }
          // else
          // {
          // console.error('error : ' + status);
          // }
          // }
          // catch (error) {
          // console.error('er : ' + error);
          //}
        }
      } catch (error) {
        console.error("er : " + error);
      }
    }
  };
  const columns = [
    // {
    // title: '',
    // dataIndex: 'ID',
    // key: 'key',
    // },
    {
      title: "",
      dataIndex: "name",
      key: "key",
      sorter: (a, b) => {
        return ;
      },
      sortOrder: "ascend",
      sortDirections: ["ASC", "DESC"],
    },
  ];
  return (
    <div className={loading ? "col-5  blur" : "col-5"}>
      <div className="">
        <span className="title">Velg geografisk område</span>
      </div>
      <div className="paddingBig_NoColor_T specialfont">
        <span
          id="uxSegmenterAnalyse_uxHeader_lblStep1"
          className="lblAnalysisHeaderStep"
        >
          1. Velg ett eller flere{" "}
          <a
            href="http://www.bring.no/radgivning/kundedialog/dm-i-postkasse"
            target="_blank"
            runat="server"
            className="KSPU_LinkInText_kw"
          >
            segmenter
          </a>{" "}
          nedenfor
        </span>
        <div>
          <span
            id="DemografiAnalyse1_uxHeader_lblStep2"
            className="lblAnalysisHeaderStepBold"
          >
            2. Velg geografisk område
          </span>
        </div>
      </div>
      {/* <br/> */}
      <p></p>
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
            <span id="uxKjoreAnalyse_uxLblMessage" className="divErrorText_kw">
              Ingen segmenter er valgt. Velg minst ett segment.
            </span>
          </div>
        </div>
      ) : null}
      {/* <br/> */}
      <p></p>
      <div className="padding_NoColor_T">
        <p
          id="DemografiAnalyse1_uxHeader_lblDesc"
          className="lblAnalysisHeaderDesc"
        >
          Velg fylker eller klikk på pluss-tegnene for å velge kommuner.
          &nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;
        </p>
      </div>
      {/* <div style={{ backgroundColor: "#E6E6E6" }}> */}
      <div>
        <div style={{ width: "350px", height: "300px", overflow: "auto" }}>
          <TableNew
            columnsArray={columns}
            data={segmenterresultarray}
            page={"segmenter"}
            defaultSelectedColumn={reolID}
            parentCallback={callback}
            setoutputDataList={setOutputData}
          />
        </div>
        <br />
      </div>
      {/* <div style={{ backgroundColor: "#E6E6E6" }}>
        <MottakerComponent
          householdvalue={0}
          Businessvalue={0}
          display={"segmenter"}
        />

        <br />
      </div> */}
      <div className="mt-2">
        <div className="div_left">
          <input
            type="submit"
            name="DemografiAnalyse1$uxFooter$uxBtForrige"
            value="Tilbake"
            onClick={goback}
            className="KSPU_button_Gray"
          />
          <div className="padding_NoColor_T">
            <a
              className="KSPU_LinkButton_Url_KW pl-2"
              onClick={GotoMain}
            >
              Avbryt
            </a>
          </div>
        </div>
        <div className="float-right">
          <div>
            <input
              type="submit"
              name="DemografiAnalyse1$uxFooter$uxBtnNeste"
              value="Lag utvalg"
              onClick={velgGeogra}
              className="KSPU_button-kw"
            />
          </div>
        </div>
      </div>
    </div>
  );
}
export default VeglGeografiskOmrade_kw;
