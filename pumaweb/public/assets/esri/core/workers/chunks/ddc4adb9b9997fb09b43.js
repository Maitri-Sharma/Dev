"use strict";(self.webpackChunkRemoteClient=self.webpackChunkRemoteClient||[]).push([[5823],{84552:(e,t,r)=>{r.d(t,{Z:()=>y});var i,s=r(43697),a=r(10736),l=r(35463),n=r(5600),o=(r(67676),r(80442),r(75215),r(52011)),u=r(78981);let p=i=class extends a.wq{constructor(e){super(e),this.value=0,this.unit="milliseconds"}toMilliseconds(){return(0,l.rJ)(this.value,this.unit,"milliseconds")}clone(){return new i({value:this.value,unit:this.unit})}};(0,s._)([(0,n.Cb)({type:Number,json:{write:!0},nonNullable:!0})],p.prototype,"value",void 0),(0,s._)([(0,n.Cb)({type:u.v.apiValues,json:{type:u.v.jsonValues,read:u.v.read,write:u.v.write},nonNullable:!0})],p.prototype,"unit",void 0),p=i=(0,s._)([(0,o.j)("esri.TimeInterval")],p);const y=p},68668:(e,t,r)=>{r.d(t,{GZ:()=>n,wF:()=>o});var i=r(66643),s=r(81049),a=r(83379),l=r(70586);async function n(e,t){return await e.load(),o(e,t)}async function o(e,t){const r=[],n=(...e)=>{for(const t of e)(0,l.Wi)(t)||(Array.isArray(t)?n(...t):s.Z.isCollection(t)?t.forEach((e=>n(e))):a.Z.isLoadable(t)&&r.push(t))};t(n);let o=null;if(await(0,i.UI)(r,(async e=>{!1!==(await(0,i.q6)(function(e){return"loadAll"in e&&"function"==typeof e.loadAll}(e)?e.loadAll():e.load())).ok||o||(o=e)})),o)throw o.loadError;return e}},75823:(e,t,r)=>{r.r(t),r.d(t,{default:()=>R});var i=r(43697),s=r(3172),a=r(92835),l=r(20102),n=r(3920),o=r(68668),u=r(70586),p=r(16453),y=r(95330),d=r(5600),m=(r(67676),r(80442),r(75215)),c=r(71715),h=r(52011),v=r(30556),g=r(31263),f=r(6570),b=r(91040),w=r(29600),I=r(54295),x=r(7944),S=r(17287),_=r(71612),C=r(17017),T=r(38009),E=r(16859),O=r(34760),D=r(72965),U=r(10343),F=r(28294),j=r(21506),L=r(15923),N=r(32073),P=r(410);const Z={visible:"visibleSublayers",definitionExpression:"layerDefs",labelingInfo:"hasDynamicLayers",labelsVisible:"hasDynamicLayers",opacity:"hasDynamicLayers",minScale:"visibleSublayers",maxScale:"visibleSublayers",renderer:"hasDynamicLayers",source:"hasDynamicLayers"};let k=class extends((0,n.p)(L.Z)){constructor(e){super(e),this.floors=null,this.scale=0}destroy(){this.layer=null}get dynamicLayers(){if(!this.hasDynamicLayers)return null;const e=this.visibleSublayers.map((e=>{const t=(0,P.ff)(this.floors,e);return e.toExportImageJSON(t)}));return e.length?JSON.stringify(e):null}get hasDynamicLayers(){return this.layer&&(0,N.FN)(this.visibleSublayers,this.layer.serviceSublayers,this.layer)}set layer(e){this._get("layer")!==e&&(this._set("layer",e),this.handles.remove("layer"),e&&this.handles.add([e.allSublayers.on("change",(()=>this.notifyChange("visibleSublayers"))),e.on("sublayer-update",(e=>this.notifyChange(Z[e.propertyName])))],"layer"))}get layers(){const e=this.visibleSublayers;return e?e.length?"show:"+e.map((e=>e.id)).join(","):"show:-1":null}get layerDefs(){var e;const t=!(null==(e=this.floors)||!e.length),r=this.visibleSublayers.filter((e=>null!=e.definitionExpression||t&&null!=e.floorInfo));return r.length?JSON.stringify(r.reduce(((e,t)=>{const r=(0,P.ff)(this.floors,t),i=(0,u.pC)(r)?(0,P.Hp)(r,t):t.definitionExpression;return e[t.id]=i,e}),{})):null}get version(){this.commitProperty("layers"),this.commitProperty("layerDefs"),this.commitProperty("dynamicLayers"),this.commitProperty("timeExtent");const e=this.layer;return e&&(e.commitProperty("dpi"),e.commitProperty("imageFormat"),e.commitProperty("imageTransparency"),e.commitProperty("gdbVersion")),(this._get("version")||0)+1}get visibleSublayers(){const e=[];if(!this.layer)return e;const t=this.layer.sublayers,r=t=>{const i=this.scale,s=0===i,a=0===t.minScale||i<=t.minScale,l=0===t.maxScale||i>=t.maxScale;t.visible&&(s||a&&l)&&(t.sublayers?t.sublayers.forEach(r):e.unshift(t))};t&&t.forEach(r);const i=this._get("visibleSublayers");return!i||i.length!==e.length||i.some(((t,r)=>e[r]!==t))?e:i}toJSON(){const e=this.layer;let t={dpi:e.dpi,format:e.imageFormat,transparent:e.imageTransparency,gdbVersion:e.gdbVersion||null};return this.hasDynamicLayers&&this.dynamicLayers?t.dynamicLayers=this.dynamicLayers:t={...t,layers:this.layers,layerDefs:this.layerDefs},t}};(0,i._)([(0,d.Cb)({readOnly:!0})],k.prototype,"dynamicLayers",null),(0,i._)([(0,d.Cb)()],k.prototype,"floors",void 0),(0,i._)([(0,d.Cb)({readOnly:!0})],k.prototype,"hasDynamicLayers",null),(0,i._)([(0,d.Cb)()],k.prototype,"layer",null),(0,i._)([(0,d.Cb)({readOnly:!0})],k.prototype,"layers",null),(0,i._)([(0,d.Cb)({readOnly:!0})],k.prototype,"layerDefs",null),(0,i._)([(0,d.Cb)({type:Number})],k.prototype,"scale",void 0),(0,i._)([(0,d.Cb)(j.qG)],k.prototype,"timeExtent",void 0),(0,i._)([(0,d.Cb)({readOnly:!0})],k.prototype,"version",null),(0,i._)([(0,d.Cb)({readOnly:!0})],k.prototype,"visibleSublayers",null),k=(0,i._)([(0,h.j)("esri.layers.mixins.ExportImageParameters")],k);var J=r(49867);let M=class extends((0,_.h)((0,F.n)((0,D.M)((0,U.x)((0,x.O)((0,S.Y)((0,T.q)((0,E.I)((0,p.R)((0,O.Q)((0,I.V)((0,C.N)((0,n.p)(w.Z)))))))))))))){constructor(...e){super(...e),this.datesInUnknownTimezone=!1,this.dpi=96,this.gdbVersion=null,this.imageFormat="png24",this.imageMaxHeight=2048,this.imageMaxWidth=2048,this.imageTransparency=!0,this.isReference=null,this.labelsVisible=!1,this.operationalLayerType="ArcGISMapServiceLayer",this.sourceJSON=null,this.sublayers=null,this.type="map-image",this.url=null}normalizeCtorArgs(e,t){return"string"==typeof e?{url:e,...t}:e}load(e){const t=(0,u.pC)(e)?e.signal:null;return this.addResolvingPromise(this.loadFromPortal({supportedTypes:["Map Service"]},e).catch(y.r9).then((()=>this._fetchService(t)))),Promise.resolve(this)}readImageFormat(e,t){const r=t.supportedImageFormatTypes;return r&&r.indexOf("PNG32")>-1?"png32":"png24"}writeSublayers(e,t,r,i){if(!this.loaded||!e)return;const s=e.slice().reverse().flatten((({sublayers:e})=>e&&e.toArray().reverse())).toArray();let a=!1;if(this.capabilities&&this.capabilities.operations.supportsExportMap&&this.capabilities.exportMap.supportsDynamicLayers){const e=(0,g.M9)(i.origin);if(3===e){const e=this.createSublayersForOrigin("service").sublayers;a=(0,N.QV)(s,e,2)}else if(e>3){const e=this.createSublayersForOrigin("portal-item");a=(0,N.QV)(s,e.sublayers,(0,g.M9)(e.origin))}}const l=[],n={writeSublayerStructure:a,...i};let o=a;s.forEach((e=>{const t=e.write({},n);l.push(t),o=o||"user"===e.originOf("visible")})),l.some((e=>Object.keys(e).length>1))&&(t.layers=l),o&&(t.visibleLayers=s.filter((e=>e.visible)).map((e=>e.id)))}createExportImageParameters(e,t,r,i){const s=i&&i.pixelRatio||1;e&&this.version>=10&&(e=e.clone().shiftCentralMeridian());const a=new k({layer:this,floors:null==i?void 0:i.floors,scale:(0,b.yZ)({extent:e,width:t})*s}),l=a.toJSON();a.destroy();const n=!i||!i.rotation||this.version<10.3?{}:{rotation:-i.rotation},o=e&&e.spatialReference,u=o.wkid||JSON.stringify(o.toJSON());l.dpi*=s;const p={};if(null!=i&&i.timeExtent){const{start:e,end:t}=i.timeExtent.toJSON();p.time=e&&t&&e===t?""+e:`${null==e?"null":e},${null==t?"null":t}`}else this.timeInfo&&!this.timeInfo.hasLiveData&&(p.time="null,null");return{bbox:e&&e.xmin+","+e.ymin+","+e.xmax+","+e.ymax,bboxSR:u,imageSR:u,size:t+","+r,...l,...n,...p}}async fetchImage(e,t,r,i){var a;const n={responseType:"image",signal:null!=(a=null==i?void 0:i.signal)?a:null,query:{...this.parsedUrl.query,...this.createExportImageParameters(e,t,r,i),f:"image",...this.refreshParameters,...this.customParameters,token:this.apiKey}},o=this.parsedUrl.path+"/export";return null==n.query.dynamicLayers||this.capabilities.exportMap.supportsDynamicLayers?(0,s.default)(o,n).then((e=>e.data)).catch((e=>{if((0,y.D_)(e))throw e;throw new l.Z("mapimagelayer:image-fetch-error",`Unable to load image: ${o}`,{error:e})})):Promise.reject(new l.Z("mapimagelayer:dynamiclayer-not-supported",`service ${this.url} doesn't support dynamic layers, which is required to be able to change the sublayer's order, rendering, labeling or source.`,{query:n.query}))}async fetchRecomputedExtents(e={}){const t={...e,query:{returnUpdates:!0,f:"json",...this.customParameters,token:this.apiKey}},{data:r}=await(0,s.default)(this.url,t),{extent:i,fullExtent:l,timeExtent:n}=r,o=i||l;return{fullExtent:o&&f.Z.fromJSON(o),timeExtent:n&&a.Z.fromJSON({start:n[0],end:n[1]})}}loadAll(){return(0,o.GZ)(this,(e=>{e(this.allSublayers)}))}async _fetchService(e){if(this.sourceJSON)return void this.read(this.sourceJSON,{origin:"service",url:this.parsedUrl});const{data:t,ssl:r}=await(0,s.default)(this.parsedUrl.path,{query:{f:"json",...this.parsedUrl.query,...this.customParameters,token:this.apiKey},signal:e});r&&(this.url=this.url.replace(/^http:/i,"https:")),this.sourceJSON=t,this.read(t,{origin:"service",url:this.parsedUrl})}};(0,i._)([(0,d.Cb)({type:Boolean})],M.prototype,"datesInUnknownTimezone",void 0),(0,i._)([(0,d.Cb)()],M.prototype,"dpi",void 0),(0,i._)([(0,d.Cb)()],M.prototype,"gdbVersion",void 0),(0,i._)([(0,d.Cb)()],M.prototype,"imageFormat",void 0),(0,i._)([(0,c.r)("imageFormat",["supportedImageFormatTypes"])],M.prototype,"readImageFormat",null),(0,i._)([(0,d.Cb)({json:{origins:{service:{read:{source:"maxImageHeight"}}}}})],M.prototype,"imageMaxHeight",void 0),(0,i._)([(0,d.Cb)({json:{origins:{service:{read:{source:"maxImageWidth"}}}}})],M.prototype,"imageMaxWidth",void 0),(0,i._)([(0,d.Cb)()],M.prototype,"imageTransparency",void 0),(0,i._)([(0,d.Cb)({type:Boolean,json:{read:!1,write:{enabled:!0,overridePolicy:()=>({enabled:!1})}}})],M.prototype,"isReference",void 0),(0,i._)([(0,d.Cb)({json:{read:!1,write:!1}})],M.prototype,"labelsVisible",void 0),(0,i._)([(0,d.Cb)({type:["ArcGISMapServiceLayer"]})],M.prototype,"operationalLayerType",void 0),(0,i._)([(0,d.Cb)({json:{read:!1,write:!1}})],M.prototype,"popupEnabled",void 0),(0,i._)([(0,d.Cb)()],M.prototype,"sourceJSON",void 0),(0,i._)([(0,d.Cb)({json:{write:{ignoreOrigin:!0}}})],M.prototype,"sublayers",void 0),(0,i._)([(0,v.c)("sublayers",{layers:{type:[J.Z]},visibleLayers:{type:[m.z8]}})],M.prototype,"writeSublayers",null),(0,i._)([(0,d.Cb)({type:["show","hide","hide-children"]})],M.prototype,"listMode",void 0),(0,i._)([(0,d.Cb)({json:{read:!1},readOnly:!0,value:"map-image"})],M.prototype,"type",void 0),(0,i._)([(0,d.Cb)(j.HQ)],M.prototype,"url",void 0),M=(0,i._)([(0,h.j)("esri.layers.MapImageLayer")],M);const R=M},16859:(e,t,r)=>{r.d(t,{I:()=>w});var i=r(43697),s=r(40330),a=r(3172),l=r(66643),n=r(20102),o=r(92604),u=r(70586),p=r(95330),y=r(17452),d=r(5600),m=(r(67676),r(80442),r(75215),r(71715)),c=r(52011),h=r(30556),v=r(65587),g=r(15235),f=r(86082);const b=o.Z.getLogger("esri.layers.mixins.PortalLayer"),w=e=>{let t=class extends e{constructor(){super(...arguments),this.resourceReferences={portalItem:null,paths:[]},this.userHasEditingPrivileges=!0}destroy(){var e;null==(e=this.portalItem)||e.destroy(),this.portalItem=null}set portalItem(e){e!==this._get("portalItem")&&(this.removeOrigin("portal-item"),this._set("portalItem",e))}readPortalItem(e,t,r){if(t.itemId)return new g.default({id:t.itemId,portal:r&&r.portal})}writePortalItem(e,t){e&&e.id&&(t.itemId=e.id)}async loadFromPortal(e,t){if(this.portalItem&&this.portalItem.id)try{const i=await r.e(8062).then(r.bind(r,18062));return(0,p.k_)(t),await i.load({instance:this,supportedTypes:e.supportedTypes,validateItem:e.validateItem,supportsData:e.supportsData},t)}catch(e){throw(0,p.D_)(e)||b.warn(`Failed to load layer (${this.title}, ${this.id}) portal item (${this.portalItem.id})\n  ${e}`),e}}async finishLoadEditablePortalLayer(e){this._set("userHasEditingPrivileges",await this.fetchUserHasEditingPrivileges(e).catch((e=>((0,p.r9)(e),!0))))}async fetchUserHasEditingPrivileges(e){const t=this.url?null==s.id?void 0:s.id.findCredential(this.url):null;if(!t)return!0;const r=I.credential===t?I.user:await this.fetchEditingUser(e);return I.credential=t,I.user=r,(0,u.Wi)(r)||null==r.privileges||r.privileges.includes("features:user:edit")}async fetchEditingUser(e){var t,r;const i=null==(t=this.portalItem)||null==(r=t.portal)?void 0:r.user;if(i)return i;const n=s.id.findServerInfo(this.url);if(null==n||!n.owningSystemUrl)return null;const o=`${n.owningSystemUrl}/sharing/rest`,p=v.Z.getDefault();if(p&&p.loaded&&(0,y.Fv)(p.restUrl)===(0,y.Fv)(o))return p.user;const d=`${o}/community/self`,m=(0,u.pC)(e)?e.signal:null,c=await(0,l.q6)((0,a.default)(d,{authMode:"no-prompt",query:{f:"json"},signal:m}));return c.ok?f.default.fromJSON(c.value.data):null}read(e,t){t&&(t.layer=this),super.read(e,t)}write(e,t){const r=t&&t.portal,i=this.portalItem&&this.portalItem.id&&(this.portalItem.portal||v.Z.getDefault());return r&&i&&!(0,y.tm)(i.restUrl,r.restUrl)?(t.messages&&t.messages.push(new n.Z("layer:cross-portal",`The layer '${this.title} (${this.id})' cannot be persisted because it refers to an item on a different portal than the one being saved to. To save the scene, set the layer.portalItem to null or save the scene to the same portal as the item associated with the layer`,{layer:this})),null):super.write(e,{...t,layer:this})}};return(0,i._)([(0,d.Cb)({type:g.default})],t.prototype,"portalItem",null),(0,i._)([(0,m.r)("web-document","portalItem",["itemId"])],t.prototype,"readPortalItem",null),(0,i._)([(0,h.c)("web-document","portalItem",{itemId:{type:String}})],t.prototype,"writePortalItem",null),(0,i._)([(0,d.Cb)()],t.prototype,"resourceReferences",void 0),(0,i._)([(0,d.Cb)({readOnly:!0})],t.prototype,"userHasEditingPrivileges",void 0),t=(0,i._)([(0,c.j)("esri.layers.mixins.PortalLayer")],t),t},I={credential:null,user:null}},28294:(e,t,r)=>{r.d(t,{n:()=>d});var i=r(43697),s=r(92835),a=r(84552),l=r(5600),n=(r(67676),r(80442),r(75215),r(71715)),o=r(52011),u=r(30547),p=r(35671),y=r(78981);const d=e=>{let t=class extends e{constructor(){super(...arguments),this.timeExtent=null,this.timeOffset=null,this.useViewTime=!0}readOffset(e,t){const r=t.timeInfo.exportOptions;if(!r)return null;const i=r.timeOffset,s=y.v.fromJSON(r.timeOffsetUnits);return i&&s?new a.Z({value:i,unit:s}):null}set timeInfo(e){(0,p.UF)(e,this.fieldsIndex),this._set("timeInfo",e)}};return(0,i._)([(0,l.Cb)({type:s.Z,json:{write:!1}})],t.prototype,"timeExtent",void 0),(0,i._)([(0,l.Cb)({type:a.Z})],t.prototype,"timeOffset",void 0),(0,i._)([(0,n.r)("service","timeOffset",["timeInfo.exportOptions"])],t.prototype,"readOffset",null),(0,i._)([(0,l.Cb)({value:null,type:u.Z,json:{write:!0,origins:{"web-document":{read:!1,write:!1}}}})],t.prototype,"timeInfo",null),(0,i._)([(0,l.Cb)({type:Boolean,json:{read:{source:"timeAnimation"},write:{target:"timeAnimation"},origins:{"web-scene":{read:!1,write:!1}}}})],t.prototype,"useViewTime",void 0),t=(0,i._)([(0,o.j)("esri.layers.mixins.TemporalLayer")],t),t}},30547:(e,t,r)=>{r.d(t,{Z:()=>b});var i,s=r(43697),a=r(92835),l=r(84552),n=r(10736),o=r(22974),u=r(70586),p=r(5600),y=(r(75215),r(71715)),d=r(52011),m=r(30556);r(67676),r(80442);let c=i=class extends n.wq{constructor(e){super(e),this.respectsDaylightSaving=!1,this.timezone=null}readRespectsDaylightSaving(e,t){return void 0!==t.respectsDaylightSaving?t.respectsDaylightSaving:void 0!==t.respectDaylightSaving&&t.respectDaylightSaving}clone(){const{respectsDaylightSaving:e,timezone:t}=this;return new i({respectsDaylightSaving:e,timezone:t})}};(0,s._)([(0,p.Cb)({type:Boolean,json:{write:!0}})],c.prototype,"respectsDaylightSaving",void 0),(0,s._)([(0,y.r)("respectsDaylightSaving",["respectsDaylightSaving","respectDaylightSaving"])],c.prototype,"readRespectsDaylightSaving",null),(0,s._)([(0,p.Cb)({type:String,json:{read:{source:"timeZone"},write:{target:"timeZone"}}})],c.prototype,"timezone",void 0),c=i=(0,s._)([(0,d.j)("esri.layers.support.TimeReference")],c);const h=c;var v,g=r(78981);let f=v=class extends n.wq{constructor(e){super(e),this.cumulative=!1,this.endField=null,this.fullTimeExtent=null,this.hasLiveData=!1,this.interval=null,this.startField=null,this.timeReference=null,this.trackIdField=null,this.useTime=!0}readFullTimeExtent(e,t){if(!t.timeExtent||!Array.isArray(t.timeExtent)||2!==t.timeExtent.length)return null;const r=new Date(t.timeExtent[0]),i=new Date(t.timeExtent[1]);return new a.Z({start:r,end:i})}writeFullTimeExtent(e,t){e&&(0,u.pC)(e.start)&&(0,u.pC)(e.end)?t.timeExtent=[e.start.getTime(),e.end.getTime()]:t.timeExtent=null}readInterval(e,t){return t.timeInterval&&t.timeIntervalUnits?new l.Z({value:t.timeInterval,unit:g.v.fromJSON(t.timeIntervalUnits)}):t.defaultTimeInterval&&t.defaultTimeIntervalUnits?new l.Z({value:t.defaultTimeInterval,unit:g.v.fromJSON(t.defaultTimeIntervalUnits)}):null}writeInterval(e,t){if(e){const r=e.toJSON();t.timeInterval=r.value,t.timeIntervalUnits=r.unit}else t.timeInterval=null,t.timeIntervalUnits=null}clone(){const{cumulative:e,endField:t,hasLiveData:r,interval:i,startField:s,timeReference:a,fullTimeExtent:l,trackIdField:n,useTime:u}=this;return new v({cumulative:e,endField:t,hasLiveData:r,interval:i,startField:s,timeReference:(0,o.d9)(a),fullTimeExtent:(0,o.d9)(l),trackIdField:n,useTime:u})}};(0,s._)([(0,p.Cb)({type:Boolean,json:{read:{source:"exportOptions.timeDataCumulative"},write:{target:"exportOptions.timeDataCumulative"}}})],f.prototype,"cumulative",void 0),(0,s._)([(0,p.Cb)({type:String,json:{read:{source:"endTimeField"},write:{target:"endTimeField",allowNull:!0}}})],f.prototype,"endField",void 0),(0,s._)([(0,p.Cb)({type:a.Z,json:{write:{enabled:!0,allowNull:!0}}})],f.prototype,"fullTimeExtent",void 0),(0,s._)([(0,y.r)("fullTimeExtent",["timeExtent"])],f.prototype,"readFullTimeExtent",null),(0,s._)([(0,m.c)("fullTimeExtent")],f.prototype,"writeFullTimeExtent",null),(0,s._)([(0,p.Cb)({type:Boolean,json:{write:!0}})],f.prototype,"hasLiveData",void 0),(0,s._)([(0,p.Cb)({type:l.Z,json:{write:{enabled:!0,allowNull:!0}}})],f.prototype,"interval",void 0),(0,s._)([(0,y.r)("interval",["timeInterval","timeIntervalUnits","defaultTimeInterval","defaultTimeIntervalUnits"])],f.prototype,"readInterval",null),(0,s._)([(0,m.c)("interval")],f.prototype,"writeInterval",null),(0,s._)([(0,p.Cb)({type:String,json:{read:{source:"startTimeField"},write:{target:"startTimeField",allowNull:!0}}})],f.prototype,"startField",void 0),(0,s._)([(0,p.Cb)({type:h,json:{write:{enabled:!0,allowNull:!0}}})],f.prototype,"timeReference",void 0),(0,s._)([(0,p.Cb)({type:String,json:{write:{enabled:!0,allowNull:!0}}})],f.prototype,"trackIdField",void 0),(0,s._)([(0,p.Cb)({type:Boolean,json:{read:{source:"exportOptions.useTime"},write:{target:"exportOptions.useTime"}}})],f.prototype,"useTime",void 0),f=v=(0,s._)([(0,d.j)("esri.layers.support.TimeInfo")],f);const b=f},78981:(e,t,r)=>{r.d(t,{v:()=>i});const i=(0,r(35454).wY)()({esriTimeUnitsMilliseconds:"milliseconds",esriTimeUnitsSeconds:"seconds",esriTimeUnitsMinutes:"minutes",esriTimeUnitsHours:"hours",esriTimeUnitsDays:"days",esriTimeUnitsWeeks:"weeks",esriTimeUnitsMonths:"months",esriTimeUnitsYears:"years",esriTimeUnitsDecades:"decades",esriTimeUnitsCenturies:"centuries",esriTimeUnitsUnknown:void 0})}}]);