import React, { useState, useRef, useContext, useEffect } from "react";
import api from "../../services/api";
import "../../App.css";
import Swal from "sweetalert2";
import $ from "jquery";
import { KSPUContext } from "../../context/Context.js";
import { groupBy } from "../../Data";
import {
  CreateActiveUtvalg,
  GetImageUrl,
  NumberFormat,
} from "../../common/Functions";
function Utvalgselectiondata(props) {
  const {
    activUtvalg,
    setActivUtvalg,
    setvalue,
    setAktivDisplay,
    setResultData,
    activUtvalglist,
    setActivUtvalglist,
    utvalglistcheck,
    setutvalglistcheck,
    setExpandListId,
    showorklist,
  } = useContext(KSPUContext);
  const [newclass, setNewClass] = useState(false);
  useEffect(() => {
    //debugger;
    if (
      activUtvalglist.basedOn !== 0 &&
      activUtvalglist.basedOn !== undefined &&
      activUtvalglist.basedOn !== ""
    ) {
      setNewClass(true);
    } else {
      setNewClass(false);
    }
  }, []);

  const openUtvalg = async (e) => {
    let url = `Utvalg/GetUtvalg?utvalgId=${e.target.id}`;
    try {
      const { data, status } = await api.getdata(url);
      if (data.length === 0) {
        $(".modal").remove();
        $(".modal-backdrop").remove();
        Swal.fire({
          text: `No Data Present`,
          confirmButtonColor: "#7bc144",
          confirmButtonText: "Lukk",
        });
      } else {
        await setResultData(await groupBy(data.reoler, "", 0, 0, 0, 0, [])); //JSON roeler needs to pass

        await setActivUtvalg(await CreateActiveUtvalg(data));
        showorklist?.map((item) => {
          if (item?.memberUtvalgs?.length > 0) {
            item?.memberUtvalgs?.map((i) => {
              if (i?.utvalgId === data?.utvalgId) {
                let arr = [];
                arr?.push(item?.listId?.toString());
                setExpandListId(arr);
              }
            });
          }
        });
        setutvalglistcheck(false);
        setActivUtvalglist([]);
      }
    } catch (error) {
      // setAlert(true);
      console.error("error : " + error);
      //setFinn(false);
    }
  };

  return (
    <div className="row col-12 p-0 m-0 pl-1 pr-1">
      <div className="col-9 m-0 p-0 selectionTable_left pl-1">
        <span
          id={props.Item.utvalgId}
          onClick={openUtvalg}
          className={newclass ? "disableDiv" : ""}
        >
          {props.Item.name}
        </span>
      </div>
      <div className="col-3 m-0 p-0 selectionTable_right pr-1">
        <span>{NumberFormat(props.Item.totalAntall)}</span>
      </div>
    </div>
  );
}

export default Utvalgselectiondata;
