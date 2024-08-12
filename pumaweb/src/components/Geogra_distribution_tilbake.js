import React, { useEffect, useState, useContext, useRef } from "react";
import { KundeWebContext } from "../context/Context";
import "../components/apne_Button_Click-kw/Apne_Button_Click-kw.scss";
import Swal from "sweetalert2";
import $ from "jquery";
import api from "../services/api.js";
import moment from "moment";
import readmore from "../assets/images/read_more.gif";
import { kundeweb_utvalg, Utvalglist, Utvalg } from "./KspuConfig";
import loadingImage from "../assets/images/callbackActivityIndicator.gif";
import { StandardreportKw } from "../components/apne_Button_Click-kw/standardreportKw";
//import { datadogLogs } from "@datadog/browser-logs";
import {
  CreateUtvalglist,
  CurrentDate,
  NumberFormat,
} from "../common/Functions";

function Geogra_distribution_tilbake_click() {
  const { householdcheckbox, sethouseholdcheckbox } =
    useContext(KundeWebContext);
  const { businesscheckbox, setbusinesscheckbox } = useContext(KundeWebContext);
  const { ActiveUtvalgObject, setActiveUtvalgObject } =
    useContext(KundeWebContext);
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
  const [loading, setloading] = React.useState([true]);
  const [kopiereUtvalgetName, setKopiereUtvalgetName] = useState("");
  const [skrivUtvalgetName, setSkrivUtvalgetName] = useState("");
  const [desinput, setdesinput] = useState("");
  const kopiermodal = useRef();
  const [melding1, setmelding1] = useState(false);
  const [errormsg1, seterrormsg1] = useState("");
  const [DefaultID, setDefaultID] = useState("");
  const [melding2, setmelding2] = useState(false);
  const [errormsg2, seterrormsg2] = useState("");
  const btnclose = useRef();
  const [ordername, setordername] = useState("");
  const { listmodal, setlistmodal } = useContext(KundeWebContext);
  const { utvalglistapiobject, setutvalglistapiobject } =
    useContext(KundeWebContext);
  const { custNos, setcustNos } = useContext(KundeWebContext);
  const { username_kw, setusername_kw } = useContext(KundeWebContext);
  const { avtaleData, setavtaleData } = useContext(KundeWebContext);
  const { CartItems, setCartItems } = useContext(KundeWebContext);

  // const [apnetext,setapnetext] = useState(utvalgapiobject[0].name ? utvalgapiobject[0].name+new Date() :utvalgapiobject[0].utvalgName+ (new Date().toUTCString().replace("GMT","")))
  const createActiveUtvalg = async (selectedDataSet) => {
    //let  Antall = getAntall(selectedDataSet.reoler);
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
    a.oldReolMapMissng = selectedDataSet.oldReolMapMissing;
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
    if (
      utvalgapiobject.antall == undefined ||
      typeof utvalgapiobject.antall == undefined ||
      utvalgapiobject.antall == "" ||
      utvalgapiobject.listId == ""
    ) {
      let url = `Utvalg/GetUtvalg?utvalgId=${utvalgapiobject.utvalgId}`;
      try {
        //api.logger.info("APIURL", url);
        const { data, status } = await api.getdata(url);
        if (data.length == 0) {
          // api.logger.error(
          //   "Error : No Data is present for mentioned Id" +
          //     utvalgapiobject.utvalgId
          // );
        } else {
          await createActiveUtvalg(data);
        }
      } catch (error) {
        //api.logger.error("errorpage API not working");
        //api.logger.error("error : " + error);
      }
    } else {
      let url =
        // `UtvalgList/GetUtvalgList?listId=${id}&getParentList=${true}&getMemberUtvalg=${true}`;
        `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${utvalgapiobject.listId}`;

      try {
        //api.logger.info("APIURL", url);
        const { data, status } = await api.getdata(url);
        if (data.length == 0) {
          // api.logger.error(
          //   "Error : No Data is present for mentioned Id" +
          //     utvalgapiobject.listId
          // );
        } else {
          let obj = await CreateUtvalglist(data);
          setutvalglistapiobject(obj);

          // setutvalglistcheck(true);
        }
      } catch (error) {
        //api.logger.error("errorpage API not working");
        //api.logger.error("error : " + error);
      }
    }
  }, []);

  useEffect(() => {
    if (listmodal) {
      window.$("#exampleModal-9").modal("show");
    }

    setUtvalgID(
      utvalgapiobject.antall == undefined ||
        typeof utvalgapiobject.antall == undefined ||
        utvalgapiobject.antall == ""
        ? utvalgapiobject.utvalgId
        : utvalgapiobject.listId
    );
    setDefaultID(
      utvalgapiobject.antall == undefined ||
        typeof utvalgapiobject.antall == undefined ||
        utvalgapiobject.antall == "" ||
        utvalgapiobject.listId == ""
        ? "U" + utvalgapiobject.utvalgId
        : "L" + utvalgapiobject.listId
    );
    setutvalgname(utvalgapiobject.name);
    setAntallvalue(
      // utvalgapiobject.antall == undefined || utvalgapiobject.antall == ""
      //   ? utvalgapiobject.totalAntall
      //   : utvalgapiobject.antall
      utvalgapiobject.antallWhenLastSaved
    );
  }, []);

  const warninput_1 = () => {
    setmelding1(false);
    let textinput = document.getElementById("warntext1").value;
    setwarninputvalue_1(textinput);
  };
  const AngiClick = async () => {
    if (apnetext == "") {
      setmelding(true);
      seterrormsg("Utvalget tilfredstiller ikke kriterier for å kunne lagres.");
    } else {
      let saveOldReoler = "false";
      let skipHistory = "false";
      let forceUtvalgListId = 0;
      let name = username_kw !== "" ? username_kw : "Internbruker";
      let url = `Utvalg/SaveUtvalg?userName=${name}&`;
      url = url + `saveOldReoler=${saveOldReoler}&`;
      url = url + `skipHistory=${skipHistory}&`;
      url = url + `forceUtvalgListId=${forceUtvalgListId}`;
      try {
        // let A = kundeweb_utvalg();
        let A = kundeweb_utvalg();

        utvalgapiobject["logo"] = apnetext;
        utvalgapiobject.kundeNavn = name;

        A.kundeNummer = custNos;
        if (avtaleData) {
          A.avtalenummer = avtaleData;
        }
        A.kundeNavn = name;
        // A.kundeNummer = custNos;
        // A.avtalenummer = avtaleData;
        A.logo = apnetext;
        A.totalAntall = utvalgapiobject.totalAntall;
        A.reoler = utvalgapiobject.reoler;
        A.Antall = utvalgapiobject.Antall;
        let criteriavalue = {};
        criteriavalue.criteriaType = "19";
        criteriavalue.criteria = "Geografi";
        A.criterias.push(criteriavalue);
        A.listId = utvalgapiobject.listId;
        A.isBasis = utvalgapiobject.isBasis;
        A.receivers = utvalgapiobject.receivers;
        A.name = utvalgapiobject.name;
        if (utvalgapiobject?.utvalgId) {
          A.utvalgId = utvalgapiobject.utvalgId;
        } else {
          A.utvalgId = 0;
        }
        A.modifications.push({
          modificationId: Math.floor(100000 + Math.random() * 900000),
          userId: name,
          modificationTime: CurrentDate(),
          listId: 0,
        });
        utvalgapiobject.modifications.push({
          modificationId: Math.floor(100000 + Math.random() * 900000),
          userId: name,
          modificationTime: CurrentDate(),
          listId: 0,
        });

        const { data, status } = await api.postdata(url, A);
        if (status === 200) {
          // setutvalgapiobject(data);
          setPage("Geogra_distribution_click");
        } else {
          setmelding(true);
          seterrormsg("noe gikk galt");
        }
      } catch (e) {
        setmelding(true);
        seterrormsg("noe gikk galt");
        console.log(e);
      }
    }
  };
  const GotoMain = () => {
    setPage("");
  };
  const warninput2 = (e) => {
    // e.preventDefault();
    setmelding2(false);
    let textinput = document.getElementById("warntext4").value;
    //setwarninputvalue(textinput);
    setordername(textinput);
  };

  const Apnetextbox = () => {
    setmelding(false);
    let textboxvalue = document.getElementById(
      "uxShowUtvalgDetails_uxForhandlerpaatrykk"
    ).value;
    setapnetext(textboxvalue);
  };
  const Jaclick = async () => {
    try {
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
        url = "UtvalgList/DeleteUtvalgList?UtvalgListId=";
        utvalgType = "UtvalgList";
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
      const { data, status } = await api.deletedata(url + ID);
      if (status === 200) {
        if (data === true) {
          Swal.fire({
            text: `Utvalg slettet`,
            confirmButtonColor: "#7bc144",
            position: "top",
            icon: "success",
          });
        }
        setPage("");
      } else {
        console.error("error : " + status);
      }
    } catch (error) {
      console.error("er : " + error);
    }
  };
  const leggtillclick = async (e) => {
    e.preventDefault();
    setmelding2(false);
    seterrormsg2("");
  };
  const LagreClick = async (e) => {
    e.preventDefault();
    if (ordername == "") {
      setmelding2(true);
      seterrormsg2("utvalget må ha minst 3 tegn.");
    }
    // setnomessagediv(false);
    //setPage("Apne_Button_Click")
    const { data, status } = await api.getdata(
      `UtvalgList/UtvalgListNameExists?utvalglistname=${encodeURIComponent(
        ordername
      )}`
    );
    if (status === 200) {
      if (data == true) {
        //datadogLogs.logger.info("UtvalgnameExistsResult", data);
        setmelding2(true);
        let msg = `Utvalget ${ordername} eksisterer allerede. Velg et annet utvalgsnavn.`;
        seterrormsg2(msg);
      } else {
        let listName = ordername;
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
            listName: ordername,
            customerName: username_kw,
            // kundeNummer: kundeNummer,
            customerNo: kundeNummer,
            avtalenummer: avtalenummer,
            logo: logo,
            // utvalgs: [ActiveUtvalgObject],
            utvalgs: [utvalgapiobject],
            // memberUtvalgs: [utvalgapiobject],
            // memberUtvalgs: [ActiveUtvalgObject],
          };
          const { data, status } = await api.postdata(url, a);

          if (status === 200) {
            utvalgapiobject.listId = `${data.listId}`;
            utvalgapiobject.listName = `${data.name}`;
            data.utvalgs = [utvalgapiobject];
            data.memberUtvalgs = [utvalgapiobject];
            setutvalglistapiobject(data);
            btnclose.current.click();
            setnewhome(true);
            setPage("");

            // window.$("#exampleModal-1").modal("dispose");
          }
        } catch (e) {
          //datadogLogs.logger.info("saveutvalgError", e);
          setloading(true);
          setmelding2(true);
          seterrormsg2("noe gikk galt. vennligst prøv etter en stund");
          console.log(e);
        }
        //    setPage("Geogra_distribution_click")
      }
    }
  };

  const LagreClickKopier = async () => {
    if (warninputvalue_1 == "") {
      setmelding1(true);
      seterrormsg1("utvalget må ha minst 3 tegn.");
    } else {
      setloading(false);
      const { data, status } = await api.getdata(
        `Utvalg/UtvalgNameExists?utvalgNavn=${warninputvalue_1}`
      );
      if (status === 200) {
        if (data == true) {
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
          let name = username_kw;
          let url = `Utvalg/SaveUtvalg?userName=${name}&`;
          url = url + `saveOldReoler=${saveOldReoler}&`;
          url = url + `skipHistory=${skipHistory}&`;
          url = url + `forceUtvalgListId=${forceUtvalgListId}`;
          try {
            let A = kundeweb_utvalg();
            A.name = warninputvalue_1;
            A.kundeNavn = desinput;
            A.totalAntall = Antallvalue;

            // A.reoler[0].description = describtion;
            // A.reoler[0].antall.households = HouseholdSum;
            A.criterias[0].criteriaType = 19;
            A.criterias[0].criteria = "Geografipulkkliste";
            const { data, status } = await api.postdata(url, A);
            if (status === 200) {
              let utvalgID = data.utvalgId;
              // setutvalgapiobject(data);
              setutvalgname(warninputvalue_1);
              kopiermodal.current.click();
              // $("#modal").modal("toggle");
              $("body").removeClass("modal-open");
              setloading(true);
              // $(".modal-backdrop").remove();
              // settest(true);
              // setloading(true);
              // setPage("");
            }
          } catch (e) {
            console.log(e);
          }
        }
      }
    }
  };
  const desinputonchange = () => {
    let desctextvalue = document.getElementById("desctext").value;
    setdesinput(desctextvalue);
  };

  const kopiereUtvalget = () => {
    setmelding1(false);
    let s = utvalgapiobject.name;
    let g = new Date();
    let formateddate = moment(g).format("DD.MM.yyy hh:mm");
    let formatedname = formateddate.replace(" ", "");
    setwarninputvalue_1(utvalgapiobject.name + "_" + formateddate);
    // setKopiereUtvalgetName("kopiereUtvalget");
  };
  const skrivUtvalget = () => {
    setSkrivUtvalgetName("skrivUtvalget");
  };

  return (
    <div className="col-5 p-2">
      <div
        className="padding_NoColor_B cursor"
        onClick={() => {
          if (CartItems.length > 0) {
            setPage("cartClick_Component_kw");
          }
        }}
      >
        <a id="uxHandlekurvSmall_uxLnkbtnHandlekurv">
          <div className="handlekurv handlekurvText pl-2">
            Du har {CartItems.length} utvalg i bestillingen din.
          </div>
        </a>
      </div>
      <br />

      <div className="padding_Color_L_R_T_B">
        <div className="AktivtUtvalg pr-2">
          {melding ? (
            <div className="pr-3">
              <div className="error WarningSign">
                <div className="divErrorHeading">Melding:</div>
                <span
                  id="uxKjoreAnalyse_uxLblMessage"
                  className="divErrorText_kw"
                >
                  {errormsg}
                </span>
              </div>
            </div>
          ) : null}
          <div className="AktivtUtvalgHeading ">
            <span id="uxShowUtvalgDetails_uxLblSavedUtvalgName">
              {utvalgname}
            </span>

            {/* <span id="uxShowUtvalgDetails_uxLblSavedUtvalgName">{  (utvalgapiobject[0].name)+new Date().toUTCString().replace("GMT","") ||  (utvalgapiobject[0].utvalgName)+new Date().toUTCString().replace("GMT","")}</span> */}
          </div>

          <div>
            <div className="gray">
              ID:
              <span id="uxShowUtvalgDetails_uxRefNr" className="gray">
                {"U" + utvalgapiobject.utvalgId || "L" + utvalgapiobject.listId}
              </span>
            </div>
          </div>
        </div>

        <div className="pt-3">
          {householdcheckbox ? (
            <div>
              {" "}
              <label className="form-check-label label-text" htmlFor="Hush">
                {" "}
                Husholdninger{" "}
              </label>
              {Page_P == "GeograVelg" ? (
                <span
                  id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                  className="divValueTextBold div_right"
                >
                  {NumberFormat(utvalgapiobject.Antall[0])}
                </span>
              ) : (
                <span
                  id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                  className="divValueTextBold div_right"
                >
                  {NumberFormat(utvalgapiobject.Antall[0])}
                </span>
              )}
            </div>
          ) : null}

          {businesscheckbox ? (
            <div>
              <label className="form-check-label label-text" htmlFor="Virk">
                {" "}
                Virksomheter{" "}
              </label>
              {Page_P == "GeograVelg" ? (
                <span
                  id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                  className="divValueTextBold div_right"
                >
                  {NumberFormat(utvalgapiobject.Antall[1])}
                </span>
              ) : (
                <span
                  id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                  className="divValueTextBold div_right"
                >
                  {NumberFormat(utvalgapiobject.Antall[1])}
                </span>
              )}
            </div>
          ) : null}
        </div>
        <div
          style={{
            width: "370px",
            borderTop: "solid 1px black",
            fontWeight: "bold",
            padding: "0px",
          }}
        >
          &nbsp;
          <span
            id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblAntallSumText"
            className="divValueTextBold  div_left"
          >
            Totalt for utvalget
          </span>
          {businesscheckbox && householdcheckbox ? (
            Page_P == "GeograVelg" ? (
              <span
                id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                className="divValueTextBold div_right pr-4"
              >
                {NumberFormat(
                  utvalgapiobject.Antall[0] + utvalgapiobject.Antall[1]
                )}
              </span>
            ) : (
              <span
                id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                className="divValueTextBold div_right pr-4"
              >
                {NumberFormat(
                  utvalgapiobject.Antall[0] + utvalgapiobject.Antall[1]
                )}
              </span>
            )
          ) : householdcheckbox ? (
            Page_P == "GeograVelg" ? (
              <span
                id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                className="divValueTextBold div_right pr-4"
              >
                {NumberFormat(utvalgapiobject.Antall[0])}
              </span>
            ) : (
              <span
                id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                className="divValueTextBold div_right pr-4"
              >
                {NumberFormat(utvalgapiobject.Antall[0])}
              </span>
            )
          ) : businesscheckbox ? (
            Page_P == "GeograVelg" ? (
              <span
                id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                className="divValueTextBold div_right pr-4"
              >
                {NumberFormat(utvalgapiobject.Antall[1])}
              </span>
            ) : (
              <span
                id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                className="divValueTextBold div_right pr-4"
              >
                {NumberFormat(utvalgapiobject.Antall[1])}
              </span>
            )
          ) : null}
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
                        <br />
                      </tbody>
                    </table>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
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
          className="modal fade  bd-example-modal-lg"
          id="exampleModal-9"
          tabIndex="-1"
          data-backdrop="false"
          role="dialog"
          aria-labelledby="exampleModalCenterTitle"
          aria-hidden="true"
        >
          {/* <div className="modal-dialog modal-dialog-centered  " role="document"> */}
          <div className="modal-dialog  " role="document">
            <div className="modal-content">
              <div className="">
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
                  {melding2 ? (
                    <span className=" sok-Alert-text pl-1">{errormsg2}</span>
                  ) : null}
                  {melding2 ? <p></p> : null}

                  <input
                    type="text"
                    maxLength="50"
                    // placeholder="Navn på bestil"
                    value={ordername}
                    id="warntext4"
                    className="inputwidth"
                    onChange={() => warninput2()}
                  />

                  <p></p>
                  <div className="">
                    <div className="div_left">
                      <input
                        type="submit"
                        name="DemografiAnalyse1$uxFooter$uxBtForrige"
                        value="Avbryt"
                        data-dismiss="modal"
                        className="KSPU_button_Gray"
                      />
                      &nbsp;&nbsp;
                      <input
                        type="submit"
                        name="uxDistribusjon$uxDistSetDelivery"
                        value="Lagre"
                        onClick={(e) => LagreClick(e)}
                        id="uxDistribusjon_uxDistSetDelivery"
                        className="KSPU_button-kw"
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
          <div className="modal-dialog " role="document">
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
                                onClick={""}
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
        <a
          className="KSPU_LinkButton_Url_KW pl-2"
          data-toggle="modal"
          data-target="#exampleModal"
        >
          Slett utvalget
        </a>
      </div>
      <br />
      <div className="div_right">
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
      <div className="div_right">
        <input
          type="submit"
          name="DemografiAnalyse1$uxFooter$uxBtForrige"
          value="Legg til flere utvalg"
          onClick={leggtillclick}
          data-toggle="modal"
          data-target="#exampleModal-9"
          className="KSPU_button_Gray"
        />
      </div>

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
        <div className="modal-dialog  viewDetail" role="document">
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
                  <br />
                  <tr>
                    <td>
                      <button
                        type="button"
                        className="KSPU_button_Gray"
                        data-dismiss="modal"
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

      <div className="col-lg-12 col-md-12 col-sm-12 col-xs-12 m-0 p-0">
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
export default Geogra_distribution_tilbake_click;
