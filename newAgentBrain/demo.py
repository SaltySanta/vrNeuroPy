import newAnn4Interface
import time
from PIL import Image

# put the IP address of the server in here
VR_IP_ADDRESS = "192.168.1.2"
VR_UNITY_PORT = 1337
VR_AGENT_NO = 0




# create object and start threads
unityInterface = newAnn4Interface.Annar4Interface(VR_IP_ADDRESS, VR_UNITY_PORT, VR_AGENT_NO, False)
unityInterface.start()

################################################
### put unity commands in this section to test
################################################

unityInterface.sendEnvironmentReset()

unityInterface.sendAgentMovement(0, 5)
unityInterface.sendAgentMovement(90, 4)
unityInterface.sendAgentMovement(0, 6)
unityInterface.sendAgentMovement(270, 4)

unityInterface.sendAgentMovement(180, 0)




#########################################
#########################################

# stop threads and delete object
unityInterface.stop(True)


