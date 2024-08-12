import React, { useState, useContext, useEffect } from "react";
import Geogra_distribution_cart_click from "./Geogra_distribution_cart_click";
import { KundeWebContext, MainPageContext } from "../context/Context.js";
import api from "../services/api.js";
import loadingImage from "../assets/images/callbackActivityIndicator.gif";
import { kundeweb_utvalg } from "./KspuConfig";
import moment from "moment";
import {
  NumberFormat,
  CurrentDate,
  filterCommonReolIds,
  ColorCodes,
  CommonColorCodes,
} from "../common/Functions";
import CalendarKW from "./CalendarKW";
import Graphic from "@arcgis/core/Graphic";
import Query from "@arcgis/core/rest/support/Query";
import * as query from "@arcgis/core/rest/query";
import * as watchUtils from "@arcgis/core/core/watchUtils";
import Spinner from "./spinner/spinner.component";

function Geogra_distribution_click() {
  const { custNos, setcustNos } = useContext(KundeWebContext);
  const { avtaleData, setavtaleData } = useContext(KundeWebContext);
  const { username_kw, setusername_kw } = useContext(KundeWebContext);
  const [errormsg, seterrormsg] = useState("");
  const [melding, setmelding] = useState(false);
  const [loading, setloading] = useState(false);
  const { mapView } = useContext(MainPageContext);
  const [gramalertvisible, setgramalertvisible] = useState(false);
  const [cartclick, setcartclick] = useState(false);
  const [mmalertvisible, setmmalertvisible] = useState(false);
  const [ShowCalenderComp, setShowCalenderComp] = useState(false);
  const [buttonenable, setbuttonenable] = useState(true);
  const { utvalgapiobject, setutvalgapiobject } = useContext(KundeWebContext);
  const { utvalglistapiobject, setutvalglistapiobject } =
    useContext(KundeWebContext);
  const [weightInGram, setWeight] = useState("");
  const [thicknessInMm, setThickness] = useState("");
  const [inputWeight, setInputWeight] = useState("");
  const [inputThickness, setInputThickness] = useState("");
  const { SavedUtvalg, setSavedUtvalg } = useContext(KundeWebContext);
  const [radio1value, setradio1value] = useState(false);
  const [radio2value, setradio2value] = useState(true);
  const { Page, setPage } = useContext(KundeWebContext);
  const { Antallvalue, setAntallvalue } = useContext(KundeWebContext);
  const [Selecteddate, setSelecteddate] = useState("");
  const { Page_P, setPage_P } = useContext(KundeWebContext);
  const [enable, setenable] = useState(true);
  const { UtvalgID, setUtvalgID } = useContext(KundeWebContext);
  const { utvalgexist, setutvalgexist } = useContext(KundeWebContext);
  const [calendarvisible, setcalendarvisible] = useState(false);
  const [Type, setType] = useState("");
  const [defultselectedday, setdefultselectedday] = useState("");
  const { businesscheckbox, setbusinesscheckbox } = useContext(KundeWebContext);
  const { householdcheckbox, sethouseholdcheckbox } =
    useContext(KundeWebContext);
  const { CartItems, setCartItems } = useContext(KundeWebContext);
  const [selectedReolId, setSelectedReolId] = useState([]);
  const [mapLoading, setMapLoading] = useState(false);

  useEffect(async () => {
    if (Page_P === "cartClick_Component_kw") {
      if (utvalglistapiobject.weight !== "" && utvalglistapiobject.weight > 0) {
        setWeight(utvalglistapiobject.weight);
        setInputWeight(utvalglistapiobject.weight);
      }
      if (
        utvalglistapiobject.thickness !== "" &&
        utvalglistapiobject.thickness > 0
      ) {
        setThickness(utvalglistapiobject.thickness);
        setInputThickness(utvalglistapiobject.thickness);
      }

      if (
        Number(utvalglistapiobject.weight) > 0 &&
        Number(utvalglistapiobject.thickness) > 0
      ) {
        setbuttonenable(false);
        setShowCalenderComp(true);
      }
    } else {
      if (utvalgapiobject.weight !== "" && utvalgapiobject.weight > 0) {
        setWeight(utvalgapiobject.weight);
        setInputWeight(utvalgapiobject.weight);
      }
      if (utvalgapiobject.thickness !== "" && utvalgapiobject.thickness > 0) {
        setThickness(utvalgapiobject.thickness);
        setInputThickness(utvalgapiobject.thickness);
      }
      if (
        Number(utvalgapiobject.weight) > 0 &&
        Number(utvalgapiobject.thickness) > 0
      ) {
        setbuttonenable(false);
        setShowCalenderComp(true);
      }
    }
    // if (
    //   Number(weightInGram) > 0 ||
    //   (Number(thicknessInMm) > 0 && Number(thicknessInMm) > 0)
    // ) {
    //   setbuttonenable(false);
    //   setShowCalenderComp(true);
    // }
    if (utvalgapiobject.distributionDate !== "") {
      let formatchange = new Date(utvalgapiobject.distributionDate);
      formatchange = new Date(formatchange);
      setdefultselectedday(formatchange);
    }
    if (utvalglistapiobject.distributionDate !== "") {
      let formatchange = new Date(utvalglistapiobject.distributionDate);
      formatchange = new Date(formatchange);
      setdefultselectedday(formatchange);
    }
    if (Page_P === "cartClick_Component_kw") {
      setUtvalgID(utvalglistapiobject?.listId);
      setType("L");
    } else if (
      Page_P === "Apne_Button_Click" ||
      Page_P === "EndreClick_kw" ||
      "LagutvalgClick"
    ) {
      setUtvalgID(utvalgapiobject?.utvalgId);
      setType("U");
    }
    const windowUrl = new URL(window.location.href);
    let utvalgType =
      windowUrl.searchParams.get("utvalgstype") !== null ||
      windowUrl.searchParams.get("utvalgstype") !== ""
        ? windowUrl.searchParams.get("utvalgstype")
        : sessionStorage.getItem("kwUtvalgsType");
    let selectionId =
      windowUrl.searchParams.get("utvalgid") !== null ||
      windowUrl.searchParams.get("utvalgid") !== ""
        ? windowUrl.searchParams.get("utvalgid")
        : sessionStorage.getItem("kwUtvalgsId");

    if (utvalgType && selectionId) {
      setMapLoading(true);
      if (utvalgType === "U") {
        try {
          let reol_id = [];
          utvalgapiobject.reoler.map((item) => {
            reol_id.push(item.reolId);
          });
          await MapRender(reol_id, "rgba(237, 54, 21, 0.25)");
          ZoomToAll();
          setMapLoading(false);
        } catch (error) {
          setMapLoading(false);
        }
      } else {
        try {
          let commonSelections = filterCommonReolIds(CartItems);
          let renderedCount = 0; //index can't be used.
          CartItems.map(async (items, index1) => {
            let Reolids = [];
            if (items.reoler && items.reoler.length > 0) {
              items.reoler.map((item) => {
                if (!commonSelections.filteredCommonItems.includes(item.reolId))
                  Reolids.push(item.reolId);
              });
            }
            if (Reolids?.length) {
              await MapRender(Reolids, ColorCodes()[index1]);
            }
            renderedCount = renderedCount + 1;
            if (renderedCount === CartItems?.length) {
              await mapRenderDoubleCoverage(commonSelections);
              ZoomToAll();
              renderedCount = 0;
              setMapLoading(false);
            }
          });
        } catch (error) {
          setMapLoading(false);
        }
      }
    }
  }, []);

  const mapRenderDoubleCoverage = async (commonSelections) => {
    if (commonSelections.filteredCommonItems?.length > 0) {
      await MapRender(
        commonSelections.filteredCommonItems,
        CommonColorCodes()[0]
      );
    }
  };

  const MapRender = async (Reolids, colorcode) => {
    await watchUtils.whenFalseOnce(mapView, "updating");
    let k = Reolids.map((element) => "'" + element + "'").join(",");
    let sql_geography = `reol_id in (${k})`;
    let BudruterUrl;
    let allLayersAndSublayers;

    allLayersAndSublayers = mapView.map.allLayers.flatten(function (item) {
      return item.layers || item.sublayers;
    });

    allLayersAndSublayers.items.forEach(function (item) {
      if (item.title === "Budruter") {
        BudruterUrl = item.url;
      }
    });
    await GetAllBurdruter();
    async function GetAllBurdruter() {
      let queryObject = new Query();

      queryObject.where = `${sql_geography}`;
      queryObject.returnGeometry = true;
      queryObject.outFields = ["tot_anta", "hh", "hh_res"];

      await query
        .executeQueryJSON(BudruterUrl, queryObject)
        .then(function (results) {
          if (results.features.length > 0) {
            let selectedSymbol = {
              type: "simple-fill",
              color: colorcode,
              style: "solid",
              outline: {
                color: [237, 54, 21],
                width: 0.75,
              },
            };

            results.features.map((item) => {
              let graphic = new Graphic(
                item.geometry,
                selectedSymbol,
                item.attributes
              );
              mapView.graphics.add(graphic);
            });
          }
        });
    }
  };

  const ZoomToAll = () => {
    let featuresGeometry = [];
    mapView.graphics.items.map((item) => {
      if (item.geometry.type === "polygon") {
        featuresGeometry.push(item.geometry);
      }
    });
    mapView.goTo(featuresGeometry);
  };

  const gramcheck = (e) => {
    setShowCalenderComp(false);
    setenable(true);
    setbuttonenable(true);
    // let distthinckness = e.target.value;
    let weightGM = e.target.value.toString().replace(",", ".");
    setWeight(weightGM);
    setInputWeight(weightGM);
    if (
      Number(weightGM) > 200 ||
      Number(weightGM) <= 3.5 ||
      Number(weightGM).toString() === "NaN" ||
      Number(weightGM) % 1 !== 0 ||
      e.target.value.toString().includes(",") ||
      e.target.value.toString().includes(".")
    ) {
      setgramalertvisible(true);
    }
    // else if (Number(weightGM) <= 3.5) {
    //   setgramalertvisible(true);
    // } else if (Number(weightGM).toString() === "NaN") {
    //   setgramalertvisible(true);
    // }
    // else if (isCharacterALetter(distthinckness_1)) {
    //   setgramalertvisible(true);
    // }
    else {
      setgramalertvisible(false);
    }
    if (mmalertvisible) {
      setbuttonenable(true);
    }
    if (gramalertvisible) {
      setbuttonenable(true);
    }

    if (weightInGram && thicknessInMm) {
      setbuttonenable(false);
    } else {
      setbuttonenable(true);
    }
  };
  const ShowCalender = async () => {
    setloading(true);
    // let url = `GetPrsCalendarAdminDetails/GetRestcapacity?id=${2379974}&type=${"L"}&vekt=${39}&distribusjonstype=${"B"}&startDato=${"01-01-2022"}&sluttDato=${"31-01-2022"}&thinckness=${3}`;
    // let admin_url1 = `https://dev.pumainternweb.bring.no/DataAccessAPI/api/GetPrsCalendarAdminDetails/GetPrsAdminData?id=2331950&type=U&vekt=1&distribusjonstype=S&startDato=01-01-2022&sluttDato=${01-31-2022}&thickness=${1}`
    // try {
    //   const { data, status } = await api.getdata(url);
    //   } catch (error) {
    //   console.log(error);
    // }
    setShowCalenderComp(true);
    setenable(true);
    setloading(false);
  };
  const cartClick = () => {
    setPage("cartClick_Component_kw");
    // setcartclick(true);
  };
  const goback = () => {
    //remove query string on tilbake
    let queryParams = new URLSearchParams(window.location.search);
    let keyParams = [];
    for (let key of queryParams.keys()) {
      keyParams.push(key);
    }
    let newUrl = new URL(window.location.href);
    for (let i = 0; i < keyParams.length; i++) {
      newUrl.searchParams.delete(keyParams[i]);
    }
    window.history.replaceState({}, document.title, newUrl.href);

    if (Page_P == "Apne_Button_Click") {
      setPage("Apne_Button_Click");
    } else if (Page_P === "Lestill_Click_Component") {
      setPage("Lestill_Click_Component");
    } else if (Page_P === "cartClick_Component_kw") {
      setPage("cartClick_Component_kw");
    } else if (Page_P === "EndreClick_kw") {
      setPage(Page_P);
    } else {
      // setutvalgexist(true);
      setPage("Geogra_distribution_tilbake_click");
      // setPage("LagutvalgClick");
    }
  };
  // const isCharacterALetter = (char) => {
  //   return /[a-zA-Z]/.test(char);
  // };
  const mmcheck = (e) => {
    setShowCalenderComp(false);
    setenable(true);
    setbuttonenable(true);
    let distthincknessmm = e.target.value;
    setInputThickness(e.target.value);
    let Temp = distthincknessmm.toString().replace(",", ".");
    setThickness(Temp);
    if (Number(Temp) > 5 || Number(Temp) <= 0) {
      setmmalertvisible(true);
    } else if (Number(Temp).toString() === "NaN") {
      setmmalertvisible(true);
    } else {
      setmmalertvisible(false);
    }

    if (mmalertvisible) {
      setbuttonenable(true);
    }
    if (gramalertvisible) {
      setbuttonenable(true);
    } else {
      setbuttonenable(false);
    }
    if (weightInGram && thicknessInMm) {
      setbuttonenable(false);
    }
  };
  const handleCallback = (childData) => {
    setenable(false);
    let parsedDate = moment(childData, "DD.MM.YYYY H:mm:ss");
    let distributiondate = parsedDate.toLocaleString().slice(0, -5);

    setSelecteddate(distributiondate);
  };
  const SaveBeforeConfiguratorCall = () => {
    setenable(false);
    let xxcu_parameters = sessionStorage.getItem("xxcu_parameters");
    let custNo = custNos;
    // var kundeurl = `${process.env.REACT_APP_CONFIGURATOR_URL}?utvalgstype=${Type}&utvalgid=${UtvalgID}&kundenummer=${custNos}&xxcu_thicknessVal=${thicknessInMm}&${xxcu_parameters}`;
    window.open(
      `${
        process.env.REACT_APP_CONFIGURATOR_URL
      }?utvalgstype=${Type}&utvalgid=${Number(
        UtvalgID
      )}&kundenummer=${custNo}&xxcu_thicknessVal=${Number(
        thicknessInMm
      )}&${xxcu_parameters}`,
      "_self"
    );
  };
  const configuratorCall = async () => {
    if (Selecteddate == "") {
      seterrormsg("velg en hvilken som helst dato");
      setmelding(true);
    } else {
      let saveOldReoler = "false";
      let skipHistory = "false";
      let forceUtvalgListId = 0;
      let customerName = "";
      if (username_kw) {
        customerName = username_kw;
      } else {
        customerName = "test";
      }
      let name = username_kw;

      try {
        let A = {};
        let url = "";
        if (Page_P === "LagutvalgClick") {
          url = `Utvalg/SaveUtvalg?userName=${customerName}&`;
          url = url + `saveOldReoler=${saveOldReoler}&`;
          url = url + `skipHistory=${skipHistory}&`;
          url = url + `forceUtvalgListId=${forceUtvalgListId}`;
          A = SavedUtvalg;
        } else if (Page_P === "cartClick_Component_kw") {
          url = `UtvalgList/UpdateUtvalgListDistributionData?userName=${customerName}`;
          A["distributionDate"] = Selecteddate;
          A["distributionType"] = 2;
          A["thickness"] = Number(thicknessInMm);
          A["weight"] = parseInt(Number(weightInGram));
          A["listId"] = utvalglistapiobject?.listId;
        } else {
          url = `Utvalg/SaveUtvalg?userName=${customerName}&`;
          url = url + `saveOldReoler=${saveOldReoler}&`;
          url = url + `skipHistory=${skipHistory}&`;
          url = url + `forceUtvalgListId=${forceUtvalgListId}`;
          A = utvalgapiobject;
        }

        if (Page_P !== "cartClick_Component_kw") {
          A["kundeNummer"] = custNos;
          Object.keys(utvalgapiobject).length > 0
            ? (utvalgapiobject.kundeNummer = custNos)
            : (utvalglistapiobject.kundeNummer = custNos);

          if (avtaleData) {
            A["avtalenummer"] = avtaleData;
            Object.keys(utvalgapiobject).length > 0
              ? (utvalgapiobject.avtalenummer = avtaleData)
              : (utvalglistapiobject.avtalenummer = avtaleData);

            //utvalgapiobject.avtalenummer = avtaleData;
          } else {
            A["avtalenummer"] = 0;
            Object.keys(utvalgapiobject).length > 0
              ? (utvalgapiobject.avtalenummer = 0)
              : (utvalglistapiobject.avtalenummer = 0);

            //utvalgapiobject.avtalenummer = 0;
          }
          A.weight = parseInt(Number(weightInGram));
          Object.keys(utvalgapiobject).length > 0
            ? (utvalgapiobject.weight = parseInt(Number(weightInGram)))
            : (utvalglistapiobject.weight = parseInt(Number(weightInGram)));
          Object.keys(utvalgapiobject).length > 0
            ? (utvalgapiobject.thickness = Number(thicknessInMm))
            : (utvalglistapiobject.thickness = Number(thicknessInMm));
          A.thickness = Number(thicknessInMm);
          Object.keys(utvalgapiobject).length > 0
            ? (utvalgapiobject.distributionDate = Selecteddate)
            : (utvalglistapiobject.distributionDate = Selecteddate);
          A.distributionDate = Selecteddate;
          Object.keys(utvalgapiobject).length > 0
            ? (utvalgapiobject.distributionType = "2")
            : (utvalglistapiobject.distributionType = "2");
          A.distributionType = "2";
          Object.keys(utvalgapiobject).length > 0
            ? utvalgapiobject.modifications.push({
                modificationId: Math.floor(100000 + Math.random() * 900000),
                userId: customerName,
                modificationTime: CurrentDate(),
                listId: 0,
              })
            : utvalglistapiobject.modifications.push({
                modificationId: Math.floor(100000 + Math.random() * 900000),
                userId: customerName,
                modificationTime: CurrentDate(),
              });
          A.modifications.push({
            modificationId: Math.floor(100000 + Math.random() * 900000),
            userId: customerName,
            modificationTime: CurrentDate(),
            listId: 0,
          });
        }
        if (Page_P === "cartClick_Component_kw") {
          const { data, status } = await api.putData(url, A);
          if (status === 200) {
            // setutvalgapiobject(data);

            // alert("success");
            await SaveBeforeConfiguratorCall();
          } else {
            setmelding(true);
            seterrormsg("noe gikk galt");
          }
        } else {
          const { data, status } = await api.postdata(url, A);
          if (status === 200) {
            await SaveBeforeConfiguratorCall();
          } else {
            setmelding(true);
            seterrormsg("noe gikk galt");
          }
        }
      } catch (e) {
        console.log(e);
      }
    }
  };

  return (
    <div className="col-5 p-2">
      <div>{mapLoading ? <Spinner /> : null}</div>
      <div
        className="padding_NoColor_B"
        style={{ cursor: "pointer" }}
        onClick={CartItems?.length > 0 ? cartClick : null}
      >
        <a id="uxHandlekurvSmall_uxLnkbtnHandlekurv">
          <div className="handlekurv handlekurvText pl-2">
            Du har {CartItems?.length} utvalg i bestillingen din.
          </div>
        </a>
      </div>

      {!cartclick ? (
        <div>
          {" "}
          <div id="uxTabDistribusjon">
            <div id="uxDistribusjon_uxContents">
              <div id="uxDistribusjon_uxInnerContents">
                <div className="titleWizard padding_NoColor_B">
                  Distribusjonsdetaljer
                </div>

                <div className="padding_Color_L_R_T_B">
                  <table style={{ width: "100%", overflow: "auto" }}>
                    <tbody>
                      <tr>
                        <td style={{ width: "75px" }} align="center">
                          <div className="divLabelText">Antall </div>
                        </td>
                        <td style={{ width: "50px" }} align="left">
                          <div
                            id="uxDistribusjon_uxDistAntallsInfo"
                            className="divValueText pl-1"
                          >
                            {Page_P === "cartClick_Component_kw"
                              ? NumberFormat(utvalglistapiobject?.antall)
                              : Page_P === "Apne_Button_Click" ||
                                Page_P === "EndreClick_kw" ||
                                Page_P === "LagutvalgClick"
                              ? NumberFormat(utvalgapiobject?.totalAntall)
                              : 0}
                          </div>
                          {/* {Type == "L" ? (
                            <div
                              id="uxDistribusjon_uxDistAntallsInfo"
                              className="divValueText pl-1"
                            >
                              {NumberFormat(
                                utvalgapiobject.antall
                                  ? utvalgapiobject.antall
                                  : utvalgapiobject.totalAntall
                                  ? utvalgapiobject.totalAntall
                                  : utvalglistapiobject
                                  ? utvalglistapiobject.antall
                                  : 0
                              )}
                            </div>
                          ) : Page_P == "cartClick_Component_kw" ? (
                            <div
                              id="uxDistribusjon_uxDistAntallsInfo"
                              className="divValueText pl-1"
                            >
                              {NumberFormat(utvalglistapiobject.antall)}
                            </div>
                          ) : Page_P == "EndreClick_kw" ? (
                            <div
                              id="uxDistribusjon_uxDistAntallsInfo"
                              className="divValueText pl-1"
                            >
                              {NumberFormat(
                                utvalgapiobject.totalAntall
                                  ? utvalgapiobject.totalAntall
                                  : utvalglistapiobject.antall
                              )}
                            </div>
                          ) : businesscheckbox &&
                            !householdcheckbox &&
                            Page_P != "cartClick_Component_kw" ? (
                            <div
                              id="uxDistribusjon_uxDistAntallsInfo"
                              className="divValueText pl-1"
                            >
                              {NumberFormat(utvalgapiobject.Antall[1])}
                            </div>
                          ) : businesscheckbox &&
                            householdcheckbox &&
                            Page_P != "cartClick_Component_kw" ? (
                            <div
                              id="uxDistribusjon_uxDistAntallsInfo"
                              className="divValueText pl-1"
                            >
                              {NumberFormat(
                                utvalgapiobject.Antall[1] +
                                  utvalgapiobject.Antall[0]
                              )}
                            </div>
                          ) : !businesscheckbox &&
                            householdcheckbox &&
                            Page_P != "cartClick_Component_kw" ? (
                            <div
                              id="uxDistribusjon_uxDistAntallsInfo"
                              className="divValueText pl-1"
                            >
                              {NumberFormat(utvalgapiobject.Antall[0])}
                            </div>
                          ) : null} */}
                        </td>
                        <td style={{ width: "100px" }}></td>
                      </tr>
                      <tr>
                        <td align="center">
                          <div className="divLabelText">Vekt pr. sending</div>
                        </td>
                        <td>
                          <div className="row">
                            &nbsp;&nbsp;
                            <input
                              type="text"
                              id="uxDistThickness"
                              value={inputWeight.toString().replace(".", ",")}
                              onChange={gramcheck}
                              className="divnumberText DistWeight_Intrn"
                              maxLength="4"
                              // onkeyup="Distr.gui.thickness.validate();Distr.gui.thickness.onEnter(event);"
                              // onblur="Distr.gui.thickness.thicknessWarning();"
                            />
                            &nbsp;{" "}
                            {gramalertvisible ? (
                              <span id="uxWeightValErr" className="red">
                                <b>!</b>
                              </span>
                            ) : null}
                            {/* <td align=""> */}
                            <div className="bold">
                              gram{" "}
                              <span
                                id="uxDistribusjon_dvWeightLimitText"
                                className="gray"
                              >
                                (maks 200g){" "}
                              </span>
                            </div>
                            {/* </td> */}
                          </div>
                        </td>
                      </tr>
                      <tr>
                        <td></td>
                        <td></td>
                        <td align="left">
                          <div className="row">
                            <a
                              href="http://www.bring.no/radgivning/sende-noe/klargjoring/klargjoring-uadressert-post"
                              id="uxDistribusjon_uxVektInformation"
                              target="_blank"
                              className="KSPU_LinkButton_Url_KW"
                            >
                              Hjelp til vektberegning
                            </a>
                          </div>
                        </td>
                      </tr>

                      <tr>
                        <td align="center">
                          <div className="divLabelText">
                            Tykkelse pr. sending
                          </div>
                        </td>
                        <td>
                          <div className="row">
                            &nbsp;&nbsp;&nbsp;
                            <input
                              type="text"
                              id="uxDistThicknessmm"
                              className="divnumberText DistWeight_Intrn"
                              maxLength="4"
                              value={inputThickness
                                .toString()
                                .replace(".", ",")}
                              onChange={mmcheck}
                            />
                            &nbsp;{" "}
                            {mmalertvisible ? (
                              <span id="uxThicknessValErr" className="red">
                                <b>!</b>
                              </span>
                            ) : null}
                            {/* </div>
                        </td>
                        <td> */}
                            <div className="bold">
                              mm{" "}
                              <span
                                id="uxDistribusjon_dvThicknessLimitText"
                                className="gray"
                              >
                                (maks 5mm)
                              </span>
                            </div>
                          </div>
                        </td>
                      </tr>
                      <tr>
                        <td></td>
                        <td></td>
                        <td>
                          <input
                            name="uxDistribusjon$uxType"
                            type="hidden"
                            id="uxDistribusjon_uxType"
                            value="U"
                          />
                          <input
                            name="uxDistribusjon$uxDistId"
                            type="hidden"
                            id="uxDistribusjon_uxDistId"
                            value="2359412"
                          />
                          <input
                            name="uxDistribusjon$uxDistInfo"
                            type="hidden"
                            id="uxDistribusjon_uxDistInfo"
                            value="0|Null|01.01.0001|false|0"
                          />
                          <input
                            name="uxDistribusjon$uxDistMK"
                            type="hidden"
                            id="uxDistribusjon_uxDistMK"
                          />
                        </td>
                      </tr>
                    </tbody>
                  </table>

                  {/* <div className="padding_NoColor_T_B">
                    <input
                      type="button"
                      id="uxBtShowDates"
                      className={
                        buttonenable ? "KSPU_button_Gray" : "KSPU_button-kw"
                      }
                      width="210px"
                      style={{ width: "210px" }}
                      value="Vis mulige distribusjonsdatoer"
                      disabled={buttonenable}
                      onClick={ShowCalender}
                    />
                  </div> */}
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

                  <div id="uxDistError" className="Hide">
                    <div className="error WarningSign">
                      <div className="divErrorHeading"> Melding: </div>
                      <span
                        id="uxShowDist_uxErrorMsg"
                        className="divErrorText"
                      ></span>
                    </div>
                  </div>
                </div>
                <div className="padding_NoColor_T_B">
                  <input
                    type="button"
                    id="uxBtShowDates"
                    className={
                      Number(inputWeight) > 200 ||
                      Number(inputWeight) % 1 !== 0 ||
                      Number(inputThickness) > 5 ||
                      Number(inputWeight) <= 0 ||
                      Number(inputThickness) <= 0 ||
                      inputThickness === "" ||
                      inputWeight === "" ||
                      mmalertvisible ||
                      gramalertvisible
                        ? "KSPU_button_Gray"
                        : "KSPU_button-kw"
                    }
                    width="210px"
                    style={{ width: "210px" }}
                    value="Vis mulige distribusjonsdatoer"
                    disabled={
                      Number(inputWeight) > 200 ||
                      Number(inputWeight) % 1 !== 0 ||
                      Number(inputThickness) > 5 ||
                      Number(inputWeight) <= 0 ||
                      Number(inputThickness) <= 0 ||
                      inputThickness === "" ||
                      inputWeight === "" ||
                      mmalertvisible ||
                      gramalertvisible
                        ? true
                        : false
                    }
                    onClick={ShowCalender}
                  />
                </div>

                <div id="uxDistKalenderBilde" className="Show">
                  <div className="paddingBig_NoColor_T clearFloat"></div>
                </div>

                {ShowCalenderComp ? (
                  <div id="uxDistKalender">
                    <div className="padding_NoColor_T clearFloat">
                      <div className="padding_Color_L_R_T_B">
                        <span id="uxSubTitle" className="subTitle">
                          Velg distribusjonsperiode
                        </span>

                        <div className="padding_Color_T_B sok-text">
                          &nbsp;&nbsp;
                          <input
                            type="radio"
                            className="sok-text hide select-plan radio-label"
                            name="select-plan"
                            disabled
                          />
                          <span className="radio-label">tidliguke</span>
                          &nbsp;&nbsp;
                          <input
                            type="radio"
                            id="uxrbDate"
                            name="disttype"
                            className="sok-text"
                            value="B"
                            checked={radio2value}
                            onChange="Distr.calendar.changeDistType(this.value);"
                          />{" "}
                          &nbsp;&nbsp; midtuke
                        </div>
                        {!melding ? <p></p> : null}
                        {melding ? (
                          <div className="pr-3">
                            <div className="error WarningSign">
                              <div className="divErrorHeading">Melding:</div>
                              <p
                                id="uxKjoreAnalyse_uxLblMessage"
                                className="divErrorText_kw"
                              >
                                {errormsg}
                              </p>
                            </div>
                          </div>
                        ) : null}
                        {melding ? (
                          <div>
                            <p></p>
                          </div>
                        ) : null}

                        <div className="padding_Color_T row">
                          <CalendarKW
                            page="DTPage1"
                            fontSize="11pt"
                            parentCallback={handleCallback}
                            weight={weightInGram}
                            thickness={thicknessInMm}
                            type={Type}
                            selection={"B"}
                            UtvalgID={UtvalgID}
                            defaultDate={defultselectedday}
                            newselectedReolId={selectedReolId}
                            Calendar={"normalCalendar"}
                          />
                        </div>

                        <div className="padding_Color_B clearFloat">
                          <table width="100%">
                            <tbody>
                              <tr></tr>
                            </tbody>
                          </table>
                        </div>
                        <div style={{ fontSize: "13px" }}>
                          Ønsker du andre datoer enn de som er valgbare her, ta
                          kontakt med kundeservice på 04045.
                        </div>
                        <div className="paddingBig_NoColor_T clearFloat">
                          <div id="uxDistInfoStar" className="Hide">
                            <span className="red">*</span>
                            <span id="Span1" className="divValueText">
                              Distribusjon på en dag merket med stjerne
                              innebærer at enkelte budruter med for liten
                              kapasitet blir utelatt. Klikk datoen for å se
                              hvilke budruter det gjelder.
                            </span>
                          </div>

                          <div
                            id="uxDistFullBookedError"
                            className="clearFloat paddingBig_NoColor_T Hide"
                          >
                            <div className="error WarningSign">
                              <div
                                id="uxShowDist_uxFullBookedHeading"
                                className="divErrorHeading"
                              >
                                {" "}
                                Fullbooket budruter i denne perioden
                              </div>
                              <span
                                id="uxShowDist_uxFullErrorMsg"
                                className="divErrorText"
                              >
                                Systemet klarte ikke å hentet kapasitetsdata,
                                prøv igjen senere.
                              </span>
                              <span
                                id="uxShowDist_uxFullBookedMsg"
                                className="divValueText"
                              >
                                Disse budrutene er markert med rødt i kartet.
                                Klikk på dem for mer informasjon eller
                              </span>
                              <div className="padding_NoColor_T_B">
                                <span
                                  id="uxShowDist_uxFullBookeRemove"
                                  className="divValueText clearFloat"
                                >
                                  <input
                                    type="checkbox"
                                    name="acceptRemove"
                                    id="acceptRemove"
                                    value="Ja"
                                    // onClick="Distr.gui.acceptRemove.clicked();"
                                  />{" "}
                                  Jeg godtar at de fullbookede rutene fjernes
                                  fra bestillingen
                                </span>
                                <span
                                  id="uxShowDist_uxFullBookeBasedOn"
                                  className="diverrorText clearFloat"
                                >
                                  Utvalget kan ikke endres, du må velge en dag
                                  med full kapasitet eller frikoble utvalget.
                                </span>
                              </div>
                              <span
                                id="uxShowDist_uxFullBookedNext"
                                className="divValueText"
                              >
                                <br />
                                Ønsker du ikke å utelate disse budrutene må du
                                endre distribusjonsdato. Neste ledige er{" "}
                              </span>
                            </div>
                          </div>
                        </div>

                        <div
                          className="padding_Color_T_B divValueText"
                          id="uxDistOutofDateRangeMsg"
                        ></div>
                      </div>
                    </div>
                  </div>
                ) : null}

                <div className="paddingBig_NoColor_T clearFloat">
                  <div className="div_left">
                    <input
                      type="submit"
                      name="uxDistribusjon$uxBtnDistBack"
                      value="Tilbake"
                      onClick={goback}
                      id="uxDistribusjon_uxBtnDistBack"
                      className="KSPU_button_Gray"
                    />
                  </div>
                  <div className="div_right">
                    <input
                      type="submit"
                      name="uxDistribusjon$uxDistSetDelivery"
                      value="Angi innleveringsdetaljer"
                      id="uxDistribusjon_uxDistSetDelivery"
                      disabled={enable}
                      onClick={() => {
                        configuratorCall();
                      }}
                      className={enable ? "KSPU_button_Gray" : "KSPU_button-kw"}
                      style={{ width: "175px" }}
                    />
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      ) : (
        <Geogra_distribution_cart_click />
      )}
    </div>
  );
}
export default Geogra_distribution_click;
