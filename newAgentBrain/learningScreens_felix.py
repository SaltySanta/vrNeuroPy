import newAnn4Interface
import time
import os
from PIL import Image

# put the IP address of the server in here
VR_IP_ADDRESS = "192.168.26.18"
VR_UNITY_PORT = 1337
VR_AGENT_NO = 0

# create object and start threads
unityInterface = newAnn4Interface.Annar4Interface(VR_IP_ADDRESS, VR_UNITY_PORT, VR_AGENT_NO, False, -1)
unityInterface.start()



##########################################

# max 3 - changes the agentposition, so the viewangle of the object also changes
distances = [0,1,2] 

#72 - each rotationstep is about 5 degrees (72*5 = 360)
rotations = 72

#8 - see Object-IDs below for further information
objects = [0,1,2,3,4,5,6,7] 

# max 5 - each step 20% darker (details in LearningScreensBehaviorScriptWBrightness.cs)
brightnesses = [0,1,2,3,4]

# max 6 - see colors below for further information
colors = [0,1,2,3,4,5]  

pictures = len(distances)*rotations*len(brightnesses)*len(colors)*len(objects)

objectNames = ["car_crane_green", "car_crane_yellow", "dog", "racecar_blue", "racecar_green", "open_top_machine", "teddy", "jelly"]

colorNames =  ["default", "blue", "orange", "green", "red", "yellow"]

# destinationfolder for the screenshots - will be created, must not exist - otherwise it won't work
TARGET_DIR = "screenshots" 
SUB_DIRS =  ["01-car_crane_green", "02-car_crane_yellow", "03-dog", "04-racecar_blue", "05-racecar_green", "06-open_top_machine", "07-teddy", "08-jelly"]

#create folders
os.mkdir(TARGET_DIR)
for folder in SUB_DIRS:
    os.mkdir(os.path.join(TARGET_DIR, folder))


#############
# Object-IDs
#############
#
# 0 car_crane_green 
# 1 car_crane_yellow
# 2 dog
# 3 racecar_blue
# 4 racecar_green
# 5 open_top_machine
# 6 teddy
# 7 jelly

#########
# colors    -   exact values in LearningScreensBehaviorScriptWBrightness.cs
#########
#
# 0 default
# 1 blue
# 2 orange
# 3 green
# 4 red
# 5 yellow


###########################################
################################################
### put unity commands in this section to test
################################################

#unityInterface.sendTrialReset(0)

count = 0

for object_id in objects:
    screencount = 0
    for brightness in brightnesses:  
        for distance in distances:
            for color in colors:
                for rotation in range (0, rotations):

                    #alle Objekte
                    trial_id = rotation + (object_id * 100) + (distance * 1000) + (brightness * 10000) + (color * 100000)
                    unityInterface.sendTrialReset(trial_id)

                    unityInterface.sendEyeMovement(0, 0, -10)            

                    time.sleep(1.0)

                    res = unityInterface.checkImages()
                    time.sleep(1.0)
                    if res:
                        rightImage = unityInterface.getImageRight()
                        time.sleep(1.0)
                    #printcount = "%06i" % screencount
                    rightImage.save(TARGET_DIR + '/' + SUB_DIRS[object_id] + "/" + "%05i" % screencount + "_" + objectNames[object_id] + '_D' + str(distance) + '_R' + str(rotation*5) + '_B' + str(brightness * 0.2) + '_' + colorNames[color] + '.png')
                    screencount = screencount + 1
                    count = count + 1
                    print("Image " + str(count) + "/" + str(pictures) + " done.")
    

#########################################
#########################################

# stop threads and delete object
unityInterface.stop(True)

