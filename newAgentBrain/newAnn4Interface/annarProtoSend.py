import Queue
import random
import sys

from annar4Interface import *
from annarProtoMain import *
from annarProtoRecv import *
from annarProtoSend import *
from MsgObject_pb2 import *

RAND_MAX = 32767

class AnnarProtoSend(object):

    def __init__(self, socketVR, socketAgent):

        self.mutex = False
        self.done = True
        self.socketVR = socketVR
        self.socketAgent = socketAgent
        self.sentQueue = Queue.Queue()

    ####################
    ## START - Messages
    ####################

    def sendVersionCheck(self, versionString):

        unit = MsgObject()

        unit.msgVersionCheck.version = versionString

        self.mutex = True
        self.sentQueue.put(unit)
        self.mutex = False


    def sendAgentMovement(self, degree, distance):

        unit = MsgObject()
        
        id = random.randrange(0, RAND_MAX)
        unit.msgAgentMovement.actionID = id
        unit.msgAgentMovement.distance = distance
        unit.msgAgentMovement.degree = degree

        self.mutex = True
        self.sentQueue.put(unit)
        self.mutex = False

        return id


    def sendEyeMovement(self, panLeft, panRight, tilt):

        unit = MsgObject()

        id = random.randrange(0, RAND_MAX)
        unit.msgAgentEyemovement.actionID = id
        unit.msgAgentEyemovement.panLeft = panLeft
        unit.msgAgentEyemovement.panRight = panRight
        unit.msgAgentEyemovement.tilt = tilt

        self.mutex = True
        self.sentQueue.put(unit)
        self.mutex = False

        return id


    def sendEyeFixation(self, targetX, targetY, targetZ):

        unit = MsgObject()

        id = random.randrange(0, RAND_MAX)
        unit.msgAgentEyeFixation.actionID = id
        unit.msgAgentEyeFixation.targetX = targetX
        unit.msgAgentEyeFixation.targetY = targetY
        unit.msgAgentEyeFixation.targetZ = targetZ

        self.mutex = True
        self.sentQueue.put(unit)
        self.mutex = False

        return id


    def sendEnvironmentReset(self, type):

        unit = MsgObject()

        unit.msgEnvironmentReset.Type = type

        self.mutex = True
        self.sentQueue.put(unit)
        self.mutex = False


    def sendTrialReset(self, type):

        unit = MsgObject()

        unit.msgTrialReset.Type = type

        self.mutex = True
        self.sentQueue.put(unit)
        self.mutex = False


    def sendGeneralMsg(self, ownMsg):

        self.mutex = True
        self.sentQueue.put(ownMsg)
        self.mutex = False

    def sendPointID(self, objectID):

        unit = MsgObject()

        id = random.randrange(0, RAND_MAX)
        unit.msgAgentPointID.actionID = id
        unit.msgAgentPointID.objectID = objectID

        self.mutex = True
        self.sentQueue.put(unit)
        self.mutex = False

        return id

    def sendPointPos(self, targetX, targetY):


        unit = MsgObject()

        id = random.randrange(0, RAND_MAX)
        unit.msgAgentPointPos.actionID = id
        unit.msgAgentPointPos.targetX = targetX
        unit.msgAgentPointPos.targetY = targetY


        self.mutex = True
        self.sentQueue.put(unit)
        self.mutex = False

        return id


    def sendGraspID(self, objectID):

        unit = MsgObject()

        id = random.randrange(0, RAND_MAX)
        unit.msgAgentGraspID.actionID = id
        unit.msgAgentGraspID.objectID = objectID


        self.mutex = True
        self.sentQueue.put(unit)
        self.mutex = False

        return id


    def sendGraspPos(self, targetX, targetY):

        unit = MsgObject()

        id = random.randrange(0, RAND_MAX)
        unit.msgAgentGraspPos.actionID = id
        unit.msgAgentGraspPos.targetX = targetX
        unit.msgAgentGraspPos.targetY = targetY


        self.mutex = True
        self.sentQueue.put(unit)
        self.mutex = False

        return id



    def sendGraspRelease(self):

        unit = MsgObject()

        id = random.randrange(0, RAND_MAX)
        unit.msgAgentGraspRelease.actionID = id


        self.mutex = True
        self.sentQueue.put(unit)
        self.mutex = False

        return id


    def sendAgentTurn(self, degree):

        unit = MsgObject()

        id = random.randrange(0, RAND_MAX)
        unit.msgAgentTurn.actionID = id
        unit.msgAgentTurn.degree = degree


        self.mutex = True
        self.sentQueue.put(unit)
        self.mutex = False

        return id

    def sendAgentMoveTo(self, x, y, z, targetMode):

        unit = MsgObject()

        id = random.randrange(0, RAND_MAX)
        unit.msgAgentMoveTo.actionID = id
        unit.msgAgentMoveTo.posX = x
        unit.msgAgentMoveTo.posY = y
        unit.msgAgentMoveTo.posZ = z
        unit.msgAgentMoveTo.targetMode = targetMode


        self.mutex = True
        self.sentQueue.put(unit)
        self.mutex = False

        return id


    def sendAgentCancelMovement(self):

        unit = MsgObject()

        id = random.randrange(0, RAND_MAX)
        unit.msgAgentCancelMovement.actionID = id


        self.mutex = True
        self.sentQueue.put(unit)
        self.mutex = False

        return id

    def sendInteractionID(self, objectID):

        unit = MsgObject()

        id = random.randrange(0, RAND_MAX)
        unit.msgAgentInteractionID.actionID = id
        unit.msgAgentInteractionID.objectID = objectID


        self.mutex = True
        self.sentQueue.put(unit)
        self.mutex = False

        return id


    def sendInteractionPos(self, targetX, targetY):

        unit = MsgObject()

        id = random.randrange(0, RAND_MAX)
        unit.msgAgentInteractionPos.actionID = id
        unit.msgAgentInteractionPos.targetX = targetX
        unit.msgAgentInteractionPos.targetY = targetY


        self.mutex = True
        self.sentQueue.put(unit)
        self.mutex = False

        return id

    def sendStopSync(self):

        unit = MsgObject()

        id = random.randrange(0, RAND_MAX)
        unit.msgStopSync

        self.mutex = True
        self.sentQueue.put(unit)
        self.mutex = False

        return id


        #####################
        ## END messages
        #####################

    # start thread for main loop
    def start(self):

        if self.done:

            self.done = False
            self.thread = threading.Thread(target=self.mainLoop)
            self.thread.start()

    # stop the main loop thread
    def stop(self, wait):

        if not self.done:

            self.done = True
            if wait:
                self.thread.join()

    # main loop executed by a thread
    # objects from the queue are serialized and sent to the server
    def mainLoop(self):

        while not self.done:

            try:

                if not self.mutex:
                    if not self.sentQueue.empty():
                        unit = self.sentQueue.get()
                        self.serializeAndSend(unit)
                time.sleep(1/1000000.0)
            except:
                print "ERROR(AnnarProtoSend): FAILED SENDING MESSAGE"
                raise

    # a protobuf message object if serialized to a string and is sent to the server after a 4 byte header containing the length of the message
    def serializeAndSend(self, unit):

        out = unit.SerializeToString()
        length = len(out)
        
        # a 4 byte header containg the length of the message after it, byte order and data type of the header is important ('<i')
        data = struct.pack('<i', length) + out

        # decision which socket to use
        if (unit.HasField("msgEnvironmentReset") or unit.HasField("msgTrialReset")):

            if (self.socketVR == -1):

                print "ERROR: SocketVR not initialized!"
                sys.exit(1)
            self.socketVR.sendall(data)

        else:
            self.socketAgent.sendall(data)

    # simple function to print the data
    def printBuffer(self, data):

        print data

