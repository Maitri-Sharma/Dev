import React, { useState, useRef, useContext, useEffect } from "react";
import "../App.css";
import FinnCard from "../components/Finn-click";
import api from "../services/api.js";
import { v4 as uuidv4 } from "uuid";
import { KSPUContext } from "../context/Context.js";
import { Buffer } from "buffer";
import loadingImage from "../assets/images/callbackActivityIndicator.gif";
import Swal from "sweetalert2";

function Sok() {
  const [Finn, setFinn] = useState(false);
  const [data, setdata] = useState([]);
  const [listdata, setlistdata] = useState([]);
  const [KundeNumber, setKundenumber] = useState([]);
  const [testapivalue, settestapivalue] = useState([]);
  const [selectedvalue, setselectedvalue] = useState("");
  const [btnDisabled, setBtnDisabled] = useState(true);
  const [alertbox, setAlert] = useState(false);
  const { showBusiness, setShowBusiness, showHousehold, setShowHousehold } =
    useContext(KSPUContext);
  const FirstInputText = useRef();
  const Checkbox = useRef();
  const SecondInputText = useRef();
  const FirstDropdownValue = useRef();
  const SecondDropdownValue = useRef();
  const [loading, setloading] = useState(false);
  const handleKeypress = (e) => {
    //it triggers by pressing the enter key
    if (e.key === "Enter") {
      FinClick();
    }
  };
  const textInput = (e) => {
    setAlert(false);
    if (FirstInputText.current.value || SecondInputText.current.value)
      setBtnDisabled(false);
    else {
      setBtnDisabled(true);
    }
  };
  const eConnectKundeNumber = async (url, SecondInputText) => {
    let uniqueId = uuidv4();

    let eConnectUrl = `ECPuma/FindCustomer380`;
    let SecondInput = SecondInputText;

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
      Navn: SecondInput,
      MaksRader: "100",
    };
    try {
      const { data, status } = await api.postdata(eConnectUrl, eConnectHeader);
      if (data.length == 0) {
        setAlert(true);
        setFinn(false);
        return 0;
      } else {
        setAlert(false);
        var kundenummerlist = [];

        data.kundedata.forEach(function (item) {
          kundenummerlist.push(item.kundenummer);
        });

        var result = kundenummerlist.join(",");

        return result;
        //setAlert(false);
      }
    } catch (error) {
      console.error("error : " + error);
      setAlert(true);
      setFinn(false);
      return 0;
    }
  };

  const FinClick = async (e) => {
    //e.preventDefault();

    let url = "";
    setloading(true);
    if (FirstInputText.current.value) {
      if (FirstDropdownValue.current.value == "0") {
        url = url + `Utvalg/SearchUtvalgByUtvalgName?`;
        if (FirstInputText.current.value) {
          url =
            url +
            `utvalgNavn=${encodeURIComponent(FirstInputText.current.value)}&`;
        }

        if (SecondInputText.current.value) {
          if (SecondDropdownValue.current.value == "0") {
            const econnectURL = eConnectKundeNumber(
              url,
              SecondInputText.current.value
            );
            if (econnectURL) url = url + `customerNos=${econnectURL}&`;
            //url=econnectURL;
          } else {
            url =
              url +
              `customerNos=${encodeURIComponent(
                SecondInputText.current.value
              )}&`;
          }
        }
        if (Checkbox.current.checked) {
          url = url + `onlyBasisUtvalg=${1}&`;
        } else {
          url = url + `onlyBasisUtvalg=${0}&`;
        }

        url = url + `extendedInfo=${false}`;

        try {
          const { data, status } = await api.getdata(url);
          if (data.length == 0) {
            setAlert(true);
            setFinn(false);
            setloading(false);
          } else {
            setAlert(false);
            setdata(data);
            setFinn(true);
            setloading(false);
          }
        } catch (error) {
          console.error("error : " + error);
          setAlert(true);
          setFinn(false);
          setloading(false);
        }
      }
      if (FirstDropdownValue.current.value == "1") {
        url = url + `UtvalgList/SearchUtvalgListSimpleByIDAndCustomerNo?`;
        if (FirstInputText.current.value) {
          url =
            url +
            `utvalglistname=${encodeURIComponent(
              FirstInputText.current.value
            )}&`;
        }
        if (SecondInputText.current.value) {
          if (SecondDropdownValue.current.value == "0") {
            const econnectURL = eConnectKundeNumber(
              url,
              encodeURIComponent(SecondInputText.current.value)
            );
            if (econnectURL) url = url + `customerNos=${econnectURL}&`;
            //url=econnectURL;
          } else {
            url =
              url +
              `customerNos=${encodeURIComponent(
                SecondInputText.current.value
              )}&`;
          }
        }
        if (Checkbox.current.checked) {
          url = url + `onlyBasisLists=${1}&`;
        } else {
          url = url + `onlyBasisLists=${0}&`;
        }

        url = url + `extendedInfo=${false}&`;
        url = url + `includeChildrenUtvalg=${false}`;

        try {
          const { data, status } = await api.getdata(url);
          if (data.length == 0) {
            setAlert(true);
            setFinn(false);
            setloading(false);
          } else {
            setAlert(false);
            //setlistdata(data);
            setdata(data);
            // setAlert(false);
            setFinn(true);
            setloading(false);
          }
        } catch (error) {
          console.error("error : " + error);
          setAlert(true);
          setFinn(false);
          setloading(false);
        }
      }
      if (FirstDropdownValue.current.value == "2") {
        // utvalgId=2315091&includeReols=true
        //url= url+`SearchUtvalgByUtvalgIdAndCustmerNo?`
        //url= url+`SearchUtvalgListSimpleByIdAndCustNoAgreeNo?`
        let checkutvalg = "";
        if (FirstInputText.current.value) {
          let name = FirstInputText.current.value;
          name = name.trim();
          checkutvalg = name.substring(0, 1);
          checkutvalg = checkutvalg.toUpperCase();
          if (checkutvalg === "U" || checkutvalg === "L") {
            name = name.substring(1);
          }
          url = `Utvalg/SearchUtvalgByUtvalgId?utvalgId=${name}&includeReols=${false}`;
          let url1 = `UtvalgList/SearchUtvalgListSimpleById?utvalglistid=${name}`;
          let isDataFound = false;
          try {
            const { data, status } = await api.getdata(url);
            if (data.length > 0) {
              setdata(data);
              isDataFound = true;
            } else {
              const { data, status } = await api.getdata(url1);
              if (data.length > 0) {
                setdata(data);
                isDataFound = true;
              }
            }
            if (isDataFound) {
              setAlert(false);
              setFinn(true);
              setloading(false);
            } else {
              setAlert(true);
              setFinn(false);
              setloading(false);
            }
          } catch (error) {
            console.log("errorpage API not working");
            console.error("error : " + error);
            setAlert(true);
            setFinn(false);
            setloading(false);
          }
        }
      }
    } else {
      if (SecondInputText.current.value) {
        if(FirstDropdownValue.current.value === "1"){
          url = url + `UtvalgList/SearchUtvalgListWithChildrenByKundeNummer?`;
        }
        else{
          url = url + `Utvalg/SearchUtvalgByKundeNr?`;
        }
        if (SecondDropdownValue.current.value == "0") {
          const econnectURL = await eConnectKundeNumber(
            url,
            SecondInputText.current.value
          );
          if (econnectURL) url = url + `KundeNummer=${econnectURL}&`;
          //url=econnectURL;
        }
        if (SecondDropdownValue.current.value == "1") {
          url = url + `KundeNummer=${SecondInputText.current.value}&`;
        }
        url = url + `includeReols=${false}`;
        if(FirstDropdownValue.current.value === "1"){
          if (Checkbox.current.checked) {
            url = url + `&onlyBasisUtvalglist=${true}`;
          } else {
            url = url + `&onlyBasisUtvalglist=${false}`;
          }
        }
        else{
          if (Checkbox.current.checked) {
            url = url + `&onlyBasisUtvalg=${true}`;
          } else {
            url = url + `&onlyBasisUtvalg=${false}`;
          }
        }
        
        try {
          const { data, status } = await api.getdata(url);
          if (data.length == 0) {
            setAlert(true);
            setFinn(false);
            setloading(false);
          } else {
            setAlert(false);
            setdata(data);
            // setAlert(false);
            setFinn(true);
            setloading(false);
          }
        } catch (error) {
          setAlert(true);
          console.error("error : " + error);
          setFinn(false);
          setloading(false);
        }
      }
    }
    // if(data.length <= 0){
    //   setAlert(true);
    //   setFinn(false);
    // }
    // else{
    //   setFinn(true);
    //   setAlert(false);
    // }
  };

  return (
    <div className="card bg-color">
      <div className="Kj-background-color pt-1 ml-1 mt-1 mr-1">
        <p className="Sok-header pl-1">
          Søk etter eksisterende utvalg eller utvalgsliste
        </p>
      </div>

      <p className="sok-text2 pb-1 pl-1 pt-1">
        Minimum 3 tegn i ett av feltene
      </p>
      <div className="row pt-1 pl-3">
        <div className="pl-1 ">
          {/* <select className="divValueText" onClick={Firstdropdown} defaultValue={selectedvalue} > */}
          <select ref={FirstDropdownValue} className="divValueText" style={{maxWidth:'100px'}}>
            {/* {optionTemplate} */}
            <option value="0" defaultValue>
              Utvalgsnavn
            </option>
            <option value="1">Utvalgsliste</option>
            <option value="2">Utvalgs/ListeID</option>
          </select>

          <span className=" divErrorText text-danger ml-1">*</span>
        </div>
        <div>
          <input
            ref={Checkbox}
            className="form-check-input ml-1 "
            type="checkbox"
            value=""
            id="defaultCheck1"
          />
          <label className="form-check-label ml-3 pl-1" htmlFor="defaultCheck1">
            Vis bare basisutvalg og-lister
          </label>
        </div>
      </div>

      <div className="row">
        <div className="input-groupco-4 pt-1 pl-3">
          <i className="fa fa-user-circle-o pl-1"></i>
          <input
            ref={FirstInputText}
            type="text"
            className="InputValueText"
            placeholder=""
            onChange={textInput}
            onKeyPress={handleKeypress}
          />
        </div>
      </div>

      <div className="row pt-1 pl-3">
        <div className="col pl-1">
          <select ref={SecondDropdownValue} className="divValueText">
            <option value="0" defaultValue>
              Kundenavn
            </option>
            <option value="1">Kundenummer</option>
          </select>
          <span className="divErrorText text-danger ml-1">*</span>
        </div>
      </div>
      <div className="row pl-3 pt-1">
        <div className="input-groupco-4">
          <i className="fa fa-user-circle-o pl-1"></i>
          <input
            ref={SecondInputText}
            type="text"
            className="InputValueText mt-1"
            placeholder=""
            onChange={textInput}
            onKeyPress={handleKeypress}
          />
        </div>
        <input
          type="submit"
          className="KSPU_button ml-2 mt-1"
          value="Finn"
          disabled={btnDisabled}
          onClick={FinClick}
        />
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
      </div>

      <p className="sok-text pt-1 pl-1">
        Her kan du søke etter utvalg og utvalgslister som du tidligere har
        lagret. I tillegg til å åpne utvalg og utvalgslister kan du også slette.
      </p>
      {alertbox ? (
        <span className=" sok-Alert-text pl-1">
          {" "}
          Oppgitte søkekriterier ga ikke noe resultat.
        </span>
      ) : null}

      {Finn ? (
        <FinnCard showBusinesscheck={showBusiness} result={data} />
      ) : null}
    </div>
  );
}
export default Sok;
