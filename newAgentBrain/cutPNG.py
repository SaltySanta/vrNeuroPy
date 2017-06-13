from PIL import Image


imcount = 7 * 72 * 3

SourceDir = 'LEARNINGSCREENS_RAW/'
ImageDir = 'LEARNINGSCREENS_CUT/'


# detailed window correction in y-direction for each object
offsetlist = []

# smallest distance
offsetlist.append(-5)   # car_crane_green
offsetlist.append(-5)   # car_crane_yellow
offsetlist.append(4)    # dog
offsetlist.append(-12)  # racecar_blue
offsetlist.append(-12)  # racecar_green
offsetlist.append(3)    # open_top_machine
offsetlist.append(0)    # teddy

# medium distance
offsetlist.append(40)
offsetlist.append(40)
offsetlist.append(40)
offsetlist.append(40)
offsetlist.append(40)
offsetlist.append(40)
offsetlist.append(40)

# biggest distance
offsetlist.append(55)
offsetlist.append(55)
offsetlist.append(55)
offsetlist.append(55)
offsetlist.append(55)
offsetlist.append(55)
offsetlist.append(55)





# window coordinates in the image
x1=173
x2=408-161
y1=117
y2=308-117

# rough correction in y-direction
PICOFFSET = -75



for i in range (1, imcount+1):

    x_1=x1
    x_2=x2
    y_1=y1-PICOFFSET-offsetlist[(((i-1)/72) % 21)]
    y_2=y2-PICOFFSET-offsetlist[(((i-1)/72) % 21)]


    img = Image.open(SourceDir + "learningscreen_" + str(i-1) + ".png")
    img2 = img.crop((x_1, y_1, x_2, y_2))
    img2.save(ImageDir + "learningscreen_" + str(i-1) + ".png")

