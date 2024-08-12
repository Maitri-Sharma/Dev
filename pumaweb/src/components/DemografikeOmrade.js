import React, { useState, useEffect, useContext, useRef } from "react";
import style from "../App.css";
import TableSegmenter from "./TableSegmenter";
import api from "../services/api.js";
import { KSPUContext } from "../context/Context.js";
import { GetData } from "../Data";
import { Utvalg, NewUtvalgName, criterias, getAntall } from "./KspuConfig";
import useCurrentStep from "../common/useCurrentStep.js";
import SelectionDetails from "./SelectionDetails";
import Submit_Button from "./Submit_Button";
import NumberFormat from "react-number-format";
function DemografikeOmrade(props) {
  const [datalist, setData] = useState([]);
  const [outputData, setOutputData] = useState([]);
  const [selectedhush, setselectedhush] = useState(0);
  const [selectedrecord, setselectedrecord] = useState([]);
  const { HouseholdSum, setHouseholdSum } = useContext(KSPUContext);
  const { BusinessSum, setBusinessSum } = useContext(KSPUContext);
  const { selecteddemografiecheckbox, setselecteddemografiecheckbox } =
    useContext(KSPUContext);
  const [specifies, setSpecifies] = useState(false);
  const { selectedKoummeIDs, setselectedKoummeIDs } = useContext(KSPUContext);
  const [reolID, setreolID] = useState([]);
  const { Demograresultarray, setDemograresultarray } = useContext(KSPUContext);
  const { selecteddemografiecheckbox_c, setselecteddemografiecheckbox_c } =
    useContext(KSPUContext);
  const { antallValue, setAntallValue } = useContext(KSPUContext);
  const { demografikmsg, setdemografikmsg } = useContext(KSPUContext);
  const { demografikAntalMsg, setDemografikAntalMsg } = useContext(KSPUContext);
  const { existingActive, setExistingActive } = useContext(KSPUContext);

  useEffect(() => {
    fetchData();
  }, []);

  function extractValue(arr, prop) {
    // extract value from property
    let extractedValue = arr.map((item) => item[prop]);
    return extractedValue;
  }

  const fetchData = async () => {
    try {
      const { data, status } = await api.getdata("Kommune/GetAllKommunes");
      if (status === 200) {
        let s = [];
        let result = [];

        data.map((u) => {
          if (!s.includes(u.fylkeID)) {
            result.push({
              ID: u.fylkeID,
              name: u.fylkeName,
              key: u.fylkeID,
              children: [
                {
                  ID: u.kommuneID,
                  key: u.kommuneID,
                  name: u.kommuneName,
                },
              ],
            });

            s.push(u.fylkeID);
          } else {
            let index = result.findIndex((element) => element.ID === u.fylkeID);
            result[index].children.push({
              ID: u.kommuneID,
              key: u.kommuneID,
              name: u.kommuneName,
            });
          }
        });
        setDemograresultarray(result);

        
      } else {
        console.error("error : " + status);
      }
    } catch (error) {
      console.error("er : " + error);
    }
  };
  const callback = (
    selectedrecord,
    SelectedKommunekeys,
    checkedRows,
    recordObject
  ) => {
    let ID = [];

    setselectedKoummeIDs(SelectedKommunekeys);
  };
  const columns = [
    {
      title: "",
      dataIndex: "name",
      key: "key",
      sorter: (a, b) => {
        return ;
      },
      sortOrder: "ascend",
      sortDirections: ["ASC", "DESC"],
    },
  ];
  const Demografike = (e) => {
    let value = document.getElementById("Antall").value;

    setAntallValue(value);
  };
  return (
    <div>
      <div>
        <div className="Kj-background-color pl-1 pb-1">
          <span className="install-text">VELG GEOGRAFISK OMRADE</span>
        </div>

        {demografikAntalMsg ? (
          <div className="pr-3">
            <span id="uxKjoreAnalyse_uxLblMessage" className="divErrorText_kw">
              Antall mottakere må spesifiseres.
            </span>
          </div>
        ) : (
          ""
        )}
        {existingActive === false ? (
          <TableSegmenter
            columnsArray={columns}
            data={Demograresultarray}
            page={"segmenter"}
            defaultSelectedColumn={reolID}
            parentCallback={callback}
            setoutputDataList={setOutputData}
          />
        ) : null}
      </div>
      <div className="KommunInputText1 demografike pl-1 pr-1 pt-2 row">
        <div className="col-lg-7 no-padding _flex-start">
          <label>Spesifiser antall mottakere</label>
        </div>
        <div className="col-lg-5 no-padding _flex-start">
          <NumberFormat
            required
            className="InputValueText_1"
            onValueChange={(values) => {
              const { formattedValue, value } = values;

              setAntallValue(value);
            }}
          />
        </div>
      </div>
    </div>
  );
}
export default DemografikeOmrade;
