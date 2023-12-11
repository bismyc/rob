# Rob
An experiment on mobile app where you can build a 3d world with objects from the assets library. It is possible to save or edit the enviroment you created. The placed objects can be moved, scaled or deleted. You can also undo your actions.

# Code Overview
The main architecture of choice is Model View Presenter. I find it very useful for developing modern touch screen apps. The features are extendable which more objects can be added to the asset library. I relied on enums and structs rather than more classes and interfaces. My main focus was to make code more readable, simple and small.

I used the Unity UI (2D) to implement an interactive menu for moving and scaling 3d objects. I preferred it because of its auto-alignment features. The undo functionality was implented used stack. 

![alt text](robdemo.gif)





