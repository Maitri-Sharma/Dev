// import ArcGISMap from "@arcgis/core/Map";
// import MapView from "@arcgis/core/views/MapView";
// import GraphicsLayer from "@arcgis/core/layers/GraphicsLayer";
import FeatureLayer from "@arcgis/core/layers/FeatureLayer";
// import MapImageLayer from "@arcgis/core/layers/MapImageLayer";
// import VectorTileLayer from "@arcgis/core/layers/VectorTileLayer";
// import Graphic from "@arcgis/core/Graphic";
// import SimpleFillSymbol from "@arcgis/core/symbols/SimpleFillSymbol";
// import esriConfig from "@arcgis/core/config.js";
// import SimpleRenderer from "@arcgis/core/renderers/SimpleRenderer";
// import Point from "@arcgis/core/geometry/Point";

const mapServiceExportEndpoint = `${process.env.REACT_APP_MapExport_API_URL}`;

export async function ExportMapImage(
  reolids,
  isCustomerWeb,
  strDayDetails,
  selectAddress
) {
  const mapServiceEndpoint = `${process.env.REACT_APP_Map_API_URL}`;

  let routeids = [];
  routeids.push(
    reolids.map((item) => {
      return "'" + `${item["reolId"]}` + "'";
    })
  );
  let cmd = "reol_id in (" + routeids.join(",") + ")";

  if (isCustomerWeb) {
    cmd = cmd + " AND UPPER(REOLTYPE) <> UPPER('Boks')";
  }

  const featureLayer = new FeatureLayer({
    url: mapServiceEndpoint + "/5",
    outFields: ["*"],
    definitionExpression: cmd,
  });

  let adrlayerId = -1;
  switch (strDayDetails) {
    case "A-uke, tidliguke":
      adrlayerId = 0;
      break;
    case "A-uke, midtuke":
      adrlayerId = 1;
      break;
    case "B-uke, tidliguke":
      adrlayerId = 2;
      break;
    case "B-uke, midtuke":
      adrlayerId = 3;
      break;
    default:
      adrlayerId = -1;
      break;
  }

  let extent;

  await featureLayer
    .load()
    .then(
      await featureLayer
        .queryExtent()
        .then((response) => (extent = response.extent))
    );

  let featuresPoint = [];
  let featuresText = [];
  if (selectAddress) {
    var selectedAddress = JSON.parse(sessionStorage.getItem("addressPoints"));
    if (selectedAddress.length > 0) {
      selectedAddress.forEach((addrPoint) => {
        featuresPoint.push({
          geometry: {
            x:
              addrPoint.location !== undefined
                ? addrPoint.location.x
                : addrPoint.geometry.x,
            y:
              addrPoint.location !== undefined
                ? addrPoint.location.y
                : addrPoint.geometry.y,
            spatialReference: {
              wkid: 32633,
            },
          },
        });
        featuresText.push({
          geometry: {
            x:
              addrPoint.location !== undefined
                ? addrPoint.location.x
                : addrPoint.geometry.x,
            y:
              addrPoint.location !== undefined
                ? addrPoint.location.y
                : addrPoint.geometry.y,
            spatialReference: {
              wkid: 32633,
            },
          },
          symbol: {
            type: "esriTS",
            color: [0, 0, 255, 255],
            verticalAlignment: "bottom",
            horizontalAlignment: "left",
            text:
              addrPoint.address !== undefined
                ? addrPoint.address
                : addrPoint.attributes.Match_addr,
            xoffset: 3,
            yoffset: 3,
            font: {
              family: "Arial",
              size: 12,
              style: "normal",
              weight: "bold",
              decoration: "none",
            },
          },
        });
      });
    }
  }

  //let dynamicLayer = [{"id":5,"source":{"type":"mapLayer","mapLayerId":5},"definitionExpression":cmd,"drawingInfo":{"showLabels":true}},{"id":7,"source":{"type":"mapLayer","mapLayerId":7},"drawingInfo":{"showLabels":true}},{"id":11,"source":{"type":"mapLayer","mapLayerId":11},"drawingInfo":{"showLabels":true}},{"id":12,"source":{"type":"mapLayer","mapLayerId":12}},{"id":16,"source":{"type":"mapLayer","mapLayerId":16},"drawingInfo":{"showLabels":true}},{"id":20,"source":{"type":"mapLayer","mapLayerId":20},"drawingInfo":{"showLabels":true}},{"id":21,"source":{"type":"mapLayer","mapLayerId":21},"drawingInfo":{"showLabels":true}},{"id":22,"source":{"type":"mapLayer","mapLayerId":22},"drawingInfo":{"showLabels":true}},{"id":23,"source":{"type":"mapLayer","mapLayerId":23},"drawingInfo":{"showLabels":true}},{"id":24,"source":{"type":"mapLayer","mapLayerId":24},"drawingInfo":{"showLabels":true}},{"id":27,"source":{"type":"mapLayer","mapLayerId":27}},{"id":29,"source":{"type":"mapLayer","mapLayerId":29}},{"id":41,"source":{"type":"mapLayer","mapLayerId":41}},{"id":45,"source":{"type":"mapLayer","mapLayerId":45}},{"id":50,"source":{"type":"mapLayer","mapLayerId":50}},{"id":51,"source":{"type":"mapLayer","mapLayerId":51}},{"id":58,"source":{"type":"mapLayer","mapLayerId":58}},{"id":65,"source":{"type":"mapLayer","mapLayerId":65},"drawingInfo":{"showLabels":true}}]
  //latest let dynamicLayer = [{"id":5,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":5},"definitionExpression": cmd,"drawingInfo":{"showLabels":true}}},{"id":7,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":7},"drawingInfo":{"showLabels":true}}},{"id":11,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":11},"drawingInfo":{"showLabels":true}}},{"id":12,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":12}}},{"id":16,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":16},"drawingInfo":{"showLabels":true}}},{"id":20,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":20},"drawingInfo":{"showLabels":true}}},{"id":21,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":21},"drawingInfo":{"showLabels":true}}},{"id":22,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":22},"drawingInfo":{"showLabels":true}}},{"id":23,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":23},"drawingInfo":{"showLabels":true}}},{"id":24,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":24},"drawingInfo":{"showLabels":true}}},{"id":27,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":27}}},{"id":29,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":29}}},{"id":41,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":41}}},{"id":45,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":45}}},{"id":50,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":50}}},{"id":51,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":51}}},{"id":53,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":53}}},{"id":55,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":55}}},{"id":58,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":58}}},{"id":60,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":62}}},{"id":65,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":65},"drawingInfo":{"showLabels":true}}},{"id":67,"layerDefinition": {"source":{"type":"mapLayer","mapLayerId":67},"drawingInfo":{"showLabels":true}}}]

  let dynamicLayer = [
    {
      id: 5,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 5 },
        definitionExpression: cmd,
        drawingInfo: {
          showLabels: true,
          renderer: {
            type: "simple",
            symbol: {
              type: "esriSFS",
              style: "esriSFSSolid",
              color: [237, 54, 21, 70],
              outline: {
                type: "esriSLS",
                color: [237, 54, 21, 255],
                width: 0.75,
                style: "esriSLSSolid",
              },
            },
          },
        },
      },
    },
    {
      id: 7,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 7 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 10,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 10 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 11,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 11 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 12,
      layerDefinition: { source: { type: "mapLayer", mapLayerId: 12 } },
    },
    {
      id: 15,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 15 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 16,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 16 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 17,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 17 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 18,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 18 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 20,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 20 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 21,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 21 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 22,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 22 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 23,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 23 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 24,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 24 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 27,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 27 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 28,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 28 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 29,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 29 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 31,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 31 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 32,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 32 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 35,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 35 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 38,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 38 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 39,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 39 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 41,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 41 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 42,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 42 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 43,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 43 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 45,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 45 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 46,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 46 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 47,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 47 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 50,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 50 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 51,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 51 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 53,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 53 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 55,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 55 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 58,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 58 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 60,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 60 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 62,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 62 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 65,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 65 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 67,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 67 },
        drawingInfo: { showLabels: true },
      },
    },
    {
      id: 69,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: 69 },
        drawingInfo: { showLabels: true },
      },
    },
  ];

  if (adrlayerId !== -1) {
    dynamicLayer.unshift({
      id: adrlayerId,
      layerDefinition: {
        source: { type: "mapLayer", mapLayerId: adrlayerId },
        definitionExpression: "reol_id in (" + routeids.join(",") + ")",
        drawingInfo: { showLabels: true },
      },
    });
  }

  const payload = PrepareReportPayload(
    cmd,
    extent,
    featuresPoint,
    featuresText,
    dynamicLayer
  );

  // let bbox =
  //   "bbox=" +
  //   extent.xmin +
  //   "," +
  //   extent.ymin +
  //   "," +
  //   extent.xmax +
  //   "," +
  //   extent.ymax +
  //   "&bboxSR=32633&imageSR=32633&size=700,400&dpi=96&format=png32&transparent=true&dynamicLayers=";

  // let url =
  //   mapServiceEndpoint +
  //   "/export?" +
  //   bbox +
  //   encodeURIComponent(JSON.stringify(dynamicLayer)) +
  //   "&f=image";

  return payload;
}

const PrepareReportPayload = (
  cmd,
  extent,
  featuresPoint,
  featuresText,
  dynamicLayer
) => {
  //"baseMap":{"title": "VectorTileLayer as BaseMap","baseMapLayers":[{"id":"VectorTile_1933","type":"VectorTileLayer","layerType":"VectorTileLayer","title":"World_Basemap","styleUrl":"https://services.geodataonline.no/arcgis/rest/services/GeocacheVector/GeocacheBasis/VectorTileServer/resources/styles/root.json","visibility": true,"opacity": 1}]}
  //"baseMap":{"title": "BaseMap","baseMapLayers":[{"url":"https://services.geodataonline.no/arcgis/rest/services/Geocache_UTM33_WGS84/GeocacheGraatone/MapServer"}]}
  const objData = {
    mapOptions: { extent: extent },
    operationalLayers: [
      {
        url: mapServiceExportEndpoint,
        layers: dynamicLayer,
      },
      {
        id: "map_graphics",
        featureCollection: {
          layers: [
            {
              layerDefinition: {
                name: "pointLayer",
                geometryType: "esriGeometryPoint",
                drawingInfo: {
                  renderer: {
                    type: "simple",
                    symbol: {
                      type: "esriSMS",
                      style: "esriSMSCircle",
                      color: [0, 0, 255, 255],
                      size: 8,
                      outline: { color: [0, 0, 255, 255], width: 1 },
                    },
                  },
                },
              },
              featureSet: { features: featuresPoint },
            },
            {
              layerDefinition: {
                name: "textLayer",
                geometryType: "esriGeometryPoint",
              },
              featureSet: { features: featuresText },
            },
          ],
        },
      },
    ],
    exportOptions: { outputSize: [750, 400], dpi: 96 },
  };

  const params = new URLSearchParams();
  params.append("Web_Map_as_JSON", JSON.stringify(objData));
  params.append("Format", "PNG8");
  params.append("Layout_Template", "map_only");
  params.append("f", "pjson");

  // const serviceBody = {
  //   Web_Map_as_JSON: objData,
  //   Format: "pdf",
  //   Layout_Template: "map_only",
  //   f: "pjson",
  // };

  return params;
};
