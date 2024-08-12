import React,{ useState } from 'react';
//import { useState } from 'react/cjs/react.production.min';
//import Selectdropdown from './Selectdropdown';
import style from '../App.css';

function Søk(props) {
    const [value,setvalue]=useState([]);
    const [option,setoption]=useState("")
    const onselectionchange =(e)=>{
        let t =[{id:1,name:"Abhishek"}]
        setvalue(t)
    }
    return (
        <div>
            <table className={style.divPanelBody} cellpadding="2" cellspacing="2">
    <tr>
        <td className={style.divPanelTop}>
            <span className={style.divHeaderText}>Hi Everyone</span>
        </td>
    </tr>
    <tr>
        <td style={{height: "21px"}}>
            Minimum 3 tegn i ett av feltene<br />
            <img id="selectDummy1" alt="Sok" src="./Assets/Images/selectDummy.gif" style={{display:"none"}} />
            <select id="uxDropDownListUtvalg" className={style.divValueText}  runat="server" AutoPostBack="False" title="Begrens søket med">
                <option value="1" Selected="True">Utvalgsnavn</option>
                <option value="2">Utvalgsliste</option>
                <option value="3">Utvalgs-/ListeID</option> 
            </select>&nbsp;<span className={style.divErrorText}>*</span>
            &nbsp;&nbsp;<input type="checkbox" id="uxChkbBasisOnly" runat="server" Text="Vis bare basisutvalg og -lister" />
        </td>
    </tr>
</table>
    </div>
    );
}

export default Søk;