# Robot Raconteur ABB IRC5 Driver

## Setup Instructions

**This driver requires the IRC5 EGM control option.**

These instructions assume that the robot is fully operational, and has the EGM option installed. Before starting, make sure that standard RAPID programs are running normally. Consult the ABB documentation and examples for instructions on how to test the robot.

*Note: If you have Robot Studio installed and connected to the robot, it is easier to follow instructions 4 - 6 below instead of the teach pendant based instructions in this section.*

1. Configure robot settings. Complete the following:
   1. Configure the control computer IP address
      1. Select (menu) -> Control Panel
      2. Select "Configuration"
      3. Select "Topics" -> "Communication"
      4. Select "Transmission Protocol"
      5. Select "UCdevice"
      6. Set "Remote Device" to the IP address of the computer running the ABB Robot Raconteur Driver
      7. Set "Interface" to "WAN"
      8. Tap "OK"
      9. Click "X" (top right) to close the configuration
   2. Configure the EGM settings
      1. Select (menu) -> Control Panel
      2. Select "Configuration"
      3. Select "Topics" -> "Motion"
      4. Select "External Motion Interface"
      5. Tap "Add"
      6. Set "Name" to "conf1"
      7. Set "Level" to "Raw"
      8. Set "Do Not Restart after Motors Off" to "Yes"
      9. Set "Default Ramp Time" to 0.1
      10. Tap "Ok"
      11. Click "X" (top right) to close the configuration
2. Load the RAPID program. Complete the following:
   1. Select (menu) -> Control Panel
   2. Select "File" -> "New Program"
   3. Enter the program listed in rapid/egm.mod
      1. (Note: If you have Robot Studio it is much easier to transfer the program using the RAPID editor on a PC. See Step 5 in the Robot Studio Setup Instructions)
3. Run the RAPID program
   1. Select (menu) -> Production Window
   2. Tap "PP to Main"
   3. If the robot is in manual mode, grasp the enabling switch
   4. Push the "Motors On" button on the controller
   5. Press "Play" on the teach pendant (physical button)
   6. Start the ABB Robot Raconteur Driver program

To start the program in the future or recover from an error, repeat Step 3. The ABB Robot Raconteur Driver program may be started before or after the robot, and should reconnect after an error.

## Robot Studio Setup Instructions

These instructions use the ABB IRB 1200-7/0.7 robot as an example. Other robots can be substituted for this model when configuring the simulator.

1. Select File ribbon tab -> New -> Solution With Sation and Virtual Controller
   1. Set "Solution Name" and "Location" to desired save path
   2. Select "Create New" with the desired Robot Model. For this example, select "IRB 1200 7kg 0.7m"
   3. Check "Customize Options"
   4. Click "Create"
2. "Change Options" will now display. Complete the following:
   1. Type "EGM" into the filter
   2. Select "Engineering Tools" under "System Options"
   3. Check "689-1 Externally Guided Motion (EGM)"
   4. Click "Ok"
3. Select "IRB1200_7_70_STD_02" when prompted to "Select library ..."
4. Configure robot settings. Complete the following:
   1. Select Controller ribbon tab -> Configuration -> Communication
      1. Select "Transmission Protocol"
      2. Double click on "UCdevice"
      3. Set the "Remote Address" to the IP address of the control computer running the Robot Raconteur ABB Driver
      4. Click "Ok"
   2. Select Controller ribbon tab -> Configuration -> Motion
      1. Select "External Motion Interface Data"
      2. Right click in the window and select "New External Motion Interface Data..."
      3. Set "Name" to "conf1"
      4. Set "Do Not Restart after Motors Off" to "Yes"
      5. Set "Level" to "Raw"
      6. Set "Default Ramp Time" to 0.1
5. Configure the RAPID program. Complete the following
   1. Select the "RAPID" ribbon tab
   2. On the Controller tab on the left, expand "RAPID" -> "T_ROB1", and double click "Module 1"
   3. Cut and paste the contents rapid/egm.mod into T_ROB1/Module1, completely replacing the contents of Module1
   4. Click "Apply" on the ribbon
6. Run the program
   1. Select Controller ribbon tab -> Operating Mode
   2. Select "Auto" in the "Operating Mode" window
   3. Select the "RAPID" ribbon tab
   4. Click "Program Pointer" -> "Set Program Pointer to Main in all tasks"
   5. Select "Run Mode" -> "Continuous"
   6. Click "Start"
   7. Start the ABB Robot Raconteur Driver

At this point, the robot and driver should be running. Save the workspace. In starting the simulation in the future, or when recovering from an error, repeat Step 6.

   