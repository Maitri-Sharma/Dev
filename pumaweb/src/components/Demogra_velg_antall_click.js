import React, { useState, useContext } from "react";
import { KundeWebContext } from "../context/Context.js";
import { MainPageContext } from "../context/Context.js";
import api from "../services/api.js";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import Extent from "@arcgis/core/geometry/Extent";
import { MapConfig } from "../config/mapconfig";
import Graphic from "@arcgis/core/Graphic";

import loadingImage from "../assets/images/callbackActivityIndicator.gif";
import { GetData, groupBy } from "../Data";
import { NumberFormatKW } from "../common/Functions.js";
import {
  Utvalg,
  NewUtvalgName,
  criterias_KW,
  getAntall_KW,
  formatData,
  getAntallUtvalg,
} from "./KspuConfig";
import NumberFormat from "react-number-format";

function Demogra_Velg_Antal_Click() {
  const { showBusiness, setShowBusiness } = useContext(KundeWebContext);
  const { custNos, setcustNos } = useContext(KundeWebContext);
  const { avtaleData, setavtaleData } = useContext(KundeWebContext);
  const { username_kw, setusername_kw } = useContext(KundeWebContext);
  const { showReservedHouseHolds, setShowReservedHouseHolds } =
    useContext(KundeWebContext);

  const { selectedKoummeIDs, setselectedKoummeIDs } =
    useContext(KundeWebContext);
  const { mapView } = useContext(MainPageContext);
  const { Page, setPage } = useContext(KundeWebContext);
  const [VelgCheck, setVelgCheck] = useState(true);
  const [AngiCheck, setAngiCheck] = useState(false);
  const [disable, setdisable] = useState(true);
  const [nomessagediv, setnomessagediv] = useState(false);
  const { Page_P, setPage_P } = useContext(KundeWebContext);

  const [textvalue, settextvalue] = useState("");
  const [loading, setloading] = useState(false);
  const { householdcheckbox, sethouseholdcheckbox } =
    useContext(KundeWebContext);
  const { businesscheckbox, setbusinesscheckbox } = useContext(KundeWebContext);
  const { utvalgapiobject, setutvalgapiobject } = useContext(KundeWebContext);
  const [viskartvalue, setviskartvalue] = useState(0);

  const { ActiveUtvalgObject, setActiveUtvalgObject } =
    useContext(KundeWebContext);
  const { ActiveMapButton, setActiveMapButton } = useContext(MainPageContext);
  const { routeUpdateEnabled, setRouteUpdateEnabled } =
    useContext(KundeWebContext);
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

  const VelgClick = () => {
    setVelgCheck(true);
    setdisable(true);
    setAngiCheck(false);
  };

  const AngiClick = () => {
    setAngiCheck(true);
    setdisable(false);
    setVelgCheck(false);
  };

  const GotoMain = () => {
    setPage("");

    //disable add remove rute widget
    setutvalgapiobject({});
    setRouteUpdateEnabled(false);

    setActiveMapButton("");
    mapView.activeTool = null;
    //set initial extent
    mapView.extent = new Extent(MapConfig.kundewebMapExtent);
    mapView.goTo(mapView.extent);
  };

  const goback = () => {
    setPage("Demogra_Velg_Click");

    //disable add remove rute widget
    setutvalgapiobject({});
    setRouteUpdateEnabled(false);

    setActiveMapButton("");
    mapView.activeTool = null;
    //set initial extent
    mapView.extent = new Extent(MapConfig.kundewebMapExtent);
    mapView.goTo(mapView.extent);
  };

  const LagAntallClick = async () => {
    if (viskartvalue > 0) {
      await viskartclick();
      // setutvalgapiobject(utvalgapiobject);
    } else {
      setutvalgapiobject(ActiveUtvalgObject);
    }
    setloading(true);

    sethouseholdcheckbox(true);
    setbusinesscheckbox(false);
    setPage("LagutvalgClick");
    setPage_P("Demogra_velg_antall_click");
    setloading(false);
  };

  const createUtvalgObject = (selectedDataSet, criteriaType, key) => {
    Antall = getAntallUtvalg(selectedDataSet);
    var a = Utvalg();
    a.hasReservedReceivers = showReservedHouseHolds ? true : false;
    a.name = NewUtvalgName();
    a.totalAntall = Antall[0];
    a.receivers = [{ ReceiverId: 1, selected: true }];
    if (showBusiness) a.receivers.push({ ReceiverId: 4, selected: true });
    if (showReservedHouseHolds)
      a.receivers.push({ ReceiverId: 5, selected: true });
    a.modifications = [];
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
    // setutvalgapiobject({});
    setutvalgapiobject(a);
  };

  const viskartclick = async () => {
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

    let stringwithcomma = r.map((element) => {
      return element + ">=0" + " " + "OR" + " ";
    });

    let lastelement = stringwithcomma[stringwithcomma.length - 1];
    let lastelement1 = lastelement.slice(0, -4);
    stringwithcomma = stringwithcomma.slice(0, -1);
    stringwithcomma.push(lastelement1);

    let k = selectedKoummeIDs;
    k = selectedKoummeIDs.map((element) => "'" + element + "'").join(",");
    let sql_geography = `AND (main.KOMMUNEID IN (${k}))`;
    stringwithcomma = stringwithcomma.map((element) => element);

    let sql_param_where_clause = ` (${stringwithcomma})`;

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
        return "indeks." + el;
      } else {
        return "main." + el;
      }
    });

    let API_PARAM = {
      options: {
        MaxAntall: Number(viskartvalue),
        SQLWhereClause: sql_param_where_clause.replaceAll(",", ""),
        SQLWhereClauseGeography: sql_geography,
        sqlOrderby: sqlOrderby,
        indexFieldSelected: indexFieldSelected,
      },
      utvalg: null,
    };

    try {
      const { data, status } = await api.postdata(
        `Reol/GetReolerByDemographySearch?isFromKundeWeb=${true}`,
        API_PARAM
      );
      if (status === 200) {
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
                let totalCount = 0;
                results.features.forEach(function (feature) {
                  featuresGeometry.push(feature.geometry);
                  totalCount = totalCount + feature.attributes.hh;
                  currentReoler.push(formatData(feature.attributes));
                  let graphic = new Graphic(
                    feature.geometry,
                    selectedSymbol,
                    feature.attributes
                  );
                  mapView.graphics.add(graphic);
                });
                mapView.goTo(featuresGeometry);
              }
            });
          await createUtvalgObject(currentReoler, "Demografi", 20);
        }
      }

      setPage_P(Page);
      setloading(false);
    } catch (error) {
      setloading(false);

      console.error("er : " + error);
    }
  };

  return (
    <div className={loading ? "col-5 p-2 blur" : "col-5 p-2"}>
      <div className="paddingBig_NoColor_B">
        <span className=" title ">Velg antall mottakere</span>

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
            2. Velg geografisk område <br />
            <b> 3. Velg antall mottakere. </b>
          </p>
        </div>
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
        <div className="pr-3 pb-4">
          <div className="error WarningSign">
            <div className="divErrorHeading">Melding:</div>
            <span id="uxKjoreAnalyse_uxLblMessage" className="divErrorText_kw">
              Antall mottakere må spesifiseres.
            </span>
          </div>
        </div>
      ) : null}
      <p></p>

      <table className="padding_NoColor_L_R_T_B wizFilled">
        <p></p>
        <tbody>
          <tr style={{ width: "100%", paddingBottom: "2px" }}>
            <td className="pl-2">
              <input
                id="DemografiAnalyse1_DemografiKriterier1_rbuxFPAlder"
                type="radio"
                name="DemografiAnalyse1$DemografiKriterier1$aa"
                value="rbuxFPAlder"
                checked={VelgCheck}
                onChange={VelgClick}
              />
            </td>
            <td align="left">
              <span
                id="DemografiAnalyse1_DemografiAntall_lblDesc1"
                className="lblAnalysisHeaderStepBold"
              >
                Velg anbefalt utvalg:
              </span>
            </td>
            <td align="right" className="pr-2">
              <span
                id="DemografiAnalyse1_DemografiAntall_lblAntall"
                className="lblAnalysisHeaderStepBold pl-2"
              >
                {NumberFormatKW(ActiveUtvalgObject.Antall[0])}
              </span>
            </td>
          </tr>
          <tr>
            <td colSpan="3 ">
              <p
                id="DemografiAnalyse1_DemografiAntall_lblDesc2"
                className="lblAnalysisHeaderStep pl-2"
              >
                Totalt finnes det{" "}
                {NumberFormatKW(Number(ActiveUtvalgObject.Antall[0]) * 5)}{" "}
                mottakere i det valgte geografiske området. Utvalget vi
                anbefaler er laget med utgangspunkt i de budrutene hvor dine
                valgte variabler er{" "}
                <a
                  data-toggle="modal"
                  data-target="#exampleModal"
                  className="KSPU_LinkInText_kw"
                >
                  best representert.{" "}
                </a>
              </p>
            </td>
          </tr>
        </tbody>
      </table>

      <br />
      <table className="padding_NoColor_L_R_T_B wizFilled">
        <p></p>
        <tbody>
          <tr style={{ width: "100%", paddingBottom: "2px" }}>
            <td className="pl-2">
              <input
                id="DemografiAnalyse1_DemografiKriterier1_rbuxFPAlder"
                type="radio"
                name="DemografiAnalyse1$DemografiKriterier1$aa"
                value="rbuxFPAlder"
                checked={AngiCheck}
                onChange={AngiClick}
              />
            </td>
            <td align="left">
              <span
                id="DemografiAnalyse1_DemografiAntall_lblDesc1"
                className="lblAnalysisHeaderStepBold"
              >
                Angi ønsket antall mottakere:
              </span>
            </td>
            <td align="right" className="pr-2">
              <NumberFormat
                style={{ width: "70px" }}
                onValueChange={(values) => {
                  const { formattedValue, value } = values;
                  settextvalue(value);
                  setviskartvalue(value);
                }}
                disabled={disable}
                // format={textvalue.length <= 4 ? "### ##" : "### ### ### ###"}
              />{" "}
            </td>
          </tr>
          <tr>
            <td colSpan="2 ">
              <p
                id="DemografiAnalyse1_DemografiAntall_lblDesc2"
                className="lblAnalysisHeaderStep pl-2"
              >
                Du kan sende til flere eller færre enn vår anbefaling. Vær
                oppmerksom på at jo færre mottakere du velger desto{" "}
                <a
                  className="KSPU_LinkInText_kw"
                  data-toggle="modal"
                  data-target="#exampleModal-1"
                >
                  mer presist{" "}
                </a>
                treffer du de kundene du ønsker.
              </p>
            </td>
            <td align="right" className="pr-2">
              <input
                type="submit"
                name="DemografiAnalyse1$DemografiAntall$btnVis"
                disabled={disable}
                value="Vis i kart"
                onClick={viskartclick}
                id="DemografiAnalyse1_DemografiAntall_btnVis"
              />
              {/* <input name="DemografiAnalyse1$DemografiAntall$txtBrukAnbefaltHidden" type="text" maxLength="10" id="DemografiAnalyse1_DemografiAntall_txtBrukAnbefaltHidden" className="buttonHidden"/> */}
            </td>
          </tr>
        </tbody>
      </table>
      <br />

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
            value="Lag utvalg"
            onClick={LagAntallClick}
            className="KSPU_button-kw"
          />
        </div>

        {/* modal box code */}
        <div
          className="modal fade "
          id="exampleModal"
          tabIndex="-1"
          role="dialog"
          aria-labelledby="exampleModalCenterTitle"
        >
          <div className="modal-dialog " role="document">
            <div className="">
              <div className="modalstyle-1">
                <div className=" divDockedPanelTop_demografi">
                  <span className="dialog-kw pl-1" id="exampleModalLabel">
                    Best representert
                  </span>
                </div>
                <div className="View_modal-body pl-2">
                  <div id="" className="sok-text">
                    <p>
                      Dette utvalget inneholder kun de beste budrutene hvor dine
                      valgte variabler er overrepresentert innenfor angitte
                      geografiske område.
                    </p>

                    <p>Eksempel:</p>

                    <p>
                      Hvis du for eksempel velger variabelen Biltype-Toyota,
                      rangeres alle budrutene i gitte område etter hvor mange
                      Toyota-eiere det bor i budruten. Det vil si at de
                      budrutene hvor det er prosentvis høyest andel Toyota-eiere
                      prioriteres først. Løsningen velger automatisk ut de 20%
                      beste budrutene av totalen.
                    </p>
                  </div>
                  <div
                    id="11.208939868046542_closeDiv"
                    className="modalAlertClose"
                  >
                    {/* <button type="button" className="" aria-label= "Close" data-dismiss="modal">Nei</button> */}

                    <input
                      value="OK"
                      type="button"
                      id="11.208939868046542_closeButton"
                      data-dismiss="modal"
                      aria-label="Close"
                    />
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        {/* code ends */}

        {/* modal box code */}
        <div
          className="modal fade bd-example-modal-lg"
          id="exampleModal-1"
          tabIndex="-1"
          role="dialog"
          aria-labelledby="exampleModalCenterTitle"
        >
          <div className="modal-dialog" role="document">
            <div className="">
              <div className="modalstyle-1">
                <div className=" divDockedPanelTop_demografi">
                  <span className="dialog-kw pl-1" id="exampleModalLabel">
                    Mer presist
                  </span>
                </div>
                <div className="View_modal-body pl-2">
                  <div id="" className="sok-text">
                    <p>Eksempel:</p>
                    <p>
                      Dersom du for eksempel benytter variabelen Biltype -
                      Toyota som kjennetegn for din målgruppe, så inkluderer vår
                      anbefaling de budrutene som har den prosentvis høyeste
                      andelen av Toyota-eiere.
                    </p>
                    <br />
                    <p>
                      Løsningen velger automatisk ut de 20% beste budrutene av
                      totalen. Velger du færre enn anbefalt antall, vil den
                      relative andelen av Toyota-eiere øke i ditt utvalg.
                    </p>
                  </div>
                  <div
                    id="11.208939868046542_closeDiv"
                    className="modalAlertClose"
                  >
                    {/* <button type="button" className="" aria-label= "Close" data-dismiss="modal">Nei</button> */}

                    <input
                      value="OK"
                      type="button"
                      id="11.208939868046542_closeButton"
                      data-dismiss="modal"
                      aria-label="Close"
                    />
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        {/* code ends */}
      </div>
    </div>
  );
}

export default Demogra_Velg_Antal_Click;
