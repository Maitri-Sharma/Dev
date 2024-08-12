import React, { useState, useRef, useContext, useEffect } from "react";
import { KundeWebContext } from "../context/Context.js";
import api from "../services/api.js";
import { v4 as uuidv4 } from "uuid";

import Swal from "sweetalert2";

import loadingImage from "../assets/images/callbackActivityIndicator.gif";

function SaveUtvalgListKW(props) {
  const [errormsg, seterrormsg] = useState("");
  const [buttonDisable, setButtonDisable] = useState(true);
  const { utvalglistapiobject, setutvalglistapiobject } =
    useContext(KundeWebContext);

  const [displayMsg, setdisplayMsg] = useState(false);
  const [name, setname] = useState("");

  const [kunde_number] = useState(utvalglistapiobject?.kundeNummer);
  const [logo, set_logo] = useState("");
  const btnClose = useRef();
  const [listDate, setListDate] = useState(true);
  const [selectionCriteria, setselectionCriteria] = useState(0);

  const [listDateTime, setListDateTime] = useState(false);
  const [listCustom, setListCustomText] = useState(false);
  const [loading, setloading] = useState(false);

  const FirstInputText = useRef();
  const ThirdInputText = useRef();

  const EnterName = () => {
    let name_utvalg = document.getElementById("utvalgnavn").value;
    setname(name_utvalg);
    setdisplayMsg(false);
  };

  const CustomText = () => {
    let logo = document.getElementById("uxText").value;
    set_logo(logo);
  };

  const uxSaveUtvalgList = async (event) => {
    // let flag = 0;
    // utvalglistapiobject?.memberUtvalgs?.map((item) => {
    //   if (item.antallBeforeRecreation > 0) {
    //     flag = 1;
    //   }
    // });

    // if (flag) {
    //   await props.AcceptAllChanges();
    //   saveAsNewList();
    // } else {
    //   saveAsNewList();
    // }
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
          let url = `UtvalgList/CreateCopyOfUtvalgList?userName=Internbruker`;

          try {
            setloading(true);

            let postRequest = {
              listId: utvalglistapiobject.listId,

              selectionCriteria: selectionCriteria,
              kundeNumber: kunde_number,
              listName: name,

              customText: logo,
            };

            const { data, status } = await api.postdata(url, postRequest);
            let newListID = data.listId;
            if (status === 200) {
              let newlistUrl = `UtvalgList/GetUtvalgListWithAllReferences?UtvalglistId=${newListID}`;
              const { data, status } = await api.getdata(newlistUrl);
              if (status === 200 && data !== undefined) {
                setutvalglistapiobject(data);
                props.loadList(data);
                props.RemoveRouteUpdateWarnings();
              }
              btnClose.current.click();
              setloading(false);
              let msg = `Listen "${data.name}" er opprettet.`;

              Swal.fire({
                text: msg,
                confirmButtonColor: "#7bc144",
                confirmButtonText: "Lukk",
                position: "top",
              });
            } else {
              seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
              setdisplayMsg(true);
              setloading(false);
            }
          } catch (error) {
            console.error("error : " + error);
            seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
            setdisplayMsg(true);
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
        className="modal-dialog"
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
  );
}

export default SaveUtvalgListKW;
