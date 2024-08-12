import React,{useState,useRef,useContext,useEffect} from 'react';
import {getCriteriaText} from '../KspuConfig' 
function Commondata(props) {
  const [selectedvalue,setselectedvalue] = useState(false);
  if(props.Item.Criteria)
  setselectedvalue(true)
    return (
      <div>
      { selectedvalue ?
      <tr className="CriteriaText">
        
        <th className="CriteriaText pl-1">{getCriteriaText(props.Item.CriteriaType)}</th>
        
        <th className="CriteriaText pl-1">{props.Item.Criteria}</th>
        
      </tr>
      : null }
      </div>
    );
}

export default Commondata;