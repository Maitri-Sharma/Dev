import React from 'react'
import useCurrentStep from '../Common/useCurrentStep.js'
import GeographieAnalysis from '../Components/Geographie_footer.js'

const iOmraadeIndex = 0
const iFylkeIndex   = 1
const iKommuneIndex   = 2
const iTeamIndex   = 3
const iReolIndex  = 4
const iPostNrIndex   = 5
const iPostBoksIndex   = 6
const iPlukklisteIndex   = 7
const iResultatIndex = 8

export function StepChanged (NewStep,oldStep , {setCurrentStep}, uxMultiViewFylkeIndex){



    var uxDropDownListGeografi =  document.getElementById('uxDropDownListGeografi');
    var RadioPlukkliste = document.getElementById('RadioPlukkliste');
    var GeografiParameter1 = document.getElementById('GeografiParameter1');
    var uxMultiViewFylke = document.getElementById('uxMultiViewFylke');


    uxMultiViewFylkeIndex = NewStep
    if(NewStep === iResultatIndex)
    {
        if(uxDropDownListGeografi.SelectedIndex === iReolIndex || uxDropDownListGeografi.SelectedIndex === iPostBoksIndex )
        {
            setCurrentStep(2);
        }
        else
        {
            setCurrentStep(3);
        }
        uxDropDownListGeografi.Visible = false
    }
    else if(NewStep === iPlukklisteIndex || iPlukklisteIndex ===9 ){
        setCurrentStep(2);
        uxDropDownListGeografi.Visible = false
    }
    else{
        setCurrentStep(1);
        uxDropDownListGeografi.Visible = true
    }

    // switch(NewStep)
    // {
    //     case 1: uxLblATitle.Text = KSPU.Framework.KSPUMessages.infoMsgGAHeadingStep1;  /// need to work on these
    //     case iPlukklisteIndex: KSPU.Framework.KSPUMessages.infoMsgGAHeadingStep2;
    //     case iResultatIndex: uxLblATitle.Text = KSPU.Framework.KSPUMessages.infoMsgGAHeadingStep3;
    //     default: uxLblATitle.Text = KSPU.Framework.KSPUMessages.infoMsgGAHeadingStep1;

    // }

    if(NewStep ===2){
       // Me.KSPUPage.DisableAddRemoveReols()  /// need to work on this
    }

}

function gotoStep2Main() {
        //KSPUPage.RemoveActiveUtvalg()  /// need to work on this
        // SessionInfo.AktivtUtvalg = Utvalg.CreateNewUtvalg(KSPUMessages.NewUtvalgName, GeografiParameter1.GetReceivers()) // need to work on this
        // var uxDropDownListGeografi =  document.getElementById('uxDropDownListGeografi');
        // if (checkGeografiSelection(uxDropDownListGeografi.SelectedIndex)) {
        //     //SessionInfo.OriginalAktivtUtvalg = Utvalg.CreateUtvalgCopy(SessionInfo.AktivtUtvalg)  // need to work on this
        //     return true;
        // }
        // else {
            return false; 
        // }                  
}


 function gotoStep3(bFromPlukkListe,FromPostNrPlukkliste){
        
        // GeografiParameter1.Visible = False;
        // if (SessionInfo.AktivtUtvalg.Criterias.Count > 0){ 
        //     while (SessionInfo.AktivtUtvalg.Criterias.Count > 0)
        //     {
        //         SessionInfo.AktivtUtvalg.Criterias.RemoveAt(SessionInfo.AktivtUtvalg.Criterias.Count - 1) // need to work
        //     }
        // }
        // var aktivtUtvalg = SessionInfo.AktivtUtvalg; // need to work
        // var utvalgs= UtvalgCollection();
        
        // if (aktivtUtvalg.Criterias.Count > 0){
        //     if (aktivtUtvalg.Criterias.Item(aktivtUtvalg.Criterias.Count - 1).CriteriaType = CriteriaType.SelectedInMap) {
        //         aktivtUtvalg.Criterias.RemoveAt(aktivtUtvalg.Criterias.Count - 1)
        //     }
        // } 

        // if( bFromPlukkListe) {
        //     aktivtUtvalg.Reoler = GeografiPlukkListe1.GetCheckedReoler
        //     utvalgs.Add(aktivtUtvalg)
        //     aktivtUtvalg.Criterias.AddCriteria(CriteriaType.GeografiPlukkliste, "")
        //     SessionInfo.AktivtUtvalg = aktivtUtvalg
        // }
        // else if (FromPostNrPlukkliste) {
        //     aktivtUtvalg.Reoler = GeografiPostNrPlukkliste.GetCheckedReoler
        // utvalgs.Add(aktivtUtvalg)
        // aktivtUtvalg.Criterias.AddCriteria(CriteriaType.GeografiPlukkliste, "")
        // SessionInfo.AktivtUtvalg = aktivtUtvalg
        // } 
        // else{
        //     aktivtUtvalg = CreateNewCollection(uxDropDownListGeografi.SelectedIndex, aktivtUtvalg)
        //     utvalgs.Add(aktivtUtvalg)
        //     SessionInfo.AktivtUtvalg = aktivtUtvalg
        // }    

   
   // KSPUPage.ShowUtvalgInMap(utvalgs)  need to work on this
 }

        
        

        

      
      

export function StepChanging(args , tabvalue){

    var uxDropDownListGeografi =  document.getElementById('uxDropDownListGeografi');
    var RadioPlukkliste = document.getElementById('RadioPlukkliste');
    var GeografiParameter1 = document.getElementById('GeografiParameter1');
    var uxMultiViewFylkeIndex = tabvalue;
    
    GeographieAnalysis.test()
    try
    {
        if(args.NewStep > args.currentStep){
            // KSPUPage.ResetAllAnalysesExcept(AnalysisTab.Geography)
            //If Not SessionInfo.CurrentSite = KSPU.WebLogic.Site.KundeWebNotLoggedIn Then KSPUPage.ArbeidsList.DeselectArbeidsListEntry()
            //KSPUPage.ClearMapGraphics()
            if( uxMultiViewFylkeIndex < iPlukklisteIndex){
                const ok = gotoStep2Main();
                if(ok)
                {
                    if(RadioPlukkliste.checked){
                        GeografiParameter1.style.visibility = 'hidden'
                        if (uxDropDownListGeografi.SelectedIndex = iPostNrIndex ){
                            args.NewStep = 9
                        }
                        else{
                            args.NewStep = iPlukklisteIndex
                        }
                        // var utvcoll = new UtvalgCollection(); ///need to work on this
                        // utvcoll.Add(SessionInfo.AktivtUtvalg)
                        // KSPUPage.ShowUtvalgInMap(utvcoll)  /// need to work on this
                    }
                    else{
                        args.NewStep = iResultatIndex
                        //gotoStep3(false, false)   /// need to work on this
                    }

                }
                else
                {
                    args.Cancel = true
                }

            }
            else if(uxMultiViewFylkeIndex === iPlukklisteIndex)
            {
                args.NewStep = iResultatIndex
                //gotoStep3(true, false)    /// need to work on this
            }
            else if(uxMultiViewFylkeIndex = 9 ){
                args.NewStep = iResultatIndex
                //gotoStep3(false, true) /// need to work on this
            }
        }
        else if(args.NewStep < args.currentStep){
                const i = uxMultiViewFylkeIndex
                var aktivtUtvalg =  sessionStorage.getItem('AktivtUtvalg');
                if(i=== iResultatIndex){
                    if(RadioPlukkliste.checked){
                        sessionStorage.setItem('AktivtUtvalg',1)//Utvalg.CreateUtvalgCopy(sessionStorage.getItem('OriginalAktivtUtvalg'), false))
                        if(uxDropDownListGeografi.SelectedIndex = iPostNrIndex)
                        {
                            args.NewStep = 9
                            GeografiParameter1.style.visibility = 'hidden'
                        }
                        else    {
                            args.NewStep = iPlukklisteIndex
                            GeografiParameter1.style.visibility = 'hidden'
                        }
                    }
                    else {
                        args.NewStep = uxDropDownListGeografi.SelectedIndex
                        GeografiParameter1.style.visibility = 'hidden'
                    }
                }
                else if (i > 0)
                {
                    if(aktivtUtvalg.UtvalgId === 0 ){
                        sessionStorage.setItem('AktivtUtvalg',null)
                    }
                    args.NewStep = uxDropDownListGeografi.SelectedIndex
                    GeografiParameter1.style.visibility = 'hidden'
                    //uxLblMessage.Visible = false
                   // KSPUPage.ClearMapGraphics()  //// Need to work on this
                }
        }
    }
    catch(error){
        console.error(error);
    }
    return args;
}