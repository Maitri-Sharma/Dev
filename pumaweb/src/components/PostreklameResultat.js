import React, { useState, useEffect, useContext } from "react";
import "../App.css";
import api from "../services/api.js";
import SelectionDetails from "./SelectionDetails.js";
import Mottakergrupper from "../components/Mottakergrupper";
import { UtvalgContext } from "../context/Context.js";
import { KundeWebContext, KSPUContext } from "../context/Context.js";

import Result from "./Result";
import UtvalDetails from "./UtvalDetails.js";
import VegGeografiskOmrade from "./VegGeografiskOmrade.js";

function PostreklameResultat(props) {
  const [datalist, setData] = useState([]);
  // const {Page,setPage} = useContext(KundeWebContext);
  // const [selectedGroups, setSelectedGroups] = useState([]);
  // const {activUtvalg, setActivUtvalg} = useContext(KSPUContext);
  // const [outputData, setOutputData] = useState([]);
  // const {searchURL,setSearchURL} = useContext(KSPUContext);
  // const [velgvalue,setVelgvalue] = useState("0");
  // const [searchData, setSearchData] = useState([]);
  // const [currentStep, setCurrentStep ] = useState(1);
  // const { resultData} = useContext(KSPUContext);
  // useEffect(() => {
  //     fetchData('Fylke/Getallfylkes');
  //   }, []);
  // const fetchData = async (url) => {
  //     try {
  //         const {data , status} = await api.getdata(url);
  //           if(status === 200)
  //           {
  //             setData(data);
  //           }
  //           else
  //           {
  //             console.error('error : ' + status);
  //           }
  //         }
  //         catch (error) {
  //           console.error('er : ' + error);
  //         }
  // };

  const vegGeografiskReturn = () => {
    // setPage('VegGeografiskOmrade')
  };

  return (
    <div className="card">
      <div className=" row pl-1 pr-1">
        <div className="col-8 span-color1">
          <span className="sok-text1">Postreklame Segmenter</span>
        </div>
        <div className="col-4 span-color1">
          <span className="d-flex float-right sok-text1 pt-1">Trinn 3 av 3</span>
        </div>
      </div>
      <div className="sok-text">
        <p className="pl-1">
          Posten har delt Norge inn i 10 ulike segmenter basert på ulike
          interesser og holdninger. Du kan velge mellom et eller flere segmenter
          som inneholder variabler til dine målgrupper.
        </p>
      </div>
      <div className="card Kj-background-color ml-1 mr-1 mt-1">
        <div className="row ">
          <p className="avan pl-4 pt-1 ">RESULTAT</p>
        </div>
        <div className="Kj-div-background-color pt-2 pb-2">
          <SelectionDetails Utvalcheck={"True"} />
        </div>
      </div>
      <br />
      <div className="row">
        <div className="col-4 ">
          <input
            type="submit"
            className="KSPU_button"
            onClick={vegGeografiskReturn}
            value="<<Forrige"
          />
        </div>
        <div className="col-4">
          <input type="submit" className="KSPU_button" value="Avbryt" />
        </div>
        <div className="col-4">
          <input type="submit" className="KSPU_button" value="Lagre" />
        </div>
      </div>
    </div>
  );
}
export default PostreklameResultat;
