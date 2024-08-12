import React, { useEffect, useState, useRef, useContext } from "react";
import "../App.css";
import api from "../services/api.js";
import { KundeWebContext } from "../context/Context";
import {
  kundeweb_utvalg,
  NewUtvalgName,
  criterias,
  getAntall,
  Utvalg,
} from "./KspuConfig";
import Extent from "@arcgis/core/geometry/Extent";
import { MapConfig } from "../config/mapconfig";
import { MainPageContext } from "../context/Context.js";
//import { datadogLogs } from "@datadog/browser-logs";
import loadingImage from "../assets/images/callbackActivityIndicator.gif";
import { NumberFormat, CurrentDate } from "../common/Functions";

function LagutvalgClick(props, { parentCallback }) {
  const btnclose = useRef();
  const { custNos, setcustNos, avtaleData, setavtaleData } =
    useContext(KundeWebContext);
  const { username_kw, setusername_kw } = useContext(KundeWebContext);
  const [loading, setloading] = useState(false);

  const [warninputvalue, setwarninputvalue] = useState("");

  const { householdcheckbox, sethouseholdcheckbox } =
    useContext(KundeWebContext);
  const { businesscheckbox, setbusinesscheckbox } = useContext(KundeWebContext);

  const { HouseholdSum, setHouseholdSum } = useContext(KundeWebContext);
  const { BusinessSum, setBusinessSum } = useContext(KundeWebContext);

  const { Page, setPage } = useContext(KundeWebContext);
  const [melding, setmelding] = useState(false);
  const { describtion, setdescribtion } = useContext(KundeWebContext);
  const { selection, setselection } = useContext(KundeWebContext);
  const [melding1, setmelding1] = useState(false);

  const [errormsg, seterrormsg] = useState("");
  const [errormsg1, seterrormsg1] = useState("");
  const { Page_P, setPage_P } = useContext(KundeWebContext);
  const { newhome, setnewhome } = useContext(KundeWebContext);
  const { utvalgapiobject, setutvalgapiobject } = useContext(KundeWebContext);

  const { selectedKoummeIDs, setselectedKoummeIDs } =
    useContext(KundeWebContext);

  const { SavedUtvalg, setSavedUtvalg } = useContext(KundeWebContext);
  const { UtvalgID, setUtvalgID } = useContext(KundeWebContext);
  const { preselection, setpreselection } = useContext(KundeWebContext);
  const { predesc, setpredesc } = useContext(KundeWebContext);
  const [listnaviagation, setlistnaviagation] = useState(false);
  const [lagutvalg, setlagutvalg] = useState(false);
  const { listmodal, setlistmodal } = useContext(KundeWebContext);
  const { CartItems, setCartItems } = useContext(KundeWebContext);
  const { utvalglistapiobject, setutvalglistapiobject } =
    useContext(KundeWebContext);

  const { ActiveUtvalgObject, setActiveUtvalgObject } =
    useContext(KundeWebContext);
  const { BudruterSelectedName, setBudruterSelectedName } =
    useContext(KundeWebContext);
  const { criteriaObject, setCriteriaObject } = useContext(KundeWebContext);
  const { selectedsegment, setselectedsegment } = useContext(KundeWebContext);
  const { selectedRowKeys, setSelectedRowKeys } =
    React.useContext(KundeWebContext);
  const { selectedrecord_s, setselectedrecord_s } = useContext(KundeWebContext);
  const { ActiveMapButton, setActiveMapButton } = useContext(MainPageContext);
  const { routeUpdateEnabled, setRouteUpdateEnabled } =
    useContext(KundeWebContext);
  const { mapView } = useContext(MainPageContext);

  useEffect(() => {
    let Antall = utvalgapiobject.Antall;
    setHouseholdSum(Antall[0]);
    setBusinessSum(Antall[1]);
  }, []);

  const LagreClick = async (e) => {
    e.preventDefault();
    if (warninputvalue == "") {
      setmelding1(true);
      seterrormsg1("utvalget må ha minst 3 tegn.");
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
        setmelding1(true);
        let msg = `Utvalget ${warninputvalue} eksisterer allerede. Velg et annet utvalgsnavn.`;
        seterrormsg1(msg);
      } else {
        let listName = warninputvalue;
        let customerName = "";
        if (username_kw) {
          customerName = username_kw;
        } else {
          customerName = "test";
        }
        // let url = `UtvalgList/AddUtvalgsToNewList?userName=${username_kw}`;
        let url = `UtvalgList/AddUtvalgsToNewList?userName=${customerName}`;

        try {
          let kundeNummer = 0;
          let avtalenummer = 0;
          let customerName = "";
          if (username_kw) {
            customerName = username_kw;
          } else {
            customerName = "Internbruker";
          }
          kundeNummer = custNos;

          if (avtaleData) {
            avtalenummer = avtaleData;
          }

          var a = {
            listName: warninputvalue,
            customerName: username_kw,
            kundeNummer: kundeNummer,
            customerNo: kundeNummer,
            avtalenummer: avtalenummer,
            logo: "",
            utvalgs: [ActiveUtvalgObject],
            // memberUtvalgs: [ActiveUtvalgObject],
            // memberUtvalgs: [utvalgapiobject],
          };
          const { data, status } = await api.postdata(url, a);

          if (status === 200) {
            utvalgapiobject.listId = `${data.listId}`;
            setutvalglistapiobject(data);
            btnclose.current.click();
            setnewhome(true);
            setPage("");

            // window.$("#exampleModal-1").modal("dispose");
          }
        } catch (e) {
          //datadogLogs.logger.info("saveutvalgError", e);
          setloading(true);
          setmelding1(true);
          seterrormsg1("noe gikk galt. vennligst prøv etter en stund");
          console.log(e);
        }
        //    setPage("Geogra_distribution_click")
      }
    }
  };

  const warninput = () => {
    setmelding1(false);
    let textinput = document.getElementById("warntext").value;
    setwarninputvalue(textinput);
  };

  const Enterdescribtion = () => {
    let desc = document.getElementById("describtion").value;
    setdescribtion(desc);
    setpredesc(desc);
    setmelding(false);
  };

  useEffect(() => {
    setselection("");
    setdescribtion("");
  }, []);

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

  useEffect(() => {
    window.scroll(0, 0);
  }, []);

  const goback = () => {
    if (Page_P == "GeograVelg") {
      setActiveMapButton("");
      mapView.activeTool = null;
      setutvalgapiobject({});
      setselectedKoummeIDs([]);
      setSelectedRowKeys([]);
      setselectedrecord_s([]);
      setHouseholdSum(0);
      setBusinessSum(0);

      setPage("Geovelg");
      setbusinesscheckbox(false);
    }
    if (Page_P == "VeglGeografiskOmrade_kw") {
      setActiveMapButton("");
      mapView.activeTool = null;
      setutvalgapiobject({});
      setselectedKoummeIDs([]);
      setSelectedRowKeys([]);
      setselectedrecord_s([]);
      setHouseholdSum(0);
      setBusinessSum(0);

      // setutvalgapiobject({});
      setPage("VeglGeografiskOmrade_kw");
    }
    if (Page_P == "Demogra_velg_antall_click") {
      setActiveMapButton("");
      mapView.activeTool = null;
      setutvalgapiobject({});
      //setselectedKoummeIDs([]);
      setSelectedRowKeys([]);
      setselectedrecord_s([]);
      setHouseholdSum(0);
      setBusinessSum(0);

      setPage("Demogra_velg_antall_click");
    }
    if (Page_P == "Burdruter_velg_KW") {
      setActiveMapButton("");
      mapView.activeTool = null;
      setutvalgapiobject({});
      setselectedKoummeIDs([]);
      setSelectedRowKeys([]);
      setselectedrecord_s([]);
      setHouseholdSum(0);
      setBusinessSum(0);

      setPage("Budrutervelg");
    }
  };

  const CheckInput = async () => {
    setlagutvalg(true);
    await CommonFun("", "", true);
  };

  const LeggTilButtonClick = async () => {
    // setlistnaviagation(true);
    await CommonFun(true, "", "");
  };

  const BestillingenClick = async () => {
    // setlistnaviagation(true);
    await CommonFun(true, "", "");
  };

  const distribution_click = async () => {
    await CommonFun("", true, "");
  };

  const CommonFun = async (param1, param2, parm3) => {
    if (describtion == "") {
      setmelding(true);
      seterrormsg("Du må fylle inn «Beskrivelse av utvalget»");
    } else if (describtion.length < 3) {
      setmelding(true);
      seterrormsg("Beskrivelse av utvalget må ha minst 3 tegn.");
    } else if (selection == "" || selection.trim().length < 3) {
      setmelding(true);
      seterrormsg("Utvalgsnavnet må ha minst 3 tegn.");
    } else if (selection.indexOf(">") > -1 || selection.indexOf("<") > -1) {
      setmelding(true);
      seterrormsg("Should not conatain special character");
    } else {
      // setmelding(false);
      // setloading(true);

      const { data, status } = await api.getdata(
        `Utvalg/UtvalgNameExists?utvalgNavn=${selection}`
      );
      if (status === 200) {
        if (data == true) {
          seterrormsg(
            `Utvalget ${selection} eksisterer allerede. Velg et annet utvalgsnavn.`
          );
          setmelding(true);
          setloading(false);
        } else {
          let saveOldReoler = "false";
          let skipHistory = "false";
          let forceUtvalgListId = 0;
          let name = "";
          if (username_kw) {
            name = username_kw;
          } else {
            name = "Internbruker";
          }
          // let name = username_kw;
          let url = `Utvalg/SaveUtvalg?userName=${name}&`;
          url = url + `saveOldReoler=${saveOldReoler}&`;
          url = url + `skipHistory=${skipHistory}&`;
          url = url + `forceUtvalgListId=${forceUtvalgListId}`;
          try {
            let A = kundeweb_utvalg();

            utvalgapiobject["name"] = selection;
            utvalgapiobject["logo"] = describtion;
            utvalgapiobject.kundeNavn = name;
            A.name = selection;
            A.kundeNummer = custNos;
            if (avtaleData) {
              A.avtalenummer = avtaleData;
            }
            A.kundeNavn = name;
            // A.kundeNummer = custNos;
            // A.avtalenummer = avtaleData;
            A.logo = describtion;
            A.totalAntall = utvalgapiobject.totalAntall;
            A.reoler = utvalgapiobject.reoler;
            A.Antall = utvalgapiobject.Antall;
            // A.reoler[0].description = describtion;
            // A.reoler[0].antall.households = HouseholdSum;
            let criteriavalue = {};
            criteriavalue.criteriaType = criteriaObject?.enum;
            let str = "";
            if (criteriaObject?.enum === "12") {
              str =
                str +
                criteriaObject?.demograFeature +
                " :" +
                criteriaObject?.demograCheckedItems?.map((item, index) => {
                  return criteriaObject?.demograCheckedItems?.length ===
                    index + 1
                    ? item + " "
                    : item;
                });
            }
            if (criteriaObject?.enum === "2") {
              selectedsegment?.map((item) => {
                if (item === "A") {
                  str = str + "Senior Ordinær , ";
                } else if (item === "B") {
                  str = str + "Senior Aktiv , ";
                } else if (item === "C1") {
                  str = str + "Urban Ung , ";
                } else if (item === "C2") {
                  str = str + "Urban Moden , ";
                } else if (item === "D") {
                  str = str + "Ola og Kari Tradisjonell , ";
                } else if (item === "E") {
                  str = str + "Ola og Kari Individualist , ";
                } else if (item === "F") {
                  str = str + "Barnefamilie Velstand og Kultur , ";
                } else if (item === "G") {
                  str = str + "Barnefamilie Barnerik , ";
                } else if (item === "H") {
                  str = str + "Barnefamilie Prisbevisst , ";
                } else {
                  str = str + "Barnefamilie Moderne Aktiv , ";
                }
              });
            }
            if (
              criteriaObject?.enum !== "11" &&
              criteriaObject?.enum !== "19"
            ) {
              criteriavalue.criteria =
                str + "Kommuner: " + criteriaObject?.KommuneIds;
            } else if (criteriaObject?.enum === "11") {
              criteriavalue.criteria = criteriaObject?.BudruterFeature;
            } else if (criteriaObject?.enum === "19") {
              criteriavalue.criteria = "Geografi";
            }

            A.criterias.push(criteriavalue);
            A.listId = utvalgapiobject.listId;
            A.isBasis = utvalgapiobject.isBasis;
            A.receivers = utvalgapiobject.receivers;

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
              utvalgapiobject.utvalgId = data.utvalgId;
              setSavedUtvalg(data);
              let utvalgID = data.utvalgId;
              setUtvalgID(utvalgID);

              let url = `Utvalg/GetUtvalg?utvalgId=${data.utvalgId}`;
              try {
                const { data, status } = await api.getdata(url);
                let resultObject = [];
                resultObject.push(data);
                if (status == 200) {
                  setActiveUtvalgObject(data);
                  setloading(false);
                  if (utvalglistapiobject?.memberUtvalgs?.length) {
                    let customername = "";
                    if (username_kw) {
                      customername = username_kw;
                    } else {
                      customername = "Internbruker";
                    }

                    // utvalglistapiobject?.memberUtvalgs?.map((item) => {
                    //   var utvalgID = item.utvalgId.toString();
                    //   if (utvalgID.charAt(0) == "U") {
                    //     utvalgID = utvalgID.slice(1);
                    //   } else {
                    //     utvalgID = utvalgID;
                    //   }
                    //   item.utvalgId = utvalgID;

                    //   resultObject.push(item);
                    // });

                    let listUrl = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${utvalglistapiobject.listId}`;
                    const { data, status } = await api.getdata(listUrl);
                    if (status === 200 && data != undefined) {
                      let url = `UtvalgList/AddUtvalgsToExistingList?userName=${customername}`;
                      utvalgapiobject.listId = Number(`${data.listId}`);

                      var listData = data;
                      listData.kundeNummer = custNos;

                      try {
                        var a = {
                          utvalgList: listData,
                          utvalgs: resultObject,
                        };

                        const { data, status } = await api.postdata(url, a);
                        //if (status === 200) {
                        //  console.log(data, "success");
                        //}
                        resultObject.map((item) => {
                          listData.memberUtvalgs.push(item);
                        });
                        setutvalglistapiobject(listData);
                        setPage("cartClick_Component_kw");
                      } catch (error) {
                        console.log(error);
                      }
                    }
                  } else {
                    if (parm3) {
                      setPage("Geogra_distribution_tilbake_click");
                      setloading(false);
                    }
                    if (param1) {
                      //window.$("#exampleModal-1").modal("show");
                      setlistmodal(true);
                      setPage("Geogra_distribution_tilbake_click");
                    }
                    if (param2) {
                      if (Page_P === "GeograVelg") {
                        setPage_P("LagutvalgClick");
                        setPage("Geogra_distribution_click");
                        setloading(false);
                      } else {
                        setPage_P("LagutvalgClick");
                        setPage("Geogra_distribution_click");
                        setloading(false);
                      }
                    }
                  }

                  // setPage("Geogra_distribution_tilbake_click");
                  // setPage("Geogra_distribution_tilbake");
                }
              } catch (e) {
                console.log(e);
              }
            }
          } catch (e) {
            console.log(e);
          }
        }
      }
    }
    //}
  };
  
  const Enterselection = () => {
    let selectionText = document.getElementById("selection").value;
    setselection(selectionText);
    setpreselection(selectionText);
    setmelding(false);
  };

  return (
    // <HousholdContext.Consumer>
    <div className={loading ? "col-5 p-2   blur" : "col-5 p-2"}>
      {utvalglistapiobject.memberUtvalgs?.length && (
        <>
          <div className="padding_NoColor_B" style={{ cursor: "pointer" }}>
            <a
              id="uxHandlekurvSmall_uxLnkbtnHandlekurv"
              onClick={() => {
                if (utvalglistapiobject.memberUtvalgs?.length > 0) {
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
      <div className="padding_NoColor_B">
        <span className="title">Navngi og lagre utvalget</span>
      </div>
      <div>
        <div style={{ display: "" }}>
          <div className="padding_Color_L_R_T_B">
            <div className="AktivtUtvalg">
              <div className="AktivtUtvalgHeading">
                <span className="">Utvalg</span>
              </div>
            </div>
            {/* {Page_P == "Burdruter_velg_KW" ? (
              <p>{BudruterSelectedName}</p>
            ) : null} */}
            {melding ? (
              <div className="pr-3">
                <div className="error WarningSign">
                  <div className="divErrorHeading">Melding:</div>
                  <span
                    id="uxKjoreAnalyse_uxLblMessage"
                    className="divErrorText_kw"
                  >
                    {describtion == "" && selection == "" ? (
                      <p>
                        {" "}
                        Feltene «Gi utvalget et navn» og «Beskrivelse av
                        utvalget» må fylles ut
                      </p>
                    ) : (
                      errormsg
                    )}
                  </span>
                </div>
              </div>
            ) : null}
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
                      {/* {NumberFormat(HouseholdSum)} */}
                    </span>
                  ) : (
                    <span
                      id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                      className="divValueTextBold div_right"
                    >
                      {NumberFormat(utvalgapiobject.Antall[0])}
                      {/* {NumberFormat(HouseholdSum)} */}
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
                      {/* {NumberFormat(BusinessSum)} */}
                    </span>
                  ) : (
                    <span
                      id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                      className="divValueTextBold div_right"
                    >
                      {NumberFormat(utvalgapiobject.Antall[1])}
                      {/* {NumberFormat(BusinessSum)} */}
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
                    {/* {NumberFormat(HouseholdSum + BusinessSum)} */}
                  </span>
                ) : (
                  <span
                    id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                    className="divValueTextBold div_right pr-4"
                  >
                    {NumberFormat(
                      utvalgapiobject.Antall[0] + utvalgapiobject.Antall[1]
                    )}
                    {/* {NumberFormat(HouseholdSum + BusinessSum)} */}
                  </span>
                )
              ) : householdcheckbox ? (
                Page_P == "GeograVelg" ? (
                  <span
                    id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                    className="divValueTextBold div_right pr-4"
                  >
                    {NumberFormat(utvalgapiobject.Antall[0])}
                    {/* {NumberFormat(HouseholdSum)} */}
                  </span>
                ) : (
                  <span
                    id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                    className="divValueTextBold div_right pr-4"
                  >
                    {NumberFormat(utvalgapiobject.Antall[0])}
                    {/* {NumberFormat(HouseholdSum)} */}
                  </span>
                )
              ) : businesscheckbox ? (
                Page_P == "GeograVelg" ? (
                  <span
                    id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                    className="divValueTextBold div_right pr-4"
                  >
                    {NumberFormat(utvalgapiobject.Antall[1])}
                    {/* {NumberFormat(BusinessSum)} */}
                  </span>
                ) : (
                  <span
                    id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                    className="divValueTextBold div_right pr-4"
                  >
                    {NumberFormat(utvalgapiobject.Antall[1])}
                    {/* {NumberFormat(BusinessSum)} */}
                  </span>
                )
              ) : null}
            </div>

            <div className="padding_NoColor_T clearFloat">
              <div className="bold">Gi utvalget et navn</div>
              <div>
                <input
                  style={{ width: "22rem" }}
                  type="text"
                  placeholder=""
                  value={selection}
                  maxLength="50"
                  id="selection"
                  onChange={Enterselection}
                  className="divValueText"
                />
              </div>
              <div className="gray">
                Navnet bør inneholde firmanavnet samt et kjennetegn slik at du
                klarer gjenfinne det senere.
              </div>
            </div>

            <div className="padding_NoColor_T clearFloat">
              <div className="bold">Beskrivelse av utvalget</div>
              <div>
                <input
                  type="text"
                  style={{ width: "22rem" }}
                  maxLength="50"
                  id="describtion"
                  value={describtion}
                  className="divValueText"
                  onChange={Enterdescribtion}
                />
              </div>
              <div className="gray">
                Beskrivelsen må inneholde referanse til navnet som er angitt på
                sendingen i tillegg til avsender.{" "}
              </div>
            </div>

            <div className="paddingBig_NoColor_T clearFloat">
              <div>
                <table style={{ width: "100px" }}>
                  <tbody>
                    <tr>
                      <td style={{ width: "80px" }}></td>
                      <td style={{ width: "160px" }}>
                        <div className="div_right"></div>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
          <div className="padding_NoColor_T paddingBig_NoColor_B clearFloat">
            <div className="div_left">
              <input
                type="submit"
                value="Tilbake"
                onClick={() => {
                  goback();
                }}
                className="KSPU_button_Gray"
              />
              <br />

              <div className="padding_NoColor_T">
                <a className="KSPU_LinkButton_Url_KW pl-2" onClick={GotoMain}>
                  Avbryt
                </a>
              </div>
            </div>
            <div className="div_right">
              <div>
                <input
                  type="submit"
                  value="Angi distribusjonsdetaljer"
                  onClick={() => {
                    distribution_click();
                  }}
                  id="uxGeografiAnalyse_ShowUtvalgDetails1_uxLagreDistribusjon"
                  className="KSPU_button-kw div_right"
                  style={{
                    width: "175px",
                    display:
                      utvalglistapiobject?.memberUtvalgs?.length > 0
                        ? "none"
                        : "block",
                  }}
                />

                <input
                  type="submit"
                  value="Legg til utvalget i bestillingen"
                  onClick={() => {
                    BestillingenClick();
                  }}
                  id="uxGeografiAnalyse_ShowUtvalgDetails1_uxLagreDistribusjon"
                  className="KSPU_button-kw div_right"
                  style={{
                    minWidth: "175px",
                    display:
                      utvalglistapiobject?.memberUtvalgs?.length > 0
                        ? "block"
                        : "none",
                  }}
                />

                <input
                  type="submit"
                  name="uxGeografiAnalyse$ShowUtvalgDetails1$uxLagreKopiDialog"
                  value="Lagre kopi"
                  onClick="hideFloatingPanel('uxDialogs_uxConnectList_uxListekoblingWindow', false, null);hideFloatingPanel('uxDialogs_uxListReportCriteria_uxRapportKriterierListe', false, null);hideFloatingPanel('uxDialogs_uxReportCriteria_uxRapportKriterier', false, null);hideFloatingPanel('uxDialogs_uxSaveListLogo_uxSaveLogoWindow', false, null);hideFloatingPanel('uxDialogs_uxSaveLogo_uxSaveLogoWindow', false, null);hideFloatingPanel('uxDialogs_uxSaveUtvalgAnonymously_uxSaveWindow', false, null);hideFloatingPanel('uxDialogs_uxCustomerSearchResult_uxCustomerResultWindow', false, null);hideFloatingPanel('uxDialogs_uxLagrePanelMultiple_uxSaveWindow', false, null);hideFloatingPanel('uxDialogs_uxConnectUtvalgListToUtvalgList_uxDialogWindow', false, null);hideFloatingPanel('uxDialogs_uxListSearchResult_uxFindListResultsWindow', false, null);hideFloatingPanel('uxDialogs_uxLagreListe_uxLagreListe', false, null);hideFloatingPanel('uxDialogs_uxCopyList_uxLagreListeSom', false, null);hideFloatingPanel('uxDialogs_uxBasedOn_uxCampaignsPanel', false, null);hideFloatingPanel('uxDialogs_uxCreateCampaignList_uxCreateCampaignPanel', false, null);hideFloatingPanel('uxDialogs_uxLagrePanelCampaign_uxSaveWindowCampaign', false, null);hideFloatingPanel('uxDialogs_uxListReportCriteriaDistr_uxRapportKriterierListeDistr', false, null);showFloatingPanel('uxDialogs_uxSaveUtvalg_uxSaveWindow', false, null);;return false;WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;uxGeografiAnalyse$ShowUtvalgDetails1$uxLagreKopiDialog&quot;, &quot;&quot;, true, &quot;&quot;, &quot;&quot;, false, false))"
                  id="uxGeografiAnalyse_ShowUtvalgDetails1_uxLagreKopiDialog"
                  className="KSPU_button"
                  style={{ width: "150px", display: "none" }}
                />

                <span
                  id="uxGeografiAnalyse_ShowUtvalgDetails1_lblhdnCopyUtvalg"
                  className="divValueTextBold div_right"
                  style={{ display: "none" }}
                >
                  0
                </span>
              </div>
              <div>
                <input
                  type="submit"
                  name="uxGeografiAnalyse$ShowUtvalgDetails1$uxAddMoreToList"
                  value="Legg til flere utvalg"
                  onClick={() => {
                    LeggTilButtonClick();
                  }}
                  id="uxGeografiAnalyse_ShowUtvalgDetails1_uxAddMoreToList"
                  className="KSPU_button_Gray div_right clearFloat"
                  style={{
                    width: "150px",
                    marginTop: "10px",
                    display:
                      utvalglistapiobject?.memberUtvalgs?.length > 0
                        ? "none"
                        : "block",
                  }}
                  // data-toggle="modal"
                  // data-target="#exampleModal-1"
                />
              </div>
            </div>
          </div>
          {/* selection modal  starting*/}
          <div
            className="modal fade  bd-example-modal-lg"
            id="exampleModal-1"
            tabIndex="-1"
            data-backdrop="false"
            role="dialog"
            aria-labelledby="exampleModalCenterTitle"
            aria-hidden="true"
          >
            {/* <div className="modal-dialog modal-dialog-centered  " role="document"> */}
            <div className="modal-dialog " role="document">
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
                    {melding1 ? (
                      <span className=" sok-Alert-text pl-1">{errormsg1}</span>
                    ) : null}
                    {melding1 ? <p></p> : null}

                    <input
                      type="text"
                      maxLength="50"
                      placeholder="Navn på bestil"
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
                          data-dismiss="modal"
                          className="KSPU_button_Gray"
                        />
                        &nbsp;&nbsp;
                        <input
                          type="submit"
                          name="uxDistribusjon$uxDistSetDelivery"
                          value="Lagre"
                          onClick={LagreClick}
                          id="uxDistribusjon_uxDistSetDelivery"
                          className="KSPU_button-kw"
                          // data-dismiss="modal"
                        />
                      </div>
                      <div className="padding_NoColor_T clearFloat sok-text">
                        Du kan senere finne igjen bestillingen ved å klikke{" "}
                        <br />
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
          </div>{" "}
          {/* ---------selection modal ending --------- */}
          <table className="wizUnfilled paddingBig_NoColor_T clearFloat">
            <tbody>
              <tr>
                <td className="bold">
                  <span id="uxGeografiAnalyse_ShowUtvalgDetails1_uxLblMoreFunc">
                    Du kan også..
                  </span>
                </td>
              </tr>
              <tr>
                <td>
                  <a
                    onClick={() => {
                      CheckInput();
                    }}
                    id="uxGeografiAnalyse_ShowUtvalgDetails1_uxLagreUtvalg"
                    className="read-more"
                    href='javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions("uxGeografiAnalyse$ShowUtvalgDetails1$uxLagreUtvalg", "", true, "", "", false, true))'
                    style={{ fontSize: "12px" }}
                  >
                    Lagre utvalget
                  </a>
                </td>
              </tr>
              <tr>
                <td></td>
              </tr>

              <tr>
                <td>
                  {/* <a onClick="showWaitAnimation();" id="uxGeografiAnalyse_ShowUtvalgDetails1_uxAddMoreToList_ORG" className="read-more" href="javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;uxGeografiAnalyse$ShowUtvalgDetails1$uxAddMoreToList_ORG&quot;, &quot;&quot;, true, &quot;&quot;, &quot;&quot;, false, true))">Legg andre utvalg til bestillingen</a> */}
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
    //   </HousholdContext.Consumer>
  );
}

export default LagutvalgClick;
