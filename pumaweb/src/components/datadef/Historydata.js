import React from "react";
import { FormatDate } from "../../common/Functions";
function Historydata(props) {
  return (
    <tr className="CriteriaText">
      <th className="CriteriaText pl-1">
        {FormatDate(props.Item.modificationTime)}
      </th>
      <th className="CriteriaText pl-1">{props.Item.userId}</th>
    </tr>
  );
}

export default Historydata;
