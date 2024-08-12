import React from 'react';

function Selectdropdown(props) {
    return (
        <div>
            <select 
            onClick={(e)=>props.onClick(e)}
            >
                <option key={'select'} hidden >Select</option>
                {props.list.length>0 &&
                props.list.map((obj)=>{
                    return <option value={obj.name} key ={obj.id}>{obj.name}</option>
                })}
            </select>

            </div>
    );
}

export default Selectdropdown;