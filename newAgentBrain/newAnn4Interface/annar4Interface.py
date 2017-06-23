from annar4Interface import *
from annarProtoMain import *
from annarProtoRecv import *
from annarProtoSend import *
from MsgObject_pb2 import *

import sys
import time
import signal

# function to wait until an action is COMPLETELY executed in the VR (only for actions which have an action execution state)
def waitForFullExec(annarInterface, id, timeout = -1):
    

    # Action Execution Status Meaning:
    #
    # 0 = InExecution
    # 1 = Finished
    # 2 = Aborted
    # 3 = Walking
    # 4 = Rotating
    # 5 = WalkingRotating
    #

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


# function to wait until an action is executed in the VR (only for actions which have an action execution state)

# NOTE: not sure if this is old (since it only checks for the 'InExecution' action execution state, or if it only
#       waits until the action was STARTED in the VR, not COMPLETED)
def waitForExec(annarInterface, id, timeout = -1):
    

    # Action Execution Status Meaning:
    #
    # 0 = InExecution
    # 1 = Finished
    # 2 = Aborted
    # 3 = Walking
    # 4 = Rotating
    # 5 = WalkingRotating
    #

    ret = False
    actionState = 0
        
    if timeout == -1:  
    
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


class Annar4Interface(object):

    # initialize all needed variables belonging to the object
    def __init__(self, srv_addr, remotePortNo, agentNo, agentOnly, softwareInterfaceTimeout):

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

        self.annarProtoMain = AnnarProtoMain(srv_addr, remotePortNo, agentNo, agentOnly, softwareInterfaceTimeout)



    # if Ctrl-C is called, threads are closed properly to avoid having to close the terminal every time something goes wrong
    def abort_signal(self, signal, frame):
        print "\nMANUAL TERMINATION: Stopping all threads..."
        self.stop(True)
        sys.exit(0)

    # start the 'annarProtoMain' instance and compare version strings with the server (if versions are different, program exits)
    def start(self):
        
        # start the Ctrl-C signal handler
        signal.signal(signal.SIGINT, self.abort_signal)

        self.annarProtoMain.start()

        self.annarProtoMain.getSender().sendVersionCheck(self.version)
        VRVersion = ""
        while(VRVersion == ""):
            VRVersion = self.annarProtoMain.getReceiver().getVersion()
        print "/////////////////////////////////////"
        print "Client Version: " + self.version
        print "VR Version: " + VRVersion
        print "/////////////////////////////////////"
        print ""
        if (self.version != VRVersion):
            print "ERROR: Versions are different, please update your client!"
            self.stop(True)
            sys.exit(1)

    # stop the 'annarProtoMain' instance and delete it
    def stop(self, wait=True):

        print ""
        print "/////////////////////////////////////"
        print "EXITING..."

        self.annarProtoMain.stop(wait)
        del self.annarProtoMain

        print "DONE."
        print "/////////////////////////////////////"

    # retrieve images and return bool for successs
    def checkImages(self):

        self.leftImage, self.rightImage, res = self.annarProtoMain.getReceiver().getImageData()

        return res

    # return left image
    def getImageLeft(self):

        return self.leftImage

    # return right image
    def getImageRight(self):

        return self.rightImage

    # retrieve grid sensor data and return bool for success
    def checkGridSensorData(self):


        self.gridSensorDataX, self.gridSensorDataY, self.gridSensorDataZ, self.gridSensorDataRotationX, self.gridSensorDataRotationY, self.gridSensorDataRotationZ, res = self.annarProtoMain.getReceiver().getGridSensorData()
        return res

    # return the previously retrieved grid sensor data
    def getGridSensorData(self):

        gridData = []

        gridData.append(self.gridSensorDataX)
        gridData.append(self.gridSensorDataY)
        gridData.append(self.gridSensorDataZ)
        gridData.append(self.gridSensorDataRotationX)
        gridData.append(self.gridSensorDataRotationY)
        gridData.append(self.gridSensorDataRotationZ)

        return gridData

    # retrieve head motion data and return bool for success
    def checkHeadMotion(self):

        self.headMotionVelocityX, self.headMotionVelocityY, self.headMotionVelocityZ, self.headMotionAccelerationX, self.headMotionAccelerationY, self.headMotionAccelerationZ, self.headMotionRotationVelocityX, self.headMotionRotationVelocityY, self.headMotionRotationVelocityZ, self.headMotionRotationAccelerationX, self.headMotionRotationAccelerationY, self.headMotionRotationAccelerationZ, res = self.annarProtoMain.getReceiver().getHeadMotion()

        return res


    # return the previously retrieved head motion data
    def getHeadMotion(self):

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

    # retrieve eye position data and return bool for success
    def checkEyePosition(self):

        self.eyeRotationPositionX, self.eyeRotationPositionY, self.eyeRotationPositionZ, self.eyeRotationVelocityX, self.eyeRotationVelocityY, self.eyeRotationVelocityZ, res = self.annarProtoMain.getReceiver().getEyePosition()

        return res

    # return the previously retrieved eye position data
    def getEyePosition(self):

        eyePosition = []

        eyePosition.append(self.eyeRotationPositionX)
        eyePosition.append(self.eyeRotationPositionY)
        eyePosition.append(self.eyeRotationPositionZ)
        eyePosition.append(self.eyeRotationVelocityX)
        eyePosition.append(self.eyeRotationVelocityY)
        eyePosition.append(self.eyeRotationVelocityZ)
    
        return eyePosition

    # retrieve external reward and return bool for success
    def checkExternalReward(self):

        self.externalReward, res = self.annarProtoMain.getReceiver().getExternalReward()

        return res

    # return external reward
    def getExternalReward(self):

        return self.externalReward

    # retrieve action execution state and return bool for success
    def checkActionExecState(self, actionID):

        self.state, res = self.annarProtoMain.getReceiver().getActionExecState(actionID)
        
        return res

    # return action execution state
    def getActionExecState(self):

        return self.state

    # retrieve collision data and return bool for success
    def checkCollision(self):

        self.actionColID, self.colliderID, res = self.annarProtoMain.getReceiver().getCollision()

        return res

    # return collision data
    def getCollision(self):

        data = []

        data.append(self.actionColID)
        data.append(self.colliderID)

        return data

    # retrieve menu item data and return bool for success
    def checkMenuItem(self):

        self.eventID, self.parameter, res = self.annarProtoMain.getReceiver().getMenuItem()

        return res

    # return menu item event id
    def getMenuItemID(self):

        return self.eventID

    # return menu item parameter
    def getMenuItemParameter(self):

        return self.parameter

    # return True if start sync has been received
    def hasStartSyncReceived(self):

        return self.annarProtoMain.getReceiver().hasStartSyncReceived()


    ############################################################################################
    ### SENDING FUNCTIONS
    ###
    ### (functions, which include an action execution status are executed with 'waitForExec')
    ############################################################################################

    def sendAgentMovement(self, degree, distance):

        print "SEND & WAIT: AgentMovement"
        waitForFullExec(self, self.annarProtoMain.getSender().sendAgentMovement(degree, distance))

    def sendEyeMovement(self, panLeft, panRight, tilt):

        print "SEND & WAIT: EyeMovement"
        waitForFullExec(self, self.annarProtoMain.getSender().sendEyeMovement(panLeft, panRight, tilt))

    def sendEyeFixation(self, targetX, targetY, targetZ):

        print "SEND & WAIT: EyeFixation"
        waitForFullExec(self, self.annarProtoMain.getSender().sendEyeFixation(targetX, targetY, targetZ))

    def sendEnvironmentReset(self, type=0):

        print "SEND: EnvironmentReset"
        res = self.annarProtoMain.getSender().sendEnvironmentReset(type)
        time.sleep(self.msgWaitingTime)
        return res

    def sendTrialReset(self, type=0):

        print "SEND: TrialReset"
        res = self.annarProtoMain.getSender().sendTrialReset(type)
        time.sleep(self.msgWaitingTime)
        return res

    def sendGraspID(self, objectID):

        print "SEND & WAIT: GraspID"
        waitForFullExec(self, self.annarProtoMain.getSender().sendGraspID(objectID))

    def sendGraspPos(self, targetX, targetY):

        print "SEND & WAIT: GraspPos"
        waitForFullExec(self, self.annarProtoMain.getSender().sendGraspPos(targetX, targetY))

    def sendPointPos(self, targetX, targetY):

        print "SEND & WAIT: PointPos"
        waitForFullExec(self, self.annarProtoMain.getSender().sendPointPos(targetX, targetY))

    def sendPointID(self, objectID):

        print "SEND & WAIT: PointID"
        waitForFullExec(self, self.annarProtoMain.getSender().sendPointID(objectID))

    def sendInteractionID(self, objectID):

        print "SEND & WAIT: InteractionID"
        waitForFullExec(self, self.annarProtoMain.getSender().sendInteractionID(objectID))

    def sendInteractionPos(self, targetX, targetY):

        print "SEND & WAIT: InteractionPos"
        waitForFullExec(self, self.annarProtoMain.getSender().sendInteractionPos(targetX, targetY))

    def sendStopSync(self):

        print "SEND: StopSync"
        res = self.annarProtoMain.getSender().sendStopSync()
        time.sleep(self.msgWaitingTime)
        return res

    def sendGraspRelease(self):

        print "SEND & WAIT: GraspRelease"
        waitForFullExec(self, self.annarProtoMain.getSender().sendGraspRelease())

    def sendAgentTurn(self, degree):

        print "SEND & WAIT: AgentTurn"
        waitForFullExec(self, self.annarProtoMain.getSender().sendAgentTurn(degree))

    def sendAgentMoveTo(self, x, y, z, targetMode=0):

        print "SEND & WAIT: AgentMoveTo"
        waitForFullExec(self, self.annarProtoMain.getSender().sendAgentMoveTo(x, y, z, targetMode))

    def sendAgentCancelMovement(self):

        print "SEND: AgentCancelMovement"
        res = self.annarProtoMain.getSender().sendAgentCancelMovement()
        time.sleep(self.msgWaitingTime)
        return res

    def sendVersionCheck(self):

        res = self.annarProtoMain.getSender().sendVersionCheck()
        time.sleep(self.msgWaitingTime)
        return res
