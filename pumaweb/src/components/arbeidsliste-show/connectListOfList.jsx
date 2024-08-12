import React, { useState, useRef, useContext, useEffect } from "react";
import { KSPUContext, UtvalgContext } from "../../context/Context.js";
import api from "../../services/api.js";
import { v4 as uuidv4 } from "uuid";

import swal from "sweetalert";
import Swal from "sweetalert2";
import $ from "jquery";
import { Buffer } from "buffer";
import loadingImage from "../../assets/images/callbackActivityIndicator.gif";
import Messagepopup from "../message-popup/messagepopup.jsx";

function Connectlisttolist(props) {
  const { errormsg, seterrormsg } = useContext(KSPUContext);
  const { issave, setissave } = useContext(KSPUContext);
  const { resultData, setResultData } = useContext(KSPUContext);
  const {
    activUtvalg,
    setActivUtvalg,
    setshoworklist,
    showorklist,
    setAktivDisplay,
    activUtvalglist,
    setActivUtvalglist,
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

  const [newList, setNewList] = useState(false);
  const [oldList, setOldList] = useState(false);
  const [listDelete, setListDelete] = useState(false);
  const btnClose = useRef();
  const [showmodel, setshowmodel] = useState(false);
  const [showSavemodel, setshowSavemodel] = useState(false);
  const [showListmodel, setshowListmodel] = useState(false);
  const [loading, setloading] = useState(false);

  const FirstInputText = useRef();
  const SecondInputText = useRef();
  const ThirdInputText = useRef();
  const ListNameInputText = useRef();
  const handleKeypress = (e) => {
    //it triggers by pressing the enter key
    if (e.key === "Enter") {
      handleSearchList();
    }
  };

  const handleKeypressKundeNumber = (e) => {
    //it triggers by pressing the enter key
    if (e.key === "Enter") {
      FinSaveUtvalg();
    }
  };
  useEffect(() => {
    if (
      props.listOfList.parentList !== "undefined" &&
      props.listOfList.parentList !== null
    ) {
      setOldList(false);
      setNewList(false);
      setListDelete(true);
    } else {
      setNewList(false);
      setListDelete(false);
      setOldList(true);
    }
    if (props.listOfList.kundeNummer) {
      setkunde_name(props.listOfList.kundeNummer);
    }
  }, []);
  const showmodel_3 = (e) => {
    setOldList(false);
    setNewList(false);
    setListDelete(true);
    setdisplayMsg(false);
  };
  const showmodel_2 = (e) => {
    setOldList(false);
    setListDelete(false);
    setdisplayMsg(false);
    // setshow(true);
    setNewList(true);
  };
  const showmodel_1 = (e) => {
    setListDelete(false);

    setNewList(false);
    setOldList(true);
    setdisplayMsg(false);
    // setshow(false);
  };
  const EnterName = () => {
    let name_utvalg = FirstInputText.current.value;
    setname(name_utvalg);
    setdisplayMsg(false);
  };
  const OnListNameChange = () => {
    //let name_utvalg = document.getElementById("utvalglistnavn").value;
    let name_utvalg = ListNameInputText.current.value;
    setListname(name_utvalg);
    setdisplayMsg(false);
  };
  const EnterKundeNumber = () => {
    setshowmodel(false);
    let kundeNumber = document.getElementById("uxKunde19").value;
    //let kundeNumber = SecondInputText.current.value;
    setkunde_name(kundeNumber);
  };
  const Enterlogo = () => {
    //let logo = document.getElementById("uxLogo").value;
    let logo = ThirdInputText.current.value;
    set_logo(logo);
  };
  const uxSaveUtvalg = async (event) => {
    if (newList) {
      if (name === "" || name.trim().length < 3) {
        setdisplayMsg(true);
        seterrormsg("Beskrivelse av utvalget må ha minst 3 tegn.");
      } else if (name.indexOf(">") > -1 || name.indexOf("<") > -1) {
        setdisplayMsg(true);
        seterrormsg("Should not conatain special character");
      }
      // else if (activUtvalglist?.parentList) {
      //   //debugger;
      //   setdisplayMsg(true);
      //   seterrormsg(
      //     "Denne listen kan ikke kobles til liste, da dette gir en struktur med lister i tre nivåer."
      //   );
      // }
      else if (
        name.indexOf(" ", 0, 1) < -1 ||
        name.indexOf(" ", 1, name.length - 1) < -1
      ) {
        setdisplayMsg(true);
        seterrormsg("Beskrivelse av utvalget må ha minst 3 tegn1233.");
      } else {
        setdisplayMsg(false);
        const { data, status } = await api.getdata(
          `UtvalgList/UtvalgListNameExists?utvalglistname=${encodeURIComponent(
            name
          )}`
        );
        if (status === 200) {
          if (data === true) {
            let msg = `Utvalget ${name} eksisterer allerede. Velg et annet utvalgsnavn.`;
            seterrormsg(msg);
            setdisplayMsg(true);
          } else {
            let url = `UtvalgList/CreateNewParentForList?userName=Internbruker`;
            let postRequest = "";
            let oldParentListId = props.listOfList?.parentList?.listId;
            if (props.listOfList !== undefined) {
              postRequest = {
                newParentName: name,
                //customerName: kunde_name,
                newParentCustomer: kunde_number,
                newParentLogo: logo,

                listToBeChanged: props.listOfList,
              };
            }
            try {
              const { data, status } = await api.postdata(url, postRequest);
              if (status === 200) {
                if (data.item1.length !== 0 && data.item1 !== undefined) {
                  btnClose.current.click();
                  let activ = showorklist;
                  activ.map((listItem) => {
                    if (listItem.listId === oldParentListId) {
                      listItem.antall =
                        listItem.antall - props.listOfList.antall;
                    }
                  });
                  activUtvalglist.parentList = data.item1;
                  activ?.map((item) => {
                    if (item.listId !== data.item1.listId) {
                      if (item.memberLists.length) {
                        item.memberLists = item?.memberLists.filter((list) => {
                          return list.listId !== props.listOfList.listId;
                        });
                      }
                    }
                  });
                  if (props.listOfList !== undefined) {
                    var newActiv = activ.filter((item) => {
                      return item.listId !== props.listOfList.listId;
                    });

                    setshoworklist(newActiv.concat(data.item1));
                  }

                  setissave(true);

                  setAktivDisplay(false);
                  setAktivDisplay(true);
                  // $(".modal").remove();
                  // $(".modal-backdrop").remove();
                  setloading(false);
                  let msg = data.item2;

                  Swal.fire({
                    text: msg,
                    confirmButtonColor: "#7bc144",
                    confirmButtonText: "Lukk",
                  });
                  //swal(data.item2);
                } else {
                  setdisplayMsg(true);
                  seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
                  setissave(false);
                }
              } else {
                setdisplayMsg(true);
                seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
                setissave(false);
              }
            } catch (error) {
              console.error("error : " + error);
              setdisplayMsg(true);
              seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
              setissave(false);
            }
          }
        } else {
          console.error("error : " + status);
          seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
          setdisplayMsg(true);
        }
      }
    } else if (oldList) {
      if (Listname === "") {
        setdisplayMsg(true);
        seterrormsg("List Name shall not be empty");
      } else if (listOrdreType === "1") {
        setdisplayMsg(true);
        seterrormsg(
          "Du forsøker nå å koble utvalg til en liste i ordre. Annuller eller lås opp ordre før endring."
        );
      } else {
        setdisplayMsg(false);
        const { data, status } = await api.getdata(
          `UtvalgList/UtvalgListNameExists?utvalglistname=${encodeURIComponent(
            Listname
          )}`
        );
        if (status === 200) {
          if (data === true && listId !== "") {
            let listUrl = `UtvalgList/GetUtvalgListSimple?listId=${listId}`;
            const { data, status } = await api.getdata(listUrl);
            if (status === 200 && data !== undefined) {
              //Once smit add flag for memberList then we have uncomment this code
              // if (data?.memberLists) {
              if (data.parentListId === 0) {
                let url = `UtvalgList/ChangeParentListOfList?userName=Internbruker`;
                let postRequest = "";
                var listData = data;
                let oldParentListId = 0;
                //var listDataId = data.listId
                if (props.listOfList !== undefined) {
                  if (listData.memberLists !== undefined) {
                    listData.memberLists = [];
                  }
                  postRequest = {
                    newParentList: listData,

                    listToBeChanged: props.listOfList,
                  };
                  if (activUtvalglist?.parentList) {
                    if (props.listOfList.memberLists !== undefined) {
                      props.listOfList.memberLists = [];
                    }
                    if (props.listOfList.parentList !== undefined) {
                      oldParentListId = props.listOfList.parentList.listId;
                      props.listOfList.parentList = null;
                    }
                  }
                }

                try {
                  api.postdata(url, postRequest).then((response) => {
                    if (response.status === 200) {
                      if (
                        response.data.item1.length !== 0 &&
                        response.data.item1 !== undefined
                      ) {
                        // await setActivUtvalglist({});
                        //debugger;
                        btnClose.current.click();
                        activUtvalglist.parentList = listData;
                        let activ = showorklist;
                        activ.map((listItem) => {
                          if (listItem.listId === oldParentListId) {
                            listItem.antall =
                              listItem.antall - props.listOfList.antall;
                          }
                        });
                        //debugger;
                        // $(".modal").remove();
                        // $(".modal-backdrop").remove();
                        setloading(false);
                        let msg = response.data.item2;

                        Swal.fire({
                          text: msg,
                          confirmButtonColor: "#7bc144",
                          confirmButtonText: "Lukk",
                        });
                        // swal(data.item2);
                        if (props.listOfList !== undefined) {
                          var newActiv = [];
                          activ?.map((item) => {
                            if (item.listId !== response.data.item1.listId) {
                              if (item.memberLists.length) {
                                item.memberLists = item?.memberLists.filter(
                                  (list) => {
                                    return (
                                      list.listId !== props.listOfList.listId
                                    );
                                  }
                                );
                              }
                            }
                          });
                          activ.map((item) => {
                            if (
                              item.listId !== props.listOfList.listId &&
                              item.listId !== JSON.parse(listId)
                            ) {
                              newActiv.push(item);
                            }
                          });
                        }
                        setshoworklist(newActiv.concat(response.data.item1));
                        // let newlistUrl = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${listId}`;
                        // api.getdata(newlistUrl).then((res) => {
                        //   if (res.status === 200 && res.data != undefined) {
                        //     setshoworklist(newActiv.concat(res.data));
                        //   }
                        // });

                        // await setActivUtvalglist(data.item1);
                        setissave(true);

                        setAktivDisplay(false);
                        setAktivDisplay(true);
                      } else {
                        setdisplayMsg(true);
                        seterrormsg(
                          "Oppgitte søkekriterier ga ikke noe resultat."
                        );
                        setissave(false);
                      }
                    } else {
                      setdisplayMsg(true);
                      seterrormsg(
                        "Oppgitte søkekriterier ga ikke noe resultat."
                      );
                      setissave(false);
                    }
                  }); //5 sec
                } catch (error) {
                  console.error("error : " + error);
                  setdisplayMsg(true);
                  seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
                  setissave(false);
                }
                //Once smit add flag for memberList then we have uncomment this code
                // } else {
                //   setdisplayMsg(true);
                //   seterrormsg(
                //     "Denne listen kan ikke kobles til liste, da dette gir en struktur med lister i tre nivåer."
                //   );
                // }
              } else {
                setdisplayMsg(true);
                seterrormsg(
                  "Denne listen kan ikke kobles til liste, da dette gir en struktur med lister i tre nivåer."
                );
              }
            }
          } else {
            let msg = `List ${Listname} does not exist.`;
            setdisplayMsg(true);
            seterrormsg(msg);
          }
        } else {
          let msg = `List ${Listname} does not exist.`;
          seterrormsg(msg);
          setdisplayMsg(true);
        }
      }
    } else {
      setdisplayMsg(false);

      let url = `UtvalgList/ChangeParentListOfList?userName=Internbruker`;
      let postRequest = "";
      let oldParentListId = 0;
      if (props.listOfList !== undefined) {
        if (props.listOfList.memberLists !== undefined) {
          props.listOfList.memberLists = [];
        }
        if (props.listOfList.parentList !== undefined) {
          oldParentListId = props.listOfList.parentList.listId;
          props.listOfList.parentList = null;
        }

        postRequest = {
          newParentList: null,

          listToBeChanged: props.listOfList,
        };
      }

      try {
        const { data, status } = await api.postdata(url, postRequest);
        if (status === 200) {
          if (data.item1.length !== 0 && data.item1 !== undefined) {
            //debugger;
            let activ = showorklist;
            var newActiv = activ.filter((item) => {
              return item.listId !== oldParentListId;
            });
            let flag = false;
            if (newActiv.length > 0) {
              var newWorklist = newActiv.filter((item) => {
                return item.listId !== props.listOfList.listId;
              });

              await setshoworklist(newWorklist.concat(data.item1));
              flag = true;
            } else {
              await setshoworklist(newActiv.concat(data.item1));
              flag = true;
            }
            if (flag) {
              let newlistUrl = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${oldParentListId}`;
              const { data, status } = await api.getdata(newlistUrl);
              if (status === 200 && data !== undefined) {
                setshoworklist((showorklist) => [...showorklist, data]);
              }
            }
            // if (props.listOfList.parentList !== undefined) {
            //   var newActiv = activ.filter((item) => {
            //     return item.listId !== props.listOfList.parentList.listId;
            //   });

            // }
            setissave(true);

            // $(".modal").remove();
            // $(".modal-backdrop").remove();
            setloading(false);
            let msg = data.item2;
            Swal.fire({
              text: msg,
              confirmButtonColor: "#7bc144",
              confirmButtonText: "Lukk",
            });
            // swal(data.item2);
            btnClose.current.click();
            setAktivDisplay(false);
            setAktivDisplay(true);
          } else {
            setdisplayMsg(true);
            seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
            setissave(false);
          }
        } else {
          setdisplayMsg(true);
          seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
          setissave(false);
        }
      } catch (error) {
        console.error("error : " + error);
        setdisplayMsg(true);
        seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
        setissave(false);
      }
    }
  };

  const velgCustomer = (e) => {
    // let value = e.target.id;
    // const answer_array = value.split(",");
    // // setkunde_number(value);
    // setkunde_number(answer_array[1]);
    // setkunde_name(answer_array[0]);
    
    // SecondInputText.current.value = answer_array[1];

    setkunde_number(Number(e.target.attributes['customerid'].value));
    setkunde_name(e.target.attributes['customername'].value);
    SecondInputText.current.value = Number(e.target.attributes['customerid'].value);
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
  const FinSaveUtvalg = async () => {
    let kunde_nummer = kunde_name;
    setdisplayMsg(false);
    if (kunde_nummer === "" || kunde_nummer.trim().length < 3) {
      setdisplayMsg(true);
      seterrormsg("Oppgi minst 3 tegn i søkefeltet.");
      setkunde_name(props.listOfList.kundeNummer);
      document.getElementById("uxKunde19").value = props.listOfList.kundeNummer;
    } else {
      if (kunde_nummer !== props.listOfList.kundeNummer) {
        setkunde_name(props.listOfList.kundeNummer);
        document.getElementById("uxKunde19").value =
          props.listOfList.kundeNummer;
        kunde_nummer = props.listOfList.kundeNummer;
      }
      window.$("#showCustomerNewList").modal("show");
      await seteConnectResult([]);
      setshowmodel(false);
      let uniqueId = uuidv4();
      let eConnectUrl = `ECPuma/FindCustomer380`;
      setloading(true);
      //setshowmodel(true);
      if (kunde_nummer) {
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
        if (!isNaN(+kunde_nummer)) {
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
            Kundenummer: kunde_nummer,
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
            Navn: kunde_nummer,
            MaksRader: "100",
          };
        }

        try {
          const { data, status } = await api.postdata(
            eConnectUrl,
            eConnectHeader
          );
          if (data.kundedata.length === 0) {
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
            // if (!data.kundedata.kundenummer) {
            //   seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
            //   setdisplayMsg(true);
            // }
          }
        } catch (error) {
          console.error("error : " + error);
          seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
          setdisplayMsg(true);
          setloading(false);
        }
      }
    }
  };

  const handleSearchList = async () => {
    window.$("#showListNewList").modal("show");
    await setListResult([]);

    let listUrl = `UtvalgList/SearchUtvalgListSimpleByIsBasis?utvalglistname=${encodeURIComponent(
      Listname
    )}&searchMethod=1&`;
    if (props.isbasis) {
      listUrl = listUrl + `onlyBasisLists=${1}&`;
    } else {
      listUrl = listUrl + `onlyBasisLists=${0}&`;
    }
    listUrl = listUrl + `isBasedOn=${false}`;
    setloading(true);
    try {
      const { data, status } = await api.getdata(listUrl);
      if (data.length === 0) {
        seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
        setdisplayMsg(true);
        setloading(false);
      } else {
        //seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.")
        setdisplayMsg(false);
        //let data1 = [];
        //data1.push(data);
        //let KundeNumber1=data1[0].Kundedata.Kundenummer

        let filteredData = data.filter((item) => {
          return item?.basedOn === 0;
        });

        if (oldList) {
          filteredData = filteredData.filter((item) => {
            return item.kundeNummer === props.listOfList.kundeNummer;
          });
          if (filteredData.length === 0) {
            seterrormsg(
              "Ingen lister ble funnet. Søkeresultatet er begrenset til kun lovlige lister som kan benyttes for å koble til valgte utvalg/utvalgsliste, se Hjelp for detaljer."
            );
            setdisplayMsg(true);
            setshowListmodel(false);
            setloading(false);
          } else {
            setListResult(filteredData);
            setshowListmodel(true);
            setloading(false);
          }
        } else {
          setListResult(filteredData);
          //setAlert(false);
          // setshowListmodel(true);
          setloading(false);
          // if (!data.kundedata.kundenummer) {
          //   seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
          //   setdisplayMsg(true);
          // }
        }
      }
    } catch (error) {
      console.error("error : " + error);
      seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
      setdisplayMsg(true);
      setloading(false);
    }
  };

  const renderList = (result, index) => {
    if (listResult.length) {
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
    }
  };

  const renderPerson = (result, index) => {
    if (eConnectResult.kundedata) {
      return eConnectResult.kundedata.map((item) => (
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
    } else {
      return null;
    }
  };
  return (
    <div>
      {showSavemodel === true ? (
        <Messagepopup headerText="KVITTERING" bodyText={successMessage} />
      ) : null}
      <div>
        <div
          className="modal fade bd-example-modal-lg"
          data-backdrop="false"
          id="showListNewList"
          tabIndex="-1"
          role="dialog"
          aria-labelledby="exampleModalCenterTitle"
          style={{ zIndex: "1051", height: "auto" }}
          aria-hidden="true"
        >
          <div
            className="modal-dialog modal-dialog-centered width-70"
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
                  <h3 className="flykecontent">
                    Søkeresultatet er begrenset til kun lovlige
                  </h3>
                  <h3 className="flykecontent">
                    lister som kan benyttes for å koble til valgte
                  </h3>
                  <h3 className="flykecontent">utvalg/utvalgsliste.</h3>
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
          id="showCustomerNewList"
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
          className="modal-dialog modal-dialog-centered width-70"
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
                {props.listOfList.parentList !== undefined &&
                props.listOfList.parentList !== null &&
                props.listOfList.parentList.listId !== undefined &&
                props.listOfList.parentList.listId !== 0 &&
                props.listOfList.parentList.listId !== "" &&
                props.listOfList.parentList.listId !== "0" ? (
                  <div>
                    <label className="radio-inline sok-text">
                      <input
                        type="radio"
                        //ref={notshowReserver}
                        onChange={showmodel_3}
                        value=""
                        checked={listDelete}
                      />
                      &nbsp;Ingen liste
                    </label>
                  </div>
                ) : null}
                <label className="radio-inline sok-text">
                  <input
                    type="radio"
                    //ref={notshowReserver}
                    onChange={showmodel_1}
                    value=""
                    checked={oldList}
                  />
                  &nbsp;Legg i eksisterende liste
                </label>
                {oldList ? (
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
                        onKeyPress={handleKeypress}
                      />
                    </div>
                    <input
                      type="submit"
                      className="KSPU_button ml-2 mt-1"
                      value="Finn"
                      data-toggle="modal"
                      data-target="#showListNewList"
                      id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxFindKunde_List"
                      onClick={handleSearchList}
                    />
                  </div>
                ) : null}
                <div className="radio-inline sok-text pt-2 pb-1">
                  <span>Valgt liste:</span>
                  <div className="pb-1 pt-1">
                    <input
                      type="radio"
                      //ref={notshowReserver}
                      onChange={showmodel_2}
                      value=""
                      checked={newList}
                    />
                    &nbsp;Opprett i ny liste
                  </div>
                </div>
              </div>
              {newList ? (
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
                            id="uxKunde19"
                            className="selection-input ml-1"
                            defaultValue={props.listOfList.kundeNummer}
                            onChange={EnterKundeNumber}
                            placeholder=""
                            onKeyPress={handleKeypressKundeNumber}
                          />
                        </td>
                        <td className="pl-2">
                          <input
                            type="submit"
                            className="KSPU_button"
                            name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxFindKunde"
                            value="Finn"
                            data-toggle="modal"
                            data-target="#showCustomerNewList"
                            onClick={FinSaveUtvalg}
                            id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxFindKunde_showCustomerlist"
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
                            // placeholder={activUtvalg.logo}
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
                    id="btnSaveUtvalgToListofList"
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

export default Connectlisttolist;
