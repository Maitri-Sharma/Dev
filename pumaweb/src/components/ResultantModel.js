import React, { useState, useContext } from "react";
import TableNew from "./TableNew";
import { KSPUContext } from "../context/Context.js";
import { getCriteriaText } from "./KspuConfig";
import { NumberFormat } from "../common/Functions";

function ResultantModel(props) {
  const [outputData, setOutputData] = useState([]);
  const { activUtvalg, setActivUtvalg } = useContext(KSPUContext);
  const { showHousehold, setShowHousehold,showBusiness, setShowBusiness } = useContext(KSPUContext);
  const { showReservedHouseHolds, setShowReservedHouseHolds } =
    useContext(KSPUContext);
  const columns = [
    {
      title: "Fylke\\Kommun\\Team\\Rute",
      dataIndex: "name",
      key: "key",
      sorter: (a, b) => {
        return;
      },
      sortOrder: "ascend",
      sortDirections: ["ASC", "DESC"],
    },
    {
      title: "Hush.",
      dataIndex: "house",
      key: "house",
      align: "right",
      render: (house) => NumberFormat(house),
    },
    {
      title: "Sone 0",
      dataIndex: "zone0",
      key: "zone0",
      align: "right",
      render: (zone0) => NumberFormat(zone0),
    },
    {
      title: "Sone 1",
      dataIndex: "zone1",
      key: "zone1",
      align: "right",
      render: (zone1) => NumberFormat(zone1),
    },
    {
      title: "Sone 2",
      dataIndex: "zone2",
      key: "zone2",
      align: "right",
      render: (zone2) => NumberFormat(zone2),
    },
    {
      title: "Total",
      dataIndex: "total",
      key: "total",
      align: "right",
      render: (total) => NumberFormat(total),
    },
  ];

  return (
    <div>
      {/* <!-- Modal --> */}
      <div
        className="modal fade bd-example-modal-lg"
        id={props.id}
        tabIndex="-1"
        role="dialog"
        aria-labelledby="exampleModalCenterTitle"
        aria-hidden="true"
      >
        <div
          className="modal-dialog modal-dialog-centered resultmaximizer"
          role="document"
        >
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title" id="exampleModalLongTitle">
                {props.title}
              </h5>
              <button
                type="button"
                className="close"
                data-dismiss="modal"
                aria-label="Close"
              >
                <span aria-hidden="true">&times;</span>
              </button>
            </div>
            <div className="modal-body">
              <div className="row pb-2">
                <div className="col-3">
                  <span id="TotalAntall" className="UtvaldivLabelText ml-2">
                    Totalt antall{" "}
                  </span>
                </div>
                <div className="col-2" style={{ textAlign: "left" }}>
                  <span id="" className="divValueText">
                    {
                      (activUtvalg.totalAntall =
                        (showHousehold ? activUtvalg.Antall[0] : 0) +
                        (showBusiness ? activUtvalg.Antall[1] : 0) +
                        (showReservedHouseHolds ? activUtvalg.Antall[2] : 0))
                    }
                  </span>
                </div>
                <div className="col-4">
                  <span id="Mottakergrupper" className="UtvaldivLabelText">
                    Mottakergrupper{" "}
                  </span>
                </div>
                <div className="col-3" style={{ textAlign: "left" }}>
                  <span id="" className="divValueText">
                  
                    {showHousehold
                      ? `Hush. : ${activUtvalg.Antall[0]} `
                      : ""}{" "}
                    
                    {showBusiness ? <br /> : ""}
                    {showBusiness
                      ? `Virk. : ${activUtvalg.Antall[1]} `
                      : ""}{" "}
                    {showReservedHouseHolds ? <br /> : ""}
                    {showReservedHouseHolds
                      ? ` Res.hush. : ${activUtvalg.Antall[2]} `
                      : ""}
                  </span>
                </div>
              </div>
              <div className="pl-2">
                <TableNew
                  width1={""}
                  columnsArray={columns}
                  data={props.dataResult}
                  setoutputDataList={setOutputData}
                  hideselection={1}
                />
                <br />
              </div>
            </div>
            <div className="modal-footer">
              {/* <button type="button" className="btn btn-secondary" data-dismiss="modal">Lukk</button> */}
              <a
                id="maksimer_id"
                className="KSPU_LinkButton float-right mr-1"
                data-dismiss="modal"
              >
                {" "}
                Minimer
              </a>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default ResultantModel;
