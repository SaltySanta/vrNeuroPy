import newAnn4Interface
import time
from PIL import Image

# put the IP address of the server in here
VR_IP_ADDRESS = "192.168.1.2"
VR_UNITY_PORT = 1337
VR_AGENT_NO = 0

# create object and start threads
unityInterface = newAnn4Interface.Annar4Interface(VR_IP_ADDRESS, VR_UNITY_PORT, VR_AGENT_NO, False, -1)
unityInterface.start()



##########################################

distances = 3

rotations = 72

objects = 7


###########################################

################################################
### put unity commands in this section to test
################################################

#unityInterface.sendTrialReset(0)


screencount = 0

for distance in range(0, distances):
    for object_id in range (0, objects):
        for rotation in range (0, rotations):

            trial_id = (object_id * 100) + (distance * 1000) + rotation

            #print 'SENDING TRIAL ID ' + str(trial_id)
            unityInterface.sendTrialReset(trial_id)

            unityInterface.sendEyeMovement(0, 0, -10)            

            time.sleep(1)

            res = unityInterface.checkImages()
            time.sleep(0.5)
            if res:
                rightImage = unityInterface.getImageRight()
                time.sleep(0.5)

            rightImage.save('learningscreen_' + str(screencount) + '.png')
            screencount = screencount + 1


#########################################
#########################################

# stop threads and delete object
unityInterface.stop(True)

