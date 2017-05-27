import Queue
import random
import struct
import sys
from PIL import Image
import StringIO
import time

from annar4Interface import *
from annarProtoMain import *
from annarProtoRecv import *
from annarProtoSend import *
from MsgObject_pb2 import *

RAND_MAX = 32767
S_MSG = 1400
MAX_MSG = 16*2024*1024

# function to receive the whole message of a certain length
def recvall(sock, count):
    buf = ''
    while count:
        newbuf = sock.recv(count)
        if not newbuf: return None
        buf += newbuf
        count -= len(newbuf)
    return buf


class AnnarProtoReceive(object):

    # initialize all needed variables belonging to the object
    def __init__(self, socketVR, socketAgent):

        self.versionString = ""

        self.messageLength = 0
        self.done = True
        self.mutex = False
        self.socketVR = socketVR
        self.socketAgent = socketAgent

        self.validExternalReward = False
        self.externalReward = 0

        self.validGridsensor = False
        self.targetX = 0
        self.targetY = 0
        self.targetZ = 0
        self.targetRotationX = 0
        self.targetRotationY = 0
        self.targetRotationZ = 0

        self.validCollision = False
        self.actionColID = 0
        self.colliderID = 0

        self.validActionState = False

        self.validMenuItem = False
        self.eventID = 0

        self.hasReceivedStartSync = False

        self.validHeadMotion = False
        self.velocityX = 0
        self.velocityY = 0
        self.velocityZ = 0
        self.accelerationX = 0
        self.accelerationY = 0
        self.accelerationZ = 0
        self.rotationVelocityX = 0
        self.rotationVelocityY = 0
        self.rotationVelocityZ = 0
        self.rotationAccelerationX = 0
        self.rotationAccelerationY = 0
        self.rotationAccelerationZ = 0

        self.validEyePosition = False
        self.rotationPositionX = 0
        self.rotationPositionY = 0
        self.rotationPositionZ = 0
        self.rotationVelocityEyeX = 0
        self.rotationVelocityEyeY = 0
        self.rotationVelocityEyeZ = 0

        self.actionExecutionMap = {}

        self.parameter = None

        self.thread = None

        self.buffer = None
        self.messageStream = None

    
    # turn image strings into PIL image objects and return them plus bool for success
    def getImageData(self):

        self.waitForMutexUnlock()

        leftBuff = StringIO.StringIO()
        rightBuff = StringIO.StringIO()

        leftBuff.write(self.leftImageString)
        rightBuff.write(self.rightImageString)

        leftBuff.seek(0)
        rightBuff.seek(0)

        leftImage = Image.open(leftBuff)
        rightImage = Image.open(rightBuff)

        if (leftImage != None and rightImage != None):
            res = True
        else:
            res = False

        return [leftImage, rightImage, res]

    # return version string
    def getVersion(self):

        self.waitForMutexUnlock()

        return self.versionString

    # return grid sensor data and bool for retrieval success
    def getGridsensorData(self):

        if not self.validGridsensor:
            res = False
        else:
            res = True
        self.waitForMutexUnlock()

        return [self.targetX, self.targetY, self.targetZ, self.targetRotationX, self.targetRotationY, self.targetRotationZ, res]

    # return external reward and bool for retrieval success
    def getExternalReward(self):

        if not self.validExternalReward:
            res = False
        else:
            res = True
        self.waitForMutexUnlock()

        return [self.externalReward, res]

    # return action execution state and bool for retrieval success
    def getActionExecState(self, actionID):

        if (not self.validActionState) or (not (actionID in self.actionExecutionMap)):
            res = False
        else:
            res = True
        self.waitForMutexUnlock()

        while not (actionID in self.actionExecutionMap):

            time.sleep(0.001)

        return [self.actionExecutionMap[actionID], res]

    # return collision data and bool for retrieval success
    def getCollision(self):

        if not self.validCollision:
            res = False
        else:
            res = True

        return [self.actionColID, self.colliderID, res]

    # return eye position data and bool for retrieval success
    def getEyePosition(self):

        if not self.validEyePosition:
            res = False
        else:
            res = True
        self.waitForMutexUnlock()

        return [self.rotationPositionX, self.rotationPositionY, self.rotationPositionZ, self.rotationVelocityEyeX, self.rotationVelocityEyeY, self.rotationVelocityEyeZ, res]

    # return head motion data and bool for retrieval success
    def getHeadMotion(self):

        if not self.validHeadMotion:
            res = False
        else:
            res = True
        self.waitForMutexUnlock()

        return [self.velocityX, self.velocityY, self.velocityZ, self.accelerationX, self.accelerationY, self.accelerationZ, self.rotationVelocityX, self.rotationVelocityY, self.rotationVelocityZ, self.rotationAccelerationX, self.rotationAccelerationY, self.rotationAccelerationZ, res]

    # return menu item data and bool for retrieval success
    def getMenuItem(self):

        if not self.validMenuItem:
            res = False
        else:
            res = True
        self.waitForMutexUnlock()

        return [self.eventID, self.parameter, res]

    # return bool for hasReceivedStartSync
    def hasStartSyncReceived(self):

        if not self.hasReceivedStartSync:
            return False
        self.waitForMutexUnlock()

        self.hasReceivedStartSync = False
        return True

    # parse Protobuf message object from string and retrieve available data
    def storeData(self, dataLength):

        tmp = MsgObject()
        tmp.ParseFromString(self.messageStream)

        if tmp.HasField("msgImages"):
            self.mutex = True

            self.leftImageString = tmp.msgImages.leftImage
            self.rightImageString = tmp.msgImages.rightImage

            self.mutex = False

        if tmp.HasField("msgReward"):
            self.mutex = True

            self.externalReward = tmp.msgReward.reward
            self.validExternalReward = True

            self.mutex = False

        if tmp.HasField("msgGridPosition"):
            self.mutex = True
            self.targetX = tmp.msgGridPosition.targetX
            self.targetY = tmp.msgGridPosition.targetY
            self.targetZ = tmp.msgGridPosition.targetZ
            self.targetRotationX = tmp.msgGridPosition.targetRotationX
            self.targetRotationY = tmp.msgGridPosition.targetRotationY
            self.targetRotationZ = tmp.msgGridPosition.targetRotationZ

            self.validGridsensor = True
            self.mutex = False


        if tmp.HasField("msgEyePosition"):
            self.mutex = True

            self.rotationPositionX = tmp.msgEyePosition.rotationPositionX
            self.rotationPositionY = tmp.msgEyePosition.rotationPositionY
            self.rotationPositionZ = tmp.msgEyePosition.rotationPositionZ
            self.rotationVelocityX = tmp.msgEyePosition.rotationVelocityX
            self.rotationVelocityY = tmp.msgEyePosition.rotationVelocityY
            self.rotationVelocityZ = tmp.msgEyePosition.rotationVelocityZ

            self.validEyePosition = True
            self.mutex = False

        if tmp.HasField("msgHeadMotion"):
            self.mutex = True

            self.velocityX = tmp.msgHeadMotion.velocityX
            self.velocityY = tmp.msgHeadMotion.velocityY
            self.velocityZ = tmp.msgHeadMotion.velocityZ
            self.accelerationX = tmp.msgHeadMotion.accelerationX
            self.accelerationY = tmp.msgHeadMotion.accelerationY
            self.accelerationZ = tmp.msgHeadMotion.accelerationZ
            self.rotationVelocityX = tmp.msgHeadMotion.rotationVelocityX
            self.rotationVelocityY = tmp.msgHeadMotion.rotationVelocityY
            self.rotationVelocityZ = tmp.msgHeadMotion.rotationVelocityZ
            self.rotationAccelerationX = tmp.msgHeadMotion.rotationAccelerationX
            self.rotationAccelerationY = tmp.msgHeadMotion.rotationAccelerationY
            self.rotationAccelerationZ = tmp.msgHeadMotion.rotationAccelerationZ

            self.validHeadMotion = True
            self.mutex = False


        if tmp.HasField("msgActionExecutationStatus"):
            self.mutex = True

            self.actionExecutionMap[tmp.msgActionExecutationStatus.actionID] = tmp.msgActionExecutationStatus.status

            self.validActionState = True
            self.mutex = False

        if tmp.HasField("msgCollision"):
            self.mutex = True
            self.actionColID = tmp.msgCollision.actionID
            self.colliderID = tmp.msgCollision.colliderID
            self.validCollision = True

            self.mutex = False


        if tmp.HasField("msgMenu"):
            self.mutex = True

            self.eventID = tmp.msgMenu.eventID
            if tmp.msgMenu.HasField("parameter"):
                self.parameter = tmp.msgMenu.parameter
            self.validMenuItem = True
            self.mutex = False


        if tmp.HasField("msgStartSync"):
            self.mutex = True

            self.hasReceivedStartSync = True

            self.mutex = False


        if tmp.HasField("msgVersionCheck"):
            self.mutex = True

            self.versionString = tmp.msgVersionCheck.version

            self.mutex = False

    # start thread for receiving loop
    def start(self):

        if self.done:

            self.done = False
            self.thread = threading.Thread(target=self.mainLoop)
            self.thread.start()

    # stop own main loop thread
    def stop(self, wait):

        if not self.done:
            self.done = True
            if wait:
                self.thread.join()

    # main receiving loop executed by a thread: receives imcoming messages
    # incoming messages consist of a 4 byte header containing the length of the rest of the message and the rest of the message
    def mainLoop(self):
        while not self.done:

            try:
                # retrieve header of message
                dataLengthbuf = recvall(self.socketAgent, 4)
                # bit order of header is important when converting it into an integer type ('<i')
                dataLength = struct.unpack('<i', dataLengthbuf)[0]
                if dataLength != 0:
                    if dataLength > 0:
                        if dataLength > MAX_MSG:
                            print "ERROR(AnnarProtoRecv): MSG TOO LONG!"
                        self.messageStream = recvall(self.socketAgent, dataLength)
                    else:
                        print "ERROR(AnnarProtoRecv): MSG IS EMPTY!"
                time.sleep(1/1000000.0)


            except:
                print "ERROR(AnnarProtoRecv): FAILED RECEIVING MESSAGE (close terminal if persists)"
                time.sleep(1/1000.0)

            self.storeData(dataLength)

    # simple function to wait until the mutex is unlocked
    def waitForMutexUnlock(self):

        while self.mutex:
            time.sleep(1/1000000.0)


