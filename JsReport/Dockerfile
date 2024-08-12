#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM jsreport/jsreport:3.2.0
RUN npm i nodemailer --save-exact 
COPY --chown=jsreport:jsreport JsReport/jsreport.config.json /app
COPY --chown=jsreport:jsreport JsReport/data /app/data
