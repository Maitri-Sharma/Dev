import React, { useEffect, useRef, useState, useContext } from "react";
import { KundeWebContext, MainPageContext } from "../../context/Context";
import "../apne_Button_Click-kw/Apne_Button_Click-kw.scss";
import Swal from "sweetalert2";
import $ from "jquery";
import api from "../../services/api.js";
import readmore from "../../assets/images/read_more.gif";
import { kundeweb_utvalg, Utvalg } from "../KspuConfig";
import loadingImage from "../../assets/images/callbackActivityIndicator.gif";
import { StandardreportKw } from "../apne_Button_Click-kw/standardreportKw";
// import VisDetaljerModal from "../VisDetaljerModal_KW";
import white from "../../assets/images/white.gif";
import warningImg from "../../assets/images/varsels_ikon_stort.png";
import "../../components/cartClick_Component_Kw/cartClick_kw.styles.scss";

import {
  NumberFormat,
  CreateUtvalglist,
  CurrentDate,
  imagePath,
} from "../../common/Functions";
import { bootstrap } from "bootstrap";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";
import Spinner from "../spinner/spinner.component";
import { MapConfig } from "../../config/mapconfig";
import VisDetaljerModal_KW from "../VisDetaljerModal_KW";
import moment from "moment";

function EndreClick_kw() {
  const btnclose = useRef();
  const { mapView } = useContext(MainPageContext);
  const kopiermodal = useRef();
  const { custNos, setcustNos } = useContext(KundeWebContext);
  const { avtaleData, setavtaleData } = useContext(KundeWebContext);
  const { username_kw, setusername_kw } = useContext(KundeWebContext);
  const { Endreapiobject, setEndreapiobject } = useContext(KundeWebContext);
  const [warninputvalue_1, setwarninputvalue_1] = useState("");
  const { UtvalgID, setUtvalgID } = useContext(KundeWebContext);
  const { utvalgname, setutvalgname } = useContext(KundeWebContext);
  const { Antallvalue, setAntallvalue } = useContext(KundeWebContext);
  const [apnetext, setapnetext] = useState("");
  const { Page, setPage } = useContext(KundeWebContext);
  const { warninputvalue, setwarninputvalue } = useContext(KundeWebContext);
  const { Page_P, setPage_P } = useContext(KundeWebContext);
  const [melding, setmelding] = useState(false);
  const [errormsg, seterrormsg] = useState("");
  const { newhome, setnewhome } = useContext(KundeWebContext);
  const [test, settest] = useState(false);
  const [loading, setloading] = React.useState(true);
  const [kopiereUtvalgetName, setKopiereUtvalgetName] = useState("");
  const [skrivUtvalgetName, setSkrivUtvalgetName] = useState("");
  const [desinput, setdesinput] = useState("");
  const [defaultID, setdefaultID] = useState(0);
  const { utvalgapiobject, setutvalgapiobject } = useContext(KundeWebContext);
  const [melding1, setmelding1] = useState(false);
  const [errormsg1, seterrormsg1] = useState("");
  const [melding3, setmelding3] = useState(false);
  const [errormsg3, seterrormsg3] = useState("");
  const [Modal, setModal] = useState("");
  const [Total, setTotal] = useState(0);
  const [ResultBeforeCreationObject, setResultBeforeCreationObject] =
    useState();
  const [ResultAfterCreationObject, setResultAfterCreationObject] = useState();
  const [commonReoler, setCommonReoler] = useState();
  const [visDetaljerModalName, setVisDetaljerModalName] = useState(" ");
  const { Endrelistapiobject, setEndrelistapiobject } =
    useContext(KundeWebContext);
  const { CartItems, setCartItems } = useContext(KundeWebContext);
  const { utvalglistapiobject, setutvalglistapiobject } =
    useContext(KundeWebContext);
  const [newcartitems, setnewcartitems] = useState([]);
  const [, updateState] = React.useState();
  const forceUpdate = React.useCallback(() => updateState({}), []);
  const { ActiveMapButton, setActiveMapButton } = useContext(MainPageContext);
  const [utvalgUpdateLoading, setUtvalgUpdateLoading] = useState(false);
  const { selectionUpdateKW, setSelectionUpdateKW } =
    useContext(KundeWebContext);
  const [routeUpdate, setRoutepdate] = useState(false);
  const [antallBeforeRecreationSum, setAntallBeforeRecreationSum] = useState(0);
  const [antallWhenSavedSum, setAntallWhenSavedSum] = useState(0);
  const [resultRestReoler, setResultRestReoler] = useState();
  const [ResultAfterRuteCreation, setResultAfterRuteCreation] = useState();
  const [showBusiness, setShowBusiness] = useState(false);
  const [showHouseHold, setShowHouseHold] = useState(false);

  // const dateRef = useRef(null);

  const AvbrytClick = () => {
    setloading(true);
  };
  const LeggTillClick = async () => {
    setnewhome(true);
    setPage("");
  };

  const MapRenderFun1 = async (Reolids, colorcode) => {
    if (Reolids.length > 0) {
      let k = Reolids.map((element) => "'" + element + "'").join(",");
      let sql_geography = `reol_id in (${k})`;
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
                // color: [237, 54, 21, 0.25],
                color: colorcode,
                style: "solid",
                outline: {
                  // autocasts as new SimpleLineSymbol()
                  color: [237, 54, 21],
                  width: 0.75,
                },
              };

              // remove previous highlighted feature
              // mapView.graphics.removeAll();

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
          });
      }
    }
  };

  const EndreClick = async (Endrevalue, CartList) => {
    setSelectionUpdateKW(false);
    setActiveMapButton("");
    mapView.activeTool = null;
    // mapView.graphics.removeAll();
    let j = mapView.graphics.items.length;
    for (var i = j; i > 0; i--) {
      if (mapView.graphics.items[i - 1].geometry.type === "polygon") {
        mapView.graphics.remove(mapView.graphics.items[i - 1]);
        //j++;
      }
    }
    createActiveUtvalg(Endrevalue);
    // let IndexRemovedEndre = JSON.parse(JSON.stringify(Endrevalue));
    // setutvalgapiobject(IndexRemovedEndre);
    // setEndreapiobject(IndexRemovedEndre);
    let reol_id = [];
    Endrevalue.reoler.map((item) => {
      reol_id.push(item.reolId);
    });
    await MapRenderFun1(reol_id, "rgba(237, 54, 21, 0.25)");
    let tempcart = JSON.parse(JSON.stringify(CartList));

    let newcart = tempcart.filter((item) => {
      return item.utvalgId !== Endrevalue.utvalgId;
    });

    // setnewcartitems(newcart);

    let sum = 0;
    for (let i = 0; i < newcart.length; i++) {
      if (newcart[i].antall) {
        sum = sum + newcart[i].antall;
      } else {
        sum = sum + newcart[i].totalAntall;
      }
    }
    setTotal(sum);
    // setnewcartitems([...newcart]);
    if (
      Object.keys(Endreapiobject).length > 0 &&
      Endreapiobject.utvalgId == "U"
    ) {
      Endreapiobject.utvalgId = Endreapiobject.utvalgId.substring(1);
    } else if (
      Object.keys(Endrelistapiobject).length > 0 &&
      Endrelistapiobject.listId == "L"
    ) {
      Endrelistapiobject.listId = Endrelistapiobject.listId.substring(1);
    }

    setUtvalgID(
      Object.keys(Endrelistapiobject).length > 0
        ? Endrelistapiobject.listId
        : Endreapiobject.utvalgId
    );
    setdefaultID(
      Object.keys(Endrelistapiobject).length > 0
        ? Endrelistapiobject.listId
        : Endreapiobject.utvalgId
    );

    setutvalgname(
      Object.keys(Endrelistapiobject).length > 0
        ? Endrelistapiobject.name
        : Endreapiobject.name
    );
    setAntallvalue(
      Object.keys(Endrelistapiobject).length == 0
        ? Endreapiobject.totalAntall
        : Endreapiobject.antall
    );

    // if (Object.keys(Endrevalue).length > 0) {
    //   setutvalgapiobject(Endrevalue);
    //   setEndreapiobject(Endrevalue);
    // } else {
    //   setutvalgapiobject(Endrelistapiobject);
    // }
    // if (
    //   Object.keys(Endreapiobject).length > 0 &&
    //   Endreapiobject.reoler.length > 0
    // ) {
    //   let reol_id = [];
    //   Endreapiobject.reoler.map((item) => {
    //     reol_id.push(item.reolId);
    //   });
    //   MapRenderFun1(reol_id, "rgba(237, 54, 21, 0.25)");
    // }
    // let tempcart = CartItems;

    // let newcart = tempcart.filter((item) => {
    //   if (Endreapiobject) {
    //     return item.utvalgId != Endreapiobject.utvalgId;
    //   } else {
    //     return item.listId != Endrelistapiobject.listId;
    //   }
    // });
    // if (Endrevalue.listId && Endrevalue.antall) {
    //   setEndreapiobject({});
    //   setEndrelistapiobject(Endrevalue);
    // } else {
    //   setEndrelistapiobject({});
    //   setEndreapiobject(Endrevalue);
    // }
    // forceUpdate();
    // setPage("EndreClick_kw");
  };

  const warninput_1 = () => {
    setmelding(false);
    setmelding1(false);
    let textinput = document.getElementById("warntext1").value;
    setwarninputvalue_1(textinput);
  };
  const AngiClick = async () => {
    if (apnetext == "") {
      setmelding3(true);
      seterrormsg3(
        `Utvalget tilfredstiller ikke kriterier for å kunne lagres.Du må fylle inn «beskrivelse av utvalget»`
      );
    } else if (apnetext.length < 3) {
      setmelding3(true);
      seterrormsg3("Beskrivelse av utvalget må ha minst 3 tegn.");
    } else {
      if (selectionUpdateKW) {
        let updated = await updateUtvalgObj();
        if (updated) {
          setPage("Geogra_distribution_click");
          setPage_P("EndreClick_kw");
        }
      } else {
        setPage("Geogra_distribution_click");
        setPage_P("EndreClick_kw");
      }

      // let saveOldReoler = "false";
      // let skipHistory = "false";
      // let forceUtvalgListId = 0;
      // let name = username_kw === "" ? "Internbruker" : username_kw;
      // let url = `Utvalg/SaveUtvalg?userName=${name}&`;
      // url = url + `saveOldReoler=${saveOldReoler}&`;
      // url = url + `skipHistory=${skipHistory}&`;
      // url = url + `forceUtvalgListId=${forceUtvalgListId}`;
      // try {
      //   let A = {};
      //   if (Object.keys(Endreapiobject).length > 0) {
      //     A = Endreapiobject;
      //   } else {
      //     A = Endrelistapiobject;
      //   }
      //   let ID = "";
      //   if (Object.keys(Endreapiobject).length == 0) {
      //     if (
      //       typeof Endrelistapiobject.listId == "string" &&
      //       Endrelistapiobject.listId.substring(1) === "L"
      //     ) {
      //       ID = Endrelistapiobject.listId.substring(1);
      //       A.listId = ID;
      //     } else {
      //       ID = Endrelistapiobject.listId;
      //       A.listId = ID;
      //     }
      //   } else {
      //     if (
      //       typeof Endreapiobject.utvalgId == "string" &&
      //       Endreapiobject.utvalgId.substring(1) === "U"
      //     ) {
      //       ID = Endreapiobject.utvalgId.substring(1);
      //       A.utvalgId = ID;
      //     } else {
      //       ID = Endreapiobject.utvalgId;
      //       A.utvalgId = ID;
      //     }
      //   }

      //   A.logo = apnetext;
      //   if (Object.keys(Endreapiobject).length > 0) {
      //     Endreapiobject.logo = apnetext;
      //   } else {
      //     Endrelistapiobject.log = apnetext;
      //   }
      //   A.ordreReferanse = "";
      //   A.kundeNavn = name;
      //   A["oldReolMapName"] = "";
      //   if (Object.keys(Endreapiobject).length > 0) {
      //     Endreapiobject["oldReolMapName"] = "";
      //   } else {
      //     Endrelistapiobject["oldReolMapName"] = "";
      //   }
      //   A.modifications.push({
      //     modificationId: Math.floor(100000 + Math.random() * 900000),
      //     userId: "Internbruker",
      //     modificationTime: CurrentDate(),
      //     listId: 0,
      //   });
      //   if (Object.keys(Endreapiobject).length > 0) {
      //     Endreapiobject.ordreReferanse = "";
      //   } else {
      //     Endrelistapiobject.ordreReferanse = "";
      //   }
      //   let kundeNummer = 0;
      //   let avtalenummer = 0;
      //   A["avtalenummer"] = avtaleData ? avtaleData : 0;
      // const { data, status } = await api.postdata(url, A);
      // if (status === 200) {
      //   setPage_P("EndreClick_kw");
      //   if (Object.keys(Endrelistapiobject).length > 0) {
      //     // setEndreapiobject(Endrelistapiobject);
      //     setutvalglistapiobject(Endrelistapiobject);
      //     let Obj = {};
      //     setutvalgapiobject(Obj);
      //   } else {
      //     setutvalgapiobject(Endreapiobject);
      //     let Obj = {};
      //     setutvalglistapiobject(Obj);
      //   }
      //   setPage("Geogra_distribution_click");
      //   // let utvalgID = data.utvalgId;
      //   // setUtvalgID(utvalgID);
      // }

      // } catch (e) {
      //   console.log(e);
      // }
    }
  };
  const GotoMain = () => {
    setPage("");
  };
  const warninput = () => {
    setmelding(false);
    let textinput = document.getElementById("warntext").value;
    setwarninputvalue(textinput);
  };

  const Apnetextbox = () => {
    setmelding3(false);
    let textboxvalue = document.getElementById(
      "uxShowUtvalgDetails_uxForhandlerpaatrykk"
    ).value;
    setapnetext(textboxvalue);
  };
  const Jaclick = async () => {
    try {
      setloading(false);
      let ID = 0;
      let url = "";
      let utvalgType = "";
      // if (
      //   Object.key(Endrelistapiobject).length > 0 &&
      //   Endrelistapiobject.listId
      // ) {
      //   if (
      //     typeof Endrelistapiobject.listId == "string" &&
      //     Endrelistapiobject.listId.substring(1) === "L"
      //   ) {
      //     ID = Endrelistapiobject.listId.substring(1);
      //   } else {
      //     ID = Endrelistapiobject.listId;
      //   }
      //   url = "UtvalgList/DeleteUtvalgList?UtvalgListId=";
      //   utvalgType = "UtvalgList";
      // }
      if (Object.keys(utvalgapiobject).length > 0 && utvalgapiobject.utvalgId) {
        if (
          typeof utvalgapiobject.utvalgId == "string" &&
          utvalgapiobject.utvalgId.substring(1) === "U"
        ) {
          ID = utvalgapiobject.utvalgId.substring(1);
        } else {
          ID = utvalgapiobject.utvalgId;
        }
        url = "Utvalg/DeleteUtvalg?utvalgId=";
        utvalgType = "Utvalg";
      }
      const { data, status } = await api.deletedata(url + ID);
      if (status === 200) {
        if (data === true) {
          setloading(true);
          Swal.fire({
            text: `Utvalg slettet`,
            confirmButtonColor: "#7bc144",
            position: "top",
            icon: "success",
          });
          await getListDetails(data.listId);
          setPage("cartClick_Component_kw");
        }
      } else {
        setloading(true);
        console.error("error : " + status);
      }
    } catch (error) {
      setloading(true);
      // swal("oops! Something Went Wrong!");

      console.error("er : " + error);
    }
  };
  const LagreClick = async () => {
    if (warninputvalue == "") {
      setmelding(true);
      seterrormsg("utvalget må ha minst 3 tegn.");
    }
    // setnomessagediv(false);
    //setPage("Apne_Button_Click")
    const { data, status } = await api.getdata(
      `UtvalgList/UtvalgListNameExists?utvalglistname=${encodeURIComponent(
        warninputvalue
      )}`
    );
    if (status === 200) {
      if (data == true) {
        //datadogLogs.logger.info("UtvalgnameExistsResult", data);
        setmelding(true);
        let msg = `Utvalget ${warninputvalue} eksisterer allerede. Velg et annet utvalgsnavn.`;
        seterrormsg(msg);
      } else {
        let listName = warninputvalue;
        let customerName = "";
        if (customerName) {
          customerName = username_kw;
        } else {
          customerName = "Internbruker";
        }
        let logo = "";
        if (apnetext) {
          logo = apnetext;
        }
        let kundeNummer = 0;
        let avtalenummer = 0;
        kundeNummer = custNos;

        if (avtaleData) {
          avtalenummer = avtaleData;
        }
        // let url = `UtvalgList/AddUtvalgsToNewList?userName=${username_kw}`;
        let url = `UtvalgList/AddUtvalgsToNewList?userName=${customerName}`;

        try {
          var a = {
            listName: warninputvalue,
            customerName: username_kw,
            kundeNummer: kundeNummer,
            avtalenummer: avtalenummer,
            logo: logo,
            // utvalgs: [ActiveUtvalgObject],

            utvalgs:
              Object.keys(Endreapiobject).length > 0
                ? [Endreapiobject]
                : [Endrelistapiobject],
            // memberUtvalgs:
            //   Object.keys(Endreapiobject).length > 0
            //     ? [Endreapiobject]
            //     : [Endrelistapiobject],

            // memberUtvalgs: [ActiveUtvalgObject],
          };
          const { data, status } = await api.postdata(url, a);

          if (status === 200) {
            if (Object.keys(Endreapiobject.length > 0)) {
              Endreapiobject.listId = `${data.listId}`;
              Endreapiobject.listName = `${data.name}`;
            }

            data.utvalgs =
              Object.keys(Endreapiobject).length > 0
                ? [Endreapiobject]
                : [Endrelistapiobject];
            data.memberUtvalgs =
              Object.keys(Endreapiobject).length > 0
                ? [Endreapiobject]
                : [Endrelistapiobject];
            setEndrelistapiobject({});
            setEndrelistapiobject(data);
            btnclose.current.click();
            setnewhome(true);
            setPage("");

            // window.$("#exampleModal-1").modal("dispose");
          }
        } catch (e) {
          //datadogLogs.logger.info("saveutvalgError", e);
          setloading(true);
          setmelding(true);
          seterrormsg("noe gikk galt. vennligst prøv etter en stund");
          console.log(e);
        }
        //    setPage("Geogra_distribution_click")
      }
    }
  };

  const LagreClickKopier = async () => {
    if (warninputvalue_1.length < 3) {
      setmelding1(true);
      seterrormsg1("utvalget må ha minst 3 tegn.");
    } else if (warninputvalue_1.length > 50) {
      setmelding1(true);
      seterrormsg1(
        "Utvalgsnavnet inneholder mer enn 50 tegn. Prøv på nytt med et kortere navn"
      );
    } else {
      setloading(false);
      const { data, status } = await api.getdata(
        `Utvalg/UtvalgNameExists?utvalgNavn=${warninputvalue_1}`
      );
      if (status === 200) {
        if (data === true) {
          seterrormsg1(
            `Utvalget ${warninputvalue_1} eksisterer allerede. Velg et annet utvalgsnavn.`
          );
          setmelding1(true);
          setloading(true);
        } else {
          setloading(false);
          let saveOldReoler = "false";
          let skipHistory = "false";
          let forceUtvalgListId = 0;
          let customername = "";
          if (username_kw) {
            customername = username_kw;
          } else {
            customername = "Internbruker";
          }
          let url = `Utvalg/SaveUtvalg?userName=${customername}&`;
          url = url + `saveOldReoler=${saveOldReoler}&`;
          url = url + `skipHistory=${skipHistory}&`;
          url = url + `forceUtvalgListId=${forceUtvalgListId}`;
          try {
            let A = kundeweb_utvalg();
            A.name = warninputvalue_1;
            // A.kundeNavn = desinput;
            A.totalAntall = Antallvalue;
            A.modifications.push({
              modificationId: Math.floor(100000 + Math.random() * 900000),
              userId: customername,
              modificationTime: CurrentDate(),
              listId: 0,
            });
            A.criterias = utvalgapiobject.criterias;
            A.receivers = utvalgapiobject.receivers;
            A.reoler = utvalgapiobject.reoler;
            A.Antall = utvalgapiobject.Antall;
            A.logo = desinput ? desinput : utvalgapiobject.logo;
            A["kundeNummer"] = custNos;
            A["avtalenummer"] = avtaleData ? avtaleData : 0;
            // A.antallBeforeRecreation = 0;
            // A.reolerBeforeRecreation = [];
            const { data, status } = await api.postdata(url, A);
            if (status === 200) {
              if (desinput) {
                setapnetext(data.logo);
              }
              mapView.graphics.items.forEach(function (item) {
                if (item?.attributes?.utvalgid !== undefined) {
                  // previousUtvalg = true;
                  mapView.graphics.remove(item);
                }
              });
              setSelectionUpdateKW(false);
              setutvalgname(warninputvalue_1);
              setutvalglistapiobject([]);
              await createActiveUtvalg(data);
              setPage("Apne_Button_Click");

              kopiermodal.current.click();
              $("body").removeClass("modal-open");
              setloading(true);
            }
          } catch (e) {
            console.log(e);
          }
        }
      } else {
        setloading(true);
        setmelding1(true);
        seterrormsg1("Something went wrong");
      }
    }
  };

  const createActiveUtvalg = async (selectedDataSet) => {
    setShowHouseHold(false);
    setShowBusiness(false);
    let routes = await getSelectedRoutes(selectedDataSet.reoler);
    let Antall = getAntall(routes);

    var a = Utvalg();
    a.name = selectedDataSet.name;
    a.reoler = selectedDataSet.reoler;
    a.Antall = Antall;
    a.Business = Antall[1];
    a.ReservedHouseHolds = Antall[2];
    a.hush = Antall[0];

    a.hasReservedReceivers = selectedDataSet.hasReservedReceivers;
    a.oldReolMapMissing = selectedDataSet.oldReolMapMissing;
    a.reolerBeforeRecreation = selectedDataSet.reolerBeforeRecreation;
    a.utvalgId = selectedDataSet.utvalgId;
    a.changed = selectedDataSet.changed;
    a.kundeNavn = selectedDataSet.selectedDataSet;
    a.logo = selectedDataSet.logo;
    a.reolMapName = selectedDataSet.reolMapName;
    a.oldReolMapName = selectedDataSet.oldReolMapName;
    a.skrivebeskyttet = selectedDataSet.skrivebeskyttet;
    a.weight = selectedDataSet.weight;
    a.distributionType = selectedDataSet.distributionType;
    a.distributionDate = selectedDataSet.distributionDate;
    a.arealAvvik = selectedDataSet.arealAvvik;
    a.isBasis = selectedDataSet.isBasis;
    a.basedOn = selectedDataSet.basedOn;
    a.basedOnName = selectedDataSet.basedOnName;
    a.wasBasedOn = selectedDataSet.wasBasedOn;
    a.wasBasedOnName = selectedDataSet.wasBasedOnName;
    a.list = selectedDataSet.list;
    a.listId = selectedDataSet.listId;
    a.antallBeforeRecreation = selectedDataSet.antallBeforeRecreation;
    a.totalAntall = selectedDataSet.totalAntall;
    a.ordreReferanse = selectedDataSet.ordreReferanse;
    a.ordreType = selectedDataSet.ordreType;
    a.ordreStatus = selectedDataSet.ordreStatus;
    a.kundeNummer = selectedDataSet.kundeNummer;
    a.innleveringsDato = selectedDataSet.innleveringsDato;
    a.avtalenummer = selectedDataSet.avtalenummer;
    a.thickness = selectedDataSet.thickness;
    a.antallWhenLastSaved = selectedDataSet.antallWhenLastSaved;
    a.modifications = selectedDataSet.modifications;
    a.kommuner = selectedDataSet.kommuner;
    a.receivers = selectedDataSet.receivers;
    a.districts = selectedDataSet.districts;
    a.postalZones = selectedDataSet.postalZones;
    a.criterias = selectedDataSet.criterias;
    a.utvalgsBasedOnMe = selectedDataSet.utvalgsBasedOnMe;

    setutvalgapiobject(a);
    if (Object.keys(a).length !== 0 && a.receivers.length > 0) {
      a.receivers.map((item) => {
        if (item.receiverId === 1 && item.selected === true) {
          setShowHouseHold(true);
        }
        if (item.receiverId === 4 && item.selected === true) {
          setShowBusiness(true);
        }
      });
    }

    if (a.antallBeforeRecreation && a.antallBeforeRecreation > 0) {
      let msg = `Budruteendringer har påvirket utvalgene`;
      Swal.fire({
        text: msg,
        confirmButtonColor: "#7bc144",
        confirmButtonText: "Lukk",
        position: "top",
      });
    }
  };

  const updateUtvalgObj = async (e) => {
    let customername = "";
    if (username_kw) {
      customername = username_kw;
    } else {
      customername = "Internbruker";
    }
    let updated = false;
    setUtvalgUpdateLoading(true);
    let saveOldReoler = "false";
    let skipHistory = "false";
    let forceUtvalgListId = 0;
    let url = `Utvalg/SaveUtvalg?userName=${customername}&`;
    url = url + `saveOldReoler=${saveOldReoler}&`;
    url = url + `skipHistory=${skipHistory}&`;

    url = url + `forceUtvalgListId=${forceUtvalgListId}`;
    try {
      utvalgapiobject.antallBeforeRecreation = 0;
      utvalgapiobject.reolerBeforeRecreation = [];
      utvalgapiobject.modifications.push({
        modificationId: Math.floor(100000 + Math.random() * 900000),
        userId: customername,
        modificationTime: CurrentDate(),
        listId: 0,
      });

      const { data, status } = await api.postdata(url, utvalgapiobject);
      if (status === 200) {
        updated = true;
        mapView.graphics.items.forEach(function (item) {
          if (item.attributes.utvalgid !== undefined) {
            // previousUtvalg = true;
            mapView.graphics.remove(item);
          }
        });
        getListDetails(data.listId);
        let newCart = [];
        CartItems?.map((item) => {
          if (item.utvalgId !== data?.utvalgId) {
            newCart.push(item);
          }
        });
        newCart.push(data);
        setCartItems(newCart);
        findRouteUpdate(newCart);
        setSelectionUpdateKW(false);
        EndreClick(utvalgapiobject, newCart);
        setutvalgapiobject(data);
        setEndreapiobject(data);
        setUtvalgUpdateLoading(false);
      }
    } catch (error) {
      console.error("error : " + error);
      setUtvalgUpdateLoading(false);
    }
    return updated;
  };

  const getListDetails = async (id) => {
    let url = `UtvalgList/GetUtvalgList?listId=${id}&getParentList=${false}&getMemberUtvalg=${true}`;
    try {
      const { data, status } = await api.getdata(url);
      if (data.length === 0) {
      } else {
        setutvalglistapiobject(data);
      }
    } catch (error) {}
  };

  const getSelectedRoutes = (data) => {
    let selectedArray = [];
    let selectedRoutes = data.reduce((acc, dt) => {
      if (!(dt.children === undefined)) {
        return acc.concat(getSelectedRoutes(dt.children));
      } else {
        selectedArray.push(dt.key);
      }
      return acc.concat(dt);
    }, []);
    // setSelectedKeys(selectedArray);
    return selectedRoutes;
  };
  const getAntall = (routes) => {
    let houseAntall = 0;
    let businessAntall = 0;
    let householdsReservedAntall = 0;

    routes.map((item) => {
      houseAntall = houseAntall + item.antall.households;
      businessAntall = businessAntall + item.antall.businesses;
      householdsReservedAntall =
        householdsReservedAntall + item.antall.householdsReserved;
    });
    return [houseAntall, businessAntall, householdsReservedAntall];
  };

  useEffect(async () => {
    setSelectionUpdateKW(false);
    setActiveMapButton("");
    mapView.activeTool = null;
    let totalBeforeCreation = 0;
    let totalWhenSaved = 0;
    let flag = 0;
    CartItems?.map((item) => {
      if (item.antallBeforeRecreation > 0) {
        flag = 1;
      }
      totalBeforeCreation = totalBeforeCreation + item?.antallBeforeRecreation;
      totalWhenSaved = totalWhenSaved + item?.antallWhenLastSaved;
    });
    if (flag) {
      setRoutepdate(true);
      setAntallBeforeRecreationSum(totalBeforeCreation);
      setAntallWhenSavedSum(totalWhenSaved);
    }
    EndreClick(utvalgapiobject, CartItems);
    // if (
    //   Endreapiobject.antall == undefined ||
    //   typeof Endreapiobject.antall == undefined ||
    //   Endreapiobject.antall == "" ||
    //   Endreapiobject.listId == ""
    // ) {
    //   let url = `Utvalg/GetUtvalg?utvalgId=${utvalgapiobject.utvalgId}`;
    //   try {
    //     //api.logger.info("APIURL", url);
    //     const { data, status } = await api.getdata(url);
    //     if (data.length == 0) {
    //       // api.logger.error(
    //       //   "Error : No Data is present for mentioned Id" +
    //       //     Endreapiobject.utvalgId
    //       // );
    //     } else {
    //       await createActiveUtvalg(data);
    //     }
    //   } catch (error) {
    //     //api.logger.error("errorpage API not working");
    //     //api.logger.error("error : " + error);
    //   }
    // } else {
    //   let url =
    //     // `UtvalgList/GetUtvalgList?listId=${id}&getParentList=${true}&getMemberUtvalg=${true}`;
    //     `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${Endrelistapiobject.listId}`;

    //   try {
    //     //api.logger.info("APIURL", url);
    //     const { data, status } = await api.getdata(url);
    //     if (data.length == 0) {
    //       // api.logger.error(
    //       //   "Error : No Data is present for mentioned Id" +
    //       //     Endrelistapiobject.listId
    //       // );
    //     } else {
    //       let obj = await CreateUtvalglist(data);
    //       setEndrelistapiobject(obj);

    //       // setutvalglistcheck(true);
    //     }
    //   } catch (error) {
    //     //api.logger.error("errorpage API not working");
    //     //api.logger.error("error : " + error);
    //   }
    // }
  }, []);

  const findRouteUpdate = (cartList) => {
    let flag = 0;
    cartList?.map((item) => {
      if (item.antallBeforeRecreation > 0) {
        flag = 1;
      }
    });
    if (flag) {
      setRoutepdate(true);
    } else {
      setRoutepdate(false);
    }
  };

  const desinputonchange = () => {
    let desctextvalue = document.getElementById("desctext").value;
    setdesinput(desctextvalue);
  };

  const kopiereUtvalget = () => {
    setKopiereUtvalgetName("kopiereUtvalget");
  };
  const skrivUtvalget = () => {
    setSkrivUtvalgetName("skrivUtvalget");
  };

  const showVisDetaljer = () => {
    let reoler = utvalgapiobject.reoler;
    let reolerBeforeRecreation = utvalgapiobject.reolerBeforeRecreation;
    let reolids = [];
    let result = reoler.filter((o1) =>
      reolerBeforeRecreation.map((o2) => {
        if (o1.reolId === o2.reolId) {
          reolids.push(o1.reolId);
          o1["businessesOld"] = o2.antall.businesses;
          o1["householdsOld"] = o2.antall.households;
        }
      })
    );

    let restReoler = [];
    let resultBeforeCreationObject = reolerBeforeRecreation.filter((item) => {
      if (reolids.includes(item.reolId)) {
        return item;
      } else {
        restReoler.push(item);
      }
    });
    setResultBeforeCreationObject(resultBeforeCreationObject);
    setResultRestReoler(restReoler);
    let restCurrentReoler = [];
    let resultAfterCreationObject = reoler.filter((item) => {
      if (reolids.includes(item.reolId)) {
        return item;
      } else {
        restCurrentReoler.push(item);
      }
    });
    setResultAfterRuteCreation(resultAfterCreationObject);

    setResultAfterCreationObject(restCurrentReoler);
    setCommonReoler(result);
    setVisDetaljerModalName("VisDetaljer");
  };

  const showVisKart = async () => {
    let previousUtvalg = false;
    // remove previous old utvalg feature
    mapView.graphics.items.forEach(function (item) {
      if (item.attributes.utvalgid !== undefined) {
        previousUtvalg = true;
        mapView.graphics.remove(item);
      }
    });

    if (!previousUtvalg) {
      let utvalgId = utvalgapiobject?.utvalgId;

      let queryObject = new Query();

      queryObject.where = `utvalgid =` + utvalgId;
      queryObject.returnGeometry = true;
      queryObject.outFields = ["utvalgid"];

      await query
        .executeQueryJSON(MapConfig.oldUtvalgGeometryUrl, queryObject)
        .then(function (results) {
          if (results.features.length > 0) {
            let selectedSymbol = {
              type: "simple-fill", // autocasts as new SimpleFillSymbol()
              color: [51, 51, 51, 0.75],
              style: "backward-diagonal",
              outline: {
                // autocasts as new SimpleLineSymbol()
                color: [18, 12, 12],
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

  return (
    <div className="col-5 p-2">
      <div>{utvalgUpdateLoading ? <Spinner /> : null}</div>
      <div
        className="padding_NoColor_B cursor"
        onClick={() => {
          setPage("cartClick_Component_kw");
        }}
      >
        <a id="uxHandlekurvSmall_uxLnkbtnHandlekurv">
          <div className="handlekurv handlekurvText pl-2">
            Du har {CartItems.length} utvalg i bestillingen din.
          </div>
        </a>
      </div>
      {melding3 ? <br /> : null}
      {melding3 ? (
        <div className="pr-3">
          <div className="error WarningSign">
            <div className="divErrorHeading">Melding:</div>
            <p id="uxKjoreAnalyse_uxLblMessage" className="divErrorText_kw">
              {apnetext == "" ? (
                <p>
                  {" "}
                  Utvalget tilfredstiller ikke kriterier for å kunne lagres.
                  <br />
                  Du må fylle inn «beskrivelse av utvalget»
                </p>
              ) : (
                { errormsg }
              )}
            </p>
          </div>
        </div>
      ) : null}
      {melding3 ? (
        <div>
          <p></p>
        </div>
      ) : null}

      <div className="padding_Color_L_R_T_B">
        <div className="AktivtUtvalg">
          <div className="AktivtUtvalgHeading">
            <span
              id="uxShowUtvalgDetails_uxLblSavedUtvalgName"
              className="wordbreak"
            >
              {utvalgapiobject?.name}
            </span>

            {/* <span id="uxShowUtvalgDetails_uxLblSavedUtvalgName">{  (Endreapiobject[0].name)+new Date().toUTCString().replace("GMT","") ||  (Endreapiobject[0].utvalgName)+new Date().toUTCString().replace("GMT","")}</span> */}
          </div>

          <div>
            <div className="gray">
              ID:
              <span id="uxShowUtvalgDetails_uxRefNr" className="gray">
                {utvalgapiobject?.utvalgId}
              </span>
            </div>
          </div>
        </div>
        {utvalgapiobject.ordreType === 1 ? (
          <div className="pr-3">
            <div className="error WarningSign">
              <div className="divErrorHeading">Melding:</div>
              <p id="uxKjoreAnalyse_uxLblMessage" className="divErrorText_kw">
                Valgt bestilling har allerede et ordrenummer. Du kan kopiere
                bestillingen.
              </p>
            </div>
          </div>
        ) : null}
        {utvalgapiobject.antallBeforeRecreation > 0 ? (
          <div className="">
            <table>
              <tbody>
                <tr>
                  <th style={{ width: "219px", padding: "0px" }}>&nbsp;</th>
                  <th className="yellowcoding">Før</th>
                  <th className="coding">Nå</th>
                </tr>
                <th></th>
                <tr>
                  <td className="hush">Husholdninger</td>
                  <td className="yellowcoding">
                    {NumberFormat(utvalgapiobject.antallBeforeRecreation)}
                  </td>
                  <td className="coding">
                    {NumberFormat(utvalgapiobject.totalAntall)}
                  </td>
                </tr>
                <th></th>
                <tr style={{ fontWeight: "bold" }}>
                  <td className="boldnumber">&nbsp;</td>
                  <td className="yellowcoding">
                    {" "}
                    {NumberFormat(utvalgapiobject.antallBeforeRecreation)}
                  </td>
                  <td className="boldnumber-one">
                    {NumberFormat(utvalgapiobject.totalAntall)}
                  </td>
                  {/* </tr></tbody></table> */}
                </tr>
              </tbody>
            </table>
            <p></p>
            <div
              className="sok-text blue cursor-pointer"
              data-toggle="modal"
              data-target="#visdetaljer"
              onClick={showVisDetaljer}
            >
              Vis detaljert oversikt over endringer per budrute
            </div>
            <p></p>
            <div className="sok-text blue cursor-pointer" onClick={showVisKart}>
              Vis/Skjul gammelt område i kartet
            </div>
          </div>
        ) : utvalgapiobject.totalAntall >= 0 ? (
          <div>
            <div className="_flex-column">
              {showHouseHold && utvalgapiobject?.Antall !== undefined ? (
                <div>
                  <label className="form-check-label label-text" htmlFor="Hush">
                    {" "}
                    Husholdninger{" "}
                  </label>
                  <span className="divValueTextBold div_right">
                    {NumberFormat(utvalgapiobject?.Antall[0])}
                  </span>
                </div>
              ) : null}
              {showBusiness && utvalgapiobject?.Antall !== undefined ? (
                <div>
                  <label className="form-check-label label-text" htmlFor="Hush">
                    {" "}
                    Virksomheter{" "}
                  </label>
                  <span className="divValueTextBold div_right">
                    {NumberFormat(utvalgapiobject?.Antall[1])}
                  </span>
                </div>
              ) : null}
            </div>
            <div className="underline_kw p-0"></div>
            <span className="divValueTextBold div_right">
              {NumberFormat(utvalgapiobject.totalAntall)}
            </span>
          </div>
        ) : null}
        {visDetaljerModalName == "VisDetaljer" ? (
          <VisDetaljerModal_KW
            title={"BUDRUTEENDRINGER"}
            id={"visdetaljer"}
            ResultBeforeCreationObject={ResultBeforeCreationObject}
            ResultAfterRuteCreation={ResultAfterRuteCreation}
            ResultAfterCreationObject={ResultAfterCreationObject}
            oldReoler={resultRestReoler}
          />
        ) : null}

        <div
          className="modal fade bd-example-modal-lg"
          id="exampleModal"
          tabIndex="-1"
          role="dialog"
          aria-labelledby="exampleModalCenterTitle"
          aria-hidden="true"
        >
          <div className="modal-dialog" role="document">
            <div className="modal-content">
              <div className="Common-modal-header">
                <div className="divDockedPanel">
                  <div className=" divDockedPanelTop">
                    <span className="dialog-kw" id="exampleModalLabel">
                      Advarsel
                    </span>
                    <button
                      type="button"
                      className="close size1"
                      data-dismiss="modal"
                      aria-label="Close"
                    >
                      <span className="size1" aria-hidden="true">
                        &times;
                      </span>
                    </button>
                  </div>
                  <div className="View_modal-body pl-2">
                    <table>
                      <tbody>
                        <tr>
                          <td>
                            <p className="p-slett">
                              &nbsp; Skal utvalget slettes?
                            </p>{" "}
                          </td>
                          <td></td>
                        </tr>

                        <tr>
                          <td>
                            <div className="ml-4">
                              <button
                                type="button"
                                className="modalMessage_button"
                                data-dismiss="modal"
                              >
                                Nei
                              </button>
                              {/* </td>
        <td></td>
        <td> */}{" "}
                              &nbsp;&nbsp;
                              <button
                                type="button"
                                onClick={Jaclick}
                                className="modalMessage_button"
                                data-dismiss="modal"
                              >
                                Ja
                              </button>
                            </div>
                          </td>
                        </tr>
                        <br />
                      </tbody>
                    </table>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* savedbeforeselection is greater thean zero */}

        <div
          className="modal fade bd-example-modal-lg"
          id="antallbeforecreationexists"
          tabIndex="-1"
          role="dialog"
          data-backdrop="false"
          aria-labelledby="exampleModalCenterTitle"
          aria-hidden="true"
        >
          <div className="modal-dialog" role="document">
            <div className="modal-content">
              <div className="Common-modal-header">
                <div className="divDockedPanel">
                  <div className=" divDockedPanelTop">
                    <span className="dialog-kw" id="exampleModalLabel">
                      Advarsel
                    </span>
                    <button
                      type="button"
                      className="close size1"
                      data-dismiss="modal"
                      aria-label="Close"
                    >
                      <span className="size1" aria-hidden="true">
                        &times;
                      </span>
                    </button>
                  </div>
                  <div className="View_modal-body pl-2">
                    <table>
                      <tbody>
                        <tr>
                          <td>
                            <p className="p-slett">
                              &nbsp;Budruteendringer har påvirket utvalg med
                              navn: {Endreapiobject.name}.
                            </p>{" "}
                          </td>
                          <td></td>
                        </tr>

                        <br />
                      </tbody>
                    </table>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* modal ends */}
        <img
          src={loadingImage}
          style={{
            width: "20px",
            height: "20px",
            display: loading ? "none" : "block",
            position: "absolute",
            top: "170px",
            left: "250px",
            zindex: 100,
          }}
        />

        <div
          className="modal fade  bd-example-modal-lg modal-legg"
          id="exampleModal-1"
          tabIndex="-1"
          data-backdrop="false"
          role="dialog"
          aria-labelledby="exampleModalCenterTitle"
          aria-hidden="true"
        >
          <div className="modal-dialog" role="document">
            <div className="modal-content">
              <div className="">
                {/* <div className=""> */}
                <div className=" divDockedPanelTop">
                  <span className="dialog-kw" id="exampleModalLabel">
                    ANGI ET FELLESNAVN PÅ BESTILLINGEN
                  </span>
                  <button
                    type="button"
                    className="close pr-2"
                    data-dismiss="modal"
                    aria-label="Close"
                    ref={btnclose}
                  >
                    <span aria-hidden="true">&times;</span>
                  </button>
                </div>
                <div className="View_modal-body-appneet pl-2">
                  {!melding ? <p></p> : null}
                  {melding ? (
                    <div className="pr-3">
                      <div className="error WarningSign">
                        <div className="divErrorHeading">Melding:</div>
                        <p
                          id="uxKjoreAnalyse_uxLblMessage"
                          className="divErrorText_kw"
                        >
                          `{warninputvalue}` er allerede blitt benyttet,
                          vennligst velg et nytt navn på bestillingen.
                        </p>
                      </div>
                    </div>
                  ) : null}
                  {melding ? (
                    <div>
                      <p></p>
                    </div>
                  ) : null}

                  <input
                    type="text"
                    maxLength="50"
                    // placeholder="Navn på bestil"
                    value={warninputvalue}
                    onChange={warninput}
                    id="warntext"
                    className="inputwidth"
                  />

                  <p></p>
                  <div className="">
                    <div className="div_left">
                      <input
                        type="submit"
                        name="DemografiAnalyse1$uxFooter$uxBtForrige"
                        value="Avbryt"
                        onClick={AvbrytClick}
                        data-dismiss="modal"
                        className="KSPU_button_Gray"
                      />
                      &nbsp;&nbsp;
                      <input
                        type="submit"
                        name="uxDistribusjon$uxDistSetDelivery"
                        value="Lagre"
                        //need to check
                        onClick={LagreClick}
                        id="uxDistribusjon_uxDistSetDelivery"
                        className="KSPU_button-kw"
                        data-dismiss="modal"
                      />
                    </div>
                    <div className="padding_NoColor_T clearFloat sok-text">
                      Du kan senere finne igjen bestillingen ved å klikke <br />
                      <a className="read-more_ref" href="javscript:">
                        ÅPNE ET LAGRET UTVALG{" "}
                      </a>{" "}
                      på forsiden
                    </div>
                  </div>
                  <br />
                </div>
              </div>
            </div>
          </div>
        </div>

        <div
          className="modal fade bd-example-modal-lg"
          id="exampleModal"
          tabIndex="-1"
          role="dialog"
          aria-labelledby="exampleModalCenterTitle"
          aria-hidden="true"
        >
          <div className="modal-dialog" role="document">
            <div className="modal-content">
              <div className="Common-modal-header">
                <div className="divDockedPanel">
                  <div className=" divDockedPanelTop">
                    <span className="dialog-kw" id="exampleModalLabel">
                      Advarsel
                    </span>
                    <button
                      type="button"
                      className="close"
                      data-dismiss="modal"
                      aria-label="Close"
                    >
                      <span aria-hidden="true">&times;</span>
                    </button>
                  </div>
                  <div className="View_modal-body pl-2">
                    <table>
                      <tbody>
                        <tr>
                          <td>
                            <p className="p-slett">
                              &nbsp; Skal utvalget slettes?
                            </p>{" "}
                          </td>
                          <td></td>
                        </tr>

                        <tr>
                          <td>
                            <div className="ml-4">
                              <button
                                type="button"
                                className="modalMessage_button"
                                data-dismiss="modal"
                              >
                                Nei
                              </button>
                              {/* </td>
        <td></td>
        <td> */}{" "}
                              &nbsp;&nbsp;
                              <button
                                type="button"
                                // onClick={""}
                                className="modalMessage_button"
                                data-dismiss="modal"
                              >
                                Ja
                              </button>
                            </div>
                          </td>
                        </tr>
                        <br />
                      </tbody>
                    </table>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div className="padding_NoColor_T clearFloat">
          <div className="bold">Beskrivelse av utvalget</div>
          <div>
            <input
              type="text"
              value={apnetext}
              onChange={Apnetextbox}
              maxLength="50"
              id="uxShowUtvalgDetails_uxForhandlerpaatrykk"
              className="tablewidth divValueText"
            />
          </div>
          <div className="gray">
            Gi en beskrivelse som gjør det lett å identifisere korrekt sending;
            skriv inn avsender og evt kjennetegn ved sendingen. Denne
            beskrivelsen vil du også finne igjen når pakningsmateriell skal
            produseres.
          </div>
        </div>
        <br />

        <div className="_flex-row">
          <div
            className={
              utvalgapiobject.ordreType === 1 ? "invisible" : "_flex-start"
            }
          >
            <a
              className="KSPU_LinkButton_Url_KW pl-2"
              data-toggle="modal"
              data-target="#exampleModal"
            >
              Slett utvalget
            </a>
          </div>
          <div className="_flex-end" style={{ flex: 1 }}>
            <input
              type="submit"
              name="saveUtvalgLagreEndringer"
              value="Lagre endringer"
              id="saveSelectionFromList"
              onClick={updateUtvalgObj}
              className={
                (utvalgapiobject?.antallBeforeRecreation > 0 ||
                  selectionUpdateKW) &&
                utvalgapiobject.ordreType !== 1
                  ? "_flex-end KSPU_button-kw"
                  : "invisible"
              }
            />
          </div>
        </div>
      </div>
      <br />
      <br />
      <br />
      <div className="titleWizard padding_NoColor_B">
        "
        <span
          id="uxShowUtvalgListDetails_uxLblListName"
          className="green wordbreak"
        >
          {utvalglistapiobject.name}
        </span>
        " har{" "}
        <span
          id="uxShowUtvalgListDetails_uxLblListCountUtvalg"
          className="green"
        >
          {CartItems.length}
          {/* {utvalgitemdisplay
                  ? LeggTilCheckedItems[0].memberUtvalgs.length > 0
                    ? LeggTilCheckedItems[0].memberUtvalgs.length + 1
                    : LeggTilCheckedItems.length + 1
                  : 1} */}
        </span>{" "}
        utvalg
      </div>
      <div className="gray">
        ID:{" "}
        <span id="uxShowUtvalgListDetails_uxLblRefNr" className="gray">
          {"L" + utvalglistapiobject.listId}
        </span>
      </div>
      {routeUpdate ? (
        <div>
          <div className="table-container">
            <table
              cellSpacing="0"
              rules="rows"
              border="0"
              id="uxShowUtvalgListDetails_uxHandlekurv_uxGrdMemberUtv"
              className="cellcollapse"
            >
              <tbody>
                <tr>
                  <th></th>
                  <th></th>
                  <th></th>
                  <th></th>
                  <th
                    style={{ textAlign: "end" }}
                    className="yellowcoding pr-1"
                  >
                    Før
                  </th>
                  <th style={{ textAlign: "end" }} className="coding pr-1">
                    Nå
                  </th>
                </tr>

                {CartItems.filter(
                  (item) => item.utvalgId !== utvalgapiobject?.utvalgId
                ).length > 0
                  ? CartItems.filter(
                      (item) => item.utvalgId !== utvalgapiobject?.utvalgId
                    ).map((item, index) => (
                      <tr
                        className="GridView_Row_kw cart-table-endre-tr"
                        key={index}
                      >
                        <td align="center">
                          <img
                            id="uxShowUtvalgListDetails_uxHandlekurv_uxGrdMemberUtv_ctl02_uxKartSymbol"
                            src={imagePath(index + 1)}
                            className="imgstyle"
                          />
                        </td>
                        <td className="padding-horizontal table-text">
                          <a
                            className="sok-text wordbreak"
                            style={{
                              wordBreak: "normal",
                              cursor: "pointer",
                            }}
                            onClick={() => EndreClick(item, CartItems)}
                          >
                            {item.name}
                          </a>
                        </td>
                        <td className="padding-horizontal">
                          <a
                            className="KSPU_LinkButton_Url sok-text custom_link"
                            onClick={() => EndreClick(item, CartItems)}
                          >
                            <div>Endre</div>
                          </a>
                        </td>
                        <td>
                          <img
                            className={
                              item?.antallBeforeRecreation > 0
                                ? "float-right pt-1 mr-1 warningLogoSmall warning-sign"
                                : "hidden"
                            }
                            src={warningImg}
                          />
                        </td>
                        <td
                          align="right"
                          className={
                            item.antallBeforeRecreation > 0
                              ? "yellowcoding tdwidth small-text"
                              : "tdwidth small-text"
                          }
                          style={{ textAlign: "end", width: "15%" }}
                        >
                          {item.antallBeforeRecreation > 0
                            ? NumberFormat(item.antallBeforeRecreation)
                            : null}
                        </td>
                        <td
                          align="right"
                          className="tdwidth small-text"
                          style={{ textAlign: "end", width: "15%" }}
                        >
                          {NumberFormat(item.totalAntall)}
                        </td>
                      </tr>
                    ))
                  : null}
              </tbody>
            </table>
          </div>

          <table className="tdwidth1">
            <tbody>
              <tr className="bold">
                <td className="sok-text-1">
                  Totalt, alle utvalg i bestillingen
                </td>
                <td></td>
                <td></td>
                <td></td>
                <td
                  style={{ textAlign: "end", width: "15%" }}
                  className="yellowcoding"
                >
                  {NumberFormat(antallBeforeRecreationSum)}
                </td>
                <td
                  style={{ textAlign: "end", width: "15%" }}
                  className="coding"
                >
                  {NumberFormat(antallWhenSavedSum)}
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      ) : (
        <div>
          <div className="table-container">
            <table
              id="uxShowUtvalgListDetails_uxHandlekurv_uxGrdMemberUtv"
              className="tablestyle"
            >
              <tbody>
                {CartItems.length > 0
                  ? CartItems.filter(
                      (item) => item.utvalgId !== utvalgapiobject?.utvalgId
                    ).length > 0
                    ? CartItems.filter(
                        (item) => item.utvalgId !== utvalgapiobject?.utvalgId
                      ).map((item, index) => (
                        <tr
                          className="GridView_Row_kw cart-table-endre-tr"
                          key={index}
                        >
                          <td align="center">
                            <img
                              id="uxShowUtvalgListDetails_uxHandlekurv_uxGrdMemberUtv_ctl02_uxKartSymbol"
                              src={imagePath(index + 1)}
                              className="imgstyle"
                            />
                          </td>
                          <td>
                            <a
                              className="table-text wordbreak"
                              onClick={() => EndreClick(item, CartItems)}
                            >
                              {item.name}
                            </a>
                          </td>
                          <td className="padding-horizontal">
                            <a
                              className="KSPU_LinkButton_Url sok-text"
                              onClick={() => EndreClick(item, CartItems)}
                              // style={loading?{display:'none'}:null}
                            >
                              <div>Endre</div>
                            </a>
                          </td>
                          <td>
                            <div
                              className={
                                item.colorCode !== undefined &&
                                item.reoler?.length > 0
                                  ? "pl-3"
                                  : "col"
                              }
                              style={
                                item.colorCode !== undefined &&
                                item.reoler?.length > 0
                                  ? {
                                      display: "flex",
                                      justifyContent: "center",
                                      alignItems: "center",
                                    }
                                  : {}
                              }
                            >
                              <img
                                className={
                                  item.colorCode !== undefined &&
                                  item.reoler?.length > 0
                                    ? " warningLogoSmall"
                                    : "hidden"
                                }
                                src={warningImg}
                              />
                            </div>
                          </td>

                          {item.antall ? (
                            <td align="right" className="tdwidth small-text">
                              {NumberFormat(item.antall)}
                            </td>
                          ) : (
                            <td align="right" className="tdwidth small-text">
                              {NumberFormat(item.totalAntall)}

                              {/* {NumberFormat(item.totalAntall)} */}
                            </td>
                          )}
                        </tr>
                      ))
                    : null
                  : null}
              </tbody>
            </table>
          </div>
          <table className="tdwidth1">
            <tbody>
              <tr className="bold">
                <td className="sok-text-1">
                  Totalt, alle utvalg i bestillingen
                </td>

                <td className="tdwidth2">
                  <span id="uxShowUtvalgListDetails_uxHandlekurv_uxLblTotAnt">
                    {/* {!utvalgitemdisplay
                            ? NumberFormat(utvalglistapiobject.antall)
                            : NumberFormat(Total)} */}
                    {NumberFormat(utvalgapiobject.totalAntall + Total)}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      )}

      <br />
      <br />
      <div className="_flex-row">
        <div className="_flex-start-top">
          <a className="KSPU_LinkButton_Url_KW pl-2" onClick={GotoMain}>
            Avbryt
          </a>
        </div>
        <div className="_flex-end" style={{ flex: 1 }}>
          <div className="_flex-column">
            <div
              className={
                utvalgapiobject?.antallBeforeRecreation !== 0 ||
                (utvalgapiobject.ordreType === 1 && utvalgapiobject.utvalgId)
                  ? "invisible"
                  : "_flex-end mt-1 mb-1"
              }
            >
              <input
                type="submit"
                name="uxDistribusjon$uxDistSetDelivery"
                value="Angi distribusjonsdetaljer"
                id="uxDistribusjon_uxDistSetDelivery"
                onClick={AngiClick}
                className="KSPU_button-kw"
                style={{ width: "175px" }}
              />
            </div>
            <div
              className={
                utvalgapiobject?.antallBeforeRecreation !== 0 ||
                (utvalgapiobject.ordreType === 1 && utvalgapiobject.utvalgId)
                  ? "invisible"
                  : "_flex-end mt-1 mb-1"
              }
            >
              <input
                type="submit"
                name="DemografiAnalyse1$uxFooter$uxBtForrige"
                value="Legg til flere utvalg"
                onClick={LeggTillClick}
                // data-toggle={Modal ? "modal" : ""}
                // data-target={Modal ? "#exampleModal-1" : ""}
                id="leggtill"
                className="KSPU_button_Gray"
              />
            </div>
            <div
              className={
                utvalgapiobject.ordreType === 1
                  ? "_flex-end mt-1 mb-1"
                  : "invisible"
              }
            >
              <input
                type="submit"
                name="saveUtvalgLagreEndringer"
                value="Lagre kopi"
                id="saveSelectionFromList"
                data-toggle="modal"
                data-target="#kopiereUtvalget"
                onClick={kopiereUtvalget}
                class="KSPU_button-kw"
              />
            </div>
          </div>
        </div>
      </div>
      {/* <div
        className={
          utvalgapiobject?.antallBeforeRecreation === 0
            ? "div_right"
            : "invisible"
        }
      >
        <input
          type="submit"
          name="uxDistribusjon$uxDistSetDelivery"
          value="Angi distribusjonsdetaljer"
          id="uxDistribusjon_uxDistSetDelivery"
          onClick={AngiClick}
          className="KSPU_button-kw"
          style={{ width: "175px" }}
        />
      </div>
      <div className="div_left">
        <a className="KSPU_LinkButton_Url_KW pl-2" onClick={GotoMain}>
          Avbryt
        </a>
      </div>
      <br />
      <br />
      <div
        className={
          utvalgapiobject?.antallBeforeRecreation === 0
            ? "div_right"
            : "invisible"
        }
      >
        <input
          type="submit"
          name="DemografiAnalyse1$uxFooter$uxBtForrige"
          value="Legg til flere utvalg"
          onClick={LeggTillClick}
          // data-toggle={Modal ? "modal" : ""}
          // data-target={Modal ? "#exampleModal-1" : ""}
          id="leggtill"
          className="KSPU_button_Gray"
        />
      </div> */}

      <div className="div_left">
        <span className="bold">Du kan også..</span>

        <br />
      </div>
      <br />

      <br />
      {/* <div
        className="modal show"
        id="kopiereUtvalget"
        tabIndex="-1"
        role="dialog"
        aria-labelledby="exampleModalCenterTitle"
        aria-hidden="true"
        data-backdrop="false"
      >
        <div className="modal-dialog modal-dialog-centered " role="document">
          <div className="modal-content">
            <div className="">
              <div className=" divDockedPanelTop">
                <span className="dialog-kw" id="exampleModalLabel">
                  LAGRE UTVALG{" "}
                </span>
                <button
                  type="button"
                  className="close pr-2"
                  data-dismiss="modal"
                  aria-label="Close"
                >
                  <span aria-hidden="true">&times;</span>
                </button>
              </div>
              <div className="View_modal-body-appneet pl-2">
                <p></p>
                {melding ? (
                  <span className=" sok-Alert-text pl-1">{errormsg}</span>
                ) : null}
                {melding ? <p></p> : null}
                <label className="divValueText_kw">Utvalgsnavn</label>
                <input
                  type="text"
                  maxLength="50"
                  value={warninputvalue_1}
                  onChange={warninput_1}
                  id="warntext"
                  className="inputwidth"
                />
                <br />
                <label className="divValueText_kw">Beskrivelse123</label>
                <input
                  type="text"
                  maxLength="50"
                  value={desinput}
                  onChange={desinputonchange}
                  id="desctext"
                  className="inputwidth"
                />

                <p></p>

                <div className="div_left">
                  <input
                    type="submit"
                    name="DemografiAnalyse1$uxFooter$uxBtForrige"
                    value="Avbryt"
                    data-dismiss="modal"
                    className="KSPU_button_Gray"
                  />
                </div>
                <div className="div-right">
                  <input
                    type="submit"
                    name="uxDistribusjon$uxDistSetDelivery"
                    value="Lagre123"
                    onClick={LagreClick}
                    id="uxDistribusjon_uxDistSetDelivery"
                    className="KSPU_button-kw float-right"
                  />
                </div>

                <br />
                <br />
              </div>
            </div>
          </div>
        </div>
      </div> */}

      <div
        className="modal fade bd-example-modal-lg"
        data-backdrop="false"
        id="kopiereUtvalget"
        tabIndex="-1"
        role="dialog"
        aria-labelledby="exampleModalCenterTitle"
        aria-hidden="true"
      >
        <div className="modal-dialog viewDetail" role="document">
          <div className="modal-content" style={{ border: "black 4px solid" }}>
            <div className="Common-modal-header">
              <span className="dialog-kw" id="exampleModalLongTitle">
                LAGRE UTVALG
              </span>
              <button
                type="button"
                className="close"
                data-dismiss="modal"
                aria-label="Close"
                ref={kopiermodal}
              >
                <span aria-hidden="true">&times;</span>
              </button>
            </div>
            <div className="View_modal-body pl-2">
              {melding1 ? (
                <span className=" sok-Alert-text pl-1">{errormsg1}</span>
              ) : null}
              {melding1 ? <p></p> : null}
              <table>
                <tbody>
                  <tr>
                    <td>
                      <span
                        id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxNameLabel"
                        className="SaveUtvaldivLabelText pl-2"
                      >
                        Utvalgsnavn
                      </span>
                    </td>
                    <td>
                      <input
                        // ref={dateRef}
                        name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxName"
                        type="text"
                        maxLength="50"
                        id="warntext1"
                        // defaultValue={
                        //   warninputvalue_1
                        //     ? warninputvalue_1
                        //     : utvalgapiobject.name +
                        //       "_" +
                        //       moment(new Date()).format("DD.MM.YYYY.hh.mm.ss")
                        // }
                        value={warninputvalue_1}
                        onChange={warninput_1}
                        className="selection-input ml-1"
                        placeholder=""
                      />
                    </td>
                  </tr>

                  <tr>
                    <td>
                      <span
                        id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxLogoLabel"
                        className="SaveUtvaldivLabelText pl-2"
                      >
                        Beskrivelse
                      </span>
                    </td>
                    <td>
                      <input
                        className="selection-input ml-1"
                        name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxLogo"
                        type="text"
                        maxLength="50"
                        value={desinput}
                        onChange={desinputonchange}
                        id="desctext"
                      />
                    </td>
                  </tr>
                  <br />
                  <tr>
                    <td>
                      <button
                        type="button"
                        className="KSPU_button_Gray"
                        data-dismiss="modal"
                        onClick={AvbrytClick}
                      >
                        Avbryt
                      </button>
                    </td>
                    <td></td>
                    <td>
                      <button
                        type="button"
                        className="KSPU_button-kw"
                        onClick={LagreClickKopier}
                      >
                        Lagre
                      </button>
                    </td>
                  </tr>
                  <br />
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>

      <div
        className={
          utvalgapiobject.ordreType === 1
            ? "invisible"
            : "col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0"
        }
      >
        <img src={readmore} />
        &nbsp;&nbsp;
        <a
          className="KSPU_LinkButton1_Url"
          data-toggle="modal"
          data-target="#kopiereUtvalget"
          onClick={kopiereUtvalget}
        >
          <b>Kopiere utvalget</b>
        </a>
      </div>
      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
        {skrivUtvalgetName === "skrivUtvalget" ? (
          <StandardreportKw
            title={"skrivUtvalget"}
            id={"skrivUtvalget"}
            modalshow={
              Object.keys(Endrelistapiobject).length > 0 ? true : false
            }
            // modalshow={defaultID.substring(0, 1) == "L" ? true : false}
            // type=""
            isList={Object.keys(Endrelistapiobject).length > 0 ? true : false}
          />
        ) : null}
        <div>
          <a
            id="uxBtnAddUtvalg"
            className="KSPU_LinkButton1_Url prevmnd"
            // href="https://externpostuat.posten.no/OA_HTML/ibeCCtpSctDspRte.jsp?minisite=10020"
            href=""
            data-toggle="modal"
            data-target="#skrivUtvalget"
            onClick={skrivUtvalget}
          >
            <b>Skriv ut utvalget</b>
          </a>
        </div>
      </div>
    </div>
  );
}
export default EndreClick_kw;
