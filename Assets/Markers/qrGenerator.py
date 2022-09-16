import pyqrcode
from PIL import Image

number = input("1: BasicElement Marker\n2: Empty Marker\n3: Instruction Marker\n")
logo_link = ""
if number == '1':
    logo_link = "letterB.png" 
if number == '2':
    logo_link = "LetterT.png"
if number == '3':
    logo_link = "LetterI.png"
 
name = input("Name of qr:\n")
# Generate the qr code and save as png
qrobj = pyqrcode.create(name)
with open(f'{name}.png', 'wb') as f:
    qrobj.png(f, scale=10)

# Now open that png image to put the logo
img = Image.open(f'{name}.png')
width, height = img.size

# How big the logo we want to put in the qr code png
logo_size = 60

# Open the logo image
logo = Image.open("resources/" + logo_link)

# Calculate xmin, ymin, xmax, ymax to put the logo
xmin = ymin = int((width / 2) - (logo_size / 2))
xmax = ymax = int((width / 2) + (logo_size / 2))

# resize the logo as calculated
logo = logo.resize((xmax - xmin, ymax - ymin))

# put the logo in the qr code
img.paste(logo, (xmin, ymin, xmax, ymax))

img.save(name + '.png')
