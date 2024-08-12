import React, {  useRef, useEffect } from "react";
import "../message-popup/messagepopup.styles.scss";

export default function Messagepopup(props) {
  const btnClose = useRef();

  useEffect(() => {
    btnClose.current.click();
  }, []);

  return (
    <div>
      <div
        className="modal fade bd-example-modal-lg"
        data-backdrop="false"
        id="showList"
        tabIndex="-1"
        role="dialog"
        aria-labelledby="exampleModalCenterTitle"
        style={{ zIndex: "1051", height: "auto" }}
        aria-hidden="true"
      >
        <div
          className="modal-dialog modal-dialog-centered width-70"
          role="document"
        >
          <div
            className="modal-content"
            style={{ border: "black 3px solid", height: "auto" }}
          >
            <div className="Common-modal-header">
              <span
                className="common-modal-title pt-1 pl-2"
                id="exampleModalLongTitle"
              >
                {props.headerText}
              </span>
              <button
                type="button"
                className="close"
                data-dismiss="modal"
                aria-label="Close"
                //ref={btnClose}
              >
                <span aria-hidden="true">&times;</span>
              </button>
            </div>
            <div className="View_modal-body View_modal-body-scrollable budrutebody">
              <p className="divValueText_popup ml-2 width-400">{props.bodyText}</p>
              <div className="row ml-0 mr-0">
                <div className="col-2">
                  <button
                    type="button"
                    className="btn KSPU_button"
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

      <div>
        <input
          type="submit"
          className="KSPU_button"
          name="uxDialogs_uxPopup"
          value="Finn"
          data-toggle="modal"
          data-target="#showList"
          id="uxDialogs_uxPopup"
          ref={btnClose}
          style={{ visibility: false }}
        />
      </div>
    </div>
  );
}
