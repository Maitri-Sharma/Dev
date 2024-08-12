using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RouteUpdateConsoleApplication.Helper
{
    public class RouteUpdate
    {
        public static void RunRouteUpdate(int input)
        {            
                if (input == 1)
                {
                    //RouteImport
                    try
                    {
                        CommonTask.GenerateLog("RunRouteUpdate step 1 RouteImport started");
                        RouteImport.RunRouteImport();
                        CommonTask.GenerateLog("RunRouteUpdate step 1 RouteImport Finished");
                        //input++;
                    }
                    catch (Exception ex)
                    {
                        CommonTask.GenerateLog("RunRouteUpdate step 1 RouteImport Failed -" + "\n" + "Message: " + ex.Message + "\n" + "StackTrace: " + ex.StackTrace);
                        CommonTask.SendMail(Config.AppTeamMail, "RunRouteUpdate step 1 RouteImport Failed -" + "<br>" + "Message: " + ex.Message + "<br>" + "StackTrace: " + ex.StackTrace);
                        throw;
                    }
                }
                if (input == 2)
                {
                    //PIBImport
                    try
                    {
                        CommonTask.GenerateLog("RunRouteUpdate step 2 PIBImport started");
                        CommonTask.SendMail(Config.UpdateReceiverEmails, "We are taking Puma down. Route Generation and PIB import will start after that.");
                        //CommonTask.StartStopIISInstance("Start", "N");
                        //InputPIBMapping importPIB = new InputPIBMapping();
                        //importPIB.DataCompare(Config.PIBImportFilePath);
                        //if (Config.BackupBoksanlegg == "true")
                        //    importPIB.BackupBookanlegg();
                        CommonTask.GenerateLog("RunRouteUpdate step 2 PIBImport Finished");
                        input++;
                    }
                    catch (Exception ex)
                    {
                        CommonTask.GenerateLog("RunRouteUpdate step 2 PIBImport Failed" + "\n" + "Message: " + ex.Message + "\n" + "StackTrace: " + ex.StackTrace);
                        CommonTask.SendMail(Config.AppTeamMail, "RunRouteUpdate step 2 PIBImport Failed" + "<br>" + "Message: " + ex.Message + "<br>" + "StackTrace: " + ex.StackTrace);
                        throw;
                    }
                }
                if (input == 3)
                {
                    //Route Generation
                    try
                    {
                        CommonTask.GenerateLog("RunRouteUpdate step 3 RouteGeneration started");
                        RouteGeneration.RunRouteGeneration();
                        CommonTask.GenerateLog("RunRouteUpdate step 3 RouteGeneration Finished");
                        //input++;
                    }
                    catch (Exception ex)
                    {
                        CommonTask.GenerateLog("RunRouteUpdate step 3 RouteGeneration Failed -" + "\n" + "Message: " + ex.Message + "\n" + "StackTrace: " + ex.StackTrace);
                        CommonTask.SendMail(Config.AppTeamMail, "RunRouteUpdate step 3 RouteGeneration Failed -" + "<br>" + "Message: " + ex.Message + "<br>" + "StackTrace: " + ex.StackTrace);
                        throw;
                    }
                }

                //Route Recreation
                if (input == 4)
                {
                    try
                    {
                        CommonTask.GenerateLog("RunRouteUpdate step 4 RouteRecreation started");
                        RouteRecreation.RunRouteRecreation();
                        CommonTask.GenerateLog("RunRouteUpdate step 4 RouteRecreation Finished");
                        //input++;
                    }
                    catch (Exception ex)
                    {
                        CommonTask.GenerateLog("RunRouteUpdate step 4 RouteRecreation Failed -" + "\n" + "Message: " + ex.Message + "\n" + "StackTrace: " + ex.StackTrace);
                        CommonTask.SendMail(Config.AppTeamMail, "RunRouteUpdate step 4 RouteRecreation Failed -" + "<br>" + "Message: " + ex.Message + "<br>" + "StackTrace: " + ex.StackTrace);
                        throw;
                    }
                }

            //PreAutoUpdate
                if (input == 5)
                {
                    try
                    {
                        CommonTask.GenerateLog("RunRouteUpdate step 5 PreAutoUpdate started");
                        PreAutoUpdate.RunPreAutoUpdate();
                        CommonTask.GenerateLog("RunRouteUpdate step 5 PreAutoUpdate Finished");
                        input++;
                    }
                    catch (Exception ex)
                    {
                        CommonTask.GenerateLog("RunRouteUpdate step 5 PreAutoUpdate Failed -" + "\n" + "Message: " + ex.Message + "\n" + "StackTrace: " + ex.StackTrace);
                        CommonTask.SendMail(Config.AppTeamMail, "RunRouteUpdate step 5 PreAutoUpdate Failed -" + "<br>" + "Message: " + ex.Message + "<br>" + "StackTrace: " + ex.StackTrace);
                        throw;
                    }
                }
                
            }

            
        }
    }

