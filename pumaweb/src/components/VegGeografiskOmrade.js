import React, { useState, useEffect, useContext, useRef } from "react";
import style from "../App.css";
import TableSegmenter from "./TableSegmenter";
import api from "../services/api.js";
import { KSPUContext, KundeWebContext } from "../context/Context.js";
import { GetData } from "../Data";
import { Utvalg, NewUtvalgName, criterias, getAntall } from "./KspuConfig";
import useCurrentStep from "../common/useCurrentStep.js";
import SelectionDetails from "./SelectionDetails";
import Submit_Button from "./Submit_Button";

function VegGeografiskOmrade(props) {
  const [outputData, setOutputData] = useState([]);
  const [currentStep, setCurrentStep] = useState(1);
  const { segmenterresultarray, setsegmenterresultarray } =
    useContext(KSPUContext);
  const [reolID, setreolID] = useState([]);
  const { BusinessSum, setBusinessSum } = useContext(KSPUContext);
  const [selectedrecord, setselectedrecord] = useState([]);
  const [HouseholdSum_tree, setHouseholdSum_tree] = useState(0);
  const [BusinessSum_tree, setBusinessSum_tree] = useState(0);
  const [selectedhush, setselectedhush] = useState(0);
  const [datalist, setData] = useState([]);
  const { selectedsegment, setselectedsegment } = useContext(KSPUContext);
  const { selectedKoummeIDs, setselectedKoummeIDs } = useContext(KSPUContext);
  const { HouseholdSum, setHouseholdSum } = useContext(KSPUContext);
  const { selectedrecord_s, setselectedrecord_s } = useContext(KSPUContext);

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
        setsegmenterresultarray(result);

        // let s =data.map(item=>{
        // 	return {
        // 		ID : item.fylkeID,
        // 		name :item.fylkeName,
        // 		children : [{
        // 		ID : item.kommuneID,
        // 		name : item.kommuneName,
        // 		}]
        // 		}
        // })
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
    // mapView.graphics.removeAll();
    // mapView.goTo(mapView.initialExtent);
    let ID = [];

    setselectedKoummeIDs(SelectedKommunekeys);
  };

  // const callback = (selectedrecord) => {
  //   // setlagutvalgenable(true)
  //   // setnomessagediv(false)
  //   let reolID = [];
  //   if (selectedrecord) {
  //     reolID = selectedrecord.map(function (item) {
  //       if (item.hasOwnProperty("children")) {
  //         return extractValue(item.children, "key");
  //       } else {
  //         return item.key;
  //       }
  //     });

  //     let s = [];
  //     let SelectedKoummneID = s.concat.apply(s, reolID);
  //     SelectedKoummneID = SelectedKoummneID.filter((element) => {
  //       return element !== undefined;
  //     });
  //     setselectedKoummeIDs(SelectedKoummneID);
  //     // setselectedsegment(state => [...state, SelectedKoummneID])
  //     //     let set = new Set();
  //     // reolID.forEach(o => {
  //     //     o.forEach(i => {
  //     //         set.add(i);
  //     //     });
  //     // });

  //     // let SelectedKoummneID = [...set]

  //     // setreolID(reolID)
  //   }
  //   // if(selectedrecord.length == 0){
  //   //     setselectedrecord(datalist)
  //   // }
  //   // else{
  //   setselectedrecord(selectedrecord);
  //   setselectedrecord_s(selectedrecord);

  //   // }

  //   // let houshold_sum = selectedrecord.reduce((accumulator, current) => accumulator + current.House, 0);
  //   // let Business_sum = selectedrecord.reduce((accumulator, current) => accumulator + current.Business, 0);
  //   //        setHouseholdSum_tree(parseInt(houshold_sum)+parseInt(selectedhush))
  //   //        setHouseholdSum(parseInt(houshold_sum)+parseInt(selectedhush))
  //   //        setBusinessSum_tree(parseInt(Business_sum))
  //   //        setBusinessSum(parseInt(Business_sum))
  // };

  const columns = [
    // {
    // title: '',
    // dataIndex: 'ID',
    // key: 'key',
    // },
    {
      title: "",
      dataIndex: "name",
      key: "key",
      sorter: (a, b) => {
        return;
      },
      sortOrder: "ascend",
      sortDirections: ["ASC", "DESC"],
    },
    // {
    //    title: 'Antall',
    //       dataIndex: 'total',
    //       key: 'total',
    //   },
  ];

  return (
    <div>
      <div>
        <div className="Kj-background-color pl-1 pb-1">
          <span className="install-text">VELG GEOGRAFISK OMRADE</span>
        </div>
        <p className="label p-2 mt-2" style={{ color: "#00B2E4" }}>
          {/* Fjern */}
        </p>

        <TableSegmenter
          columnsArray={columns}
          data={segmenterresultarray}
          page={"segmenter"}
          defaultSelectedColumn={reolID}
          parentCallback={callback}
          setoutputDataList={setOutputData}
        />
      </div>
    </div>
  );
}
export default VegGeografiskOmrade;
