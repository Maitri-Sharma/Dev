import React, { useState, useEffect } from 'react';

export default function  useCurrentStep() {
    
    const getCurrentStep = () => {
        return sessionStorage.getItem('_CurrentStep');
    };

    const [currentStep , setCurrentStep] = useState(getCurrentStep());
    
    const saveCurrentStep = currentStep => {        
        sessionStorage.setItem('_CurrentStep', currentStep);
        setCurrentStep(currentStep);
    };
    return {
        setCurrentStep: saveCurrentStep,
        currentStep
    }   
}