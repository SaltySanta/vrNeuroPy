"""
Script for the Annar4Interface class, used to communicate with the according SimpleNetwork API for Unity.

"""

from annar4Interface import *
from annarProtoRecv import *
from annarProtoSend import *
from MsgObject_pb2 import *


import signal
import socket
import struct
import os
import threading
import time
import sys

MONITOR_INTERVAL = 1000


def waitForFullExec(annarInterface, id, timeout = -1):
    """
    
    Function to wait until an action is completely executed in the VR
    (only for actions which have an action execution state).

    Arguments:
        annarInterface: Annar4Interface object instance.
        id: Id of an action.
        timeout: Timeout parameter (default = -1).

    Returns:
        actionState: Action Execution Status Meaning:
            0 = InExecution
            1 = Finished
            2 = Aborted
            3 = Walking
            4 = Rotating
            5 = WalkingRotating

    """

    ret = False
    actionState = 0
        
    if timeout == -1:  
    
        while actionState != 1 and actionState != 2:
            ret = annarInterface.checkActionExecState(id)
            if ret:
                actionState = annarInterface.getActionExecState()
        return actionState
            
    else:
        
        start_time = time.time()
        dur = 0      
        while (actionState == 0) and dur < timeout:
            dur = time.time() - start_time
            ret = annarInterface.checkActionExecState(id)
            if ret:
                actionState = annarInterface.getActionExecState()
                
        if(dur >= timeout):
            print "Timeout"

        return actionState


def waitForExec(annarInterface, id, timeout = -1):
    """
    
    Function to wait until an action is completely executed in the VR
    (only for actions which have an action execution state).

    NOTE: Not sure if this is old (since it only checks for the 'InExecution' 
    action execution state, or if it only waits until the action was started 
    in the VR, not completed).

    Arguments:
        annarInterface: Annar4Interface object instance.
        id: Id of an action.
        timeout: Timeout parameter (default = -1).

    Returns:
        actionState: Action execution state.
            0 = InExecution
            1 = Finished
            2 = Aborted
            3 = Walking
            4 = Rotating
            5 = WalkingRotating

    """

    ret = False
    actionState = 0
        
    if timeout == -1:  
    Annar4Interface class, used to communicate with the according SimpleNetwork API for Unity.
        while actionState == 0:
            ret = annarInterface.checkActionExecState(id)
            if ret:
                actionState = annarInterface.getActionExecState()
        return actionState
            
    else:
        
        start_time = time.time()
        dur = 0      
        while (actionState == 0) and dur < timeout:
            dur = time.time() - start_time
            ret = annarInterface.checkActionExecState(id)
            if ret:
                actionState = annarInterface.getActionExecState()
                
        if(dur >= timeout):
            print "Timeout"

        return actionState

# 
def timeout_loop(self):
    """ Loop function executed by a thread, which terminates object if a given timeout is exceeded. """

    while not done:

        if (softwareInterfaceTimeout != -1) and (interfaceNotUsed > softwareInterfaceTimeout):

            stop(False)

        

        time.sleep(MONITOR_INTERVAL/1000000.0)

        self.interfaceNotUsed = self.interfaceNotUsed + 1


class Annar4Interface(object):
    """

    Annar4Interface class, used to communicate with the according SimpleNetwork API for Unity.
    
    Arguments:
        srv_addr: IP-address of the Unity-VR host.
        remotePortNo: Port of the Unity-VR host (usually 1337, if not: look at APPConfig.config).
        agentNo: Id of the agent to be controlled.
        agentOnly: If True: Only sockets for agent controlling will be created.
        softwareInterfaceTimeout: Set a timeout threshold for the network communication (default = -1).

    """


    def __init__(self, srv_addr, remotePortNo, agentNo, agentOnly, softwareInterfaceTimeout=-1):

        ######################################
        ##### VERSION OF THE WHOLE INTERFACE
        ######################################

        self.version = "1.0"

        ######################################

        # since not all actions have an action execution state (for example sendEnvironmentReset), 
        # we wait a bit before sending the next message, so the VR has time to process the last Msg
        self.msgWaitingTime = 0.5

        self.leftImage = None
        self.rightImage = None

        self.gridSensorDataX = None
        self.gridSensorDataY = None
        self.gridSensorDataZ = None
        self.gridSensorDataRotationX = None
        self.gridSensorDataRotationY = None
        self.gridSensorDataRotationZ = None

        self.headMotionVelocityX = None
        self.headMotionVelocityY = None
        self.headMotionVelocityZ = None
        self.headMotionAccelerationX = None
        self.headMotionAccelerationX = None
        self.headMotionAccelerationX = None
        self.headMotionRotationVelocityX = None
        self.headMotionRotationVelocityY = None
        self.headMotionRotationVelocityZ = None
        self.headMotionRotationAccelerationX = None
        self.headMotionRotationAccelerationX = None
        self.headMotionRotationAccelerationX = None


        self.eyeRotationPositionX = None
        self.eyeRotationPositionY = None
        self.eyeRotationPositionZ = None
        self.eyeRotationVelocityX = None
        self.eyeRotationVelocityY = None
        self.eyeRotationVelocityZ = None

        self.externalReward = None

        self.state = None
        self.actionColID = None
        self.colliderID = None
        self.eventID = None
        self.parameter = None

        #################
        # CREATE SOCKETS
        #################
        if (not agentOnly):
       
        # create socket for VR

            try:

                self.socketVR = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
                #timeval = struct.pack('ll', 1, 0)
                #self.socketVR.setsockopt(socket.SOL_SOCKET, socket.SO_RCVTIMEO, timeval)
                self.socketVR.settimeout(1.0)
                self.socketVR.connect((srv_addr, remotePortNo))
            except socket.error as e:
                print "ERROR CONNECTING: " + str(e)
                sys.exit(1)      
        else:
            self.socketVR = -1

        # create socket for Agent

        try:
            #print "creating socket"
            self.socketAgent = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            #timeval = struct.pack('ll', 1, 0)
            #self.socketAgent.setsockopt(socket.SOL_SOCKET, socket.SO_RCVTIMEO, timeval)
            self.socketAgent.settimeout(1.0)
            self.socketAgent.connect((srv_addrabort_signal, remotePortNo + agentNo + 1))
        except socket.error as e:
            print "ERROR CONNECTING: " + str(e)
            sys.exit(1)

        # create sender and receiver

        self.sender = AnnarProtoSend(self.socketVR, self.socketAgent)
        self.receiver = AnnarProtoReceive(self.socketVR, self.socketAgent)


        if (softwareInterfaceTimeout == -1):
            self.softwareInterfaceTimeout = -1
        else:
            self.softwareInterfaceTimeout = softwareInterfaceTimeout*1000*1000/MONITOR_INTERVAL

        self.done = True
        self.interfaceNotUsed = 0


    def abort_signal(self, signal, frame):
        """

        If Ctrl-C is called, threads are closed properly to avoid having to 
        close the terminal every time something goes wrong.

        """

        print "\nMANUAL TERMINATION: Stopping all threads..."
        self.stop(True)
        sys.exit(0)


    def start(self):
        """

        Start Sender & Receiver and compare version 
        strings with the server (if versions are different, program exits).

        """

        
        # start the Ctrl-C signal handler
        signal.signal(signal.SIGINT, self.abort_signal)

        if (self.softwareInterfaceTimeout != -1) and (self.done):
            
            self.done = False
            self.thread = threading.Thread(target=timeout_loop)


        self.sender.start()
        self.receiver.start()

        self.sender.sendVersionCheck(self.version)
        VRVersion = ""
        while(VRVersion == ""):
            VRVersion = self.receiver.getVersion()
        print "/////////////////////////////////////"
        print "Client Version: " + self.version
        print "VR Version: " + VRVersion
        print "/////////////////////////////////////"
        print ""
        if (self.version != VRVersion):
            print "ERROR: Versions are different, please update your client!"
            self.stop(True)
            sys.exit(1)


    def stop(self, wait=True):
        """
        
        Terminates the Sender & Receiver threads.
        
        Arguments:
            wait: If True, wait for threads to terminate before exiting (default = True).
        
        """

        print ""
        print "/////////////////////////////////////"
        print "EXITING..."

        if self.sender is not None:

            self.sender.stop(wait)

        if self.receiver is not None:

            self.receiver.stop(wait)

        if (self.softwareInterfaceTimeout != -1) and (not self.done):
            
            self.done = True
            
            if wait:
                self.thread.join()

        print "DONE."
        print "/////////////////////////////////////"


    ############################################################################################
    ### RECEIVING FUNCTIONS
    ###
    ### The receiving functions have 2 steps:
    ### 1) check the data, which loads new data from the receiving buffer, returning a True is successfully retrieved
    ### 2) get the data, which actually returns the wanted data
    ############################################################################################


    def checkImages(self):
        """

        Retrieves images and returns bool for successs (needs to be executed if you want to load new images).

        Returns:
            res: True, if data retrieval was successfull.

        """

        self.leftImage, self.rightImage, res = self.receiver.getImageData()

        return res


    def getImageLeft(self):
        """

        Returns left image previously retrieved by checkImages().

        Returns:
            leftImage: Image of the left eye camera of the agent.

        """

        return self.leftImage


    def getImageRight(self):
        """

        Returns right image previously retrieved by checkImages().

        Returns:
            rightImage: Image of the right eye camera of the agent.

        """

        return self.rightImage


    def checkGridSensorData(self):
        """

        Retrieves grid sensor data and returns bool for success (needs to be executed if you want to get new grid sensor data).

        Returns:
            res: True, if data retrieval was successful.

        """


        self.gridSensorDataX, self.gridSensorDataY, self.gridSensorDataZ, self.gridSensorDataRotationX, self.gridSensorDataRotationY, self.gridSensorDataRotationZ, res = self.receiver.getGridSensorData()
        return res


    def getGridSensorData(self):
        """

        Returns the grid sensor data previously retrieved by checkGridSensorData().

        Returns:
            gridData: List with grid sensor data.

        """

        gridData = []

        gridData.append(self.gridSensorDataX)
        gridData.append(self.gridSensorDataY)
        gridData.append(self.gridSensorDataZ)
        gridData.append(self.gridSensorDataRotationX)
        gridData.append(self.gridSensorDataRotationY)
        gridData.append(self.gridSensorDataRotationZ)

        return gridData


    def checkHeadMotion(self):
        """

        Retrieves head motion data and returns bool for success (needs to be executed if you want to get new head motion data).

        Returns:
            res: True, if data retrieval was successful.

        """

        self.headMotionVelocityX, self.headMotionVelocityY, self.headMotionVelocityZ, self.headMotionAccelerationX, self.headMotionAccelerationY, self.headMotionAccelerationZ, self.headMotionRotationVelocityX, self.headMotionRotationVelocityY, self.headMotionRotationVelocityZ, self.headMotionRotationAccelerationX, self.headMotionRotationAccelerationY, self.headMotionRotationAccelerationZ, res = self.receiver.getHeadMotion()

        return res


    def getHeadMotion(self):
        """

        Returns the head motion data previously retrieved by checkHeadMotion().

        Returns:
            headMotion: List with head motion data.

        """

        headMotion = []

        headMotion.append(self.headMotionVelocityX)
        headMotion.append(self.headMotionVelocityY)
        headMotion.append(self.headMotionVelocityZ)
        headMotion.append(self.headMotionAccelerationX)
        headMotion.append(self.headMotionAccelerationY)
        headMotion.append(self.headMotionAccelerationZ)
        headMotion.append(self.headMotionRotationVelocityX)
        headMotion.append(self.headMotionRotationVelocityY)
        headMotion.append(self.headMotionRotationVelocityZ)
        headMotion.append(self.headMotionRotationAccelerationX)
        headMotion.append(self.headMotionRotationAccelerationY)
        headMotion.append(self.headMotionRotationAccelerationZ)

        return headMotion


    def checkEyePosition(self):
        """

        Retrieves eye position data and returns bool for success (needs to be executed if you want to get new eye position data).

        Returns:
            res: True, if data retrieval was successful.

        """

        self.eyeRotationPositionX, self.eyeRotationPositionY, self.eyeRotationPositionZ, self.eyeRotationVelocityX, self.eyeRotationVelocityY, self.eyeRotationVelocityZ, res = self.receiver.getEyePosition()

        return res


    def getEyePosition(self):
        """

        Returns the eye position data previously retrieved by checkEyePosition().

        Returns:
            eyePosition: List with eye position data.

        """

        eyePosition = []

        eyePosition.append(self.eyeRotationPositionX)
        eyePosition.append(self.eyeRotationPositionY)
        eyePosition.append(self.eyeRotationPositionZ)
        eyePosition.append(self.eyeRotationVelocityX)
        eyePosition.append(self.eyeRotationVelocityY)
        eyePosition.append(self.eyeRotationVelocityZ)
    
        return eyePosition


    def checkExternalReward(self):
        """

        Retrieves external reward data and returns bool for success (needs to be executed if you want to get new external reward data).

        Returns:
            res: True, if data retrieval was successful.

        """

        self.externalReward, res = self.receiver.getExternalReward()

        return res


    def getExternalReward(self):
        """

        Returns the external reward data previously retrieved by checkExternalReward().

        Returns:
            externalReward: External reward.

        """

        return self.externalReward


    def checkActionExecState(self, actionID):
        """

        Retrieves action execution state for a specific action and returns bool for success (needs to be executed if you want to get new action execution state data).

        Returns:
            res: True, if data retrieval was successful.

        """

        self.state, res = self.receiver.getActionExecState(actionID)
        
        return res


    def getActionExecState(self):
        """

        Returns the action execution data previously retrieved by checkActionExecState().

        Returns:
            state: Action execution state.
                0 = InExecution
                1 = Finished
                2 = Aborted
                3 = Walking
                4 = Rotating
                5 = WalkingRotating

        """

        return self.state


    def checkCollision(self):
        """

        Retrieves collision data and returns bool for success (needs to be executed if you want to get new collision data).

        Returns:
            res: True, if data retrieval was successful.

        """
        self.actionColID, self.colliderID, res = self.receiver.getCollision()

        return res


    def getCollision(self):
        """

        Returns the collision data previously retrieved by checkCollision().

        Returns:
            collisionData: List of collision data.

        """
        collisionData = []

        collisionData.append(self.actionColID)
        collisionData.append(self.colliderID)

        return collisionData


    def checkMenuItem(self):
        """

        Retrieves menu item data and returns bool for success (needs to be executed if you want to get new menu item data).

        Returns:
            res: True, if data retrieval was successful.

        """
        self.eventID, self.parameter, res = self.receiver.getMenuItem()

        return res

    # return menu item event id previously retrieved by checkMenuItem()
    def getMenuItemID(self):
        """

        Returns the menu item id previously retrieved by checkMenuItem().

        Returns:
            eventID: Menu item id.

        """
        return self.eventID

    # return menu item parameter previously retrieved by checkMenuItem()
    def getMenuItemParameter(self):

        return self.parameter

    # return True if start sync has been received
    def hasStartSyncReceived(self):

        return self.receiver.hasStartSyncReceived()


    ############################################################################################
    ### SENDING FUNCTIONS
    ###
    ### Functions with HAVE an Action Execution State, are executed with the waitForFullExec() function.
    ### If you don't want to wait for the full execution, you can look at the waitForExec() function at the top of the file.
    ############################################################################################

    # send the agent to walk a certain distance in a certain direction (degrees)
    def sendAgentMovement(self, degree, distance):

        print "SEND & WAIT: AgentMovement"
        waitForFullExec(self, self.sender.sendAgentMovement(degree, distance))

    # the eyes (cameras) of the agent can be moved individually in vertical directions, but only together horizontally
    def sendEyeMovement(self, panLeft, panRight, tilt):

        print "SEND & WAIT: EyeMovement"
        waitForFullExec(self, self.sender.sendEyeMovement(panLeft, panRight, tilt))

    # the agent fixates the eyes on a given point in the 3-dimensional space
    def sendEyeFixation(self, targetX, targetY, targetZ):

        print "SEND & WAIT: EyeFixation"
        waitForFullExec(self, self.sender.sendEyeFixation(targetX, targetY, targetZ))

    # resets the environment (exact function needs to be specified in your own Unity BehaviourScript)
    #
    # waiting time, because EnvironmentReset does NOT return an execution status. if you experience
    # the reset not being finished in time, increase msgWaitingTime
    def sendEnvironmentReset(self, type=0):

        print "SEND: EnvironmentReset"
        res = self.sender.sendEnvironmentReset(type)
        time.sleep(self.msgWaitingTime)
        return res

    # resets the trial (exact function needs to be specified in your own Unity BehaviourScript)
    #
    # waiting time, because TrialReset does NOT return an execution status. if you experience
    # the reset not being finished in time, increase msgWaitingTime
    def sendTrialReset(self, type=0):

        print "SEND: TrialReset"
        res = self.sender.sendTrialReset(type)
        time.sleep(self.msgWaitingTime)
        return res

    # the agents grasps for a certain object (the objectID needs to be assigned to an existing object in the BehaviourScript)
    def sendGraspID(self, objectID):

        print "SEND & WAIT: GraspID"
        waitForFullExec(self, self.sender.sendGraspID(objectID))

    # the agent grasps for whatever is located in the position in its current view, given by a 2-dimentional point
    def sendGraspPos(self, targetX, targetY):

        print "SEND & WAIT: GraspPos"
        waitForFullExec(self, self.sender.sendGraspPos(targetX, targetY))

    # NOT TESTED
    def sendPointPos(self, targetX, targetY):

        print "SEND & WAIT: PointPos"
        waitForFullExec(self, self.sender.sendPointPos(targetX, targetY))

    # NOT TESTED
    def sendPointID(self, objectID):

        print "SEND & WAIT: PointID"
        waitForFullExec(self, self.sender.sendPointID(objectID))

    # NOT TESTED
    def sendInteractionID(self, objectID):

        print "SEND & WAIT: InteractionID"
        waitForFullExec(self, self.sender.sendInteractionID(objectID))

    # NOT TESTED
    def sendInteractionPos(self, targetX, targetY):

        print "SEND & WAIT: InteractionPos"
        waitForFullExec(self, self.sender.sendInteractionPos(targetX, targetY))

    # NOT TESTED
    def sendStopSync(self):

        print "SEND: StopSync"
        res = self.sender.sendStopSync()
        time.sleep(self.msgWaitingTime)
        return res

    # the agent lets go of whatever object it holds in its hand
    def sendGraspRelease(self):

        print "SEND & WAIT: GraspRelease"
        waitForFullExec(self, self.sender.sendGraspRelease())

    # the agent turns in the given direction (degrees)
    def sendAgentTurn(self, degree):

        print "SEND & WAIT: AgentTurn"
        waitForFullExec(self, self.sender.sendAgentTurn(degree))

    # the agent moves towards a given point in the 3-dimensional space (NOT TESTED)
    def sendAgentMoveTo(self, x, y, z, targetMode=0):

        print "SEND & WAIT: AgentMoveTo"
        waitForFullExec(self, self.sender.sendAgentMoveTo(x, y, z, targetMode))

    # the agent interrupts whatever movement it's currently executing (only possible WITHOUT the use of the waitForFullExec() function)
    def sendAgentCancelMovement(self):

        print "SEND: AgentCancelMovement"
        res = self.sender.sendAgentCancelMovement()
        time.sleep(self.msgWaitingTime)
        return res

    # checks the version of the Unity server (this is called automatically when initializing the interface object)
    def sendVersionCheck(self):

        res = self.sender.sendVersionCheck()
        time.sleep(self.msgWaitingTime)
        return res
