MODULE egm
            
    VAR egmident egmID1;
    VAR egmstate egmSt1;
    
    CONST egm_minmax egm_minmax_joint1:=[-0.5,0.5];
                
    PROC main()
        VAR jointtarget joints;
        
        ! "Move" a tiny amount to start EGM
        joints:= CJointT();
        !joints.robax.rax_6 := joints.robax.rax_6 + .0001;
        MoveAbsj joints, v100, fine, tool0;
    
        StartEGM;
    
        EGMActJoint egmID1 \Tool:=tool0 \WObj:=wobj0, \J1:=egm_minmax_joint1 \J2:=egm_minmax_joint1 \J3:=egm_minmax_joint1
        \J4:=egm_minmax_joint1 \J5:=egm_minmax_joint1 \J6:=egm_minmax_joint1 \LpFilter:=100 \Samplerate:=4 \MaxPosDeviation:=1000 \MaxSpeedDeviation:=1000;
                      
        EGMRunJoint egmID1, EGM_STOP_HOLD \J1 \J2 \J3 \J4 \J5 \J6 \CondTime:=2000000 \RampInTime:=0.01 \PosCorrGain:=1;
            
        WaitUntil FALSE;
            
        ExitCycle;           
        
    ERROR
        IF ERRNO = ERR_UDPUC_COMM THEN
            TPWrite "EGM UDP Command Timeout, Restarting!";
            WaitTime 1;
            ExitCycle;
        ELSE            
            RAISE;
        ENDIF
    ENDPROC
    
    
    
    PROC StartEGM()
        
        !This call to EGMReset seems to be problematic.
        !It is shown in all documentation. Is it really necessary?
        !EGMReset egmID1;
        
        EGMGetId egmID1;
        egmSt1 := EGMGetState(egmID1);        
        
        IF egmSt1 <= EGM_STATE_CONNECTED THEN            
            EGMSetupUC ROB_1, egmID1, "conf1", "UCdevice:" \Joint \CommTimeout:=10000;
        ENDIF
        
        EGMStreamStart egmID1;
    ENDPROC   
        
ENDMODULE