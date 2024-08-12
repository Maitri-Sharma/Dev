import React, { useState, useRef, useContext, useEffect } from "react";
import { KSPUContext, UtvalgContext } from "../../context/Context.js";
import api from "../../services/api.js";
import { v4 as uuidv4 } from "uuid";
import Swal from "sweetalert2";
import $ from "jquery";
import loadingImage from "../../assets/images/callbackActivityIndicator.gif";
import Messagepopup from "../message-popup/messagepopup.jsx";
import { filterCommonReolIds } from "../../common/Functions";
function ConnectSelectiontolist(props) {
  const { errormsg, seterrormsg } = useContext(KSPUContext);
  const { issave, setissave } = useContext(KSPUContext);
  const {
    activUtvalg,
    setActivUtvalg,
    setshoworklist,
    showorklist,
    setAktivDisplay,
    activUtvalglist,
    setActivUtvalglist,
    setutvalglistcheck,
    setExpandListId,
    setCheckedList,
    setDemografieDisplay,
    setSegmenterDisplay,
    setAddresslisteDisplay,
    setGeografiDisplay,
    setKjDisplay,
    setAdresDisplay,
    setvalue,
  } = useContext(KSPUContext);

  const [displayMsg, setdisplayMsg] = useState(false);
  const [eConnectResult, seteConnectResult] = useState([]);
  const [listResult, setListResult] = useState([]);
  const [name, setname] = useState("");
  const [Listname, setListname] = useState("");
  const [listOrdreType, setListOrdreType] = useState("");
  const [kunde_name, setkunde_name] = useState("");
  const [successMessage, setSuccessMessage] = useState("");

  const [kunde_number, setkunde_number] = useState("");
  const [listId, setListId] = useState("");
  const [logo, set_logo] = useState("");
  const [show, setshow] = useState(false);
  const btnClose = useRef();
  const [showmodel, setshowmodel] = useState(false);
  const [showSavemodel, setshowSavemodel] = useState(false);
  const [showListmodel, setshowListmodel] = useState(false);
  const [loading, setloading] = useState(false);
  const FirstInputText = useRef();
  const SecondInputText = useRef();
  const ThirdInputText = useRef();
  const ListNameInputText = useRef();

  const EnterName = () => {
    let name_utvalg = FirstInputText.current.value;
    setname(name_utvalg);
    setdisplayMsg(false);
  };
  const OnListNameChange = () => {
    let name_utvalg = ListNameInputText.current.value;
    setListname(name_utvalg);
    setdisplayMsg(false);
  };
  const EnterKundeNumber = () => {
    // setshowmodel(false);
    let kundeNumber = document.getElementById("uxKundeNummber").value;

    setkunde_name(kundeNumber);
  };
  const Enterlogo = () => {
    let logo = ThirdInputText.current.value;
    set_logo(logo);
  };
  const uxSaveUtvalg = async (event) => {
    if (show) {
      if (name === "" || name.trim().length < 3) {
        setdisplayMsg(true);
        seterrormsg("Beskrivelse av utvalget må ha minst 3 tegn.");
      } else if (name.indexOf(">") > -1 || name.indexOf("<") > -1) {
        setdisplayMsg(true);
        seterrormsg("Should not conatain special character");
      } else if (
        name.indexOf(" ", 0, 1) < -1 ||
        name.indexOf(" ", 1, name.length - 1) < -1
      ) {
        setdisplayMsg(true);
        seterrormsg("Beskrivelse av utvalget må ha minst 3 tegn1233.");
      } else {
        document.getElementById("btnSaveUtvalgToList").disabled = true;
        setdisplayMsg(false);
        const { data, status } = await api.getdata(
          `UtvalgList/UtvalgListNameExists?utvalglistname=${encodeURIComponent(
            name
          )}`
        );
        if (status === 200) {
          if (data === true) {
            let msg = `Utvalget ${name} eksisterer allerede. Velg et annet utvalgsnavn.`;
            document.getElementById("btnSaveUtvalgToList").disabled = false;
            seterrormsg(msg);
            setdisplayMsg(true);
          } else {
            let url = `UtvalgList/AddUtvalgsToNewList?userName=Internbruker`;
            let postRequest = "";
            if (props.list !== undefined) {
              let customerNumberFlag = false;
              let kundeNumberMessage = "";
              props.list.map((item) => {
                if (item.kundeNummer !== 0 && item.kundeNummer !== "") {
                  if (item.kundeNummer.toString() !== kunde_number.toString()) {
                    kundeNumberMessage = item.kundeNummer;
                    customerNumberFlag = true;
                  } 
                }
              });
              if (customerNumberFlag) {
                let msg = `Samling av utvalg i liste ble avbrutt da utvalg i listen tilhører forskjellige kundenummer. 
                 Dersom utvalgslisten hadde blitt oppdatert ville den hatt følgende fordeling av kundenummer og utvalg:
                 Kundenummer "${kundeNumberMessage}": "${name}"
og utvalgslisten ville hatt følgende kundenummer: "${kunde_number}"`;
                setSuccessMessage(msg);
                setshowSavemodel(true);
                document.getElementById("btnSaveUtvalgToList").disabled = false;
                setloading(false);
              } else {
                postRequest = {
                  listName: name,
                  customerName: kunde_name,
                  customerNo: kunde_number,
                  logo: logo,

                  utvalgs: props.list,
                };

                try {
                  const { data, status } = await api.postdata(url, postRequest);
                  if (status === 200) {
                    document.getElementById(
                      "btnSaveUtvalgToList"
                    ).disabled = false;
                    btnClose.current.click();
                    let activ = showorklist;

                    if (props.list !== undefined) {
                      activ = activ.filter(function (obj) {
                        return props.list.indexOf(obj) === -1;
                      });

                      // props.list.map((item) => {
                      //   item.listId = data.listId;
                      //   data.memberUtvalgs.push(item);
                      // });
                    }

                    activ.push(data);
                    setshoworklist(activ);
                    if (Object.keys(activUtvalg).length !== 0) {
                      if (props.list?.utvalgId === activUtvalg?.utvalgId) {
                        activUtvalg.listId = `${data.listId}`;
                        activUtvalg.listName = `${data.name}`;
                      }
                    }

                    setissave(true);
                    if (data?.memberUtvalgs) {
                      var doubleCoverageItems = filterCommonReolIds(
                        data?.memberUtvalgs
                      );
                    }

                    let msg = "";
                    if (
                      doubleCoverageItems?.filteredCommonSelectionNames
                        ?.length > 1
                    ) {
                      let commonRuteCount = 0;
                      if (
                        doubleCoverageItems?.filteredCommonItems?.length > 0
                      ) {
                        doubleCoverageItems?.filteredCommonItems?.map(
                          (item) => {
                            commonRuteCount = commonRuteCount + 1;
                          }
                        );
                      }
                      msg = `Utvalg er koblet til liste "${name}".
                      Det er dobbeltdekning på ${commonRuteCount} ruter på denne utvalgslisten."${doubleCoverageItems?.filteredCommonSelectionNames?.map(
                        (item) => {
                          return " " + item;
                        }
                      )}" `;
                    } else {
                      msg = `Utvalg er koblet til liste "${name}".`;
                    }
                    await setActivUtvalglist(data);
                    setCheckedList([]);
                    setutvalglistcheck(true);
                    setActivUtvalg({});
                    // setAktivDisplay(false);
                    setDemografieDisplay(false);
                    setSegmenterDisplay(false);
                    setAddresslisteDisplay(false);
                    setGeografiDisplay(false);
                    setKjDisplay(false);
                    setAdresDisplay(false);
                    setvalue(false);
                    setAktivDisplay(true);

                    Swal.fire({
                      text: msg,
                      confirmButtonColor: "#7bc144",
                      confirmButtonText: "Lukk",
                    });
                    setloading(false);
                  } else {
                    setdisplayMsg(true);
                    seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
                    document.getElementById(
                      "btnSaveUtvalgToList"
                    ).disabled = false;
                    setissave(false);
                  }
                } catch (error) {
                  console.error("error : " + error);
                  setdisplayMsg(true);
                  seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
                  document.getElementById(
                    "btnSaveUtvalgToList"
                  ).disabled = false;
                  setissave(false);
                }
              }
            }
          }
        } else {
          console.error("error : " + status);
          seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
          document.getElementById("btnSaveUtvalgToList").disabled = false;
          setdisplayMsg(true);
        }
      }
    } else {
      if (Listname === "") {
        setdisplayMsg(true);
        seterrormsg("Ingen liste er valgt.");
      } else if (listOrdreType === "1") {
        setdisplayMsg(true);
        seterrormsg(
          "Du forsøker nå å koble utvalg til en liste i ordre. Annuller eller lås opp ordre før endring."
        );
      } else {
        setdisplayMsg(false);
        document.getElementById("btnSaveUtvalgToList").disabled = true;
        const { data, status } = await api.getdata(
          `UtvalgList/UtvalgListNameExists?utvalglistname=${encodeURIComponent(
            Listname
          )}`
        );
        if (status === 200) {
          if (data === true && listId !== "") {
            let listUrl = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${listId}`;
            const { data, status } = await api.getdata(listUrl);
            if (status === 200 && data !== undefined) {
              props.parentCallback(props.list);
              let url = `UtvalgList/AddUtvalgsToExistingList?userName=Internbruker`;
              let customerNumberFlag = false;
              let kundeNumberMessage = "";
              props.list.map((item) => {
                if (item.kundeNummer !== 0 && item.kundeNummer !== "") {
                  if (
                    item?.kundeNummer.toString() !==
                    data?.kundeNummer.toString()
                  ) {
                    kundeNumberMessage = item.kundeNummer;
                    customerNumberFlag = true;
                  }
                }
              });
              if (customerNumberFlag) {
                let msg = `Samling av utvalg i liste ble avbrutt da utvalg i listen tilhører forskjellige kundenummer. 
                 Dersom utvalgslisten hadde blitt oppdatert ville den hatt følgende fordeling av kundenummer og utvalg:
                 Kundenummer "${kundeNumberMessage}": "${data.name}"
og utvalgslisten ville hatt følgende kundenummer: "${data.kundeNummer}"`;
                setSuccessMessage(msg);
                setshowSavemodel(true);
                document.getElementById("btnSaveUtvalgToList").disabled = false;
                setloading(false);
              } else {
                props.list.map((item) => {
                  data.memberUtvalgs.push(item);
                });

                var listData = data;
                try {
                  var a = {
                    utvalgList: listData,
                    utvalgs: props.list,
                  };
                  if (listData?.memberUtvalgs) {
                    var doubleCoverageItems = filterCommonReolIds(
                      listData?.memberUtvalgs
                    );
                  }
                  const { data, status } = await api.postdata(url, a);
                  if (status === 200) {
                    setloading(true);
                    document.getElementById(
                      "btnSaveUtvalgToList"
                    ).disabled = false;
                    btnClose.current.click();
                    if (Object.keys(activUtvalg).length !== 0) {
                      if (props.list?.utvalgId === activUtvalg?.utvalgId) {
                        activUtvalg.listId = listData.listId;
                        activUtvalg.listName = listData.name;
                      }
                    }

                    // var workListArray = showorklist.filter(function (obj) {
                    //   return props.list.indexOf(obj) === -1;
                    // });

                    let Arr = props.list?.map((item) => {
                      return item?.utvalgId;
                    });
                    var workListArray = showorklist.filter((item) => {
                      return !Arr?.includes(item?.utvalgId);
                    });

                    let arr = [];
                    arr?.push(listData?.listId?.toString());
                    setExpandListId(arr);
                    let newWorklistArray = [];
                    let listIdForWorklist = listData.listId;
                    workListArray.map((item) => {
                      if (
                        item.listId?.toString() !== listData.listId?.toString()
                      ) {
                        let flag = false;
                        if (item?.memberLists?.length > 0) {
                          item?.memberLists.map((newItemMember) => {
                            if (
                              newItemMember.listId?.toString() ===
                              listData.listId?.toString()
                            ) {
                              flag = true;
                            }
                          });
                          if (!flag) {
                            newWorklistArray.push(item);
                          } else {
                            listIdForWorklist = item.listId;
                          }
                        } else {
                          newWorklistArray.push(item);
                        }
                      } else {
                        newWorklistArray.push(item);
                      }
                    });
                    var newActiv = newWorklistArray.filter((item) => {
                      return item.listId !== listData.listId;
                    });
                    let newlistUrl = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${listIdForWorklist}`;
                    const { data, status } = await api.getdata(newlistUrl);
                    if (status === 200 && data !== undefined) {
                      await setshoworklist(newActiv.concat(data));
                      // if (data?.memberUtvalgs) {
                      //   var doubleCoverageItems = filterCommonReolIds(
                      //     data?.memberUtvalgs
                      //   );
                      // }
                      if (
                        activUtvalglist?.listId?.toString() ===
                        data?.listId?.toString()
                      ) {
                        // let obj = await CreateUtvalglist(data);
                        setActivUtvalglist(data);
                      } else {
                        if (activUtvalglist?.listId) {
                          let apiCallUrl = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${activUtvalglist?.listId}`;
                          const { data, status } = await api.getdata(
                            apiCallUrl
                          );
                          if (status === 200 && data !== undefined) {
                            // let obj = await CreateUtvalglist(data);
                            await setActivUtvalglist(data);
                          }
                        }
                      }
                    }

                    // await setActivUtvalglist(data);
                    if (activUtvalglist?.listId) {
                      setutvalglistcheck(true);
                      setActivUtvalg({});
                    } else {
                      setutvalglistcheck(false);
                    }

                    // setAktivDisplay(false);
                    setDemografieDisplay(false);
                    setSegmenterDisplay(false);
                    setAddresslisteDisplay(false);
                    setGeografiDisplay(false);
                    setKjDisplay(false);
                    setAdresDisplay(false);
                    setvalue(false);
                    setAktivDisplay(true);
                    let msg = "";
                    if (
                      doubleCoverageItems?.filteredCommonSelectionNames
                        ?.length > 1
                    ) {
                      let commonRuteCount = 0;
                      if (
                        doubleCoverageItems?.filteredCommonItems?.length > 0
                      ) {
                        doubleCoverageItems?.filteredCommonItems?.map(
                          (item) => {
                            commonRuteCount = commonRuteCount + 1;
                          }
                        );
                      }
                      msg = `Utvalg er koblet til liste "${listData.name}".
                      Det er dobbeltdekning på ${commonRuteCount} ruter på denne utvalgslisten. "${doubleCoverageItems?.filteredCommonSelectionNames?.map(
                        (item) => {
                          return " " + item;
                        }
                      )}" `;
                    } else {
                      msg = `Utvalg er koblet til liste "${listData.name}".`;
                    }
                    setCheckedList([]);
                    // $(".modal").remove();
                    // $(".modal-backdrop").remove();
                    Swal.fire({
                      text: msg,
                      confirmButtonColor: "#7bc144",
                      confirmButtonText: "Lukk",
                    });
                    setloading(false);
                    setissave(true);
                  } else {
                    setdisplayMsg(true);
                    document.getElementById(
                      "btnSaveUtvalgToList"
                    ).disabled = false;
                    seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
                    setissave(false);
                  }
                } catch (error) {
                  console.error("error : " + error);
                  document.getElementById(
                    "btnSaveUtvalgToList"
                  ).disabled = false;
                  setdisplayMsg(true);
                  seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
                  setissave(false);
                }
              }
            } else {
              let msg = `List ${Listname} does not exist.`;
              document.getElementById("btnSaveUtvalgToList").disabled = false;
              setdisplayMsg(true);
              seterrormsg(msg);
            }
          } else {
            let msg = `List ${Listname} does not exist.`;
            document.getElementById("btnSaveUtvalgToList").disabled = false;
            seterrormsg(msg);
            setdisplayMsg(true);
          }
        }
      }
    }
  };

  const showmodel_2 = (e) => {
    setshow(true);
  };
  const showmodel_1 = (e) => {
    setshow(false);
  };
  const velgCustomer = (e) => {
    // let value = e.target.id;
    // const answer_array = value.split(",");
    // // setkunde_number(value);
    // setkunde_number(answer_array[1]);
    // setkunde_name(answer_array[0]);
    // SecondInputText.current.value = answer_array[1];

    setkunde_number(Number(e.target.attributes["customerid"].value));
    setkunde_name(e.target.attributes["customername"].value);
    SecondInputText.current.value = Number(
      e.target.attributes["customerid"].value
    );
  };
  const velgList = (e) => {
    let value = e.target.id;
    const answer_array = value.split(";");
    // setkunde_number(value);
    setListId(answer_array[0]);
    setListname(answer_array[1]);
    setListOrdreType(answer_array[2]);
    ListNameInputText.current.value = answer_array[1];
  };
  const handleKeypress = (e) => {
    //it triggers by pressing the enter key
    if (e.key === "Enter") {
      FinSaveUtvalg();
    }
  };
  const FinSaveUtvalg = async () => {
    document.getElementById("btnSaveUtvalgToList").disabled = true;
    window.$("#showCustomerNew").modal("show");
    // await seteConnectResult([]);
    // setshowmodel(false);
    let uniqueId = uuidv4();
    let eConnectUrl = `ECPuma/FindCustomer380`;
    setloading(true);
    //setshowmodel(true);
    if (kunde_name) {
      let eConnectHeader = {
        Header: {
          SystemCode: "Analytiker",
          MessageId: uniqueId,
          SecurityToken: null,
          UserName: null,
          Version: null,
          Timestamp: null,
        },
        Aktornummer: null,
        Kundenummer: null,
        Navn: null,
        MaksRader: "100",
      };
      if (!isNaN(+kunde_name)) {
        eConnectHeader = {
          Header: {
            SystemCode: "Analytiker",
            MessageId: uniqueId,
            SecurityToken: null,
            UserName: null,
            Version: null,
            Timestamp: null,
          },
          Aktornummer: null,
          Kundenummer: kunde_name,
          Navn: null,
          MaksRader: "100",
        };
      } else {
        eConnectHeader = {
          Header: {
            SystemCode: "Analytiker",
            MessageId: uniqueId,
            SecurityToken: null,
            UserName: null,
            Version: null,
            Timestamp: null,
          },
          Aktornummer: null,
          Kundenummer: null,
          Navn: kunde_name,
          MaksRader: "100",
        };
      }

      try {
        const { data, status } = await api.postdata(
          eConnectUrl,
          eConnectHeader
        );
        if (data.kundedata.length === 0) {
          document.getElementById("btnSaveUtvalgToList").disabled = false;
          seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
          setdisplayMsg(true);
          setloading(false);
        } else {
          //seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.")
          setdisplayMsg(false);
          //let data1 = [];
          //data1.push(data);
          //let KundeNumber1=data1[0].Kundedata.Kundenummer

          await seteConnectResult(data);

          //setAlert(false);
          setshowmodel(true);
          setloading(false);
          document.getElementById("btnSaveUtvalgToList").disabled = false;
          // if (!data.kundedata.kundenummer) {
          //   seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
          //   setdisplayMsg(true);
          // }
        }
      } catch (error) {
        console.error("error : " + error);
        document.getElementById("btnSaveUtvalgToList").disabled = false;
        seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
        setdisplayMsg(true);
        setloading(false);
      }
    }
  };

  const handleKeypressUtvalgName = (e) => {
    //it triggers by pressing the enter key
    if (e.key === "Enter") {
      handleSearchList();
    }
  };

  const handleSearchList = async () => {
    window.$("#showListNew").modal("show");
    document.getElementById("btnSaveUtvalgToList").disabled = true;
    await setListResult([]);

    let listUrl = `UtvalgList/SearchUtvalgListSimpleByIsBasis?utvalglistname=${encodeURIComponent(
      Listname
    )}&`;
    if (props.basicListFlag) {
      listUrl = listUrl + `onlyBasisLists=${1}&`;
    } else {
      listUrl = listUrl + `onlyBasisLists=${0}&`;
    }
    listUrl = listUrl + `isBasedOn=${false}&`;
    listUrl = listUrl + `searchMethod=1`;
    setloading(true);
    try {
      const { data, status } = await api.getdata(listUrl);
      if (data.length === 0) {
        seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
        document.getElementById("btnSaveUtvalgToList").disabled = false;
        setdisplayMsg(true);
        setloading(false);
      } else {
        setdisplayMsg(false);

        await setListResult(data);

        setshowListmodel(true);
        setloading(false);
        document.getElementById("btnSaveUtvalgToList").disabled = false;
      }
    } catch (error) {
      console.error("error : " + error);
      document.getElementById("btnSaveUtvalgToList").disabled = false;
      seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
      setdisplayMsg(true);
      setloading(false);
    }
  };

  const renderList = (result, index) => {
    return listResult.map((item) => (
      <tr key={index}>
        <th className="tabledataRow">
          {" "}
          <tr>
            <td className="flykecontent">
              <tr>
                <td className="flykecontent">{item.name}</td>
              </tr>
            </td>
          </tr>
        </th>

        <th className="tabledataRow">
          {" "}
          <tr>
            <td className="flykecontent">
              <p
                id={item.listId + ";" + item.name + ";" + item.ordreType}
                data-dismiss="modal"
                className="KSPU_LinkButton float-right mr-1"
                onClick={velgList}
              >
                velg
              </p>
            </td>
          </tr>
        </th>
      </tr>
    ));
  };

  const renderPerson = (result, index) => {
    return eConnectResult?.kundedata.map((item) => (
      <tr key={index}>
        <th className="tabledataRow">
          {" "}
          <tr>
            <td className="flykecontent">
              <tr>
                <td className="flykecontent">{item.juridisknavn}</td>
              </tr>
            </td>
          </tr>
        </th>

        <th className="tabledataRow">
          {" "}
          <tr>
            <td className="flykecontent">
              <tr>
                <td className="flykecontent">{item.kundenummer}</td>
              </tr>
            </td>
          </tr>
        </th>

        <th className="tabledataRow">
          {" "}
          <tr>
            <td className="flykecontent">
              <p
                id={item.juridisknavn + "," + item.kundenummer}
                customername={item.juridisknavn}
                customerid={item.kundenummer}
                data-dismiss="modal"
                className="KSPU_LinkButton float-right mr-1"
                onClick={velgCustomer}
              >
                velg
              </p>
            </td>
          </tr>
        </th>
      </tr>
    ));
  };
  return (
    <div>
      {showSavemodel === true ? (
        <Messagepopup
          headerText="Ulovlig listekoblingsforsøk"
          bodyText={successMessage}
        />
      ) : null}
      <div>
        <div
          className="modal fade bd-example-modal-lg"
          data-backdrop="false"
          id="showListNew"
          tabIndex="-1"
          role="dialog"
          aria-labelledby="exampleModalCenterTitle"
          style={{ zIndex: "1051", height: "auto" }}
          aria-hidden="true"
        >
          <div
            className="modal-dialog modal-dialog-centered viewDetail"
            role="document"
          >
            <div
              className="modal-content"
              style={{ border: "black 3px solid", height: "auto" }}
            >
              <div className="Common-modal-header">
                <span
                  className="common-modal-title pt-1 pl-2"
                  id="exampleModalLongTitle"
                >
                  SØKERESULTAT
                </span>
                <button
                  type="button"
                  className="close"
                  data-dismiss="modal"
                  aria-label="Close"
                  //ref={btnClose}
                >
                  <span aria-hidden="true">&times;</span>
                </button>
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
              ) : (
                <div className="View_modal-body budrutebody">
                  {displayMsg ? (
                    <span className=" addList-Alert-text">{errormsg}</span>
                  ) : null}
                  <p className="flykecontent">
                    Søkeresultatet er begrenset til kun lovlige lister som kan
                    benyttes for å koble til valgte utvalg/utvalgsliste.
                  </p>
                  <table className="tableRow" style={{ height: "auto" }}>
                    <thead>
                      <tr className="flykeHeader">
                        <th className="tabledataRow budruteRow">Navn</th>
                        <th className="tabledataRow budruteRow">
                          &nbsp;&nbsp;&nbsp;&nbsp;
                        </th>
                      </tr>
                    </thead>
                    <tbody>{showListmodel ? renderList() : null}</tbody>
                  </table>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>

      <div>
        <div
          className="modal fade bd-example-modal-lg"
          data-backdrop="false"
          id="showCustomerNew"
          tabIndex="-1"
          role="dialog"
          aria-labelledby="exampleModalCenterTitle"
          style={{ zIndex: "1051" }}
          aria-hidden="true"
        >
          <div
            className="modal-dialog modal-dialog-centered viewDetail"
            role="document"
          >
            <div
              className="modal-content"
              style={{ border: "black 3px solid" }}
            >
              <div className="Common-modal-header">
                <span
                  className="common-modal-title pt-1 pl-2"
                  id="exampleModalLongTitle"
                >
                  SØKERESULTAT
                </span>
                <button
                  type="button"
                  className="close"
                  data-dismiss="modal"
                  aria-label="Close"
                  //ref={btnClose}
                >
                  <span aria-hidden="true">&times;</span>
                </button>
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
              ) : (
                <div className="View_modal-body budrutebody">
                  {displayMsg ? (
                    <span className=" sok-Alert-text pl-1">{errormsg}</span>
                  ) : null}
                  <table className="tableRow">
                    <thead>
                      <tr className="flykeHeader">
                        <th className="tabledataRow budruteRow">Kundenavn</th>
                        <th className="tabledataRow budruteRow">Kundenummer</th>
                        <th className="tabledataRow budruteRow">
                          &nbsp;&nbsp;&nbsp;&nbsp;
                        </th>
                      </tr>
                    </thead>
                    <tbody>{showmodel ? renderPerson() : null}</tbody>
                  </table>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
      {/* <!-- Modal --> */}
      <div
        className="modal fade bd-example-modal-lg"
        data-backdrop="false"
        id={props.id}
        tabIndex="-1"
        role="dialog"
        aria-labelledby="exampleModalCenterTitle"
        aria-hidden="true"
      >
        <div
          className="modal-dialog modal-dialog-centered viewDetail"
          role="document"
        >
          <div className="modal-content" style={{ border: "black 3px solid" }}>
            <div className="Common-modal-header">
              <span
                className="common-modal-title pt-1 pl-2"
                id="exampleModalLongTitle"
              >
                {props.name}
              </span>
              <button
                type="button"
                className="close"
                data-dismiss="modal"
                aria-label="Close"
                ref={btnClose}
              >
                <span aria-hidden="true">&times;</span>
              </button>
            </div>
            <div className="View_modal-body pl-2">
              {displayMsg ? (
                <span className=" sok-Alert-text pl-1">{errormsg}</span>
              ) : null}
              <div>
                <label className="radio-inline sok-text">
                  <input
                    type="radio"
                    //ref={notshowReserver}
                    onChange={showmodel_1}
                    value=""
                    checked={!show}
                  />
                  &nbsp;Legg i eksisterende liste
                </label>
                {!show ? (
                  <div className="row pl-3">
                    <div className="input-groupco-4">
                      <i className="fa fa-user-circle-o pl-1"></i>
                      <input
                        ref={ListNameInputText}
                        type="text"
                        className="InputValueText mt-1"
                        id="utvalglistnavn"
                        placeholder=""
                        onChange={OnListNameChange}
                        onKeyPress={handleKeypressUtvalgName}
                      />
                    </div>
                    <input
                      type="submit"
                      className="KSPU_button ml-2 mt-1"
                      value="Finn"
                      data-toggle="modal"
                      data-target="#showListNew"
                      id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxFindKunde"
                      onClick={handleSearchList}
                    />
                  </div>
                ) : null}
                <div className="radio-inline sok-text pt-2 pb-1">
                  <span>Valgt liste:</span>
                  <div className="pb-1 pt-1">
                    <input
                      type="radio"
                      onChange={showmodel_2}
                      value=""
                      checked={show}
                    />
                    &nbsp;Opprett i ny liste
                  </div>
                </div>
              </div>
              {show ? (
                <div>
                  <table>
                    <tbody>
                      <tr>
                        <td>
                          <span
                            id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxNameLabel"
                            className="SaveUtvaldivLabelText pl-2"
                          >
                            Navn
                          </span>
                        </td>
                        <td>
                          <input
                            ref={FirstInputText}
                            name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxName"
                            type="text"
                            id="utvalgnavn"
                            onChange={EnterName}
                            className="selection-input ml-1"
                            placeholder=""
                            maxLength="50"
                          />
                        </td>
                      </tr>
                      <tr>
                        <td>
                          <span
                            id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxKundeLabel"
                            className="SaveUtvaldivLabelText pl-2"
                          >
                            Kundenr/navn
                          </span>
                        </td>
                        <td>
                          <input
                            ref={SecondInputText}
                            name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxKunde_kn"
                            type="text"
                            id="uxKundeNummber"
                            className="selection-input ml-1"
                            onChange={EnterKundeNumber}
                            placeholder={kunde_name}
                            onKeyPress={handleKeypress}
                          />
                        </td>
                        <td className="pl-2">
                          <input
                            type="submit"
                            className="KSPU_button"
                            name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxFindKunde"
                            value="Finn"
                            data-toggle="modal"
                            data-target="#showCustomerNew"
                            onClick={FinSaveUtvalg}
                            id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxFindKunde_showCustomer"
                            disabled={kunde_name.length < 3 ? true : false}
                          />
                        </td>
                      </tr>
                      <tr>
                        <td>
                          <span
                            id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxLogoLabel"
                            className="SaveUtvaldivLabelText pl-2"
                          >
                            Forhandlerpåtrykk
                          </span>
                        </td>
                        <td>
                          <input
                            ref={ThirdInputText}
                            className="selection-input ml-1"
                            name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxLogo"
                            type="text"
                            id="uxLogo"
                            onChange={Enterlogo}
                          />
                        </td>
                      </tr>
                      <br />
                    </tbody>
                  </table>
                </div>
              ) : null}
              <div className="row">
                <div className="col-5">
                  <button
                    type="button"
                    className="btn KSPU_button"
                    data-dismiss="modal"
                  >
                    Avbryt
                  </button>
                </div>
                <div className="col-2"></div>
                <div className="col-4 mr-3">
                  <button
                    type="button"
                    id="btnSaveUtvalgToList"
                    onClick={uxSaveUtvalg}
                    className="btn KSPU_button"
                  >
                    Lagre
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default ConnectSelectiontolist;
