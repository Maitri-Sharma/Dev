import React from "react";
import Calendar from "./Calendar";
import Helmet from "react-helmet";

const Modal = (props) => {
  const divStyle = props.displayModal ? "block" : "none";
  
  function closeModal(e) {
    e.stopPropagation();
    props.closeModal();
  }

  const handleCallback = (childData) => {
    props.parentCallback(childData);
  };

  return (
    <div style={{ width: "55rem" }}>
      <table
        className="overlay table_menu"
        style={{
          border: "2px solid red",
        }}
      >
        <tr>
          <td
            style={{
              width: "17%",
            }}
          >
            <div className="calender-color">
              Kalender
              <button
                type="button"
                className="close float-right"
                data-dismiss="modal"
                aria-label="Close"
                onClick={closeModal}
              >
                <span aria-hidden="true">&times;</span>
              </button>
            </div>
            <td>
              <Calendar
                page="Modelpage"
                fontSize="10px"
                parentCallback={handleCallback}
              />
            </td>
          </td>
        </tr>
      </table>
    </div>
  );
};

export default Modal;
