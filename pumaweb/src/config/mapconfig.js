export const MapConfig = {
  zoom: 2,
  center: [12, 64],

  inernwebMapExtent: {
    xmin: -915020.1799341175,
    ymin: 6373074.350009031,
    xmax: 1564024.778155799,
    ymax: 7833407.937342872,
    spatialReference: {
      wkid: 32633,
    },
  },

  kundewebMapExtent: {
    xmin: -392943.35479620297,
    ymin: 6363722.055130746,
    xmax: 1137833.0400899202,
    ymax: 7918882.498784967,
    spatialReference: {
      wkid: 32633,
    },
  },

  vectorTileLayerUrl:
    "https://services.geodataonline.no/arcgis/rest/services/GeocacheVector/GeocacheBasis_WGS84/VectorTileServer/",

  kspuLayerUrl:
    "https://dev.pumamapservices.bring.no/arcgis/rest/services/KSPU/MapServer/",

  kundewebLayerUrl:
    "https://dev.pumamapservices.bring.no/arcgis/rest/services/Pumakundeweb/MapServer",

  geoKodingUrl:
    "https://dev.pumamapservices.bring.no/arcgis/rest/services/GeoKoding/GeocodeServer",

  geoSokUrl:
    "https://dev.pumamapservices.bring.no/arcgis/rest/services/Geosok/GeocodeServer",

  driveTimeAnalysisUrl:
    "https://dev.pumamapservices.bring.no/arcgis/rest/services/DriveTime/GPServer",

  oldUtvalgGeometryUrl:
    "https://dev.pumamapservices.bring.no/arcgis/rest/services/OldUtvalgGeometry/MapServer/0",

  budruterOutField: [
    "reolnavn",
    "reolnr",
    "beskrivelse",
    "kommentar",
    "reol_id",
    "kommuneid",
    "kommune",
    "fylkeid",
    "fylke",
    "teamnr",
    "teamnavn",
    "postnr",
    "poststed",
    "segment",
    "hh",
    "hh_res",
    "gb",
    "gb_res",
    "er",
    "er_res",
    "vh",
    "p_hh_u_res",
    "np_hh_u_res",
    "p_vh_u_res",
    "np_vh_u_res",
    "prissone",
    "reoltype",
    "pbkontnavn",
    "prsnr",
    "prsnavn",
    "prsbeskrivelse",
    "rutedistfreq",
    "sondagflag",
  ],
};
