import React,{useState} from 'react';
import smallbox from "../assets/images/DekningLegend.png"
import  '../App.css'
function Denkn(props) {
    const [selectvalue,setselectvalue] =useState([]);
    const [desiredvalue,setselectdesiredvalue] = useState("");
    const handleChange = (e) => {
        let value = Array.from(e.target.selectedOptions, option => option.value);
        setselectvalue(value)
      }
      const populate =() =>{
          let text = selectvalue.toString()
          let dataToArray = text.split(',').map(item => item.trim());
          setselectdesiredvalue(dataToArray.join("\n"))

           
      }

return (
    
         <div className="card bg-color"  >
         
  <div className="card Kj-background-color pb-2 pt-1" >
  <div className="row">
          <div className="col-8">
        <span className="dekning-header pl-1" >       
        Dekning
          </span>
          </div>
          <div className="col-4 ">
          <button className="btn  btn-work float-right p-1 mr-1"  type="submit">Lukk Denking</button>
          </div>
 </div>
 </div>
  <div className="dentext">
      <p>Viser prosentvis dekning for ulike aviser og Postreklame Uadressert pr fylke og kommune i tabell. Dekning kan også visualiseres i temakart funksjonen. Det kan være noen avvik mellom avisenes og Postens geografiske grenser.</p>
      </div>
            {/* <div className="p-0 label">
          <span>Aviser</span>
          <span className="">Velg temakart</span>
          </div> */}
       <div className="row">   
      <div className="col-7">
      <span className="label ml-1">Aviser</span>
      <select className="dekning-select btn-work ml-2" multiple onChange={handleChange}>
  <option selected>ADRESSEVISAN</option>
  
  <option value="One">One</option>
  <option value="Two">Two</option>
  <option value="Three">Three</option>
  <option value="Four">Four</option>
  <option value="Five">Five</option>
</select>
<div className="p-1">
<button className="btn  btn-work1 float-right pt-1"  type="submit" onClick={populate}>Velg</button>
</div>

<div className="form-group p-2">
<span className="label">Valgte adviser</span>
    <textarea className="dekning-formControl" value={desiredvalue} id="exampleFormControlTextarea1" rows="5"></textarea>
    <button className="btn  btn-work1 float-right mt-1 mr-0 pb-1 p-1"  type="submit">Fjern</button>
  </div>
  
</div>
<div className="col-5">
<span className="label">Velg temakart</span>
<select className="dekningdivValueText" >
<option selected>Select menu</option>
  <option value="1">One</option>
  <option value="2">Two</option>
  <option value="3">Three</option>
</select>
<button className="btn  btn-work float-right mr-1 p-1 mt-1"  type="submit">Vis temakart</button>

<div className="spanstyle label">
<span>Tegnforklaring</span>
</div>
<div className="pb-4">
<img src={smallbox} alt="Tegnforklaring for dekning" />
</div>
</div>
</div>



        </div>
)

}

export default Denkn;