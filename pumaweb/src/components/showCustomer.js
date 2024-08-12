// import React, { useState, useRef, useContext, useEffect } from "react";
// import { KSPUContext } from "../context/Context";
// import ".././App.css";

// function ShowCustomer(props) {
//   const [kunde_name, setkunde_name] = useState(
//     "POSTEN OG BRING TESTKUNDE KONSERNINTERN PPOST"
//   );
//   const [kunde_number, setkunde_number] = useState("1964");

//   const Velg = () => {
//     let value = kunde_number;
//     setkunde_number(value);
//     };
//   return (
//     <div>
//       <div
//         className="modal fade bd-example-modal-lg"
//         data-backdrop="false"
//         id={props.id}
//         tabIndex="-1"
//         role="dialog"
//         aria-labelledby="exampleModalCenterTitle"
//         style={{ zIndex: "1051" }}
//         aria-hidden="true"
//       >
//         <div
//           className="modal-dialog modal-dialog-centered viewDetail"
//           role="document"
//         >
//           <div className="modal-content" style={{ border: "red 3px solid" }}>
//             <div className="Common-modal-header">
//               <span
//                 className="common-modal-title pt-1 pl-2"
//                 id="exampleModalLongTitle"
//               >
//                 SØKERESULTAT
//               </span>
//               <button
//                 type="button"
//                 className="close"
//                 data-dismiss="modal"
//                 aria-label="Close"
//                 //ref={btnClose}
//               >
//                 <span aria-hidden="true">&times;</span>
//               </button>
//             </div>
//             <div className="View_modal-body budrutebody">
//               <table className="tableRow">
//                 <tbody>
//                   <tr className="flykeHeader">
//                     <th className="tabledataRow budruteRow">Kundenavn</th>
//                     <th className="tabledataRow budruteRow">Kundenummer</th>
//                     <th className="tabledataRow budruteRow">
//                       &nbsp;&nbsp;&nbsp;&nbsp;
//                     </th>
//                   </tr>
//                   <th className="tabledataRow">
//                     {" "}
//                     <tr>
//                       <td className="flykecontent">
//                         <tr>
//                           <td className="flykecontent">{kunde_name}</td>
//                         </tr>
//                       </td>
//                     </tr>
//                   </th>

//                   <th className="tabledataRow">
//                     {" "}
//                     <tr>
//                       <td className="flykecontent">
//                         <tr>
//                           <td className="flykecontent">{kunde_number}</td>
//                         </tr>
//                       </td>
//                     </tr>
//                   </th>

//                   <th className="tabledataRow">
//                     {" "}
//                     <tr>
//                       <td className="flykecontent">
//                         <p
//                           id={kunde_number}
//                           data-dismiss="modal"
//                           className="KSPU_LinkButton float-right mr-1"
//                           onClick={Velg}
//                         >
//                           velg
//                         </p>
//                       </td>
//                     </tr>
//                   </th>
//                 </tbody>
//               </table>
//               {/* <div className="row">
//                 <div className="col-4">Kundenavn</div>
//                 <div className="col-4">Kundenummer</div>
//                 <div className="col-4">
//                   <a
//                     id=""
//                     href=""
//                     className="KSPU_LinkButton float-right mr-1"
//                     onClick={Velg}
//                   >
//                     velg
//                   </a>
//                 </div>
//               </div> */}
//             </div>
//           </div>
//         </div>
//       </div>
//     </div>
//   );
// }

// export default ShowCustomer;
