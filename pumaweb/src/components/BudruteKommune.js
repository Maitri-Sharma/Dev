// import React, { useState, useContext, useEffect } from "react";
// import "../App.css";
// import { KundeWebContext } from "../context/Context.js";

// function Budrutekommune(props) {
//   const [displayMsg, setDisplayMsg] = useState(false);
//   const [selectedvalue, setselectedvalue] = useState("");
//   const [gateValue, setGateValue] = useState(KundeWebContext);

//   const dropdownselection = (e) => {
//     setselectedvalue(e.target.value);
//   };
//   const buderuteVelg = () => {};

//   return (
//     <div>
//       {/* <!-- Modal --> */}
//       <div
//         className="modal fade bd-example-modal-lg"
//         id={props.id}
//         tabIndex="-1"
//         role="dialog"
//         aria-labelledby="exampleModalCenterTitle"
//         aria-hidden="true"
//       >
//         <div
//           className="modal-dialog modal-dialog-centered kommuneModel "
//           role="document"
//         >
//           <div className="modal-content">
//             <div className="modal-body">
//               <label>Legg inn et navn htmlFor merking av adressepunktet:</label>
//               <input
//                 type="text"
//                 className="form-control kommuneModel"
//                 id="text"
//                 aria-describedby="emailHelp"
//                 placeholder=""
//               />{" "}
//             </div>
//             <div className="modal-footer">
//               <button type="button" className="btn btn-primary">
//                 Ok
//               </button>
//               <button
//                 type="button"
//                 className="btn btn-secondary"
//                 data-dismiss="modal"
//               >
//                 Cancel
//               </button>
//             </div>
//           </div>
//         </div>
//       </div>
//     </div>
//   );
// }

// export default Budrutekommune;
