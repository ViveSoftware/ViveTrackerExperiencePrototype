# Vive Tracker Experience Prototypes
Copyright (c) 2017, HTC Corporation. All rights reserved.

## Introduction:

- In this project, we provide serveral prototypes demonstrating different 
  experiences using Vive Trackers.

- Some experiences in this project provide a "shared" experience between Vive 
  Player and co-located audience, which can transform passive audience into active
  participants of VR universe.

- Experience list:

  1. Model Viewer: "Shared" experience. 
     Vive Player control (drag, throw) a 3D model using Vive controllers, and the 
	 audience can view the 3D model through smartphone screen with tracker mounted.

  2. Simple Shooter: "Shared" experience.
     Each audience hold a smartphone with tracker mounted. In limited time, the Vive
	 player and mobile players shoot each other with virtual bullet to gain score.

## Hardware requirements:

To run the experiences in this project, you need the following hardware:

 - Vive
 - at least 1 Vive Tracker

To run the shared experiences, you need these addutional hardware:
 
 - Smartphone (Android 6.0 or above)
 - One Vive tracker per smartphone; trackers should be mounted on smartphones
 - Wireless LAN environment for PC and smartphones

## How to run pre-build examples

For non-shared experience, just run the executable.

For shared experiences, follow these steps:
1. Install the APK files to your smartphones
2. Make sure all smartphones can connect to the PC hosting Vive via Wireless LAN
   (you can get diagnostic tools from Google Play store)
3. Run the executable on PC, and enjoy the experience!

Optionally, you can bind the trackers to certain TrackerRole, so the the tracker assigned as 1st will always
be marked as tracker1 (see Vive Input Utility documentation for more details).

## Software Requirements:

To build the prototypes from Unity Editor, please get these plugins from Unity Asset Store:

 - SteamVR plugin (ver 1.2.1 or newer)
 - Vive Input Utility (ver 1.6.0 or newer)
