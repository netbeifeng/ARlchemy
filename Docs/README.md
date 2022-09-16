# Documentation for using own markers
`RuntimeReferenceImageLibrary` is currently not supported    
You can only change markers in Unity Editor
## Create new markers
- Navigate to Assets/Markers
- Install `pip` dependencies with
```$ python3 -m pip install requirements.txt```
- Generate new markers
```$ python3 qrGenerator.py```

## Map new Marker with new Prefab
1. Create a new prefab
2. Generate a new Marker (current intuitive solution with `Markers/qrGenerator.py`)
3. In Folder `XR`->`ReferenceImageLibrary`, add the new marker  
![image](https://user-images.githubusercontent.com/58142398/177011250-5cfbe70c-8f6c-4939-8ad1-c7f029dc5702.png)
4. In Hierarchy `AR Session Origin` -> `ImageObjectMapper`, add the prefabs. `ImageObjectMapper` automatically creates a list that has the same length with the `ImageReferenceLibrary` and shows the name of each image
![image](https://user-images.githubusercontent.com/58142398/177011270-ad507601-85dd-4dc2-8a2a-9d4533f037cf.png)
