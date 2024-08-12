import React, { useState, useRef, useContext, useEffect } from "react";
import { KSPUContext, UtvalgContext } from "../context/Context.js";
import api from "../services/api.js";

import Swal from "sweetalert2";

function ShowRecreateKW(props) {
  // const { errormsg, seterrormsg } = useContext(KSPUContext);

  const [displayMsg, setdisplayMsg] = useState(false);

  const [Email, setEmail] = useState("");

  const btnClose = useRef();

  const [loading, setloading] = useState(false);

  const useEmailValidation = (email) => {
    const isEmailValid = /@/.test(email); // use any validator you want
    return isEmailValid;
  };
  const EmailChange = () => {
    let emailId = document.getElementById("emailText").value;
    setEmail(emailId);
  };

  const UxSaveUtvalg = async (event) => {
    let typeRecreate;
    let idRecreate;
    const isEmailValid = useEmailValidation(Email);
    if (isEmailValid) {
      setdisplayMsg(false);
      if (props.recreateId) {
        idRecreate = props.recreateId;
      }
      if (props.recreateType) {
        typeRecreate = props.recreateType;
      }

      let bodyRequest = {
        type: typeRecreate,
        id: idRecreate,
        email: Email,
      };

      let url = `RecreateOnFly`;

      const { data, status } = await api.postdata(url, bodyRequest);

      if (status === 200) {
        btnClose.current.click();
        let msg = `Utvalget/Utvalgslista er sendt for gjenskaping. Du vil bli varslet på e-post når dette er klart. `;
        let title = "KVITTERING";

        Swal.fire({
          title: title,
          text: msg,
          confirmButtonColor: "#7bc144",
          confirmButtonText: "Lukk",
          position: "top"
        });
      } else {
        // seterrormsg("Oppgitte søkekriterier ga ikke noe resultat.");
        setdisplayMsg(true);

        setloading(false);
      }
    } else {
      setdisplayMsg(true);
      // seterrormsg("email should be correct");
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
          className="modal-dialog"
          role="document"
        >
          <div className="modal-content" style={{ border: "black 3px solid" }}>
            <div className="Common-modal-header">
              <span
                className="common-modal-title pt-1 pl-2"
                id="exampleModalLongTitle"
              >
                Advarsel
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
              {/* {displayMsg ? (
                <span className=" sok-Alert-text pl-1">{errormsg}</span>
              ) : null} */}
              <p className="flykecontent width-400">
                Datagrunnlaget må oppdateres i disse utvalgene/utvalgslistene.
                Det vil bli bestilt en gjenskapningsjobb som du får respons på
                via e-post. Sende jobb?
              </p>
              <p className="flykecontent width-400">
                Skriv inn e-postadresse du skal varsles på når jobben er klar:
              </p>
              <table>
                <tbody>
                  <tr>
                    <td>
                      <input
                        name="uxemailId"
                        type="email"
                        id="emailText"
                        onChange={EmailChange}
                        className="selection-input-email"
                        maxLength="50"
                        placeholder=""
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
                        Nei
                      </button>
                    </td>
                    <td></td>
                    <td>
                      <button
                        type="button"
                        onClick={UxSaveUtvalg}
                        className="btn KSPU_button"
                      >
                        Send jobb
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

export default ShowRecreateKW;
