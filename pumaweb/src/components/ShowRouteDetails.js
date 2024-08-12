import React from "react";

function ShowRouteDetails(props) {
  return (
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
          <div className="modal-header segFord">
            <h5 className="modal-title " id="exampleModalLongTitle">
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
          <div className="modal-body flykebody">
            <div>
              <div className="scrollbarmodal">
                <table className="tableRow">
                  <tbody>
                    <tr className="flykeHeader">
                      <th className="tabledataRow">Fylke</th>
                      <th className="tabledataRow">Kommune</th>
                      <th className="tabledataRow">Team</th>
                      <th className="tabledataRow">Område</th>
                      <th className="tabledataRow">Ant HH</th>
                      <th className="tabledataRow">Rest vekt</th>
                      <th className="tabledataRow">Rest Thickness</th>
                      <th className="tabledataRow">Rest Sendinger</th>
                    </tr>
                    {props.routeData?.ruteInfo
                      .sort((a, b) => (a.fylke > b.fylke ? 1 : -1))
                      .map((item, index) => (
                        <tr>
                          <td className="tabledataRow"> {item.fylke}</td>

                          <td className="tabledataRow"> {item.kommune}</td>

                          <td className="tabledataRow"> {item.team}</td>

                          <td className="tabledataRow"> {item.ruteNavn}</td>

                          <td className="tabledataRow">
                            {" "}
                            {item?.ruteAntallMotakere}
                          </td>
                          <td className="tabledataRow"> {item?.restVekt}</td>
                          <td className="tabledataRow">
                            {" "}
                            {item?.restThickness}
                          </td>
                          <td className="tabledataRow"> {item?.restAntall} </td>
                        </tr>
                      ))}
                  </tbody>
                </table>
              </div>
              <div className="row flykebody">
                <div className="col-9 pt-3">
                  <button
                    type="button"
                    className="btn btn-default SegmenterCancel_Button float-right "
                    id="maksimer_id"
                    data-dismiss="modal"
                  >
                    Lukk
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default ShowRouteDetails;
