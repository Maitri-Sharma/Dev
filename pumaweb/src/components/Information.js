import React, {useState, useContext} from "react";
import expand from "../assets/images/esri/expand.png";
import collapse from "../assets/images/esri/collapse.png";
import {KSPUContext} from "../context/Context.js";

function Information(props) {
    const [vistogglevalue,
        setvistogglevalue] = useState(false);
    const {showDenking, setShowDenking} = useContext(KSPUContext);
    const {showReserverte, setShowReserverte} = useContext(KSPUContext);

    const vistoggle = () => {
        setvistogglevalue(!vistogglevalue);
    };
    const DenkingClick = (e) => {
        // setShowDenking(e.target.checked);
        if (e.target.checked) {

            setShowDenking(true);
        } else {
            setShowDenking(false);
        }
    };
    const ReserverteClick = (e) => {
        if (e.target.checked) {

            setShowReserverte(true);
        } else {
            setShowReserverte(false);
        }
    };

    return (
        <div className="card Kj-background-color ml-1 mr-1">
            <div className="row ">
                <div className="col-10">
                    <p className="Information p-1">VIS INFORMASJONSELEMENTER</p>
                </div>
                <div className="col-2">
                    {!vistogglevalue
                        ? (<img className="d-flex float-right pt-1 mr-1" src={collapse} onClick={vistoggle}/>)
                        : (<img className="d-flex float-right pt-1  mr-1" src={expand} onClick={vistoggle}/>)}
                </div>
            </div>
            {/* <div className="row mr-1  ">
            <div className="col-7">
               <span className="Information">VIS INFORMASJONSELEMENTER</span>
            </div>
            <div className="col-5">
               {vistogglevalue ?
               <img className="d-flex float-right" src={collapse} onClick={vistoggle}/> :
               <img className="d-flex float-right" src={expand} onClick={vistoggle}/> }
            </div>
         </div> */}
            {vistogglevalue
                ? (
                    <div className="Kj-div-background-color p-1 pt-2 pb-2">
                        <div className=" row">
                            <div className="col-7 ">
                                <div className="pl-4">
                                    <input
                                        className="form-check-input mt-2 "
                                        type="checkbox"
                                        value=""
                                        id="Reserverte"
                                        checked={showReserverte}
                                        onChange={ReserverteClick}/>
                                    <label className="form-check-label label-text" htmlFor="Hush">
                                        {" "}
                                        Reserverte{" "}
                                    </label>
                                </div>
                            </div>
                            <div className="col-5">
                                <div>
                                    <input
                                        className="form-check-input mt-2 "
                                        type="checkbox"
                                        value=""
                                        id="Virk"
                                        checked={showDenking}
                                        onChange={DenkingClick}/>
                                    <label className="form-check-label label-text" htmlFor="Virk">
                                        {" "}
                                        Dekningsgrad{" "}
                                    </label>
                                </div>
                            </div>
                        </div>
                        <br></br>
                    </div>
                )
                : null}
        </div>
    );
}

export default Information;
