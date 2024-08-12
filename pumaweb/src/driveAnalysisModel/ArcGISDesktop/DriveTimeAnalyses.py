# Konfigurerbare parametere:
Vegnett = r"C:\Workspace\AnalysisServices\Vegnett\ELVEG_Nettverk.gdb\ELVEG_Nettverk\ELVEG_Nettverk_ND"
ReolMap = r"C:\Workspace\DriveTimeArcMap\KSPU.sde\puma.kspu_gdb.norway_reol"
# Nettverksavhengige variabler:
TimeName = "Minutes"
LengthName = "Meters"

import arcpy
arcpy.CheckOutExtension("Network")
#arcpy.AddMessage("Running in Utvikling")
arcpy.AddMessage("Running in Test")
#arcpy.AddMessage("Running in Production")
# Script arguments
BreakValue = arcpy.GetParameterAsText(0)
StartingPoints = arcpy.GetParameter(1)
AnalysisType = arcpy.GetParameterAsText(3)
arcpy.AddMessage("Analysis type: " + AnalysisType)
arcpy.AddMessage("Input parameter BreakValue: " + BreakValue)

arcpy.AddMessage("Creating rute feature layer.")
fields = "REOL_ID REOL_ID VISIBLE NONE"
for field in arcpy.ListFields(ReolMap):
    if field.name != "REOL_ID":
        fields = fields + ";" + field.name + " " + field.name + " HIDDEN NONE;"
ReolLayer = arcpy.management.MakeFeatureLayer(ReolMap, "ReolLayer", "", "", fields)
arcpy.AddMessage("Created feature layer ReolLayer.")

if AnalysisType == "DriveTime" or AnalysisType == "DriveDistance":
    arcpy.AddMessage("Creating service area layer.")
    if AnalysisType == "DriveTime":
        ServiceAreaLayer = arcpy.na.MakeServiceAreaLayer(Vegnett, "ServiceAreaLayer", TimeName, "TRAVEL_TO", BreakValue, "SIMPLE_POLYS", "NO_MERGE", "RINGS", "NO_LINES", "OVERLAP", "NO_SPLIT", "", "", "ALLOW_UTURNS", "OneWay", "NO_TRIM_POLYS", "100 Meters", "NO_LINES_SOURCE_FIELDS")
    else:
        ServiceAreaLayer = arcpy.na.MakeServiceAreaLayer(Vegnett, "ServiceAreaLayer", LengthName, "TRAVEL_TO", BreakValue, "SIMPLE_POLYS", "NO_MERGE", "RINGS", "NO_LINES", "OVERLAP", "NO_SPLIT", "", "", "ALLOW_UTURNS", "OneWay", "NO_TRIM_POLYS", "100 Meters", "NO_LINES_SOURCE_FIELDS")
    arcpy.AddMessage("Created service area layer. Adding locations.")
    arcpy.na.AddLocations(ServiceAreaLayer, "Facilities", StartingPoints, "Name Name #;CurbApproach # 0;Attr_Time # 0;Attr_Length # 0", "200 Meters", "", "ELVEG_Nettverk SHAPE", "MATCH_TO_CLOSEST", "APPEND", "NO_SNAP", "5 Meters", "INCLUDE", "ELVEG_Nettverk #")
    arcpy.AddMessage("Added locations. Solving.")

    arcpy.AddMessage("PROCESSING")
    arcpy.na.Solve(ServiceAreaLayer, "SKIP", "TERMINATE")
    x = 0
    messageCount = arcpy.GetMessageCount() 
    while x < messageCount:
        msg = arcpy.GetMessage(x)
        if msg.startswith("Location \"") and msg.endswith("\" in \"Facilities\" is unlocated."):
            arcpy.AddWarning("UNLOCATED LOCATION:" + msg.split("\"",2)[1])
        x += 1
    arcpy.AddMessage("Solve performed. Selecting result.")
    arcpy.AddMessage("FINISHING")

    ServiceArea = arcpy.SelectData_management(ServiceAreaLayer, "Polygons")
    arcpy.AddMessage("Selected service area polygons. Selecting ruter.")
    arcpy.management.SelectLayerByLocation(ReolLayer, "INTERSECT", ServiceArea, "", "NEW_SELECTION")
    arcpy.AddMessage("Selected ruter intersecting service area polygons. Creating result table.")
    Result = arcpy.CreateUniqueName("ResultTable", "in_memory")
    arcpy.management.CopyRows(ReolLayer, Result, "")
    arcpy.AddMessage("Result table created. Returning result.")
    arcpy.SetParameter(2, Result)
elif AnalysisType == "ITRTime" or AnalysisType == "ITRDistance":
    arcpy.AddMessage("Setting product level to ArcInfo.")
    import arcgisscripting
    gp = arcgisscripting.create(10.0)
    gp.setProduct("ArcInfo")
    #arcpy.env.workspace = "C:\\arcgisserver\\arcgisjobs\\kspu_ts2\\test.gdb"
    arcpy.AddMessage("Product level set. Creating service area layer.")
    if AnalysisType == "ITRTime":
        ServiceAreaLayer = arcpy.na.MakeServiceAreaLayer(Vegnett, "ServiceAreaLayer", TimeName, "TRAVEL_TO", BreakValue, "NO_POLYS", "NO_MERGE", "RINGS", "TRUE_LINES_WITH_MEASURES", "OVERLAP", "NO_SPLIT", "", LengthName + ";" + TimeName, "ALLOW_UTURNS", "OneWay", "TRIM_POLYS", "100 Meters", "NO_LINES_SOURCE_FIELDS", "NO_HIERARCHY")
    else:
        ServiceAreaLayer = arcpy.na.MakeServiceAreaLayer(Vegnett, "ServiceAreaLayer", LengthName, "TRAVEL_TO", BreakValue, "NO_POLYS", "NO_MERGE", "RINGS", "TRUE_LINES_WITH_MEASURES", "OVERLAP", "NO_SPLIT", "", LengthName + ";" + TimeName, "ALLOW_UTURNS", "OneWay", "TRIM_POLYS", "100 Meters", "NO_LINES_SOURCE_FIELDS", "NO_HIERARCHY")
    arcpy.AddMessage("Created service area layer. Adding locations.")
    arcpy.na.AddLocations(ServiceAreaLayer, "Facilities", StartingPoints, "CurbApproach # 0;Attr_Length # 0;Attr_Time # 0;Name Name #", "200 Meters", "", "ELVEG_Nettverk SHAPE", "MATCH_TO_CLOSEST", "APPEND", "NO_SNAP", "5 Meters", "INCLUDE", "ELVEG_Nettverk #")
    arcpy.AddMessage("Added locations. Solving.")

    arcpy.AddMessage("PROCESSING")
    arcpy.na.Solve(ServiceAreaLayer, "SKIP", "TERMINATE")
    x = 0
    messageCount = arcpy.GetMessageCount() 
    while x < messageCount:
        msg = arcpy.GetMessage(x)
        if msg.startswith("Location \"") and msg.endswith("\" in \"Facilities\" is unlocated."):
            arcpy.AddWarning("UNLOCATED LOCATION:" + msg.split("\"",2)[1])
        x += 1
    arcpy.AddMessage("Solve performed. Selecting line result.")
    arcpy.AddMessage("FINISHING")

    ServiceAreaLines = arcpy.SelectData_management(ServiceAreaLayer, "Lines")
    #fields = ""
    #for field in arcpy.ListFields(ServiceAreaLines):
    #    fields = fields + ";" + field.name
    #arcpy.AddMessage("Lines fields: " + fields)
    arcpy.AddMessage("Lines selected. Performing identity.")
    #ReolerWithTimeAndDistance = arcpy.CreateUniqueName("ReolerWithTimeAndDistance", arcpy.env.workspace)
    ReolerWithTimeAndDistance = arcpy.CreateUniqueName("ReolerWithTimeAndDistance", "in_memory")
    ReolerWithTimeAndDistance = arcpy.analysis.Identity(ServiceAreaLines, ReolLayer, ReolerWithTimeAndDistance, "ALL", "", "NO_RELATIONSHIPS")
    #fields = ""
    #for field in arcpy.ListFields(ReolerWithTimeAndDistance):
    #    fields = fields + ";" + field.name
    #arcpy.AddMessage("ReolerWithTimeAndDistance fields: " + fields)
    arcpy.AddMessage("Identity performed. Calculating statistics (Result).")
    Result = arcpy.CreateUniqueName("ResultTable", "in_memory")
    #Result = arcpy.CreateUniqueName("ResultTable", arcpy.env.workspace)
    Result = arcpy.analysis.Statistics(ReolerWithTimeAndDistance, Result, "FromCumul_" + TimeName + " MIN;ToCumul_" + TimeName + " MIN;ToCumul_" + LengthName + " MIN;FromCumul_" + LengthName + " MIN", "REOL_ID")
    arcpy.SetParameter(2, Result)
else:
    arcpy.AddError("Unsupported analysis type: " + AnalysisType)