import React, { useState, useContext } from "react";
import "../App.css";
import expand from "../assets/images/esri/expand.png";
import collapse from "../assets/images/esri/collapse.png";
// import pin_box from "../assets/images/symboler/aktivtutvalgsymbol_10.JPG";
import pin_box from "../assets/images/Icons/redicon.png";
import { KundeWebContext } from "../context/Context";
import { NumberFormat } from "../common/Functions";

function Mottaker_KW(props) {
  // let houshold_sum = props.reduce((accumulator, current) => accumulator + current.House, 0);
  // let Business_sum = props.reduce((accumulator, current) => accumulator + current.Business, 0);
  const [togglevalue, settogglevalue] = useState(false);
  const [alertopen, setalertopen] = useState(false);
  // const [householdcheckstatus,sethouseholdcheckstatus] = useState(true);
  // const [Businessholdcheckstatus,setBusinessholdcheckstatus] = useState(false);

  const [Totalvalue, setTotalvalue] = useState(props.householdvalue);
  const { householdcheckbox, sethouseholdcheckbox } =
    useContext(KundeWebContext);
  const { businesscheckbox, setbusinesscheckbox } = useContext(KundeWebContext);
  const { utvalgapiobject, setutvalgapiobject } = useContext(KundeWebContext);
  // const {Total,setTotal} = useContext(KundeWebContext);
  const toggle = () => {
    settogglevalue(!togglevalue);
  };
  const openalert = () => {
    setalertopen(true);
  };
  const householdcheck = (e) => {
    let housecheck = document.getElementById("Hush").checked;
    let a = utvalgapiobject;
    let receivers = [];

    if (housecheck) {
      if (a.receivers?.length) {
        receivers = a.receivers.filter((i) => {
          return i.receiverId !== 1;
        });
        a.receivers = receivers;
      }
      a.receivers.push({ receiverId: 1, selected: true });
    } else {
      receivers = a.receivers.filter((i) => {
        return i.receiverId !== 1;
      });
      a.receivers = receivers;
    }
    // sethouseholdcheckstatus(housecheck);
    sethouseholdcheckbox(housecheck);
    setutvalgapiobject(a);

    // parentCallback({"housholdcheckbox":householdcheckstatus,"businesscheckboxstatus":Businessholdcheckstatus})
  };
  const VirkCheck = () => {
    let BusinessCheck = document.getElementById("Virk").checked;

    let a = utvalgapiobject;
    let receivers = [];

    if (BusinessCheck) {
      if (a.receivers?.length) {
        receivers = a.receivers.filter((i) => {
          return i.receiverId !== 4;
        });
        a.receivers = receivers;
      }
      a.receivers.push({ receiverId: 4, selected: true });
    } else {
      receivers = a.receivers.filter((i) => {
        return i.receiverId !== 4;
      });
      a.receivers = receivers;
    }
    // setBusinessholdcheckstatus(BusinessCheck);
    if (BusinessCheck) {
      setTotalvalue(parseInt(Totalvalue) + parseInt(props.Businessvalue));
      a.totalAntall = parseInt(Totalvalue) + parseInt(props.Businessvalue);
    }

    setbusinesscheckbox(BusinessCheck);
    setutvalgapiobject(a);
    // parentCallback({"housholdcheckbox":householdcheckstatus,"businesscheckboxstatus":Businessholdcheckstatus})
  };

  return (
    <div className="card border-dark  ml-1 mr-1 mt-1">
      <div className="avan Kj-background-color">
        <div className="row">
          <div className="col-8">
            <p className="motta-heading p-2">MOTTAKERGRUPPER</p>
          </div>
          <div className="col-4">
            {togglevalue ? (
              <img
                className="d-flex float-right pt-1 mr-1"
                src={collapse}
                onClick={toggle}
              />
            ) : (
              <img
                className="d-flex float-right pt-1 mr-1"
                src={expand}
                onClick={toggle}
              />
            )}
          </div>
        </div>
      </div>
      <div style={{ backgroundColor: "#E6E6E6" }}>
        <div className="p-text pt-1">
          &nbsp;&nbsp;
          <span id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblRecivers">
            Kryss av for ønskede{" "}
            <a
              className="KSPU_LinkInText_kw"
              data-toggle="modal"
              data-target="#exampleModalCenter"
              onClick={openalert}
            >
              mottakere{" "}
            </a>
          </span>
          <br />
          <div className="row pl-3">
            <div className="col-md-.5">
              &nbsp;&nbsp;
              <img
                src={pin_box}
                style={{ width: "11px" }}
                alt="Utvalg under arbeid"
              />
            </div>
            <div className="col-3">
              <p className="p-text">Utvalg</p>
            </div>
          </div>
          <p></p>
          {props.display == "segmenter" ? (
            <div className="pl-5">
              <div className="col-12  ">
                <div>
                  <input
                    className="form-check-input mt-0"
                    type="checkbox"
                    value=""
                    id="Hush"
                    defaultChecked
                    onClick={householdcheck}
                  />
                  <label className="form-check-label label-text" htmlFor="Hush">
                    {" "}
                    Husholdninger{" "}
                  </label>
                </div>
              </div>
            </div>
          ) : (
            <div className="pl-5">
              <div className="col-12  ">
                <div>
                  <input
                    className="form-check-input mt-0"
                    type="checkbox"
                    value=""
                    id="Hush"
                    checked={householdcheckbox}
                    onClick={householdcheck}
                  />
                  <label className="form-check-label label-text" htmlFor="Hush">
                    {" "}
                    Husholdninger{" "}
                  </label>
                  {householdcheckbox ? (
                    <span
                      id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                      className="divValueTextBold div_right"
                    >
                      {NumberFormat(props.householdvalue)}
                    </span>
                  ) : (
                    <span
                      id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                      className="divValueTextBold div_right"
                    >
                      0
                    </span>
                  )}
                </div>
              </div>
              <p></p>
              <div className="col-12">
                <div>
                  <input
                    className="form-check-input mt-0 "
                    type="checkbox"
                    value=""
                    id="Virk"
                    onClick={VirkCheck}
                    checked={businesscheckbox}
                  />
                  <label className="form-check-label label-text" htmlFor="Virk">
                    {" "}
                    Virksomheter{" "}
                  </label>
                  {businesscheckbox ? (
                    <span
                      id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                      className="divValueTextBold div_right"
                    >
                      {NumberFormat(props.Businessvalue)}
                    </span>
                  ) : (
                    <span
                      id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                      className="divValueTextBold div_right"
                    >
                      0
                    </span>
                  )}
                </div>
              </div>
              <div
                align="left"
                className="col UpperLine pt-1 padding_NoColor_B clearFloat"
              >
                <span
                  id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblAntallSumText"
                  className="divValueTextBold  div_left"
                >
                  Totalt for utvalget
                </span>
                {businesscheckbox && householdcheckbox ? (
                  <span
                    id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                    className="divValueTextBold div_right"
                  >
                    {NumberFormat(props.householdvalue + props.Businessvalue)}
                  </span>
                ) : householdcheckbox ? (
                  <span
                    id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                    className="divValueTextBold div_right"
                  >
                    {NumberFormat(props.householdvalue)}
                  </span>
                ) : businesscheckbox ? (
                  <span
                    id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                    className="divValueTextBold div_right"
                  >
                    {NumberFormat(props.Businessvalue)}
                  </span>
                ) : (
                  <span
                    id="uxGeografiAnalyse_GeografiGeografi_uxReceivers_uxPnlReceivers_lblUtvalgAntallSum"
                    className="divValueTextBold div_right"
                  >
                    0
                  </span>
                )}
              </div>
            </div>
          )}
        </div>
        <br />
        <p></p>
      </div>

      <div
        className="modal fade"
        id="exampleModalCenter"
        tabIndex="-1"
        role="dialog"
        aria-labelledby="exampleModalCenterTitle"
        aria-hidden="true"
      >
        <div className="modal-dialog" role="document">
          <div className="modal-content">
            <div
              className="modal-header "
              style={{ backgroundColor: "#E4E1CC" }}
            >
              <div>
                <h6 className="modal-title" id="exampleModalCenterTitle">
                  Mottakere
                </h6>
              </div>
              {/* <button type="button" className="close" data-dismiss="modal" aria-label="Close">
          <span aria-hidden="true">&times;</span>
        </button> */}
            </div>
            <div className="pl-2">
              <b className="alert-text pr-2">
                Minste distribusjonsområde er én budrute / ett postboksanlegg
              </b>
              <br />
              <br />
              <b className="alert-text">Husholdninger</b>
              <br />
              <span className="p-text">
                Alle husholdninger (inkludert gårdbrukere) i det valgte
                geografiske området som ikke har reservert seg mot å motta
                uadressert reklame.
              </span>
              <br /> <br />
              <b className="alert-text">Reserverte husholdninger</b>
              <br />
              <span className="p-text">
                Dersom du ønsker å sende ut <b>Informasjon</b> (av
                ikke-kommersiell art) reserverte til husholdninger må dette
                velges i tillegg til Husholdninger. MERK: Følg Forbrukerrådets
                anvisning.
              </span>
              <br />
              <br />
              <b className="alert-text">Virksomheter</b>
              <br />
              <span className="p-text">
                Alle virksomheter i det valgte geografiske området. (Gårdbrukere
                er ikke med i denne mottakergruppen.) Virksomheter kan ikke
                reservere seg mot uadressert reklame.
              </span>
              <br />
              <br />
            </div>
            <div className="modalAlertClose">
              <input type="button" value="OK" data-dismiss="modal" />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Mottaker_KW;
