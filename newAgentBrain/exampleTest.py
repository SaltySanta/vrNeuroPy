import newAnn4Interface
import time
from PIL import Image

# put the IP address of the server in here
VR_IP_ADDRESS = "134.109.204.12"
VR_UNITY_PORT = 1337
VR_AGENT_NO = 0




# create object and start threads
unityInterface = newAnn4Interface.Annar4Interface(VR_IP_ADDRESS, VR_UNITY_PORT, VR_AGENT_NO, False, -1)
unityInterface.start()

################################################
### put unity commands in this section to test
################################################







#########################################
#########################################

# stop threads and delete object
unityInterface.stop(True)


