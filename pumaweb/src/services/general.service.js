import http from './http';
 const api = 'users';

 export const sampleapi = () => {
     
    return http.get(`${api}`)
 }

 export default {
     sampleapi
 }