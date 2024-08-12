import axios from 'axios';

const url = `${process.env.REACT_APP_API_URL}`




/****************
 purpose : USed to call the GET method in http request

 Parameter : {

    apiUrl : accept string (/apiName)

    data : accept Object
 }
Retrun : Object

 *******************/

 export const get = (apiUrl, data = null) => {
     let qryString = `${process.env.REACT_APP_API_URL}${apiUrl}`;
     
     if (data !== null) {

        const keys = Object.keys(data);

        keys.forEach((key, i) => qryString += `${i===0 ? '?' : '&'} ${key}=${data[key]}`)
     }

     const options = {
         method : 'GET',

         headers : {
             'content-type' : 'application/json',

             'Access-control-Allow-Origin' : '*',

         },

         url : qryString
         }

         return axios(options)
            .then(function (response) {
                return response;
            })
         .catch( function (err){
             console.log("err", err.response);
            
          if(!err.response)
          
            return {status: 500}
            

            return err.response;
         })   
     
 }

 /****************
 purpose : Used to call the POST method in http request

 Parameter : {

    apiUrl : accept string (/apiName)

    data : accept Object
 }
Retrun : Object

 *******************/

export const post = (apiUrl, data) => {
   // let qryString = `${url}${apiUrl}`;

    

    const options = {
        method : 'POST',

        headers : {
            'content-type' : 'application/json',

            'Access-control-Allow-Origin' : '*',

        },
        data : data,
        url : `${url}${apiUrl}`
        }

        return axios(options)
           .then(function (response) {
               return response;
           })
        .catch( function (err){
            console.log("err", err.response);
           
         if(!err.response)
         
           return {status: 500}
           

           return err.response;
        })   
    
}

 /****************
 purpose : Used to call the PUT method in http request

 Parameter : {

    apiUrl : accept string (/apiName)

    data : accept Object
 }
Retrun : Object

 *******************/

export const put = (apiUrl, query = null, data = null) => {
    let qryString = `${url}${apiUrl}`;

    if (query !== null) {

       const keys = Object.keys(data);

       keys.forEach((key, i) => qryString += `${i===0 ? '?' : '&'} ${key}=${query[key]}`)
    }

    const options = {
        method : 'PUT',

        headers : {
            'content-type' : 'application/json',

            'Access-control-Allow-Origin' : '*',

        },

        url : qryString,

        data : data
        }

        return axios(options)
           .then(function (response) {
               return response;
           })
        .catch( function (err){
            console.log("err", err.response);
           
         if(!err.response)
         
           return {status: 500}
           

           return err.response;
        })   
    
}

 /****************
 purpose : Used to call the DELETE method in http request

 Parameter : {

    apiUrl : accept string (/apiName)

    data : accept Object
 }
Retrun : Object

 *******************/


export const delete_ = (apiUrl, data = null) => {
    let qryString = `${url}${apiUrl}`;

    if (data !== null) {

       const keys = Object.keys(data);

       keys.forEach((key, i) => qryString += `${i===0 ? '?' : '&'} ${key}=${data[key]}`)
    }

    const options = {
        method : 'DELETE',

        headers : {
            'content-type' : 'application/json',

            'Access-control-Allow-Origin' : '*',

        },

        url : qryString
        }

        return axios(options)
           .then(function (response) {
               return response;
           })
        .catch( function (err){
            console.log("err", err.response);
           
         if(!err.response)
         
           return {status: 500}
          

           return err.response;
        })   
    
}

export default {

    get,
    post,
    put,
    delete : delete_

}




