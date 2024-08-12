import React, { useContext, useState, createRef } from "react";
import { KSPUContext } from "../../context/Context.js";
import OpplastedeAdresser from "../opplastede_adresser/opplastede_adresser.component";
import iconv from "iconv-lite";
import { useEffect } from "react";
function LastOppAdressepunkter(props, { parentCallback }) {
  const [filedata, setfiledata] = useState([]);
  const [currentStep, setCurrentStep] = useState(1);
  const [semicolonradio, setsemicolonradio] = useState(false);
  const [taboular, settaboular] = useState("on");
  const [commaradio, setcommaradio] = useState(false);
  const [Type, setType] = useState("");
  const [errormsg, seterrormsg] = useState("");
  const [showerrordiv, setshowerrordiv] = useState(false);
  const [showheader, setshowheader] = useState(true);
  const [TextData, setTextData] = useState("");
  const [encodingvalue, setencodingvalue] = useState("no");
  const [showHeader, setShowHeader] = useState(true);
  const [btnDisabled, setBtnDisabled] = useState(true);
  const { setAddresslisteDisplay, setvalue } = useContext(KSPUContext);
  const fileInput = createRef();
  const handleprevclick = () => {
    setCurrentStep(currentStep - 1);
  };
  const handleheader = (e) => {
    if (e.target.checked) setshowheader(true);
    else setshowheader(false);
  };
  useEffect(async () => {
    if (props.header === "false") {
      await setShowHeader(false);
    } else {
      await setShowHeader(true);
    }
  }, []);
  const handleCancel = () => {
    setAddresslisteDisplay(false);
    setvalue(true);
  };
  const showFile = async (e) => {
    setshowerrordiv(false);

    setencodingvalue("");

    let filename = fileInput.current.files[0].name;
    let fileextension = filename.split(".")[1];
    if (fileextension === "txt") {
      e.preventDefault();

      const reader = new FileReader();
      reader.onload = function (e) {
        const text = e.target.result;
        setBtnDisabled(false);
        setTextData(text);
      };
      reader.readAsText(e.target.files[0]);
    } else {
      setshowerrordiv(true);
      seterrormsg(
        "Det er kun mulig å laste opp tekstfiler (filer som ender med .txt)."
      );
    }
  };

  const encoding = (e) => {
    setencodingvalue(e.target.value);
  };

  const semiColonFun = (e) => {
    setType("semicolon");
    settaboular(false);
    setcommaradio(false);

    let semicolonvalue = e.target.value;
    setsemicolonradio(semicolonvalue);
  };
  const commaFun = (e) => {
    settaboular(false);
    setType("comma");
    setsemicolonradio(false);
    let commavalue = e.target.value;
    setcommaradio(commavalue);
  };
  const taboularFun = (e) => {
    setType("tab");
    let taboularvalue = e.target.value;
    settaboular(taboularvalue);
  };
  const nextClick = () => {
    let lines = TextData.split(/(\n|\r\n)/);
    let result1 = lines.filter((item) => item !== "\r\n" && item !== "");

    let result = "";
    if (encodingvalue !== "") {
      result = result1.map((item) => {
        let c = iconv.encode(item, encodingvalue);
        return c.toString();
      });
    } else {
      result = result1;
    }

    if (taboular === "on") {
      result = result.map((item) => item.split("\t"));
      setType("tab");
      setfiledata(result);
    } else if (semicolonradio === "on") {
      result = result.map((item) => item.split(";"));

      setfiledata(result);
    } else if (commaradio === "on") {
      result = result.map((item) => item.split(","));
      setfiledata(result);
    }
    if (
      result[0].length !== 4 ||
      result[0][0].toLowerCase() !== "butikk" ||
      result[0][1].toLowerCase() !== "besøksadresse" ||
      result[0][2].toLowerCase() !== "postnr" ||
      result[0][3].toLowerCase() !== "poststed"
    ) {
      setshowerrordiv(true);
      seterrormsg(
        "Adresselisten må ha disse overskriftene og dette formatet: Butikk Besøksadresse Postnr Poststed"
      );
    } else {
      setCurrentStep(currentStep + 1);
    }
  };
  const callback = (step) => {
    setCurrentStep(step - 1);
  };
  return (
    <div
      className="card container addressContainer "
      style={{ position: "absolute" }}
    >
      {showHeader ? (
        <div className=" row pl-1 pr-2">
          <div className="col-8 span-color1">
            <span className="sok-text1"> Last opp adressepunkter </span>{" "}
          </div>{" "}
          <div className="col-4 span-color1">
            <span className="d-flex float-right sok-text1 pt-1">
              {" "}
              Trinn {currentStep} av 3{" "}
            </span>{" "}
          </div>{" "}
        </div>
      ) : null}
      {showHeader ? (
        <div className="sok-text">
          <p className="pl-1">
            Dersom du har flere utsalgssteder kan du gjennom dette verktøyet
            laste opp en liste med adresser.Disse kan du videre bruke i
            forskjellige analyser.{" "}
          </p>{" "}
        </div>
      ) : null}
      <div className="Kj-div-background-color pl-1 pr-1 pt-1 ">
        {showHeader ? (
          <div className="Kj-background-color pl-1 pb-1 ">
            <span className="install-text"> INNSTILLINGER FOR ADRESSEFIL </span>{" "}
          </div>
        ) : null}
        {currentStep === 1 ? (
          <div>
            <p className="label p-2">
              {" "}
              {showerrordiv ? (
                <div className="red">
                  <p>{errormsg}</p>
                </div>
              ) : null}
              Velg en tekstfil med adressene du vil laste opp{" "}
            </p>{" "}
            <div className="input-group pl-1 pr-1">
              {" "}
              <input
                className="KSPU_button"
                type="file"
                name="uxUploadAddresses$uxUploadFile"
                id="uxUploadFile"
                ref={fileInput}
                onChange={(e) => showFile(e)}
              ></input>{" "}
            </div>{" "}
            <div>
              <p className="label p-2 mt-1">
                {" "}
                Kolonnene i filen er separert med{" "}
              </p>{" "}
            </div>{" "}
            <div className="sok-text ml-3 pt-1">
              <div className="pb-1">
                <input
                  type="radio"
                  name="optradio"
                  checked={taboular === "on" ? true : false}
                  onChange={taboularFun}
                />{" "}
                Tabulator{" "}
              </div>{" "}
              <div className="pb-1">
                <input
                  type="radio"
                  name="optradio"
                  checked={semicolonradio === "on" ? true : false}
                  onChange={semiColonFun}
                />{" "}
                Semikolon{" "}
              </div>{" "}
              <div className="pb-1">
                <input
                  type="radio"
                  name="optradio"
                  checked={commaradio === "on" ? true : false}
                  onChange={commaFun}
                />{" "}
                Komma{" "}
              </div>{" "}
            </div>{" "}
            <div className="sok-text ml-2">
              <input
                type="checkbox"
                checked={showheader}
                onChange={(e) => {
                  handleheader(e);
                }}
              />{" "}
              Første linje i filen inneholder kolonnenavn{" "}
            </div>{" "}
            <div className="p-2 ml-0">
              <p className="label"> Filen bruker følgende encoding </p>
              <select
                className="form-select btn-work form-select-size_1 mb-1"
                aria-label=".form-select-sm example"
                onChange={encoding}
              >
                <option value="vetikke"> Vet ikke </option>{" "}
                <option value="windows-1252"> Windows - 1252(Latinsk) </option>{" "}
                <option value="UTF16"> Unicode(UTF16) </option>{" "}
                <option value="ASCII"> Ascii </option>{" "}
                <option value="UTF7"> UTF7 </option>{" "}
                <option value="UTF8"> UTF8 </option>{" "}
                <option value="UTF32"> UTF32 </option>{" "}
                <option value="UTF16BE"> Big - Endian Unicode(UTF16) </option>{" "}
              </select>{" "}
            </div>{" "}
            <div className="row col-12 m-0 p-0 mt-2 mb-2">
              <input
                type="submit"
                id="uxBtForrige"
                className="KSPU_button"
                value="<< Forrige"
                onClick={handleprevclick}
                style={{
                  visibility: currentStep > 1 ? "visible" : "hidden",
                  text: "",
                  float: "left",
                }}
              />

              <input
                type="submit"
                id="uxBtnAvbryt"
                className="KSPU_button"
                value="Avbryt"
                onClick={handleCancel}
                style={{ text: "Avbryt", marginLeft: "auto" }}
              />

              <input
                type="submit"
                id="uxBtnNeste"
                className="KSPU_button float-right"
                value="Neste >>"
                onClick={nextClick}
                disabled={btnDisabled}
                style={{
                  display: currentStep < 3 ? "block" : "none",
                  float: "right",
                  marginLeft: "auto",
                }}
              />
            </div>
          </div>
        ) : currentStep === 2 ? (
          <OpplastedeAdresser
            Data={filedata}
            Type={Type}
            header={showheader}
            currentStep={currentStep}
            parentCallback={callback}
          />
        ) : null}
      </div>{" "}
    </div>
  );
}

export default LastOppAdressepunkter;
