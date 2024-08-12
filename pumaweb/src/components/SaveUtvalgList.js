import React, { useState, useRef, useContext, useEffect } from "react";
import { KSPUContext, UtvalgContext } from "../context/Context.js";
import api from "../services/api.js";
import { v4 as uuidv4 } from "uuid";

import Swal from "sweetalert2";
import $ from "jquery";
import { Buffer } from "buffer";

import loadingImage from "../assets/images/callbackActivityIndicator.gif";

function SaveUtvalgList(props) {
  const { errormsg, seterrormsg } = useContext(KSPUContext);
  const { issave, setissave } = useContext(KSPUContext);
  const [buttonDisable, setButtonDisable] = useState(true);
  const {
    setActivUtvalglist,
    activUtvalglist,

    setAktivDisplay,
  } = useContext(KSPUContext);

  const [displayMsg, setdisplayMsg] = useState(false);
  const [eConnectResult, seteConnectResult] = useState([]);
  const [name, setname] = useState("");
  const [kunde_name, setkunde_name] = useState(activUtvalglist?.kundeNummer);

  const [kunde_number, setkunde_number] = useState(
    activUtvalglist?.kundeNummer
  );
  const [logo, set_logo] = useState("");
  const [test, settest] = useState(false);
  const [showmodel, setshowmodel] = useState(false);
  const btnClose = useRef();
  const [listDate, setListDate] = useState(true);
  const [selectionCriteria, setselectionCriteria] = useState(0);

  const [listDateTime, setListDateTime] = useState(false);
  const [listCustom, setListCustomText] = useState(false);
  const [loading, setloading] = useState(false);

  const { setshoworklist, showorklist } = useContext(KSPUContext);

  const FirstInputText = useRef();
  const SecondInputText = useRef();
  const ThirdInputText = useRef();

  const EnterName = () => {
    let name_utvalg = document.getElementById("utvalgnavn").value;
    setname(name_utvalg);
    setdisplayMsg(false);
  };
  const EnterKundeNumber = () => {
    let kundeNumber = document.getElementById("uxKunde").value;
    setkunde_name(kundeNumber);
    setkunde_number(kundeNumber);
  };
  const CustomText = () => {
    let logo = document.getElementById("uxText").value;
    set_logo(logo);
  };

  const velgCustomer = (e) => {
    // let value = e.target.id;
    // const answer_array = value.split(",");

    // setkunde_number(answer_array[1]);
    // setkunde_name(answer_array[0]);

    setkunde_number(Number(e.target.attributes['customerid'].value));
    setkunde_name(e.target.attributes['customername'].value);
  };
  const handleKeypress = (e) => {
    //it triggers by pressing the enter key
    if (e.key === "Enter") {
      FinCustomerNo();
    }
  };

  const FinCustomerNo = async () => {
    window.$("#showCustomerList").modal("show");
    let uniqueId = uuidv4();
    let eConnectUrl = `ECPuma/FindCustomer380`;
    setloading(true);

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

        if (data.length == 0) {
          seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
          setdisplayMsg(true);
          setloading(false);
        } else {
          setdisplayMsg(false);

          await seteConnectResult(data);

          setshowmodel(true);
          setloading(false);
        }
      } catch (error) {
        console.error("error : " + error);
        seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
        setdisplayMsg(true);
        setloading(false);
      }
    }
  };

  const uxSaveUtvalgList = async (event) => {
    if (name == "" || name.trim().length < 3) {
      setdisplayMsg(true);
      seterrormsg("Beskrivelse av utvalget må ha minst 3 tegn.");
    } else if (name.indexOf(">") > -1 || name.indexOf("<") > -1) {
      setdisplayMsg(true);
      seterrormsg("Should not conatain special character");
    } else {
      setdisplayMsg(false);
      const { data, status } = await api.getdata(
        `UtvalgList/UtvalgListNameExists?utvalglistname=${encodeURIComponent(
          name
        )}`
      );
      if (status === 200) {
        if (data == true) {
          let msg = `Utvalget ${name} eksisterer allerede. Velg et annet utvalgsnavn.`;
          seterrormsg(msg);
          setdisplayMsg(true);
        } else {
          // call to save fnctionality

          let url = `UtvalgList/CreateCopyOfUtvalgList?userName=Internbruker`;

          try {
            setloading(true);

            let postRequest = {
              listId: activUtvalglist.listId,

              selectionCriteria: selectionCriteria,
              kundeNumber: kunde_number,
              listName: name,

              customText: logo,
            };

            const { data, status } = await api.postdata(url, postRequest);
            //let status = 200;
            if (status === 200) {
              btnClose.current.click();

              let activ = showorklist;
              activ.push(data);
              setshoworklist(activ);

              await setActivUtvalglist(data);
              setissave(true);
              settest(true);

              // $(".modal").remove();
              // $(".modal-backdrop").remove();
              setloading(false);
              let msg = `Listen "${data.name}" er opprettet.`;

              Swal.fire({
                text: msg,
                confirmButtonColor: "#7bc144",
                confirmButtonText: "Lukk",
              });

              setAktivDisplay(false);
              setAktivDisplay(true);
            } else {
              seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
              setdisplayMsg(true);
              setissave(false);
              setloading(false);
            }
          } catch (error) {
            console.error("error : " + error);
            seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
            setdisplayMsg(true);
            setissave(false);
            setloading(false);
          }
        }
      } else {
        console.error("error : " + status);
        seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
        setdisplayMsg(true);
      }
    }
  };
  const renderPerson = (result, index) => {
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
  };
  const onlyDate = (e) => {
    setListCustomText(false);
    setListDateTime(false);
    setListDate(true);
    setdisplayMsg(false);
    setButtonDisable(true);
    setselectionCriteria(0);
  };
  const dateWithTime = (e) => {
    setListCustomText(false);
    setListDateTime(true);
    setListDate(false);
    setdisplayMsg(false);
    setButtonDisable(true);
    setselectionCriteria(1);
  };
  const customText = (e) => {
    setListCustomText(true);
    setListDateTime(false);
    setListDate(false);
    setdisplayMsg(false);
    setButtonDisable(false);
    setselectionCriteria(2);
  };
  return (
    <div>
      <div>
        <div
          className="modal fade bd-example-modal-lg"
          data-backdrop="false"
          id="showCustomerList"
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
            <div className="modal-content" style={{ border: "black 3px solid" }}>
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
                LAGRE UTVALG
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
                        value={kunde_number ? kunde_number : null}
                        name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxKunde"
                        type="text"
                        id="uxKunde"
                        className="selection-input ml-1"
                        onChange={EnterKundeNumber}
                        placeholder=""
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
                        data-target="#showCustomerList"
                        onClick={FinCustomerNo}
                        id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxFindKunde"
                      />
                    </td>
                  </tr>
                </tbody>
              </table>
              <div className="row SaveUtvaldivLabelText pl-3 mt-2">
                <p>De nye utvalgene skal ha navn som ender med:</p>
              </div>
              <div className="row pl-2">
                <div className="col divValueText ">
                  <input
                    type="radio"
                    //ref={notshowReserver}
                    onChange={onlyDate}
                    value=""
                    checked={listDate}
                  />
                  &nbsp;Dagens dato
                </div>
              </div>
              <div className="row pl-2">
                <div className="col divValueText ">
                  <input
                    type="radio"
                    //ref={notshowReserver}
                    onChange={dateWithTime}
                    value=""
                    checked={listDateTime}
                  />
                  &nbsp;Dagens dato og klokkeslett
                </div>
              </div>
              <div className="row pl-2">
                <div className="col divValueText ">
                  <input
                    type="radio"
                    //ref={notshowReserver}
                    onChange={customText}
                    value=""
                    checked={listCustom}
                  />
                  &nbsp;Egendefinert tekst
                </div>
              </div>

              <div className="row">
                <div className="col">
                  <span
                    id="uxDialogs_uxSaveUtvalg_uxSaveWindow_uxTextLabel"
                    className="SaveUtvaldivLabelText pl-2"
                  >
                    Egendefinert tekst
                  </span>
                </div>

                <div className="col">
                  <input
                    ref={ThirdInputText}
                    className="selection-input ml-1"
                    name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxText"
                    type="text"
                    id="uxText"
                    disabled={buttonDisable}
                    onChange={CustomText}
                    placeholder=""
                  />
                </div>
              </div>
              <br />
              <table>
                <tbody>
                  <tr>
                    <td>
                      <button
                        type="button"
                        className="btn KSPU_button"
                        data-dismiss="modal"
                      >
                        Avbryt
                      </button>
                    </td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td>
                      <button
                        type="button"
                        onClick={uxSaveUtvalgList}
                        className="btn KSPU_button"
                      >
                        Lagre
                      </button>
                    </td>
                    <td>
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
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default SaveUtvalgList;
