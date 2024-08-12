import React,{useState,useRef,useEffect} from 'react';
import MottakerComponent from './Mottakergrupper'
import  "../App.css";
import Submit_Button from './Submit_Button';
import Geographie_footer from "./Geographie_footer";
import api from '../services/api.js';


function Postboks () {
    const searchRute = useRef(null);
    const searchKommune = useRef(null);
    const summaryBox = useRef(null);
    const kommuneBox = useRef(null);
    const resultBox = useRef(null);
    const [disableBtn, setBtnDisable] = useState(true);
    const [kommunedatalist, setKommuneData] = useState([]);

    const fetchData = async (url) => {
        try {            
               
            const {data , status} = await api.getdata(url);
              if(status === 200)
              { 
                  if(data.length > 0)
                  {                   
                    data.sort((a, b) => {
                        return a.reolNumber - b.reolNumber;
                    });

                    data.map(function(d, i){
                           
                        var newOption= new Option(d.name,d.reolId);      
                        summaryBox.current.add(newOption,i);
                      });                
                   }
              }
              else
              {
                console.error('error : ' + status);
              }
            } 
            catch (error) {
              console.error('er : ' + error);
            }
    };

       
    useEffect(() => {
        fetchKommuneData();
        
      }, []);

      const fetchKommuneData = async () => {
        try {            
          const {data , status} = await api.getdata('Kommune/GetAllKommunes');
              if(status === 200)
              {
                setKommuneData(data);
              }
              else
              {
                console.error('error : ' + status);
              }
            } 
            catch (error) {
              console.error('er : ' + error);
            }
    };


    function fetchRouteData(){
        fetchData('Reol/SearchReolPostboksByReolName?reolName='+searchRute.current.value +'&kommuneName='+searchKommune.current.value);
    };

    const handlenextchange =(e)=>{
        kommuneBox.current.options.length = 0;
        kommuneBox.current.style.display = "block";
        if(e.target.value == '')
        { 
            if(kommuneBox.current.options.length > 0) kommuneBox.current.options.splice(kommuneBox.current.options.length - 1, 0);    
            return;
        }        
        for (var i = 0; i < kommunedatalist.length; i++) {
            var txt = kommunedatalist[i].kommuneName.toUpperCase();
            
            if(txt.toLowerCase().startsWith(e.target.value.toLowerCase()))
            {
              var newOption = new Option(txt, txt);
              kommuneBox.current.add(newOption,kommuneBox.current.length);     
            }
          }
        
    }

    const handleBoxClick =(e) => {
        searchKommune.current.value =e.target.value;
        kommuneBox.current.style.display = "none"; 
        
    }

    function addRouteData(){        
        Array.from(summaryBox.current.options)
                    .filter((x) => x.selected)
                    .map((d)=>{
                       if(!Array.from(resultBox.current.options).map((opt) => opt.value).includes(d.value)){
                        var newOption = new Option(d.text,d.value);
                        resultBox.current.add(newOption,resultBox.current.length); 
                       }
                                               
                    });        
        setBtnDisable(false);       
    };

    const handleRemoveClick =(e)=>{
        Array.from(resultBox.current.options)
        .filter((x) => x.selected)
        .map((d)=>{
            resultBox.current.remove(d.index);                     
        });       
        if(resultBox.current.length === 0)
        {
          setBtnDisable(true);  
        }      
      }


    return  (
        <div className="pb-2">
        <span className="sok-text p-1" >       
        Navn på boksanlegg          </span>
        <div className="row pl-3 pt-1">
<div className="input-groupco-4">
<i className="fa fa-user-circle-o pl-1"></i>
<input type="text" ref={searchRute} className="KommunInputText mt-1" placeholder=""/>
</div>
<input type="submit" onClick={fetchRouteData} className="KSPU_button ml-3 mt-1" value="Søk"/>
</div>
<span className="sok-text p-1" >       
Spesifiser evt. kommune          </span>
<div className="row pl-3 pt-1">
<div className="input-groupco-4">
<i className="fa fa-user-circle-o pl-1"></i>
<input type="text" ref={searchKommune} className="KommunInputText mt-1" onChange={handlenextchange} placeholder=""/>
</div>
<input type="submit" className="KSPU_button ml-3 mt-1" value="Velg"/>
</div>
<select size="3" ref={kommuneBox} className="KommunListbox buttonHidden mt-1" onClick={handleBoxClick} multiple></select>
<div className="pt-2">

<span className="sok-text p-1" >       
Finn boksanlegg          </span>
<div className="row ml-1 pt-2 pb-2">


<select size="4" ref={summaryBox} className="PostbokListbox  buttonHidden" multiple>
</select>

<div>
<input type="submit" onClick={addRouteData} className="KSPU_button ml-3 mt-1" value="Velg" l/>
</div>
</div>

</div>

<div>

<span className="sok-text p-1" >       
Finn boksanlegg          </span>

<div className="row ml-1 pt-2">
<select size="4" ref={resultBox} className="PostbokListbox buttonHidden" multiple>
</select>
<div className="col-2 mr-1">
<input type="submit" className="KSPU_button" disabled={disableBtn} value="Vis i kart" />
 <input type="submit" onClick={handleRemoveClick} className="KSPU_button" disabled={disableBtn} value="Fjern" />
</div>

</div>
{/* <MottakerComponent/> */}
<Geographie_footer name=" PostBoks" />

</div>

</div>


    )
}


export default Postboks;