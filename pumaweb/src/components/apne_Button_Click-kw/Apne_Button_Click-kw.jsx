import React, { useEffect, useRef, useState, useContext } from "react";
import { KundeWebContext, MainPageContext } from "../../context/Context";
import "./Apne_Button_Click-kw.scss";
import Swal from "sweetalert2";
import $ from "jquery";
import api from "../../services/api.js";
import readmore from "../../assets/images/read_more.gif";
import { kundeweb_utvalg, Utvalg } from "../KspuConfig";
import loadingImage from "../../assets/images/callbackActivityIndicator.gif";
import { StandardreportKw } from "./standardreportKw";
import VisDetaljerModal from "../VisDetaljerModal_KW";
import {
  NumberFormat,
  CreateUtvalglist,
  CurrentDate,
} from "../../common/Functions";
import { bootstrap } from "bootstrap";
import * as query from "@arcgis/core/rest/query";
import Query from "@arcgis/core/rest/support/Query";
import Graphic from "@arcgis/core/Graphic";
import { MapConfig } from "../../config/mapconfig";
import VisDetaljerModal_KW from "../VisDetaljerModal_KW";
import moment from "moment";

function Apne_Button_Click() {
  const { ActiveUtvalgObject, setActiveUtvalgObject } =
    useContext(KundeWebContext);
  const btnclose = useRef();
  const { mapView } = useContext(MainPageContext);
  const kopiermodal = useRef();
  const { custNos, setcustNos } = useContext(KundeWebContext);
  const { avtaleData, setavtaleData } = useContext(KundeWebContext);
  const { username_kw, setusername_kw } = useContext(KundeWebContext);
  const { utvalgapiobject, setutvalgapiobject } = useContext(KundeWebContext);
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
  const [loading, setloading] = useState(false);
  const [kopiereUtvalgetName, setKopiereUtvalgetName] = useState("");
  const [skrivUtvalgetName, setSkrivUtvalgetName] = useState("");
  const [desinput, setdesinput] = useState("");
  const [defaultID, setdefaultID] = useState(0);
  const { utvalglistapiobject, setutvalglistapiobject } =
    useContext(KundeWebContext);
  const [melding1, setmelding1] = useState(false);
  const [errormsg1, seterrormsg1] = useState("");
  const [melding3, setmelding3] = useState(false);
  const [errormsg3, seterrormsg3] = useState("");
  const [Modal, setModal] = useState("");
  const [ResultBeforeCreationObject, setResultBeforeCreationObject] =
    useState();
  const [ResultAfterCreationObject, setResultAfterCreationObject] = useState();
  const [commonReoler, setCommonReoler] = useState();
  const [visDetaljerModalName, setVisDetaljerModalName] = useState(" ");
  const { householdcheckbox, sethouseholdcheckbox } =
    useContext(KundeWebContext);
  const { businesscheckbox, setbusinesscheckbox } = useContext(KundeWebContext);
  const [HouseHoldSum, setHouseHoldSum] = useState(0);
  const [BusinessSum, setBusinessSum] = useState(0);
  const { CartItems, setCartItems } = useContext(KundeWebContext);
  const [contentLoading, setContentLoading] = useState(true);
  const { selectionUpdateKW, setSelectionUpdateKW } =
    useContext(KundeWebContext);
  const [resultRestReoler, setResultRestReoler] = useState();
  // const [apnetext,setapnetext] = useState(utvalgapiobject[0].name ? utvalgapiobject[0].name+new Date() :utvalgapiobject[0].utvalgName+ (new Date().toUTCString().replace("GMT","")))
  const [ResultAfterRuteCreation, setResultAfterRuteCreation] = useState();
  const [showBusiness, setShowBusiness] = useState(false);
  const [showHouseHold, setShowHouseHold] = useState(false);

  // const dateRef = useRef(null);

  useEffect(async () => {
    setContentLoading(true);
    sethouseholdcheckbox(false);

    setUtvalgID(
      typeof utvalgapiobject.totalAntall == "undefined"
        ? utvalgapiobject.listId
        : utvalgapiobject.utvalgId
    );
    setdefaultID(
      typeof utvalgapiobject.totalAntall == "undefined"
        ? utvalgapiobject.listId
        : utvalgapiobject.utvalgId
    );

    setutvalgname(utvalgapiobject.name);
    setAntallvalue(
      typeof utvalgapiobject.antall == "undefined"
        ? utvalgapiobject.totalAntall
        : utvalgapiobject.antall
    );
    if (
      utvalgapiobject.antall == undefined ||
      typeof utvalgapiobject.antall == undefined
    ) {
      let url = `Utvalg/GetUtvalg?utvalgId=${utvalgapiobject.utvalgId}`;
      try {
        const { data, status } = await api.getdata(url);
        if (data.length == 0) {
        } else {
          await createActiveUtvalg(data);
          let householdarray = [];

          let households = data.reoler.map((item) => {
            householdarray.push(item.antall.households);
          });
          let householdsum = householdarray.reduce((a, b) => a + b, 0);
          if (householdsum > 0) {
            sethouseholdcheckbox(true);
            setHouseHoldSum(householdsum);
          }
          let businessArray = [];

          let businesses = data.reoler.map((item) => {
            businessArray.push(item.antall.businesses);
          });
          let businesssum = businessArray.reduce((a, b) => a + b, 0);
          if (data.totalAntall == householdsum) {
            setbusinesscheckbox(false);
          } else if (data.totalAntall == businesssum) {
            setbusinesscheckbox(true);
            sethouseholdcheckbox(false);
            setBusinessSum(businesssum);
          } else {
            sethouseholdcheckbox(true);
            setHouseHoldSum(householdsum);
            setbusinesscheckbox(true);
            setBusinessSum(businesssum);
          }
        }
      } catch (error) {}
    } else {
      let url = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${utvalgapiobject.listId}`;

      try {
        const { data, status } = await api.getdata(url);
        if (data.length == 0) {
        } else {
          let obj = await CreateUtvalglist(data);

          setutvalglistapiobject(obj);
        }
      } catch (error) {}
    }
    setContentLoading(false);
  }, []);

  const showVisDetaljer = () => {
    let reoler = utvalgapiobject.reoler;
    let reolerBeforeRecreation = utvalgapiobject.reolerBeforeRecreation;
    let reolids = [];
    let result = reoler.filter((o1) =>
      reolerBeforeRecreation.map((o2) => {
        if (o1.reolId === o2.reolId) {
          o1["businessesOld"] = o2.antall.businesses;
          o1["householdsOld"] = o2.antall.households;
          reolids.push(o1.reolId);
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

  const AvbrytClick = () => {
    setloading(false);
  };
  const LeggTillClick = async () => {
    if (apnetext == "") {
      setmelding3(true);
      seterrormsg3(
        `Utvalget tilfredstiller ikke kriterier for å kunne lagres.Du må fylle inn «beskrivelse av utvalget»`
      );
    } else if (apnetext.length < 3) {
      setmelding3(true);
      seterrormsg3("Beskrivelse av utvalget må ha minst 3 tegn.");
    } else {
      commonFn("leggtil");
    }
  };

  const commonFn = async (param) => {
    setContentLoading(true);
    let saveOldReoler = "false";
    let skipHistory = "false";
    let forceUtvalgListId = 0;
    let name = username_kw;

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
      utvalgapiobject.modifications.push({
        modificationId: Math.floor(100000 + Math.random() * 900000),
        userId: customername,
        modificationTime: CurrentDate(),
        listId: 0,
      });
      if (apnetext) {
        utvalgapiobject.logo = apnetext;
      }

      const { data, status } = await api.postdata(url, utvalgapiobject);

      if (status === 200) {
        if (param === "angi") {
          setActiveUtvalgObject(data);

          setPage_P("Apne_Button_Click");
          setPage("Geogra_distribution_click");
        } else if (param === "leggtil") {
          $("#exampleModal-1").modal("show");
        }
      }
    } catch (e) {
      console.log("error", e);
    }

    setContentLoading(false);
  };

  const warninput_1 = () => {
    setmelding(false);
    setmelding1(false);
    let textinput = document.getElementById("warntext1").value;
    setwarninputvalue_1(textinput);
  };
  const AngiClick = async () => {
    if (utvalgapiobject?.logo?.length || apnetext?.length > 3) {
      commonFn("angi");
    } else if (apnetext === "") {
      setmelding3(true);
      seterrormsg3(
        `Utvalget tilfredstiller ikke kriterier for å kunne lagres.Du må fylle inn «beskrivelse av utvalget»`
      );
    } else if (apnetext.length < 3) {
      setmelding3(true);
      seterrormsg3("Beskrivelse av utvalget må ha minst 3 tegn.");
    }
  };

  const GotoMain = () => {
    setPage("");
  };

  const DoNothing = () => {};

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
      setloading(true);
      let ID = 0;
      let url = "";
      let utvalgType = "";
      if (utvalgapiobject.listId) {
        if (
          typeof utvalgapiobject.listId == "string" &&
          utvalgapiobject.listId.substring(1) === "L"
        ) {
          ID = utvalgapiobject.listId.substring(1);
        } else {
          ID = utvalgapiobject.listId;
        }
        if (utvalgapiobject.basedOn > 0) {
          url = `UtvalgList/DeleteCampaignList?ListId=${ID}&BasedOn=${utvalgapiobject.basedOn}`;
          utvalgType = "UtvalgList";
        } else {
          url = "UtvalgList/DeleteUtvalgList?UtvalgListId=";
          utvalgType = "UtvalgList";
        }
      }
      if (utvalgapiobject.utvalgId) {
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
      let { data, status } = await api.deletedata(
        utvalgapiobject.basedOn > 0 ? url : url + ID
      );

      if (status === 200) {
        if (data === true) {
          setloading(false);
          Swal.fire({
            text: `Utvalg slettet`,
            confirmButtonColor: "#7bc144",
            position: "top",
            icon: "success",
          });

          setPage("");
        }
      } else {
        setloading(false);
        console.error("error : " + status);
      }
    } catch (error) {
      setloading(false);
      // swal("oops! Something Went Wrong!");

      console.error("er : " + error);
    }
  };
  const LagreClick = async () => {
    if (warninputvalue == "") {
      setmelding(true);
      seterrormsg("utvalget må ha minst 3 tegn.");
    }

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

        let url = `UtvalgList/AddUtvalgsToNewList?userName=${customerName}`;

        try {
          var a = {
            listName: warninputvalue,
            customerName: username_kw,
            kundeNummer: kundeNummer,
            avtalenummer: avtalenummer,
            logo: logo,
            utvalgs: utvalgapiobject
              ? [utvalgapiobject]
              : [utvalglistapiobject],
            // utvalgs: [utvalgapiobject],
            // memberUtvalgs: [utvalgapiobject],
            // memberUtvalgs: utvalgapiobject
            //   ? [utvalgapiobject]
            //   : [utvalglistapiobject],
          };
          const { data, status } = await api.postdata(url, a);

          if (status === 200) {
            utvalgapiobject.listId = `${data.listId}`;
            utvalgapiobject.listName = `${data.name}`;
            data.utvalgs = [utvalgapiobject];
            data.memberUtvalgs = [utvalgapiobject];
            setutvalglistapiobject({});
            setutvalglistapiobject(data);
            btnclose.current.click();
            setnewhome(true);
            setPage("");

            // window.$("#exampleModal-1").modal("dispose");
          }
        } catch (e) {
          //datadogLogs.logger.info("saveutvalgError", e);
          setloading(false);
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
      seterrormsg1("Du må navngi utvalget (max 50 tegn)");
    } else {
      setloading(true);
      const { data, status } = await api.getdata(
        `Utvalg/UtvalgNameExists?utvalgNavn=${warninputvalue_1}`
      );
      if (status === 200) {
        if (data === true) {
          seterrormsg1(
            `Utvalget eksisterer allerede. Velg et annet utvalgsnavn.`
          );
          setmelding1(true);
          setloading(false);
        } else {
          setloading(true);
          let saveOldReoler = "false";
          let skipHistory = "false";
          let forceUtvalgListId = 0;
          //let name = username_kw;
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
            A.reoler = utvalgapiobject.reoler;
            A.receivers = utvalgapiobject.receivers;
            A.Antall = utvalgapiobject.Antall;
            A.logo = desinput ? desinput : utvalgapiobject.logo;
            A["kundeNummer"] = custNos;
            A["avtalenummer"] = avtaleData ? avtaleData : 0;
            A.antallBeforeRecreation = 0;
            A.reolerBeforeRecreation = [];
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
              await createActiveUtvalg(data);
              setutvalgname(warninputvalue_1);
              kopiermodal.current.click();
              $("body").removeClass("modal-open");
              setloading(false);
            }
          } catch (e) {
            console.log(e);
          }
        }
      } else {
        setloading(false);
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
    let newArray = [];
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

  const updateUtvalgObj = async (e) => {
    let customername = "";
    if (username_kw) {
      customername = username_kw;
    } else {
      customername = "Internbruker";
    }
    let updated = false;
    setloading(true);
    let saveOldReoler = "false";
    let skipHistory = "false";
    let forceUtvalgListId = 0;
    let url = `Utvalg/SaveUtvalg?userName=${customername}&`;
    url = url + `saveOldReoler=${saveOldReoler}&`;
    url = url + `skipHistory=${skipHistory}&`;

    url = url + `forceUtvalgListId=${forceUtvalgListId}`;
    try {
      utvalgapiobject.modifications.push({
        modificationId: Math.floor(100000 + Math.random() * 900000),
        userId: customername,
        modificationTime: CurrentDate(),
        listId: 0,
      });

      const { data, status } = await api.postdata(url, utvalgapiobject);
      if (status === 200) {
        updated = true;
        await createActiveUtvalg(data);
        setloading(false);
      }
    } catch (error) {
      console.error("error : " + error);
      setloading(false);
    }
    return updated;
  };

  return (
    <div className="col-5 p-2">
      <div
        className="padding_NoColor_B cursor"
        onClick={() => {
          if (typeof utvalgapiobject.totalAntall == "undefined") {
            if (utvalglistapiobject.memberUtvalgs?.length > 0) {
              setPage("cartClick_Component_kw");
            }
          }
        }}
      >
        <a id="uxHandlekurvSmall_uxLnkbtnHandlekurv">
          {contentLoading ? (
            <div className="handlekurv handlekurvText pl-2">loading..</div>
          ) : (
            <div className="handlekurv handlekurvText pl-2">
              Du har{" "}
              {utvalglistapiobject?.memberUtvalgs
                ? utvalglistapiobject?.memberUtvalgs?.length
                : 1}{" "}
              utvalg i bestillingen din.
            </div>
          )}
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
              {utvalgname}
            </span>
          </div>

          <div>
            <div className="gray">
              ID:
              <span id="uxShowUtvalgDetails_uxRefNr" className="gray">
                {utvalgapiobject?.utvalgId
                  ? "U" + (utvalgapiobject?.utvalgId).toString()
                  : null}
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

        <div className={contentLoading ? "invisible" : ""}>
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
              <div
                className="sok-text blue cursor-pointer"
                onClick={showVisKart}
              >
                Vis/Skjul gammelt område i kartet
              </div>
            </div>
          ) : (
            <div>
              <div className="_flex-column">
                {showHouseHold ? (
                  <div>
                    <label
                      className="form-check-label label-text"
                      htmlFor="Hush"
                    >
                      {" "}
                      Husholdninger{" "}
                    </label>
                    <span className="divValueTextBold div_right">
                      {NumberFormat(utvalgapiobject?.Antall[0])}
                    </span>
                  </div>
                ) : null}
                {showBusiness ? (
                  <div>
                    <label
                      className="form-check-label label-text"
                      htmlFor="Hush"
                    >
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
          )}
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
                        <tr>
                          <td>&nbsp;</td>
                        </tr>
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
                              navn: {utvalgapiobject.name}.
                            </p>{" "}
                          </td>
                          <td></td>
                        </tr>
                        <tr>
                          <td colSpan={2}>&nbsp;</td>
                        </tr>
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
            display: !loading ? "none" : "block",
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
                                onClick={DoNothing}
                                className="modalMessage_button"
                                data-dismiss="modal"
                              >
                                Ja
                              </button>
                            </div>
                          </td>
                        </tr>
                        <tr>
                          <td>&nbsp;</td>
                        </tr>
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

          <div
            className={
              (utvalgapiobject?.antallBeforeRecreation > 0 ||
                selectionUpdateKW) &&
              utvalgapiobject.ordreType !== 1
                ? "_flex-end"
                : "invisible"
            }
            style={{ flex: 1 }}
          >
            <input
              type="submit"
              name="saveUtvalgLagreEndringer"
              value="Lagre endringer"
              id="saveSelectionFromList"
              onClick={updateUtvalgObj}
              class="KSPU_button-kw"
              // style={
              //   !selectionUpdateKW ? { display: "none" } : { display: "block" }
              // }
            />
          </div>
        </div>
      </div>

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
                (utvalgapiobject.ordreType === 1 && utvalgapiobject.utvalgId) ||
                contentLoading
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
                (utvalgapiobject.ordreType === 1 && utvalgapiobject.utvalgId) ||
                contentLoading
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
                utvalgapiobject.ordreType === 1 && !contentLoading
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
      <br />
      <br />

      <div className="div_left">
        <span className="bold">Du kan også..</span>

        <br />
      </div>
      <br />

      <br />

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
                  <tr>
                    <td colSpan={2}>&nbsp;</td>
                  </tr>
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
                  <tr>
                    <td colSpan={2}>&nbsp;</td>
                  </tr>
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
            modalshow={true}
            // type=""
            isList={false}
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
export default Apne_Button_Click;
