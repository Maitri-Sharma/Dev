import React, { useState, useRef, useContext, useEffect } from "react";
import { KSPUContext} from "../context/Context.js";
import api from "../services/api.js";
import Swal from "sweetalert2";
import $ from "jquery";
function SaveCampaignList(props) {
  const { errormsg, seterrormsg } = useContext(KSPUContext);
  const { issave, setissave } = useContext(KSPUContext);

  const {
    setAktivDisplay,
    setDemografieDisplay,
    setSegmenterDisplay,
    setAddresslisteDisplay,
    setGeografiDisplay,
    setKjDisplay,
    setAdresDisplay,
    activUtvalglist,
    setActivUtvalglist,
    setshoworklist,
  } = useContext(KSPUContext);

  const [displayMsg, setdisplayMsg] = useState(false);

  const [name, setname] = useState("");

  const [test, settest] = useState(false);
  const btnClose = useRef();

  const [loading, setloading] = useState(false);
  const FirstInputText = useRef();
  const SecondInputText = useRef();

  const EnterName = () => {
    let name_utvalg = document.getElementById("utvalgnavnList").value;
    setname(name_utvalg);
    setdisplayMsg(false);
  };

  const uxSaveUtvalg = async (event) => {
    document.getElementById("btnSaveKampUtvalgList").disabled = true;
    if (name == "" || name.trim().length < 3) {
      document.getElementById("btnSaveKampUtvalgList").disabled = false;
      setdisplayMsg(true);
      seterrormsg("Beskrivelse av utvalget må ha minst 3 tegn.");
    } else if (name.indexOf(">") > -1 || name.indexOf("<") > -1) {
      document.getElementById("btnSaveKampUtvalgList").disabled = false;
      setdisplayMsg(true);
      seterrormsg("Should not conatain special character");
    } else if (activUtvalglist.totalAntall == 0) {
      document.getElementById("btnSaveKampUtvalgList").disabled = false;
      setdisplayMsg(true);
      seterrormsg(
        "Utvalget har ingen mottakere og kan derfor ikke lagres. Kontroller at utvalget inneholder budruter og at minst en mottakergruppe er valgt."
      );
    } else {
      document.getElementById("btnSaveKampUtvalgList").disabled = true;
      setdisplayMsg(false);
      const { data, status } = await api.getdata(
        `UtvalgList/UtvalgListNameExists?utvalglistname=${encodeURIComponent(
          name
        )}`
      );
      if (status === 200) {
        if (data == true) {
          document.getElementById("btnSaveKampUtvalgList").disabled = false;
          let msg = `Utvalget ${name} eksisterer allerede. Velg et annet utvalgsnavn.`;
          seterrormsg(msg);
          setdisplayMsg(true);
        } else {
          let url = `UtvalgList/CreateCampaignList?userName=Internbruker&listId=${
            activUtvalglist?.listId
          }&antall=${activUtvalglist?.antall}&campaignName=${encodeURIComponent(
            name
          )}`;

          try {
            setloading(true);


            const { data, status } = await api.postdata(url);
            
            if (status === 200) {
              document.getElementById("btnSaveKampUtvalgList").disabled = false;
              await setActivUtvalglist(data);
              btnClose.current.click();
              
              setshoworklist((newWorklist) => [...newWorklist, data]);
              
              setissave(true);
              settest(true);

              let sucessMessage = `Kampanjelisten "${name}" er opprettet.`;

              setloading(false);

              setAktivDisplay(true);
              setDemografieDisplay(false);
              setSegmenterDisplay(false);
              setAddresslisteDisplay(false);
              setGeografiDisplay(false);
              setKjDisplay(false);
              setAdresDisplay(false);
              $(".modal").remove();
              $(".modal-backdrop").remove();
              Swal.fire({
                text: sucessMessage,
                confirmButtonColor: "#7bc144",
                confirmButtonText: "Lukk",
              });
            } else {
              document.getElementById("btnSaveKampUtvalgList").disabled = false;
              seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
              setdisplayMsg(true);
              setissave(false);
              setloading(false);
            }
          } catch (error) {
            document.getElementById("btnSaveKampUtvalgList").disabled = false;
            console.error("error : " + error);
            seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
            setdisplayMsg(true);
            setissave(false);
            setloading(false);
          }
        }
      } else {
        document.getElementById("btnSaveKampUtvalgList").disabled = false;
        console.error("error : " + status);
        seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
        setdisplayMsg(true);
      }
    }
  };

  return (
    <div>
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
                OPPRETT KAMPANJE
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
                        Listenavn
                      </span>
                    </td>
                    <td>
                      <input
                        ref={FirstInputText}
                        name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxName"
                        type="text"
                        id="utvalgnavnList"
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
                        value={
                          activUtvalglist.listId
                            ? activUtvalglist.kundeNummer
                            : null
                        }
                        name="uxDialogs$uxSaveUtvalg$uxSaveWindow$uxKunde"
                        type="text"
                        id="uxKunde19"
                        className="selection-input ml-1"
                        disabled={true}
                      />
                    </td>
                  </tr>
                  <br />
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
                    <td>
                      <button
                        type="button"
                        id="btnSaveKampUtvalgList"
                        onClick={uxSaveUtvalg}
                        className="btn KSPU_button"
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
    </div>
  );
}

export default SaveCampaignList;
